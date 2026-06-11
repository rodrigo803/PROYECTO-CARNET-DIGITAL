using Microservicio.Usuario.Repository;
using Microservicio.Usuario.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. JWT
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

// 2. Base de Datos EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UsuariosDb")));

// 3. Inyección de tu Servicio
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

// 4. HttpClient para BitacoraApiClient
builder.Services.AddHttpClient<IBitacoraService, BitacoraApiClient>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
