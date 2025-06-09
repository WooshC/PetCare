using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Models;
using PetCare.Services;


var builder = WebApplication.CreateBuilder(args);

// Configuración de DbContext (solo una vez)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PetCareConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// Autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax; // Relaxed para mejor compatibilidad
    });

// Servicios
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ISolicitudService, SolicitudService>();

// Registro de estrategias
builder.Services.AddScoped<AdminStrategy>();
builder.Services.AddScoped<CuidadorStrategy>();
builder.Services.AddScoped<ClienteStrategy>();
builder.Services.AddScoped<RoleStrategyFactory>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();

// Orden CORRECTO: Authentication antes de Authorization
app.UseAuthentication();
app.UseAuthorization();

// Configuración de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "redirect",
    pattern: "Redirect/ToDashboard",
    defaults: new { controller = "Redirect", action = "ToDashboard" });

app.Run();