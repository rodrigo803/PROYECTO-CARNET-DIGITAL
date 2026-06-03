using AuthService.Data;    
using AuthService.Services;
using AuditService.Data; 
using CarnetDigital.Core.Interfaces;
using CarnetDigital.Core.Services;
using CarnetDigital.Data.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. SEGURIDAD Y TOKENS
// ==========================================
var key = builder.Configuration["Jwt:SigningKey"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
        };
    });

// ==========================================
// 2. BASES DE DATOS INDIVIDUALES 
// ==========================================
// A. Tu base de datos (Entity Framework Core)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UsuariosDb")));

// B. Base de datos de Autenticación (Dapper)
builder.Services.AddScoped<AuthDb>();

// C. Base de datos de Bitácora/Auditoría (Dapper)
builder.Services.AddScoped<AuditDb>();


// ==========================================
// 3. INYECCIÓN DE SERVICIOS
// ==========================================
builder.Services.AddScoped<IUsuarioService, UsuarioService>(); // El tuyo
builder.Services.AddScoped<JwtService>(); // El de tu compañero


// ==========================================
// 4. CONTROLADORES
// ==========================================
builder.Services.AddControllers();
// (Se eliminó AddEndpointsApiExplorer y AddSwaggerGen para usar Postman)

var app = builder.Build();

// ==========================================
// 5. MIDDLEWARES DE EJECUCIÓN
// ==========================================
app.UseHttpsRedirection();

// ¡El orden aquí es vital!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();