using FrameCloud.Data;
using FrameCloud.Entities;
using FrameCloud.Models;
using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers;

public class AdminController(IUserRepository users) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var list = await users.GetAllAsync();
		return View(list);
	}

	[HttpGet]
	public IActionResult Ban(int userId)
	{
		return View(new BanViewModel { UserId = userId });
	}

	[HttpPost]
	public async Task<IActionResult> Ban(BanViewModel model)
	{
		if (!ModelState.IsValid)
			return View(model);

		await users.BanUserAsync(model.UserId, model.Reason);
		return RedirectToAction("Index");
	}

	[HttpGet]
	public async Task<IActionResult> Logs(int userId)
	{
		var logs = await users.GetLogsByUserAsync(userId);
		ViewBag.UserId = userId;
		return View(logs);
	}


}
