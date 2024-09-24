namespace Lab_1;

public static class AppBuilderExtensions
{
    public static IApplicationBuilder UseRequestHandlerMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestHandlerMiddleware>();
    }
}
