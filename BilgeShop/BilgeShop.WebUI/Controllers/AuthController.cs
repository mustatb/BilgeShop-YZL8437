using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.WebUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;

namespace BilgeShop.WebUI.Controllers
{
	// Authentication - Authorization
	// (Kimlik doğrulama - Yetkilendirme)
	public class AuthController : Controller
	{
		private readonly IUserService _userService;

		public string? CookieAuthentiticationDefaults { get; private set; }

		public AuthController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Register(RegisterViewModel formdata) 
		{ 
		
		   if(!ModelState.IsValid)
			{
				//Model istediğim şartlara uygun hazırlanmadı ise forma geri dönücem.
				return View(formdata); // form yeniden açılınca girilmiş olan veriler kaybolmasın. 
			}
			//Eğer her şey yolunda ise beni ana sayfaya geri göndersin.(kayıt işlemleri etc.)

			var addUserDto = new AddUserDto()
			{
				FirstName = formdata.FirstName.Trim(),
				LastName = formdata.LastName.Trim(),
				Email = formdata.Email.Trim().ToLower(),
				Password = formdata.Password,
			};

			var result = _userService.AddUser(addUserDto);

			if (result.IsSucceed)
			{
				return RedirectToAction("Index", "Home"); // nameof(Index),"Home" //aynı şey
			}
			else
			{
				ViewBag.ErrorMessage = result.Message;
				return View(formdata);
			}
		     
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel formData)
		{
			if (!ModelState.IsValid)
		    {
				return RedirectToAction("Index", "Home");
				// Veriler istediğim formatta/şekilde girilmediyse, ana sayfaya geri gönder.
			}
			var loginUserDto = new LoginUserDto()
			{
				Email = formData.Email,
				Password = formData.Password,
			};
			var UserInfo =_userService.LoginUser(loginUserDto);

			if(UserInfo is null)
			{
				return RedirectToAction("Index", "Home");
			}

			//Buraya kadar kodlar geldiyse demek ki email ve şifre eşleşmiş. Gerekli bilgiler (id,email,fname,lname,usertype) veritabanından çekilip bu aşamaya userınfo içersinde gelmiş.

			//oturumda tutacağım her veri -> Claim
			//Claimlerin listesi -> Claims

			var claims = new List<Claim>();

			claims.Add(new Claim("id",UserInfo.Id.ToString()));
			claims.Add(new Claim("email", UserInfo.Email));
			claims.Add(new Claim("firstName", UserInfo.FirstName));
			claims.Add(new Claim("lastName", UserInfo.LastName));
			claims.Add(new Claim("userType",UserInfo.UserType.ToString()));

			//Yetkilendirme işlemleri için özel olarak bir claim açmak gerekiyor.
			claims.Add(new Claim(ClaimTypes.Role, UserInfo.UserType.ToString())); // .Net metotlarının kullanacağı

			var claimIdentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
			// Claims içerisindeki verilerle bir oturum açılacağını söylüyorum.

			var autProperties = new AuthenticationProperties
			{
				AllowRefresh = true,  // yenilenebilir enerji kaynakları.
				ExpiresUtc = new DateTimeOffset(DateTime.Now.AddHours(48)) // oturum 48 saat geçerli
			};
			// oturumun özelliklerini belirliyorum.
			// Asenkronize (async) bir metot kullanılıyorsa, await keywordü ile kullanılır.

			 await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity), autProperties);

			return RedirectToAction("Index", "Home");
				
		}

		public async Task<IActionResult>  Logout()
		{
			await HttpContext.SignOutAsync(); // oturumu kapat.

			return RedirectToAction("Index", "Home");
		}


	}
}
