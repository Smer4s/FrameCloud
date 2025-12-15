using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers;

[Authorize]
public class HistoryController(IWatchHistoryRepository history) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
		var list = await history.GetByUserAsync(userId);
		return View(list);
	}
}