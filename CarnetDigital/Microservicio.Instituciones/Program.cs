using Microservicio.Instituciones;
using Microservicio.Instituciones.Data;
using Microservicio.Instituciones.Repository;
using Microservicio.Instituciones.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<InstitucionesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InstitucionesDb")));

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

// BitacoraService
builder.Services.AddHttpClient<IBitacoraService, BitacoraService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalServices:AuditServiceUrl"]!);
});

builder.Services.AddHttpContextAccessor();

// Repository y Service de Instituciones
builder.Services.AddScoped<IInstitucionesRepository, InstitucionesRepository>();
builder.Services.AddScoped<IInstitucionesService, InstitucionesService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapInstitucionesEndpoints();

app.Run();