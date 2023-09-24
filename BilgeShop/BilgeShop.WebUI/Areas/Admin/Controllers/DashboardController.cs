using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilgeShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")] // program.cs'teki area:exists kısmı ile eşleşir,
    [Authorize(Roles ="Admin")] //Claimlerdeki ClaimTyps.Role şeklinde tutulan yetki ile bağlantılı (login action)

    // Yukarıdaki yazdığımız authorize sayesinde yetkisi admin olmayan kişiler buraya istek atmaya çalıştığında accessDenied veriyoruz ve istediğimiz yere yönlendiriyoruz. (program.cs kısmında) 
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
      
    }
}
