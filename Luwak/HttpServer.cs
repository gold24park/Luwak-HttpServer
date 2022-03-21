using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using WebServerProgram.Http;

namespace WebServerProgram;

public class HttpServer : IHttpServer
{
    private TcpClient client;
    private NetworkStream stream;
    private List<RouteHandler> routeHandlers = new List<RouteHandler>();

    private BaseRouteHandler baseRouteHandler = new BaseRouteHandler();

    public HttpServer(List<RouteHandler> routeHandlers) {
        this.routeHandlers = routeHandlers;
    }

    public void OnConnect(TcpClient client) {
        this.client = client;
        this.stream = client.GetStream();
    }
    
    public void OnClose() {
        this.stream.Close();
        this.client.Close();
    }
    public async Task OnRequest(HttpRequest req, HttpResponse res)
    {
        try {
            Logger.Log($"HttpServer::OnRequest - [{req.method}] {req.path}");
            RouteHandler routeHandler = baseRouteHandler;

            foreach (RouteHandler rh in routeHandlers) {
                if (IsPathMatches(rh.path, req.path)) {
                    Logger.Log($"Found path {rh.path}");
                    routeHandler = rh;
                    break;
                }
            }

            routeHandler.Invoke(req, res);

            // Response를 구성한다.
            await WriteResponse(res);
        } catch (Exception exception) {
            this.OnError(exception);
        }
        
    }

    private bool IsPathMatches(string handlerPath, string requestPath) {        
        if (handlerPath == requestPath) return true;

        string regex = @"(?<path>.*)\?.*";

        foreach(Match match in Regex.Matches(requestPath, regex)) {
            requestPath = match.Groups["path"].Value;
        }

        var idx = requestPath.LastIndexOf("/");
        if (idx > 0) {
            requestPath = requestPath.Substring(0, idx);
        }
        return handlerPath == requestPath;
    }

    private async Task WriteResponse(HttpResponse res) {
        byte[] writeBuffer = Encoding.UTF8.GetBytes(BuildResponse(res));
        if (res.content.Length > 0) {
            // concat byte array
            byte[] wb = new byte[writeBuffer.Length + res.content.Length];
            Buffer.BlockCopy(writeBuffer, 0, wb, 0, writeBuffer.Length);
            Buffer.BlockCopy(res.content, 0, wb, writeBuffer.Length, res.content.Length);
            writeBuffer = wb;
        }
        await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length);
        stream.Close();
    }

    private string BuildResponse(HttpResponse res) {
        StringBuilder sb = new StringBuilder();
        sb.Append($"HTTP/1.1 {res.status} {res.statusMessage}\r\n");
        foreach (string headerKey in res.headers.Keys) {
            sb.Append($"{headerKey}: {res.headers[headerKey]}\r\n");
        }
        sb.Append("\r\n");
        return sb.ToString();
    }

    public async Task OnError(Exception exception)
    {
        // TODO: 에러났을때? 뭘할지...
        var res = new HttpResponse();
        if (exception is FileNotFoundException) {
            res.status = 404;
            res.statusMessage = "File Not Found";
        } 
        else if (exception is TimeoutException) {
            res.status = 408;
            res.statusMessage = "Request Timeout";
        }
        else {
            res.status = 500;
            res.statusMessage = "Internal server error";
        }
        res.content = Encoding.UTF8.GetBytes(@$"
        <html>
            <head>
                <meta charset=""UTF-8"">
            </head>
            <body style=""padding: 16px"">
                <h1>{res.status} {res.statusMessage}</h1>
                <b>{exception.Message}</b>
                <pre>
                    {exception.StackTrace}
                </pre>
            </body>
        </html>
        ");
        await WriteResponse(res);
    }
}
