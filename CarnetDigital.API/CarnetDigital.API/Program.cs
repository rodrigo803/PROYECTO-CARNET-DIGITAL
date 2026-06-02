using CarnetDigital.Data.Data;
using CarnetDigital.Core.Interfaces;
using CarnetDigital.Core.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Ya no necesitamos AddEndpointsApiExplorer ni AddSwaggerGen

// 1. CONFIGURACIÓN DE LA BASE DE DATOS (Entity Framework)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. INYECCIÓN DE TUS SERVICIOS 
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();