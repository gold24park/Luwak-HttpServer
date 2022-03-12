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
            Util.CountConnections(port);
        }
    }
    
    private async Task HandleConnection(object o)
    {
        try
        {
            TcpClient client = (TcpClient) o;
            Process(client.GetStream());
        }
        catch (Exception e)
        {
            Logger.Log("Connection closed.");
            Util.CountConnections(port);
        }
    }

    private async Task Process(NetworkStream stream)
    {
        // TODO; 여기에서 일어나는 모든 Exception 다 돌리는 처리 필요
        HttpRequest req = new HttpRequest();
        
        int totalByteCount = 0, readByteCount = 0;
        byte[] buffer = new byte[4096];
        byte[] tempBuffer = new byte[0];
        while ((readByteCount = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            totalByteCount += readByteCount;
            Logger.Log($"read: {readByteCount} bytes / total: {totalByteCount} bytes");
            Array.Resize(ref tempBuffer, readByteCount);
            Array.Copy(buffer, tempBuffer, readByteCount);

            req.Receive(tempBuffer);

            if (req.readFlag == HttpRequest.ReadFlag.End) break;
        }


        HttpResponse res = new HttpResponse();
        byte[] writeBuffer = Encoding.UTF8.GetBytes(res.PrintRequest(req));
        await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length);
        stream.Close();
    }
}
