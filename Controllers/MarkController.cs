using FrameCloud.Data;
using FrameCloud.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers;

[Authorize]
public class MarkController(IMarkRepository marks) : Controller
{
	[HttpPost]
	public async Task<IActionResult> Like(int videoId)
	{
		var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

		var mark = new Mark
		{
			VideoId = videoId,
			UserId = userId,
			IsLike = true
		};

		await marks.AddOrUpdateAsync(mark);
		return RedirectToAction("Watch", "Video", new { id = videoId });
	}

	[HttpPost]
	public async Task<IActionResult> Dislike(int videoId)
	{
		var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

		var mark = new Mark
		{
			VideoId = videoId,
			UserId = userId,
			IsLike = false
		};

		await marks.AddOrUpdateAsync(mark);
		return RedirectToAction("Watch", "Video", new { id = videoId });
	}
}
