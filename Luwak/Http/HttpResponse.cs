using System.Net;
using System.Text;

namespace WebServerProgram.Http;

public class HttpResponse
{
    private const string PROTOCOL_VERSION = "HTTP/1.1";
    public HttpHeader Headers = new HttpHeader();
    public int status = (int) HttpStatusCode.OK;

    public string _body = HttpStatusCode.OK.ToString();

    public string Body
    {
        set { _body = value; }
        get { return _body; }
    }

    public string PrintRequest(HttpRequest req)
    {
        StringBuilder hb = new StringBuilder();
        foreach (string headerKey in req.headers.getDict().Keys)
        {
            hb.Append($"<li><b>{headerKey}</b> {req.headers.Get(headerKey)}</li>");
        }

        StringBuilder rb = new StringBuilder();
        rb.Append("HTTP/1.1 200 OK\n");
        rb.Append("Content-Type: text/html;\n\n");
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
                <h3>Body</h3>
            </body>
        </html>");
        return rb.ToString().Trim();
    }

    public string GetTestResponse()
    {
        return "HTTP/1.1 200 OK\r\nContent-Type: text/html;\r\n\r\n<html><body><h1>Hi</h1></body></html>";
    }
}