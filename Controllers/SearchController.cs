using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers;

public class SearchController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}
