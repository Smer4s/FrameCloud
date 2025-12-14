namespace FrameCloud.Entities;

public class Channel : Entity
{
	public int OwnerId { get; set; }
	public required string Name { get; set; }
	public required string Description { get; set; }
	public long SubscribersCount { get; set; }
}
