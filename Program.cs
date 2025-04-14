using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using marimon_defaultd.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using marimon_defaultd.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuración de PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection") 
    ?? throw new InvalidOperationException("Connection string 'PostgreSQLConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

// Configuración simplificada y correcta de Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => 
{
    options.SignIn.RequireConfirmedAccount = true; // Confirma por email
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false; // Cambiado a false para facilitar pruebas
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configuración del servicio de email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();