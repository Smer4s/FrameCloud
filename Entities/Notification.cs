namespace FrameCloud.Entities;

public class Notification : Entity
{
	public int UserId { get; set; }
	public required string Message { get; set; }
	public bool IsRead { get; set; }
	public DateTime CreatedAt { get; set; }
}
