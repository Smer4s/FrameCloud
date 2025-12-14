namespace FrameCloud.Entities;

public class Session : Entity
{
	public int UserId { get; set; }
	public required string Token { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime ExpiresAt { get; set; }
}
