using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentACarAPI.Application.Abstractions;
using RentACarAPI.Application.Auth.Contracts;
using RentACarAPI.Application.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RentACarAPI.Application.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IAppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IAppDbContext dbContext, IConfiguration configuration, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var emailExists = await _dbContext.Users.AnyAsync(u => u.Email.ToLower() == email, cancellationToken);
        if (emailExists)
        {
            throw new ApiConflictException("Email already registered.");
        }

        var customerRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Customer", cancellationToken);
        if (customerRole is null)
        {
            customerRole = new Role { Name = "Customer" };
            _dbContext.Roles.Add(customerRole);
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Phone = request.Phone.Trim(),
            DrivingLicenseNumber = request.DrivingLicenseNumber.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        user.Roles.Add(customerRole);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new RegisterResponse(user.Id, user.Email, user.FullName);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = await _dbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null || user.PasswordHash != passwordHash || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var jwtSection = _configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"]!;
        var audience = jwtSection["Audience"]!;
        var key = jwtSection["Key"]!;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("fullName", user.FullName)
        };

        foreach (var role in user.Roles.Select(r => r.Name))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(6),
            signingCredentials: creds);

        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
