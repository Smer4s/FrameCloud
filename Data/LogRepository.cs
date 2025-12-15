using Dapper;
using Npgsql;

namespace FrameCloud.Data;

public interface ILogRepository
{
	Task<IEnumerable<LogView>> GetAllAsync();
}

public class LogRepository : ILogRepository
{
	private readonly string _cs;
	public LogRepository(IConfiguration cfg) => _cs = cfg.GetConnectionString("Default")!;

	public async Task<IEnumerable<LogView>> GetAllAsync()
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"
            SELECT l.""Id"", l.""Date"",
                   u.""Login"" AS UserLogin,
                   a.""Name"" AS ActionName
            FROM ""Log"" l
            JOIN ""User"" u ON l.""UserId"" = u.""Id""
            JOIN ""Action"" a ON l.""ActionId"" = a.""Id""
            ORDER BY l.""Date"" DESC;";
		return await con.QueryAsync<LogView>(sql);
	}
}

public class LogView
{
	public int Id { get; set; }
	public string UserLogin { get; set; } = string.Empty;
	public string ActionName { get; set; } = string.Empty;
	public DateTime Date { get; set; }
}