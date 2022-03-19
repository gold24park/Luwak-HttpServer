using System.Net.Sockets;
using System.Text;
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
        Logger.Log($"HttpServer::OnRequest - [{req.method}] {req.path}");
        RouteHandler routeHandler = baseRouteHandler;

        foreach (RouteHandler rh in routeHandlers) {
            if (rh.path == req.path) {
                routeHandler = rh;
                break;
            }
        }

        routeHandler.Invoke(req, res);

        // Response를 구성한다.
        byte[] writeBuffer = Encoding.UTF8.GetBytes(BuildResponse(res));
        await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length);
        stream.Close();
    }

    private string BuildResponse(HttpResponse res) {
        StringBuilder sb = new StringBuilder();
        sb.Append($"HTTP/1.1 {res.status} {res.statusMessage}\r\n");
        foreach (string headerKey in res.headers.Keys) {
            sb.Append($"{headerKey}: {res.headers[headerKey]}\r\n");
        }
        if (Util.ToInt(res.headers.Get("Content-Length")) > 0) {
            sb.Append($"\r\n{res.content}");
        }
        return sb.ToString().Trim();
    }

    public async Task OnError(int statusCode, string message)
    {
        // TODO: 에러났을때? 뭘할지...
        Logger.Log($"error {statusCode} {message}");
        stream.Close();
    }
}
