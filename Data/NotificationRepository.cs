using Dapper;
using FrameCloud.Entities;
using Npgsql;

namespace FrameCloud.Data;

public interface INotificationRepository
{
	Task<IEnumerable<Notification>> GetByUserAsync(int userId);
	Task<bool> MarkAsReadAsync(int id, int userId);
}

public class NotificationRepository : INotificationRepository
{
	private readonly string _cs;
	public NotificationRepository(IConfiguration cfg) => _cs = cfg.GetConnectionString("Default")!;

	public async Task<IEnumerable<Notification>> GetByUserAsync(int userId)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"SELECT ""Id"", ""UserId"", ""Message"", ""IsRead"", ""CreatedAt""
                    FROM ""Notification""
                    WHERE ""UserId"" = @UserId
                    ORDER BY ""CreatedAt"" DESC;";
		return await con.QueryAsync<Notification>(sql, new { UserId = userId });
	}

	public async Task<bool> MarkAsReadAsync(int id, int userId)
	{
		await using var con = new NpgsqlConnection(_cs);
		var sql = @"UPDATE ""Notification"" 
                    SET ""IsRead"" = TRUE
                    WHERE ""Id"" = @Id AND ""UserId"" = @UserId;";
		var rows = await con.ExecuteAsync(sql, new { Id = id, UserId = userId });
		return rows > 0;
	}
}