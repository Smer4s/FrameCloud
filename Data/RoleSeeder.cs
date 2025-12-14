using Dapper;
using Npgsql;
using Serilog;

namespace FrameCloud.Data;

public class RoleSeeder(NpgsqlConnection connection) : Seeder(connection)
{
	public override async Task SeedAsync()
	{
		var sql = @"INSERT INTO ""Role""(""Id"", ""Name"") VALUES (@Id, @Name)
                    ON CONFLICT (""Id"") DO NOTHING;";

		await Connection.ExecuteAsync(sql, new { Id = 1, Name = "Пользователь" });
		await Connection.ExecuteAsync(sql, new { Id = 2, Name = "Администратор" });

		Log.Information("Roles seeded");
	}
}
