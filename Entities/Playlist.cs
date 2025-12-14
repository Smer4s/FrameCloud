namespace FrameCloud.Entities;

public class Playlist : Entity
{
	public int UserId { get; set; }
	public required string Name { get; set; }
	public int VideoCount { get; set; }
	public int Duration { get; set; }
}
