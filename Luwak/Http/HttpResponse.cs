using System.Net;

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

    public string GetTestResponse()
    {
        return "HTTP/1.1 200 OK\r\nContent-Type: text/html;\r\n\r\n<html><body><h1>Hi</h1></body></html>";
    }
}