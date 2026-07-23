using CarnetDigital.Frontend.Http;
using CarnetDigital.Frontend.Options;
using CarnetDigital.Frontend.Services.Areas;
using CarnetDigital.Frontend.Services.Auth;
using CarnetDigital.Frontend.Services.Carreras;
using CarnetDigital.Frontend.Services.Instituciones;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

// TEMPORAL - reemplazar con Web1: solo esta línea cambia cuando llegue el login real.
builder.Services.AddScoped<ITokenProvider, SessionTokenProvider>();

builder.Services.AddTransient<BearerTokenHandler>();

// TEMPORAL - reemplazar con Web1
builder.Services.AddHttpClient("AuthServiceClient", (sp, client) =>
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
