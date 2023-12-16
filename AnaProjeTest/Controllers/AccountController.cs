using AnaProjeTest.Entities;
using AnaProjeTest.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;
using System.Security.Claims;

namespace AnaProjeTest.Controllers
{
	public class AccountController : Controller
	{
		private readonly DataBaseContext _dataBaseContext;
		private readonly IConfiguration _configuration;
		public AccountController(DataBaseContext dataBaseContext, IConfiguration configuration)
		{
			_dataBaseContext = dataBaseContext;
			_configuration = configuration;
		}

		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				string md5salt = _configuration.GetValue<string>("AppSettings:MD5Salt");
				string saltedPassword = model.Password + md5salt;
				string hashedPassword = saltedPassword.MD5();

				User user = _dataBaseContext.Users.SingleOrDefault
					(x => x.Username.ToLower() == model.UserName.ToLower() && x.Password == hashedPassword);

				if (user != null)
				{

					if (user.Locked)
					{
						ModelState.AddModelError(nameof(model.UserName), "Kullanıcı Aktif Değil");
						return View(model);
					}

					var claims = new List<Claim>()
					{
						new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
						new Claim(ClaimTypes.Name, user.FullName ?? String.Empty),
						new Claim("Username", user.Username)
					};
					ClaimsIdentity identity = new(claims,
						CookieAuthenticationDefaults.AuthenticationScheme);

					ClaimsPrincipal principal = new(identity);

					HttpContext.SignInAsync(principal);

					return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError("", "Kullanıcı adı veya Şifre hatalı");
				}

			}

			return View(model);
		}
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Register(RegisterViewModel model)
		{
			if (_dataBaseContext.Users.Any(x => x.Username.ToLower() == model.UserName.ToLower()))
			{
				ModelState.AddModelError(nameof(model.UserName), "Kullanıcı tekrar kayıt edilemez");
				return View(model);
			}

			if (ModelState.IsValid)
			{
				string md5salt = _configuration.GetValue<string>("AppSettings:MD5Salt");
				string saltedPassword = model.Password + md5salt;
				string hashedPassword = saltedPassword.MD5();

				User user = new()
				{
					Username = model.UserName,
					Password = hashedPassword
				};

				_dataBaseContext.Users.Add(user);
				int affectedRowCount = _dataBaseContext.SaveChanges();

				if (affectedRowCount == 0)
				{
					ModelState.AddModelError("", "Kullanıcı Eklenemedi.");
				}
				else
				{
					return RedirectToAction(nameof(Login));
				}

			}
			return View(model);
		}
		public IActionResult Profile()
		{
			return View();
		}
	}
}
