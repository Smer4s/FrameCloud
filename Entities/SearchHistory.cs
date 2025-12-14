namespace FrameCloud.Entities
{
	public class SearchHistory : Entity
	{
		public int UserId { get; set; }
		public required string Query { get; set; }
		public DateTime SearchedAt { get; set; }
	}
}
