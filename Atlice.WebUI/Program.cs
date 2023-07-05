using Atlice.WebUI.Hubs;
using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using System.Net.WebSockets;
using Nethereum.Contracts.QueryHandlers.MultiCall;
using Atlice.WebUI.Models;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var assembly = Assembly.GetExecutingAssembly();
builder.Services.AddDetection();
builder.Services.AddSession(options => { options.Cookie.HttpOnly = true; options.Cookie.IsEssential = true; options.IdleTimeout = TimeSpan.FromDays(7); });
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDbContext<EFDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole<Guid>>().AddEntityFrameworkStores<EFDbContext>().AddDefaultUI().AddDefaultTokenProviders();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<MvcRazorRuntimeCompilationOptions>(options => { options.FileProviders.Clear(); options.FileProviders.Add(new EmbeddedFileProvider(assembly)); });
builder.Services.AddSignalR();
builder.Services.AddProgressiveWebApp();
builder.Services.AddSingleton<IBlobService, BlobService>();
builder.Services.AddTransient<ConnectionManager>();
builder.Services.AddSingleton<ChatHandler>();
builder.Services.AddScoped<IDataRepository, EFDataRepository>();
builder.Services.AddScoped<IServices, Services>();
builder.Services.ConfigureApplicationCookie(options => { options.LoginPath = "/Identity/Account/Login"; options.LogoutPath = "/Identity/Account/Logout"; options.AccessDeniedPath = "/Identity/Account/AccessDenied"; options.ExpireTimeSpan = TimeSpan.FromHours(24); options.SlidingExpiration = true; });
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MobileUser", policy => policy.Requirements.Add(new IMobileAuthorizationHandler()));
});
builder.Services.AddSingleton<IAuthorizationHandler, MobileAuthorizationHandler>();


builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.Conventions.AddPageRoute("/Tap/Index", "Tap/{id}");
});
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    //IdentityModelEventSource.ShowPII = true;
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseWebSockets();
var wsOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(120),
    ReceiveBufferSize = 4 * 1024
};
app.UseWebSockets(wsOptions);
var service = (ChatHandler)app.Services.GetService(typeof(ChatHandler));
app.UseMiddleware<WebSocketMiddleware>(service);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});

app.Run();
