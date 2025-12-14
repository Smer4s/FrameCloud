using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers;

public class AccountController : Controller
{
	[HttpGet]
	public IActionResult Login()
	{
		return View();
	}

	[HttpPost]
	public IActionResult Login(string login, string password)
	{
		// здесь будет логика проверки пользователя
		// пока просто редиректим на профиль
		return RedirectToAction("Index", "Profile");
	}

	[HttpGet]
	public IActionResult Register()
	{
		return View();
	}

	[HttpPost]
	public IActionResult Register(string login, string password)
	{
		// здесь будет логика создания пользователя
		// пока просто редиректим на страницу входа
		return RedirectToAction("Login");
	}

	public IActionResult Logout()
	{
		// здесь будет логика выхода
		return RedirectToAction("Index", "Home");
	}
}
