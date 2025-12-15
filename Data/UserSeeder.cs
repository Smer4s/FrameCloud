using Dapper;
using FrameCloud.Auth;
using Npgsql;

namespace FrameCloud.Data;

public class UserSeeder(NpgsqlConnection connection) : Seeder(connection)
{
	public override async Task SeedAsync()
	{
		var roleId = await Connection.ExecuteScalarAsync<int>(
				@"SELECT ""Id"" FROM ""Role"" WHERE ""Name"" = 'Администратор';"
		);

		var userExists = await Connection.ExecuteScalarAsync<int>(
				@"SELECT COUNT(*) FROM ""User"" WHERE ""Login"" = 'admin';"
		);

		if (userExists == 0)
		{
			var userSql = @"INSERT INTO ""User""(""Login"", ""Password"", ""RoleId"") 
                            VALUES ('admin', @Password, @RoleId);";

			var passwordHash = PasswordHasher.Hash("admin");
			await Connection.ExecuteAsync(userSql, new
			{
				Password = passwordHash,
				RoleId = roleId
			});
		}
	}
}

