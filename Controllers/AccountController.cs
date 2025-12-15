using FrameCloud.Auth;
using FrameCloud.Data;
using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers;

public class AccountController(IUserRepository users, ITokenService tokens) : Controller
{
	[HttpGet]
	public IActionResult Login() => View();

	[HttpPost]
	public async Task<IActionResult> Login(LoginRequest request)
	{
		var user = await users.GetByLoginAsync(request.Login);
		if (user is null)
		{
			ModelState.AddModelError(string.Empty, "Неправильный логин или пароль");
			return View(request);
		}

		var hash = PasswordHasher.Hash(request.Password);
		if (!string.Equals(hash, user.Password, StringComparison.OrdinalIgnoreCase))
		{
			ModelState.AddModelError(string.Empty, "Неправильный логин или пароль");
			return View(request);
		}

		var ban = await users.GetActiveBanAsync(user.Id);
		if (ban is not null)
		{
			ModelState.AddModelError(string.Empty, $"Вы забанены: {ban.Reason}");
			return View(request);
		}

		var token = tokens.CreateToken(user.Id, user.RoleId, user.Login);

		Response.Cookies.Append("AuthToken", token, new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict,
			Expires = DateTime.UtcNow.AddHours(2)
		});

		return RedirectToAction("Index", "Home");
	}



	[HttpGet]
	public IActionResult Register() => View();

	[HttpPost]
	public async Task<IActionResult> Register(RegisterRequest request)
	{
		var existing = await users.GetByLoginAsync(request.Login);
		if (existing is not null) return Conflict();

		var hash = PasswordHasher.Hash(request.Password);
		var userId = await users.CreateUserAsync(roleId: 1, login: request.Login, passwordHash: hash);
		if (userId is null) return BadRequest();

		var token = tokens.CreateToken(userId.Value, 1, request.Login);

		Response.Cookies.Append("AuthToken", token, new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict,
			Expires = DateTime.UtcNow.AddHours(2)
		});

		return RedirectToAction("Index", "Home");
	}

	[HttpPost]
	public IActionResult Logout()
	{
		Response.Cookies.Delete("AuthToken");
		return RedirectToAction("Index", "Home");
	}
}
