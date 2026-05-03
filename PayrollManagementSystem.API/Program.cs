using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PayrollManagementSystem.API.Helpers;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddScoped<DatabaseHelper>();
builder.Services.AddScoped<JwtHelper>();

// ── Controllers ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── CORS ─────────────────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                     ?? ["http://localhost:5173"];
builder.Services.AddCors(o => o.AddPolicy("ReactApp", p =>
    p.WithOrigins(allowedOrigins)
     .AllowAnyMethod()
     .AllowAnyHeader()
     .AllowCredentials()));

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtSecret  = builder.Configuration["Jwt:Secret"]!;
var jwtIssuer  = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtIssuer,
            ValidAudience            = jwtAudience,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// ── Swagger with JWT support ──────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Payroll Management API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Enter your JWT token (without 'Bearer ' prefix)"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            []
        }
    });
});

var app = builder.Build();

// ── Pipeline ──────────────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payroll API v1"));

app.UseCors("ReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
