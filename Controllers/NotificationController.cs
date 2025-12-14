using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers
{
	public class NotificationController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
