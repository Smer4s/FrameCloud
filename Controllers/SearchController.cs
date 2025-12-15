using FrameCloud.Data;
using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers;

public class SearchController : Controller
{
	private readonly IVideoRepository _videos;
	private readonly IChannelRepository _channels;

	public SearchController(IVideoRepository videos, IChannelRepository channels)
	{
		_videos = videos;
		_channels = channels;
	}

	[HttpGet]
	public IActionResult Index() => View();

	[HttpPost]
	public async Task<IActionResult> Index(string query, string type)
	{
		if (string.IsNullOrWhiteSpace(query))
		{
			ViewBag.Query = query;
			return View();
		}

		if (type == "video")
		{
			var videos = await _videos.SearchByNameAsync(query);
			ViewBag.Videos = videos;
		}
		else if (type == "channel")
		{
			var channels = await _channels.SearchByNameAsync(query);
			ViewBag.Channels = channels;
		}

		ViewBag.Query = query;
		ViewBag.Type = type;
		return View();
	}
}