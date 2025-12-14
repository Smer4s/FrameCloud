using Npgsql;

namespace FrameCloud.Data;

public abstract class Seeder
{
	protected readonly NpgsqlConnection Connection;
	protected Seeder(NpgsqlConnection connection) => Connection = connection;
	public abstract Task SeedAsync();
}
