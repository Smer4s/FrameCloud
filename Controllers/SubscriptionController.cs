using FrameCloud.Data;
using FrameCloud.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrameCloud.Controllers;

[Authorize]
public class SubscriptionController(ISubscriptionRepository subs) : Controller
{
	[HttpPost]
	public async Task<IActionResult> Subscribe(int channelId)
	{
		var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
		await subs.AddAsync(new Subscription { ChannelId = channelId, UserId = userId });
		return RedirectToAction("Index", "Channel", new { id = channelId });
	}

	[HttpPost]
	public async Task<IActionResult> Unsubscribe(int channelId)
	{
		var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
		await subs.RemoveAsync(channelId, userId);
		return RedirectToAction("Index", "Channel", new { id = channelId });
	}
}