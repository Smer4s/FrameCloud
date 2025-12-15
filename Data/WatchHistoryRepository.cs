using Dapper;
using FrameCloud.Entities;
using Npgsql;

public interface IWatchHistoryRepository
{
	Task<IEnumerable<WatchHistory>> GetByUserAsync(int userId);
}

public class WatchHistoryRepository : IWatchHistoryRepository
{
	private readonly string _cs;
	public WatchHistoryRepository(IConfiguration cfg) => _cs = cfg.GetConnectionString("Default")!;

	public async Task<IEnumerable<WatchHistory>> GetByUserAsync(int userId)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"
            SELECT wh.""Id"", wh.""UserId"", wh.""VideoId"", wh.""WatchedAt"",
                   v.""Name"" AS VideoName, v.""Description"", v.""Url""
            FROM ""WatchHistory"" wh
            JOIN ""Video"" v ON wh.""VideoId"" = v.""Id""
            WHERE wh.""UserId"" = @UserId
            ORDER BY wh.""WatchedAt"" DESC;";
		return await con.QueryAsync<WatchHistory>(sql, new { UserId = userId });
	}
}
