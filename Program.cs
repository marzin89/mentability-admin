using Admin.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// Inst�llningar f�r sessioner
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(600);
    options.Cookie.HttpOnly = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// St�d f�r statiska filer
app.UseStaticFiles();

// Aktiverar routing
app.UseRouting();

// Aktiverar sessioner
app.UseSession();

// Routing
// Admin nyheter (startsida)
app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=NewsArticle}/{action=NewsArticle}"
);

// Admin nyheter (startsida)
app.MapControllerRoute(
    name:     "NewsArticle",
    pattern:  "NewsArticle",
    defaults: new { controller = "NewsArticle", action = "NewsArticle" }
);

// Admin aktiviteter
app.MapControllerRoute(
    name:     "Activity",
    pattern:  "Activity",
    defaults: new { controller = "Activity", action = "Activity" }
);

// Admin citat
app.MapControllerRoute(
    name:     "Quote",
    pattern:  "Quote",
    defaults: new { controller = "Quote", action = "Quote" }
);

// Logga in
app.MapControllerRoute(
    name:     "Login",
    pattern:  "Login",
    defaults: new { controller = "User", action = "Login" }
);

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
