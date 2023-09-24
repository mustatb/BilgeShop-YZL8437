using BilgeShop.Business.Managers;
using BilgeShop.Business.Services;
using BilgeShop.Data.Context;
using BilgeShop.Data.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<BilgeShopContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped(typeof(IRepository<>), typeof(SqlRepository<>));
// IRepository'e dependency injection yapýldýðýnda,SqlRepository nesnesi gönder.

builder.Services.AddScoped<IUserService, UserManager>();
// AddScoped -> her istek için yeni bir kopya
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<IProductService, ProductManager>();

builder.Services.AddAuthentication
	(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie
	(options =>
{
	options.LoginPath = new PathString("/");
	options.LogoutPath = new PathString("/");
	options.AccessDeniedPath = new PathString("/"); // giriþ - çýkýþ - eriþim reddi durumlarýnda ana sayfaya yönlendiriliyorum
});




var app = builder.Build();

app.UseStaticFiles(); // wwwroot kullanýlacak

app.UseAuthentication ();
app.UseAuthorization ();
// Kimlik belirleme ve yetkilendirme iþlemleri için gerekli.

app.MapControllerRoute(
	name:"areas",
	pattern:"{area:exists}/{controller=dashboard}/{action=index}/{id?}"
	);



// default route her zaman en altta. app.MapDefaultControllerRoute();


app.MapDefaultControllerRoute(); // app.MapControllerRoute( name: "default", pattern: "{controller = home}/ {action=index}/{id?});"

app.Run();
