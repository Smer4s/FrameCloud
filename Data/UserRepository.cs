using Dapper;
using FrameCloud.Entities;
using Npgsql;

namespace FrameCloud.Data;

public interface IUserRepository
{
	Task<int?> CreateUserAsync(int roleId, string login, string passwordHash);
	Task<User?> GetByLoginAsync(string login);
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
}
