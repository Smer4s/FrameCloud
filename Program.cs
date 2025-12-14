using FrameCloud.Extensions;
using Serilog;

namespace FrameCloud;

public class Program
{
	public static async Task Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Information()
			.WriteTo.Console()
			.CreateLogger();

		var builder = WebApplication.CreateBuilder(args);

		builder.Host.UseSerilog();

		builder.Services.AddControllersWithViews();

		var app = builder.Build();

		await app.InitializeDatabase();

		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Home/Error");
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthorization();

		app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}");

		app.Run();
	}
}
