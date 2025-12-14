namespace FrameCloud.Entities;

public class Video : Entity
{
	public int ChannelId { get; set; }
	public required string Name { get; set; }
	public required string Description { get; set; }
	public DateTime Date { get; set; }
	public required string Url { get; set; }
	public bool IsPublic { get; set; }
	public long ViewersCount { get; set; }
	public float Rating { get; set; }

	// Дополнительное поле для отображения
	public string? ChannelName { get; set; }
}

