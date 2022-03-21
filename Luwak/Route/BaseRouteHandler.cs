using WebServerProgram.Http;
using System.Text;


namespace WebServerProgram;

public class BaseRouteHandler : RouteHandler {
    public override void OnPost(HttpRequest request, HttpResponse response) {
        CopyRequest(request, response);
    }
    public override void OnGet(HttpRequest request, HttpResponse response) {
        CopyRequest(request, response);
    }
    public override void OnPut(HttpRequest request, HttpResponse response) {
        CopyRequest(request, response);
    }
    public override void OnDelete(HttpRequest request, HttpResponse response) {
        CopyRequest(request, response);
    }

    private void CopyRequest(HttpRequest req, HttpResponse res) {
        StringBuilder hb = new StringBuilder();
        foreach (string headerKey in req.headers.Keys)
        {
            hb.Append($"<li><b>{headerKey}</b> {req.headers.Get(headerKey)}</li>");
        }

        string bodyText = "";
        if (req.content.Length > 0) {
            bodyText = "<h3>Body</h3>";
        }

        StringBuilder rb = new StringBuilder();
        rb.Append(@$"<html>
            <body style=""padding: 16px;"">
                <h1>Your Request</h1>
                <h3>Startline</h3>
                <ul>
                    <li><b>Method</b> {req.method}</li>
                    <li><b>Path</b> {req.path}</li>
                    <li><b>Protocol Version</b> {req.protocolVersion}</li>
                </ul>
                <h3>Headers</h3>
                <ul>{hb.ToString()}</ul>
                {bodyText}
                <pre style=""word-break: break-all;"">{Encoding.UTF8.GetString(req.content)}</pre>
            </body>
        </html>");
        var response = rb.ToString().Trim();
        res.content = Encoding.UTF8.GetBytes(response);
        res.headers.AddHeader("Content-Type", "text/html;");
        res.headers.AddHeader("Content-Length", res.content.Length.ToString());
    }

}