using Dapper;
using Npgsql;

namespace FrameCloud.Data;

public interface IChannelRepository
{
	Task<Channel?> GetByOwnerIdAsync(int ownerId);
	Task<int?> CreateAsync(Channel channel);
	Task<int> GetVideoCountAsync(int channelId);
	Task<Channel?> GetByIdAsync(int id);
	Task<IEnumerable<Channel>> SearchByNameAsync(string query);
}

public class ChannelRepository(IConfiguration cfg) : IChannelRepository
{
	readonly string _cs = cfg.GetConnectionString("Default")!;

	public async Task<Channel?> GetByOwnerIdAsync(int ownerId)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"SELECT ""Id"", ""OwnerId"", ""Name"", ""Description"", ""SubscribersCount"" 
                    FROM ""Channel"" WHERE ""OwnerId"" = @OwnerId;";
		return await con.QuerySingleOrDefaultAsync<Channel>(sql, new { OwnerId = ownerId });
	}

	public async Task<int?> CreateAsync(Channel channel)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"INSERT INTO ""Channel""(""OwnerId"", ""Name"", ""Description"", ""SubscribersCount"") 
                    VALUES (@OwnerId, @Name, @Description, @SubscribersCount) RETURNING ""Id"";";
		return await con.ExecuteScalarAsync<int?>(sql, channel);
	}

	public async Task<int> GetVideoCountAsync(int channelId)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"SELECT COUNT(*) FROM ""Video"" WHERE ""ChannelId"" = @ChannelId;";
		return await con.ExecuteScalarAsync<int>(sql, new { ChannelId = channelId });
	}

	public async Task<Channel?> GetByIdAsync(int id)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"
        SELECT
            ""Id"",
            ""OwnerId"",
            ""Name"",
            ""Description"",
            ""SubscribersCount""
        FROM ""Channel""
        WHERE ""Id"" = @Id;";

		return await con.QuerySingleOrDefaultAsync<Channel>(sql, new { Id = id });
	}

	public async Task<IEnumerable<Channel>> SearchByNameAsync(string query)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"SELECT ""Id"", ""Name"", ""Description"", ""SubscribersCount""
                    FROM ""Channel""
                    WHERE LOWER(""Name"") LIKE LOWER(@q);";
		return await con.QueryAsync<Channel>(sql, new { q = "%" + query + "%" });
	}
}
