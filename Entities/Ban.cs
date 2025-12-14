namespace FrameCloud.Entities
{
	public class Ban : Entity
	{
		public int UserId { get; set; }
		public required string Reason { get; set; }
		public DateTime BannedAt { get; set; }
		public DateTime? ExpiresAt { get; set; }
	}
}
