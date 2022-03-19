using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WebServerProgram.Http;
using System.Threading.Tasks;
using System;

namespace WebServerProgram;

public class Luwak
{
    private int port;
    private TcpListener listener;
    private bool isOpen = true;
    private List<RouteHandler> routeHandlers = new List<RouteHandler>();
    
    public async Task Listen(int port = 8080)
    {
        isOpen = true;
        this.port = port;
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        
        Logger.Log($"서버가 시작되었습니다. port={port}");
        
        TcpClient? client = null;
        while (isOpen)
        {
            client = await listener.AcceptTcpClientAsync();
            Task.Factory.StartNew(CreateConnection, client);
            Util.CountConnections(port);
        }
        client?.Close();
    }

    public void Stop()
    {
        isOpen = false;
        listener.Stop();
    }
    
    public async Task CreateConnection(object o)
    {
        HttpServer server = new HttpServer(routeHandlers);
        TcpClient client = (TcpClient) o;
        
        server.OnConnect(client);

        var readTask = ReadRequest(client);
        var timeoutTask = Task.Delay(10 * 1000);
        var doneTask = await Task.WhenAny(timeoutTask, readTask).ConfigureAwait(false);

        if (doneTask == readTask)
        {
            await server.OnRequest(readTask.Result, new HttpResponse());    
        }
        else
        {
            await server.OnError(500, "Timeout");
        }
    }

    private async Task<HttpRequest> ReadRequest(TcpClient client)
    {
        HttpRequestParser parser = new HttpRequestParser();
        
        int totalByteCount = 0;
        int readByteCount = 0;
        byte[] buffer = new byte[4096];
        byte[] tempBuffer = new byte[0];
        
        while ((readByteCount = await client.GetStream().ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            totalByteCount += readByteCount;
            Logger.Log($"read: {readByteCount} bytes / total: {totalByteCount} bytes");
            Array.Resize(ref tempBuffer, readByteCount);
            Array.Copy(buffer, tempBuffer, readByteCount);
    
            parser.Receive(tempBuffer);
            if (parser.readFlag == HttpRequestParser.ReadFlag.End) break;
        }

        return parser.GetRequest();
    }

    public void RegisterRoute(string path, RouteHandler routeHandler) {
        if (IsDuplicatePath(path)) {
            throw new Exception("동일한 Path를 등록하셨습니다.");
        }
        routeHandler.path = path;
        routeHandlers.Add(routeHandler);
    }

    private bool IsDuplicatePath(string path) {
        foreach (RouteHandler rh in routeHandlers) {
            if (rh.path == path) return true;
        }
        return false;
    }
}
