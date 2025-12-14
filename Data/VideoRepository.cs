using Dapper;
using Npgsql;
using FrameCloud.Entities;

namespace FrameCloud.Data;

public interface IVideoRepository
{
	Task<int?> CreateAsync(Video video);
	Task<IEnumerable<Video>> GetByChannelAsync(int channelId);
	Task<Video?> GetByIdAsync(int id);
	Task<bool> UpdateAsync(Video video);
	Task<bool> DeleteAsync(int id);
	Task<IEnumerable<Video>> GetAllAsync();
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

	public async Task<IEnumerable<Video>> GetByChannelAsync(int channelId)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"SELECT * FROM ""Video"" WHERE ""ChannelId"" = @ChannelId;";
		return await con.QueryAsync<Video>(sql, new { ChannelId = channelId });
	}

	public async Task<Video?> GetByIdAsync(int id)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"SELECT * FROM ""Video"" WHERE ""Id"" = @Id;";
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
        ORDER BY v.""Date"" DESC;";
		return await con.QueryAsync<Video>(sql);
	}



	public async Task<bool> DeleteAsync(int id)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"DELETE FROM ""Video"" WHERE ""Id""=@Id;";
		var rows = await con.ExecuteAsync(sql, new { Id = id });
		return rows > 0;
	}
}
