namespace FrameCloud.Entities;

public class WatchHistory : Entity
{
	public int UserId { get; set; }
	public int VideoId { get; set; }
	public DateTime WatchedAt { get; set; }

	public string VideoName { get; set; } = string.Empty;
	public string? VideoDescription { get; set; }
	public string VideoUrl { get; set; } = string.Empty;
}
