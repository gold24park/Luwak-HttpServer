using System.Net;
using System.Text;

namespace WebServerProgram.Http;

public class HttpResponse
{
    public HttpHeader Headers = new HttpHeader();
    private const string PROTOCOL_VERSION = "HTTP/1.1";
    public int status = (int) HttpStatusCode.OK;

    public string PrintRequest(HttpRequest req)
    {
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
        rb.Append($"{PROTOCOL_VERSION} 200 OK\r\n");
        rb.Append("Content-Type: text/html;\r\n\r\n");
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
        return rb.ToString().Trim();
    }
}
