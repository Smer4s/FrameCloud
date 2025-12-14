using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FrameCloud.Auth;

public interface ITokenService
{
	string CreateToken(int userId, int roleId, string login);
}

public class TokenService(
	string issuer,
	string audience,
	SymmetricSecurityKey key,
	int expiresMinutes) : ITokenService
{
	public string CreateToken(int userId, int roleId, string login)
	{
		var claims = new[]
		{
						new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
						new Claim(ClaimTypes.Name, login),
						new Claim(ClaimTypes.Role, roleId.ToString()),
						new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
				};

		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		var token = new JwtSecurityToken(issuer, audience, claims,
				expires: DateTime.UtcNow.AddMinutes(expiresMinutes), signingCredentials: creds);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}
