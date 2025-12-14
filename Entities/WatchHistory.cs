namespace FrameCloud.Entities;

public class WatchHistory : Entity
{
	public int UserId { get; set; }
	public int VideoId { get; set; }
	public DateTime WatchedAt { get; set; }
}
