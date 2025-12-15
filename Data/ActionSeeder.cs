using Dapper;
using Npgsql;

namespace FrameCloud.Data;

public class ActionSeeder(NpgsqlConnection connection) : Seeder(connection)
{
	public override async Task SeedAsync()
	{
		var actions = new[]
		{
					new { PublicId = 1, Name = "Просмотр видео" },
					new { PublicId = 2, Name = "Создание комментария" },
					new { PublicId = 3, Name = "Создание канала" },
					new { PublicId = 4, Name = "Создание видео" },
					new { PublicId = 5, Name = "Редактирование видео" },
					new { PublicId = 6, Name = "Удаление видео" },
					new { PublicId = 7, Name = "Редактирование комментария" },
					new { PublicId = 8, Name = "Удаление комментария" },
					new { PublicId = 9, Name = "Редактирование канала" },
					new { PublicId = 10, Name = "Удаление канала" },
					new { PublicId = 11, Name = "Подписка на канал" },
					new { PublicId = 12, Name = "Отписка от канала" },
					new { PublicId = 13, Name = "Регистрация пользователя" },
					new { PublicId = 14, Name = "Редактирование пользователя" },
					new { PublicId = 15, Name = "Удаление пользователя" },
					new { PublicId = 16, Name = "Вход в систему" },
					new { PublicId = 17, Name = "Выход из системы" },
					new { PublicId = 18, Name = "Постановка лайка" },
					new { PublicId = 19, Name = "Снятие лайка" },
					new { PublicId = 20, Name = "Добавление видео в избранное" },
					new { PublicId = 21, Name = "Удаление видео из избранного" }
			};

		foreach (var action in actions)
		{
			var sql = @"INSERT INTO ""Action""(""PublicId"", ""Name"") 
                        VALUES (@PublicId, @Name)
                        ON CONFLICT (""PublicId"") DO NOTHING;";
			await Connection.ExecuteAsync(sql, action);
		}
	}
}