using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers
{
	public class ProfileController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}


}
