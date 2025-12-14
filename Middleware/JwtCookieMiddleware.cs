namespace FrameCloud.Middleware;

public class JwtCookieMiddleware(RequestDelegate next)
{
	public async Task InvokeAsync(HttpContext context)
	{
		if (context.Request.Cookies.TryGetValue("AuthToken", out var token))
		{
			context.Request.Headers.Append("Authorization", $"Bearer {token}");
		}

		await next(context);
	}
}

public static class JwtCookieMiddlewareExtensions
{
	public static IApplicationBuilder UseJwtCookieAuth(this IApplicationBuilder builder)
	{
		return builder.UseMiddleware<JwtCookieMiddleware>();
	}
}
