using System.Security.Claims;
using Dapper;
using Npgsql;

namespace FrameCloud.Middleware;

public class ActionLoggingMiddleware(RequestDelegate next, IConfiguration cfg)
{
	private readonly string _connectionString = cfg.GetConnectionString("Default")!;

	public async Task InvokeAsync(HttpContext context)
	{
		await next(context);

		var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
		if (userIdClaim == null) return;

		var userId = int.Parse(userIdClaim.Value);

		int? actionPublicId = null;

		var path = context.Request.Path.Value?.ToLower() ?? "";
		var method = context.Request.Method.ToUpper();

		if (path.StartsWith("/video/watch"))
			actionPublicId = 1; // просмотр видео
		else if (path.StartsWith("/comment/add") && method == "POST")
			actionPublicId = 2; // добавление комментария
		else if (path.StartsWith("/channel/create") && method == "POST")
			actionPublicId = 3; // создание канала
		else if (path.StartsWith("/video/upload") && method == "POST")
			actionPublicId = 4; // загрузка видео
		else if (path.StartsWith("/video/edit") && method == "POST")
			actionPublicId = 5; // редактирование видео
		else if (path.StartsWith("/video/delete") && method == "POST")
			actionPublicId = 6; // удаление видео
		else if (path.StartsWith("/channel/edit") && method == "POST")
			actionPublicId = 9; // редактирование канала
		else if (path.StartsWith("/channel/delete") && method == "POST")
			actionPublicId = 10; // удаление канала
		else if (path.StartsWith("/channel/subscribe") && method == "POST")
			actionPublicId = 11; // подписка
		else if (path.StartsWith("/channel/unsubscribe") && method == "POST")
			actionPublicId = 12; // отписка
		else if (path.StartsWith("/auth/login") && method == "POST")
			actionPublicId = 16; // вход
		else if (path.StartsWith("/auth/logout"))
			actionPublicId = 17; // выход
		else if (path.StartsWith("/mark/like") && method == "POST")
			actionPublicId = 18; // лайк
		else if (path.StartsWith("/mark/dislike") && method == "POST")
			actionPublicId = 19; // дизлайк
		else if (path.StartsWith("/video/favorite/add") && method == "POST")
			actionPublicId = 20; // добавление в избранное
		else if (path.StartsWith("/video/favorite/remove") && method == "POST")
			actionPublicId = 21; // удаление из избранного


		if (actionPublicId.HasValue)
		{
			await using var con = new NpgsqlConnection(_connectionString);
			await con.ExecuteAsync(@"CALL log_action(@ActionPublicId, @UserId);",
					new { ActionPublicId = actionPublicId.Value, UserId = userId });
		}
	}
}
