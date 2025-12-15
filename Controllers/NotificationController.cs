using FrameCloud.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers;

[Authorize]
public class NotificationController(INotificationRepository notifications) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
		var list = await notifications.GetByUserAsync(userId);
		return View(list);
	}

	[HttpPost]
	public async Task<IActionResult> MarkAsRead(int id)
	{
		var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
		await notifications.MarkAsReadAsync(id, userId);
		return RedirectToAction("Index");
	}
}
