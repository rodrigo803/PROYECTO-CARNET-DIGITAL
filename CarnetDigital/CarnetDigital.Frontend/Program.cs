using CarnetDigital.Frontend.Http;
using CarnetDigital.Frontend.Options;
using CarnetDigital.Frontend.Services.Areas;
using CarnetDigital.Frontend.Services.Auth;
using CarnetDigital.Frontend.Services.Bitacoras;
using CarnetDigital.Frontend.Services.Carreras;
using CarnetDigital.Frontend.Services.Instituciones;
using CarnetDigital.Frontend.Services.Pantallas;
using CarnetDigital.Frontend.Services.Parametros;
using CarnetDigital.Frontend.Services.Roles;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Añadir servicios a contenedor
builder.Services.AddControllersWithViews();

builder.Services.Configure<MicroserviciosOptions>(builder.Configuration.GetSection("MicroserviciosUrls"));
builder.Services.Configure<PaginacionOptions>(builder.Configuration.GetSection("Paginacion"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<ITokenProvider, SessionTokenProvider>();

builder.Services.AddTransient<BearerTokenHandler>();

builder.Services.AddHttpClient<IAuthService, AuthService>((sp, client) =>
{
    var opciones = sp.GetRequiredService<IOptions<MicroserviciosOptions>>().Value;
    client.BaseAddress = new Uri(opciones.AuthServiceUrl);
});

builder.Services.AddHttpClient<IAreasApiService, AreasApiService>((sp, client) =>
{
    var opciones = sp.GetRequiredService<IOptions<MicroserviciosOptions>>().Value;
    client.BaseAddress = new Uri(opciones.AreasServiceUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IInstitucionesApiService, InstitucionesApiService>((sp, client) =>
{
    var opciones = sp.GetRequiredService<IOptions<MicroserviciosOptions>>().Value;
    client.BaseAddress = new Uri(opciones.InstitucionesServiceUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<ICarrerasApiService, CarrerasApiService>((sp, client) =>
{
    var opciones = sp.GetRequiredService<IOptions<MicroserviciosOptions>>().Value;
    client.BaseAddress = new Uri(opciones.CarrerasServiceUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IInstitucionesCrudApiService, InstitucionesCrudApiService>((sp, client) =>
{
    var opciones = sp.GetRequiredService<IOptions<MicroserviciosOptions>>().Value;
    client.BaseAddress = new Uri(opciones.InstitucionesServiceUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IParametrosApiService, ParametrosApiService>((sp, client) =>
{
    var opciones = sp.GetRequiredService<IOptions<MicroserviciosOptions>>().Value;
    client.BaseAddress = new Uri(opciones.ParametrosServiceUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IPantallasApiService, PantallasApiService>((sp, client) =>
{
    var opciones = sp.GetRequiredService<IOptions<MicroserviciosOptions>>().Value;
    client.BaseAddress = new Uri(opciones.AccessControlServiceUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IRolesApiService, RolesApiService>((sp, client) =>
{
    var opciones = sp.GetRequiredService<IOptions<MicroserviciosOptions>>().Value;
    client.BaseAddress = new Uri(opciones.AccessControlServiceUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IBitacorasApiService, BitacorasApiService>((sp, client) =>
{
    var opciones = sp.GetRequiredService<IOptions<MicroserviciosOptions>>().Value;
    client.BaseAddress = new Uri(opciones.AuditServiceUrl);
}).AddHttpMessageHandler<BearerTokenHandler>();

var app = builder.Build();

// Configur el request de http
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
