using FrameCloud.Data;
using FrameCloud.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers;

[Authorize]
public class ChannelController(IChannelRepository channels, IVideoRepository videos) : Controller
{
	public async Task<IActionResult> Index()
	{
		var ownerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
		var channel = await channels.GetByOwnerIdAsync(ownerId);
		if (channel is null) return View("CreatePrompt");

		var videoList = await videos.GetByChannelAsync(channel.Id);
		ViewBag.Videos = videoList.ToList();

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
