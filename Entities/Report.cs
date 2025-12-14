namespace FrameCloud.Entities;

public class Report : Entity
{
	public int ReporterId { get; set; }
	public int? VideoId { get; set; }
	public int? CommentId { get; set; }
	public required string Reason { get; set; }
	public DateTime CreatedAt { get; set; }
}
