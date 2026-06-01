using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentACarAPI.API.Middleware;
using RentACarAPI.Application.Abstractions;
using RentACarAPI.Application.Auth;
using RentACarAPI.Application.CarCategories;
using RentACarAPI.Application.Cars;
using RentACarAPI.Application.CurrentUser;
using RentACarAPI.Application.ExtraServices;
using RentACarAPI.Application.Favorites;
using RentACarAPI.Application.Locations;
using RentACarAPI.Application.Payments;
using RentACarAPI.Application.Rentals;
using RentACarAPI.Application.Reservations;
using RentACarAPI.Application.TariffPlans;
using RentACarAPI.Application.Users;
using RentACarAPI.Infrastructure.Seeding;
using RentACarAPI.Infrastructure.Security;
using RentACarAPI.Infrastructure.Storage;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RentACarAPI.API.CurrentUser;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CarRentalDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<CarRentalDbContext>());

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IPasswordHasher, Sha256PasswordHasher>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IExtraServiceService, ExtraServiceService>();
builder.Services.AddScoped<IFavoritesService, FavoritesService>();
builder.Services.AddScoped<ITariffPlanService, TariffPlanService>();
builder.Services.AddScoped<ICarImageStorage, LocalCarImageStorage>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ICarCategoryService, CarCategoryService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection["Issuer"]!;
var audience = jwtSection["Audience"]!;
var key = jwtSection["Key"]!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = ClaimTypes.Role,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ViteDev", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync(CancellationToken.None);
}

app.UseCors("ViteDev");

// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseMiddleware<ApiExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();