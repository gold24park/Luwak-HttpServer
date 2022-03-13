using System.Net.Sockets;
using System.Text;
using WebServerProgram.Http;

namespace WebServerProgram;

public class HttpServer : IHttpServer
{
    private TcpClient client;
    private NetworkStream stream;

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
        byte[] writeBuffer = Encoding.UTF8.GetBytes(res.PrintRequest(req));
        await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length);
        stream.Close();
    }

    public async Task OnError(int statusCode, string message)
    {
        // TODO: 에러났을때? 뭘할지...
        Logger.Log($"error {statusCode} {message}");
        stream.Close();
    }
}
