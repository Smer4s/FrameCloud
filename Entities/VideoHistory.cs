namespace FrameCloud.Entities;

public class VideoHistory : Entity
{
	public int VideoId { get; set; }
	public int ChangedBy { get; set; }
	public required string ChangeDescription { get; set; }
	public DateTime ChangedAt { get; set; }
}
