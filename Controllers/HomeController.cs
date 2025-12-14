using FrameCloud.Data;
using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers;

public class HomeController(IVideoRepository videos) : Controller
{
	public async Task<IActionResult> Index()
	{
		var allVideos = await videos.GetAllAsync();
		return View(allVideos);
	}
}
