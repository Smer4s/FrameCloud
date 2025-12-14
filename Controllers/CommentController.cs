using FrameCloud.Data;
using FrameCloud.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers;

public class CommentController(ICommentRepository comments) : Controller
{
	[Authorize]
	[HttpPost]
	public async Task<IActionResult> Add(int videoId, string text)
	{
		var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

		var comment = new Comment
		{
			VideoId = videoId,
			UserId = userId,
			Text = text,
			Date = DateTime.UtcNow
		};

		await comments.AddAsync(comment);
		return RedirectToAction("Watch", "Video", new { id = videoId });
	}
}
