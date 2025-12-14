namespace FrameCloud.Entities;

public class User : Entity
{
	public int RoleId { get; set; }
	public required string Login { get; set; }
	public required string Password { get; set; }
}
