using FrameCloud.Data;
using FrameCloud.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers;

public class VideoController(
	IVideoRepository videos,
	IChannelRepository channels,
	ICommentRepository comments,
	IMarkRepository marks,
	IWebHostEnvironment env) : Controller
{
	[HttpGet]
	public async Task<IActionResult> Upload()
	{
		var ownerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
		var channel = await channels.GetByOwnerIdAsync(ownerId);
		if (channel is null) return RedirectToAction("Index", "Channel");

		return View(new Video
		{
			ChannelId = channel.Id,
			Description = string.Empty,
			Name = string.Empty,
			Url = string.Empty
		});
	}

	[HttpPost]
	public async Task<IActionResult> Upload(
		IFormFile file,
		string name,
		string description,
		bool isPublic)
	{
		var ownerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
		var channel = await channels.GetByOwnerIdAsync(ownerId);
		if (channel is null) return RedirectToAction("Index", "Channel");

		if (file is null || file.Length == 0)
		{
			ModelState.AddModelError(string.Empty, "Файл не выбран");
			return View();
		}

		var uploadsPath = Path.Combine(env.ContentRootPath, "uploads");
		Directory.CreateDirectory(uploadsPath);

		var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
		var filePath = Path.Combine(uploadsPath, fileName);

		using (var stream = new FileStream(filePath, FileMode.Create))
		{
			await file.CopyToAsync(stream);
		}

		var video = new Video
		{
			ChannelId = channel.Id,
			Name = name,
			Description = description,
			Url = $"/uploads/{fileName}",
			IsPublic = isPublic,
			Date = DateTime.UtcNow,
			ViewersCount = 0,
			Rating = 0
		};

		var id = await videos.CreateAsync(video);
		if (id is null) return BadRequest();

		return RedirectToAction("Index", "Channel");
	}

	[HttpGet]
	public async Task<IActionResult> Edit(int id)
	{
		var video = await videos.GetByIdAsync(id);
		if (video is null) return NotFound();
		return View(video);
	}

	[HttpPost]
	public async Task<IActionResult> Edit(Video model)
	{
		var success = await videos.UpdateAsync(model);
		if (!success) return BadRequest();
		return RedirectToAction("Index", "Channel");
	}

	[HttpPost]
	public async Task<IActionResult> Delete(int id)
	{
		var success = await videos.DeleteAsync(id);
		if (!success) return BadRequest();
		return RedirectToAction("Index", "Channel");
	}

	[HttpGet]
	public async Task<IActionResult> Watch(int id)
	{
		Mark? userMark = null;
		int userId = -1;
		if (User.Identity?.IsAuthenticated ?? false)
		{
			userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
			userMark = await marks.GetByUserAndVideoAsync(userId, id);
		}

		var video = await videos.GetVideoForWatchAsync(id, userId);
		if (video is null) return NotFound();

		var allVideos = await videos.GetAllAsync();
		var otherVideos = allVideos.Where(v => v.Id != id).ToList();

		var videoComments = await comments.GetByVideoIdAsync(id);

		ViewBag.OtherVideos = otherVideos;
		ViewBag.Comments = videoComments;
		ViewBag.UserMark = userMark;

		return View(video);
	}





}
