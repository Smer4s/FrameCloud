using FrameCloud.Data;
using FrameCloud.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers;

public class ChannelController(IChannelRepository channels, IVideoRepository videos) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Index(int? id = default)
	{
		IEnumerable<Video> videoList;
		Channel? channel;

		// Если id не передан → открываем свой канал
		if (id == null)
		{
			var ownerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
			channel = await channels.GetByOwnerIdAsync(ownerId);
			if (channel is null) return View("CreatePrompt");

			// Владелец видит все свои видео
			videoList = await videos.GetByChannelAsync(channel.Id, ownerId);
			ViewBag.Videos = videoList.ToList();

			return View(channel);
		}

		// Если id передан → открываем чужой канал
		channel = await channels.GetByIdAsync(id.Value);
		if (channel is null) return NotFound();

		int? currentUserId = null;
		if (User.Identity?.IsAuthenticated ?? false)
			currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

		// Если владелец = текущий пользователь → все видео, иначе только публичные
		videoList = await videos.GetByChannelAsync(channel.Id, currentUserId);
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
