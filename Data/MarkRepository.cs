using Dapper;
using Npgsql;
using FrameCloud.Entities;

namespace FrameCloud.Data;

public interface IMarkRepository
{
	Task<int?> AddOrUpdateAsync(Mark mark);
	Task<Mark?> GetByUserAndVideoAsync(int userId, int videoId);
}

public class MarkRepository : IMarkRepository
{
	readonly string _cs;
	public MarkRepository(IConfiguration cfg) => _cs = cfg.GetConnectionString("Default")!;

	public async Task<int?> AddOrUpdateAsync(Mark mark)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"
            INSERT INTO ""Mark""(""VideoId"", ""UserId"", ""IsLike"")
            VALUES (@VideoId, @UserId, @IsLike)
            ON CONFLICT (""VideoId"", ""UserId"")
            DO UPDATE SET ""IsLike"" = EXCLUDED.""IsLike""
            RETURNING ""Id"";";
		return await con.ExecuteScalarAsync<int?>(sql, mark);
	}

	public async Task<Mark?> GetByUserAndVideoAsync(int userId, int videoId)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"SELECT * FROM ""Mark"" WHERE ""VideoId""=@VideoId AND ""UserId""=@UserId;";
		return await con.QuerySingleOrDefaultAsync<Mark>(sql, new { VideoId = videoId, UserId = userId });
	}

}
