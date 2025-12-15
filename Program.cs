using FrameCloud.Auth;
using FrameCloud.Data;
using FrameCloud.Extensions;
using FrameCloud.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace FrameCloud;

public class Program
{
	public static async Task Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();

		var builder = WebApplication.CreateBuilder(args);
		builder.Host.UseSerilog();

		var jwtSection = builder.Configuration.GetSection("Jwt");
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));

		builder.Services.AddControllersWithViews();
		builder.Services.AddSingleton<IUserRepository, UserRepository>();
		builder.Services.AddSingleton<IVideoRepository, VideoRepository>();
		builder.Services.AddSingleton<ICommentRepository, CommentRepository>();
		builder.Services.AddSingleton<IMarkRepository, MarkRepository>();
		builder.Services.AddSingleton<ISubscriptionRepository, SubscriptionRepository>();
		builder.Services.AddSingleton<INotificationRepository, NotificationRepository>();
		builder.Services.AddSingleton<IChannelRepository, ChannelRepository>();
		builder.Services.AddSingleton<ILogRepository, LogRepository>();
		builder.Services.AddSingleton<IWatchHistoryRepository, WatchHistoryRepository>();
		builder.Services.AddSingleton<ITokenService>(sp => new TokenService(
				jwtSection["Issuer"]!, jwtSection["Audience"]!, key, int.Parse(jwtSection["ExpiresMinutes"]!)
		));

		builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = jwtSection["Issuer"],
						ValidAudience = jwtSection["Audience"],
						IssuerSigningKey = key
					};
				});

		var app = builder.Build();

		await app.InitializeDatabase();

		app.Use(async (context, next) =>
		{
			context.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpMaxRequestBodySizeFeature>()!
					.MaxRequestBodySize = 1000_000_000_000;
			await next();
		});


		app.UseJwtCookieAuth();
		app.UseMiddleware<ActionLoggingMiddleware>();
		app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseStaticFiles(new StaticFileOptions
		{
			FileProvider = new PhysicalFileProvider(
						Path.Combine(builder.Environment.ContentRootPath, "uploads")),
			RequestPath = "/uploads"
		});

		app.UseRouting();
		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

		app.Run();
	}
}
