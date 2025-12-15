using Dapper;
using Npgsql;
using Serilog;
using FrameCloud.Data;

namespace FrameCloud.Extensions;

public static class WebApplicationExtensions
{
	public static async Task InitializeDatabase(this WebApplication app)
	{
		var connectionString = "Host=localhost;Port=5432;Database=videohostingdb;Username=admin;Password=admin123";

		using var connection = new NpgsqlConnection(connectionString);
		connection.Open();

		var sqlScript = File.ReadAllText("schema.sql");
		await connection.ExecuteAsync(sqlScript);

		Log.Information("Database Scheme Created/Updated");

		var seeders = new List<Seeder>
			{
					new RoleSeeder(connection),
					new UserSeeder(connection)
			};

		foreach (var seeder in seeders)
			await seeder.SeedAsync();
	}
}