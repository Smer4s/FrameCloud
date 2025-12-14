namespace FrameCloud.Auth;

public record RegisterRequest(string Login, string Password);
public record LoginRequest(string Login, string Password);
public record AuthResponse(string Token, int UserId, int RoleId, string Login);

public static class PasswordHasher
{
	public static string Hash(string password)
	{
		using var sha = System.Security.Cryptography.SHA256.Create();
		var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

		return Convert.ToHexString(bytes);
	}
}
