namespace FrameCloud.Entities;

public class Favorite
{
	public int UserId { get; set; }
	public int VideoId { get; set; }
	public DateTime AddedAt { get; set; }
}
