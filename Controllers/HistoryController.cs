using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers
{
	public class HistoryController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
