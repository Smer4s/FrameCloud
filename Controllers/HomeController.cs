using FrameCloud.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FrameCloud.Controllers
{
	public class HomeController(ILogger<HomeController> logger) : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

	}
}
