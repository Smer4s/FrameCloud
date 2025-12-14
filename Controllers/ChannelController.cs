using Microsoft.AspNetCore.Mvc;


namespace FrameCloud.Controllers;

public class ChannelController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}
