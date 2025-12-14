namespace FrameCloud.Entities;

public class Mark : Entity
{
	public int VideoId { get; set; }
	public int UserId { get; set; }
	public bool IsLike { get; set; }
}
