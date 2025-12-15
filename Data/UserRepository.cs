using Dapper;
using FrameCloud.Entities;
using Npgsql;

namespace FrameCloud.Data;

public interface IUserRepository
{
	Task BanUserAsync(int userId, string reason, DateTime? expiresAt = null);
	Task<int?> CreateUserAsync(int roleId, string login, string passwordHash);
	Task<Ban?> GetActiveBanAsync(int userId);
	Task<IEnumerable<UserView>> GetAllAsync();
	Task<User?> GetByLoginAsync(string login);
	Task<IEnumerable<LogView>> GetLogsByUserAsync(int userId);
	Task UnbanUserAsync(int userId);
}

public class UserRepository(IConfiguration cfg) : IUserRepository
{
	readonly string _cs = cfg.GetConnectionString("Default")!;

	public async Task<int?> CreateUserAsync(int roleId, string login, string passwordHash)
	{
		await using var connection = new NpgsqlConnection(_cs);
		var sql = @"INSERT INTO ""User""(""RoleId"", ""Login"", ""Password"") 
                    VALUES (@RoleId, @Login, @Password) 
                    RETURNING ""Id"";";

		return await connection.ExecuteScalarAsync<int?>(sql, new
		{
			RoleId = roleId,
			Login = login,
			Password = passwordHash
		});
	}

	public async Task<User?> GetByLoginAsync(string login)
	{
		await using var connection = new NpgsqlConnection(_cs);
		var sql = @"SELECT ""Id"", ""RoleId"", ""Login"", ""Password"" FROM ""User"" WHERE ""Login"" = @Login;";
		return await connection.QuerySingleOrDefaultAsync<User>(sql, new
		{
			Login = login
		});
	}

	public async Task<IEnumerable<UserView>> GetAllAsync()
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"
        SELECT u.""Id"", u.""Login"",
               CASE WHEN EXISTS (
                   SELECT 1 FROM ""Channel"" c WHERE c.""OwnerId"" = u.""Id""
               ) THEN TRUE ELSE FALSE END AS HasChannel,
               CASE WHEN EXISTS (
                   SELECT 1 FROM ""Ban"" b WHERE b.""UserId"" = u.""Id""
               ) THEN TRUE ELSE FALSE END AS IsBanned
        FROM ""User"" u
        WHERE u.""Login"" <> 'admin'
          AND u.""RoleId"" <> 2;";
		return await con.QueryAsync<UserView>(sql);
	}



	public async Task<IEnumerable<LogView>> GetLogsByUserAsync(int userId)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"
            SELECT l.""Id"", l.""Date"", a.""Name"" AS ActionName, u.""Login"" AS UserLogin
            FROM ""Log"" l
            JOIN ""Action"" a ON l.""ActionId"" = a.""Id""
            JOIN ""User"" u ON l.""UserId"" = u.""Id""
            WHERE l.""UserId"" = @UserId
            ORDER BY l.""Date"" DESC;";
		return await con.QueryAsync<LogView>(sql, new { UserId = userId });
	}

	public async Task BanUserAsync(int userId, string reason, DateTime? expiresAt = null)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"INSERT INTO ""Ban""(""UserId"", ""Reason"", ""BannedAt"", ""ExpiresAt"")
                    VALUES (@UserId, @Reason, NOW(), @ExpiresAt);";
		await con.ExecuteAsync(sql, new { UserId = userId, Reason = reason, ExpiresAt = expiresAt });
	}

	public async Task UnbanUserAsync(int userId)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"DELETE FROM ""Ban"" WHERE ""UserId"" = @UserId;";
		await con.ExecuteAsync(sql, new { UserId = userId });
	}

	public async Task<Ban?> GetActiveBanAsync(int userId)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"
        SELECT ""UserId"", ""Reason"", ""BannedAt"", ""ExpiresAt""
        FROM ""Ban""
        WHERE ""UserId"" = @UserId
          AND (""ExpiresAt"" IS NULL OR ""ExpiresAt"" > NOW())
        LIMIT 1;";
		return await con.QueryFirstOrDefaultAsync<Ban>(sql, new { UserId = userId });
	}

}



public class UserView
{
	public int Id { get; set; }
	public string Login { get; set; } = string.Empty;
	public bool HasChannel { get; set; }
	public bool IsBanned { get; set; }
}
