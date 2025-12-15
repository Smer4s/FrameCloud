using FrameCloud.Data;
using FrameCloud.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers;

public class ChannelController(
	IChannelRepository channels,
	IVideoRepository videos,
	ISubscriptionRepository subs) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index(int? id = default)
	{
		Channel? channel;
		IEnumerable<Video> videoList;

		int? currentUserId = null;
		if (User.Identity?.IsAuthenticated ?? false)
			currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

		if (id == null)
		{
			var ownerId = currentUserId!.Value;
			channel = await channels.GetByOwnerIdAsync(ownerId);
			if (channel is null) return View("CreatePrompt");

			videoList = await videos.GetByChannelAsync(channel.Id, ownerId);
			ViewBag.Videos = videoList.ToList();
			ViewBag.IsSubscribed = false;
			return View(channel);
		}

		channel = await channels.GetByIdAsync(id.Value);
		if (channel is null) return NotFound();

		videoList = await videos.GetByChannelAsync(channel.Id, currentUserId);
		ViewBag.Videos = videoList.ToList();

		if (currentUserId.HasValue)
			ViewBag.IsSubscribed = await subs.ExistsAsync(channel.Id, currentUserId.Value);

		return View(channel);
	}



	[HttpGet]
	public IActionResult Create() => View();

	[HttpPost]
	public async Task<IActionResult> Create(Channel model)
	{
		var ownerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
		model.OwnerId = ownerId;
		model.SubscribersCount = 0;

		var id = await channels.CreateAsync(model);
		if (id is null) return BadRequest();

		return RedirectToAction("Index");
	}
}
