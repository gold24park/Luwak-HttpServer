using System.Text;
using WebServerProgram.Http;

namespace WebServerProgram;

class FileIndexRouteHandler : RouteHandler {
    public override void OnGet(HttpRequest request, HttpResponse response) {
        string path = Path.Join(Directory.GetCurrentDirectory(), "static");
        
        var fileNames = new List<String>();
        foreach (var file in Directory.GetFiles(path)) {
            fileNames.Add(Path.GetFileName(file));
        }

        var listHtml = new StringBuilder();
        foreach (var fileName in fileNames) {
            listHtml.Append(@$"<li><a href=""/download/{fileName}"">{fileName}</a></li>");
        }
        
    
        response.content = Encoding.UTF8.GetBytes(@$"
        <html>
            <head>
                <meta charset=""UTF-8"">
            </head>
            <body style=""padding: 16px"">
                <h1>Files</h1>
                <ul>
                    {listHtml}
                </ul>
            </body>
        </html>
        ");
        response.headers.AddHeader("Content-Type", "text/html;");
        response.headers.AddHeader("Content-Length", response.content.Length.ToString());
    }
}