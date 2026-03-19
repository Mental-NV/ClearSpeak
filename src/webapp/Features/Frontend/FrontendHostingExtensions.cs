namespace ClearSpeak.WebApp.Features.Frontend;

public static class FrontendHostingExtensions
{
    public static WebApplication UseClearSpeakFrontend(this WebApplication app)
    {
        app.UseDefaultFiles();
        app.UseStaticFiles();
        return app;
    }

    public static WebApplication MapFrontendFallback(this WebApplication app)
    {
        var indexPath = Path.Combine(app.Environment.WebRootPath ?? string.Empty, "index.html");
        if (File.Exists(indexPath))
        {
            app.MapFallbackToFile("index.html");
        }
        else
        {
            app.MapGet("/", () => Results.Text("ClearSpeak frontend is not built yet. Run `npm --prefix src/clientspa run build` first.", "text/plain"));
        }

        return app;
    }
}
