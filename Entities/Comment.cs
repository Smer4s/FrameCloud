namespace FrameCloud.Entities;

public class Comment : Entity
{
	public int VideoId { get; set; }
	public int UserId { get; set; }
	public required string Text { get; set; }
	public DateTime Date { get; set; }
}
