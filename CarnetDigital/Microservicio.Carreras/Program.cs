using Microservicio.Carreras;
using Microservicio.Carreras.Data;
using Microservicio.Carreras.Repository;
using Microservicio.Carreras.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Evitar ciclos al serializar JSON
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<CarrerasDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CarrerasDb")));

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// BitacoraService → AuditService
builder.Services.AddHttpClient<IBitacoraService, BitacoraService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalServices:AuditServiceUrl"]!);
});

// InstitucionesValidator → Microservicio.Instituciones
builder.Services.AddHttpClient<IInstitucionesValidator, InstitucionesValidator>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalServices:InstitucionesServiceUrl"]!);
});

builder.Services.AddHttpContextAccessor();

// Repository y Service
builder.Services.AddScoped<ICarrerasRepository, CarrerasRepository>();
builder.Services.AddScoped<ICarrerasService, CarrerasService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapCarrerasEndpoints();

app.Run();