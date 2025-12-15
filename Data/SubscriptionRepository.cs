using Dapper;
using FrameCloud.Entities;
using Npgsql;

namespace FrameCloud.Data;

public interface ISubscriptionRepository
{
	Task<bool> AddAsync(Subscription sub);
	Task<bool> RemoveAsync(int channelId, int userId);
	Task<bool> ExistsAsync(int channelId, int userId);
}

public class SubscriptionRepository : ISubscriptionRepository
{
	readonly string _cs;
	public SubscriptionRepository(IConfiguration cfg) => _cs = cfg.GetConnectionString("Default")!;

	public async Task<bool> AddAsync(Subscription sub)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"INSERT INTO ""Subscription""(""ChannelId"", ""UserId"")
                    VALUES (@ChannelId, @UserId)
                    ON CONFLICT DO NOTHING;";
		var rows = await con.ExecuteAsync(sql, sub);
		return rows > 0;
	}

	public async Task<bool> RemoveAsync(int channelId, int userId)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"DELETE FROM ""Subscription"" WHERE ""ChannelId""=@ChannelId AND ""UserId""=@UserId;";
		var rows = await con.ExecuteAsync(sql, new { ChannelId = channelId, UserId = userId });
		return rows > 0;
	}

	public async Task<bool> ExistsAsync(int channelId, int userId)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"SELECT 1 FROM ""Subscription"" WHERE ""ChannelId""=@ChannelId AND ""UserId""=@UserId;";
		var result = await con.ExecuteScalarAsync<int?>(sql, new { ChannelId = channelId, UserId = userId });
		return result.HasValue;
	}
}
