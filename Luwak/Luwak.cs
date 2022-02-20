using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using WebServerProgram.Http;

namespace WebServerProgram;

public class Luwak
{
    private int port;
    private TcpListener listener;
    private HttpServer httpServer;

    public Luwak(HttpServer httpServer)
    {
        this.httpServer = httpServer;
    }

    public async Task Start(int port = 8080)
    {
        this.port = port;
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        
        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            Task.Factory.StartNew(HandleConnection, client);
            CountConnections();
        }
    }
    
    private async void HandleConnection(object o)
    {
        TcpClient client = (TcpClient) o;
        NetworkStream stream = client.GetStream();
        await httpServer.process(stream);
        Console.WriteLine("Close connection");
        stream.Close();
        client.Close();
    }
    
    private void CountConnections()
    {
        int count = 0;
        IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
        TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();
        foreach (TcpConnectionInformation c in connections)
        {
            if (c.RemoteEndPoint.Port == port)
            {
                count++;
            }
        }
        Console.WriteLine($"Active TCP Connections: {count}");
    }
}