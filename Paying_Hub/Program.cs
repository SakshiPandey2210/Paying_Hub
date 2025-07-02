using Microsoft.AspNetCore.Authentication.Cookies;
using Paying_Hub.Interface;
using Paying_Hub.Repository;

var builder = WebApplication.CreateBuilder(args);

// MVC + Razor + Repositories
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ILogin, LoginRepo>();
builder.Services.AddScoped<IMember, MemberRepo>();

// ? Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// ? Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ? Add cookie auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/AdminLogin";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Middlewares
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ? Required: Use session before MVC
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
