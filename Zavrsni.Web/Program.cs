using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using Zavrsni.DAL;
using Zavrsni.Model;
using Zavrsni.Web.Auth;
using Zavrsni.Web.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataManagerDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DataManagerDbContext"),opt => opt.MigrationsAssembly("Zavrsni.DAL")));
builder.Services.AddHttpClient<DailyCoService>();
builder.Services.AddSingleton(sp => new DailyCoService( sp.GetRequiredService<HttpClient>(), builder.Configuration.GetConnectionString("DailyCoApiKey")));
builder.Services.AddScoped<IAuthorizationHandler, UserTypeHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireUserTypeDoktor", policy =>
        policy.Requirements.Add(new UserTypeRequirement(UserType.Doktor)));
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireUserTypePacijent", policy =>
        policy.Requirements.Add(new UserTypeRequirement(UserType.Pacijent)));
});
builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<DataManagerDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

var supportedCultures = new[]
{
    new CultureInfo("hr"), new CultureInfo("en-US")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("hr"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.MapDefaultControllerRoute();
app.MapRazorPages();

app.MapControllerRoute(
    name: "O aplikaciji",
    pattern: "o-nama",
    defaults: new { controller = "Home", action = "About" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
