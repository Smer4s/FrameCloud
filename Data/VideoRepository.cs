using Dapper;
using Npgsql;
using FrameCloud.Entities;

namespace FrameCloud.Data;

public interface IVideoRepository
{
	Task<int?> CreateAsync(Video video);
	Task<Video?> GetByIdAsync(int id);
	Task<bool> UpdateAsync(Video video);
	Task<bool> DeleteAsync(int id);
	Task<IEnumerable<Video>> GetAllAsync();
	Task<IEnumerable<Video>> GetByChannelAsync(int channelId, int? currentUserId = null);
	Task<Video?> GetVideoForWatchAsync(int id);
}

public class VideoRepository : IVideoRepository
{
	readonly string _cs;
	public VideoRepository(IConfiguration cfg) => _cs = cfg.GetConnectionString("Default")!;

	public async Task<int?> CreateAsync(Video video)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"INSERT INTO ""Video""(""ChannelId"", ""Name"", ""Description"", ""Date"", ""Url"", ""IsPublic"", ""ViewersCount"", ""Rating"") 
                    VALUES (@ChannelId, @Name, @Description, @Date, @Url, @IsPublic, @ViewersCount, @Rating)
                    RETURNING ""Id"";";
		return await con.ExecuteScalarAsync<int?>(sql, video);
	}

	public async Task<IEnumerable<Video>> GetByChannelAsync(int channelId, int? currentUserId = null)
	{
		await using var con = new NpgsqlConnection(_cs);

		var sql = @"
        SELECT v.""Id"", v.""ChannelId"", v.""Name"", v.""Description"", v.""Date"",
               v.""Url"", v.""IsPublic"", v.""ViewersCount"", v.""Rating"",
               c.""Name"" AS ChannelName
        FROM ""Video"" v
        JOIN ""Channel"" c ON v.""ChannelId"" = c.""Id""
        WHERE v.""ChannelId"" = @ChannelId
          AND (@CurrentUserId = c.""OwnerId"" OR v.""IsPublic"" = TRUE)
        ORDER BY v.""Date"" DESC;";

		return await con.QueryAsync<Video>(sql, new { ChannelId = channelId, CurrentUserId = currentUserId });
	}



	public async Task<Video?> GetByIdAsync(int id)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"
        SELECT 
            ""Id"",
            ""ChannelId"",
            ""Name"",
            ""Description"",
            ""Date"",
            ""Url"",
            ""IsPublic"",
            ""ViewersCount"",
            ""Rating""
        FROM ""Video""
        WHERE ""Id"" = @Id;";

		return await con.QuerySingleOrDefaultAsync<Video>(sql, new { Id = id });
	}


	public async Task<bool> UpdateAsync(Video video)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"UPDATE ""Video"" 
                SET ""Name""=@Name, ""Description""=@Description, ""IsPublic""=@IsPublic 
                WHERE ""Id""=@Id;";
		var rows = await con.ExecuteAsync(sql, video);
		return rows > 0;
	}

	public async Task<IEnumerable<Video>> GetAllAsync()
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"
        SELECT v.""Id"", v.""ChannelId"", v.""Name"", v.""Description"", v.""Date"",
               v.""Url"", v.""IsPublic"", v.""ViewersCount"", v.""Rating"",
               c.""Name"" AS ChannelName
        FROM ""Video"" v
        JOIN ""Channel"" c ON v.""ChannelId"" = c.""Id""
        WHERE v.""IsPublic"" = TRUE
        ORDER BY v.""Date"" DESC;";
		return await con.QueryAsync<Video>(sql);
	}

	public async Task<Video?> GetVideoForWatchAsync(int id)
	{
		await using var con = new NpgsqlConnection(_cs);

		var procSql = @"CALL view_video(@Id);";
		await con.ExecuteAsync(procSql, new { Id = id });

		var sql = @"
        SELECT ""Id"", ""ChannelId"", ""Name"", ""Description"", ""Date"",
               ""Url"", ""IsPublic"", ""ViewersCount"", ""Rating""
        FROM ""Video""
        WHERE ""Id"" = @Id;";

		return await con.QuerySingleOrDefaultAsync<Video>(sql, new { Id = id });
	}


	public async Task<bool> DeleteAsync(int id)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"DELETE FROM ""Video"" WHERE ""Id""=@Id;";
		var rows = await con.ExecuteAsync(sql, new { Id = id });
		return rows > 0;
	}
}
