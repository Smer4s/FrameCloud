using System.ComponentModel.DataAnnotations;

namespace FrameCloud.Models;

public class BanViewModel
{
	public int UserId { get; set; }

	[Required(ErrorMessage = "Причина обязательна")]
	public string Reason { get; set; } = string.Empty;
}
