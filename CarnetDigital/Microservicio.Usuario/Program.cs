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

// 3. Herramientas del Sistema
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

// 4. INYECCIÓN DE DEPENDENCIAS (Minimal APIs Architecture)
// AddHttpClient inyecta el HttpClient y registra automáticamente la Bitácora. ¡No ocupa AddSingleton!
builder.Services.AddHttpClient<IBitacoraService, BitacoraApiClient>();

// Registramos el servicio principal como Singleton (utilizando el ScopeFactory internamente)
builder.Services.AddSingleton<IUsuarioService, UsuarioService>();

var app = builder.Build();

// 5. MAPEO DE MINIMAL APIs
app.MapUsuarioEndpoints();

// 6. PIPELINE HTTP
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
