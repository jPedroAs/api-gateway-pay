using System.Text;
using banco.Services;
using banco.TokenServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<BlueBankContext>(options =>
    options.UseNpgsql(connectionString));


var key = Encoding.ASCII.GetBytes(builder.Configuration["Secret"]);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ActiveUser", policy =>
        policy.Requirements.Add(new ActiveRequirement()));
});

builder.Services.AddSingleton<IAuthorizationHandler, ActiveHandler>(); 

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddDbContext<BlueBankContext>();
builder.Services.AddControllers();
builder.Services.AddScoped<BlueBankContext>();
builder.Services.AddTransient<TokenService>();
builder.Services.AddScoped<PasswordHash>();


var app = builder.Build();
app.UseCors("AllowAllOrigins");
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
