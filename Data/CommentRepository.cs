using Dapper;
using Npgsql;
using FrameCloud.Entities;

namespace FrameCloud.Data;

public interface ICommentRepository
{
	Task<int?> AddAsync(Comment comment);
	Task<IEnumerable<Comment>> GetByVideoIdAsync(int videoId);
}

public class CommentRepository(IConfiguration cfg) : ICommentRepository
{
	readonly string _cs = cfg.GetConnectionString("Default")!;

	public async Task<int?> AddAsync(Comment comment)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"INSERT INTO ""Comment""(""VideoId"", ""UserId"", ""Text"", ""Date"") 
                    VALUES (@VideoId, @UserId, @Text, @Date) RETURNING ""Id"";";
		return await con.ExecuteScalarAsync<int?>(sql, comment);
	}

	public async Task<IEnumerable<Comment>> GetByVideoIdAsync(int videoId)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"
        SELECT c.""Id"", c.""VideoId"", c.""UserId"", c.""Text"", c.""Date"", u.""Login"" AS UserLogin
        FROM ""Comment"" c
        JOIN ""User"" u ON c.""UserId"" = u.""Id""
        WHERE c.""VideoId"" = @VideoId
        ORDER BY c.""Date"" DESC;";
		return await con.QueryAsync<Comment>(sql, new { VideoId = videoId });
	}

}
