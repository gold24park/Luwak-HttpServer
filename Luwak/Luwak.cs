using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
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
        
        Logger.Log($"서버가 시작되었습니다. port={port}");

        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            // ThreadPool에서 새로운 Thread를 생성할지, Queue에 넣고 기존의 Thread가 끝나길 기다릴지 결정
            Task.Factory.StartNew(HandleConnection, client);
            CountConnections();
        }
    }
    
    private async void HandleConnection(object o)
    {
        TcpClient client = (TcpClient) o;
        NetworkStream stream = client.GetStream();
        await httpServer.process(stream);
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