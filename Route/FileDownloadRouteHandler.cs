using WebServerProgram;
using WebServerProgram.Http;
using System.Net;
using System.Text;

class FileDownloadRouteHandler : RouteHandler {
    public override void OnGet(HttpRequest request, HttpResponse response)
    {
        string path = Path.Join(Directory.GetCurrentDirectory(), "static");
        var fileName = WebUtility.UrlDecode(Path.GetFileName(request.path));
        var filePath = Path.Join(path, fileName);

        byte[] bytes = File.ReadAllBytes(filePath);

        Console.WriteLine($"no? {bytes.Length}");

        response.headers.AddHeader("Content-Type", Util.GetMimeType(filePath));
        response.headers.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
        response.headers.AddHeader("Content-Length", bytes.Length.ToString());
        response.content = bytes;
}
}