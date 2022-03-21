using System.Net.Sockets;
using WebServerProgram.Http;

interface IHttpServer
{
    void OnConnect(TcpClient client);
    void OnClose();
    Task OnRequest(HttpRequest req, HttpResponse res);

    Task OnError(Exception exception);
}