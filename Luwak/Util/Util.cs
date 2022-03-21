using System;
using System.Net.NetworkInformation;

namespace WebServerProgram;

public static class Util
{

    public static string GetMimeType(string fileName) {
        int index = fileName.IndexOf(".");
        if (index > -1) {
            string ext = fileName.Substring(index + 1);
            switch (ext) {
                case "css":
                    return "text/css";
                case "html":
                    return "text/html";
                case "js":
                    return "text/javascript";
                case "gif":
                    return "image/gif";
                case "jpeg":
                    return "image/jpeg";
                case "png":
                    return "image/png";
                case "svg":
                    return "image/svg+xml";
                case "webp":
                    return "image/webp";
                case "jpg":
                    return "image/jpeg";
                case "wav":
                    return "audio/wav";
                case "webm":
                    return "video/webm";
                case "pdf":
                    return "application/pdf";
            }
        }
        return "application/octet-stream";
    }

    public static int ToInt(string s)
    {
        try
        {
            return Int32.Parse(s);
        }
        catch (Exception e)
        {
            return 0;
        }
    }
    
    public static int GetPort(string[] args)
    {
        int port = 8080;
        if (args.Length > 0)
        {
            try
            {
                port = Int16.Parse(args[0]);
            }
            catch (FormatException e)
            {
                Logger.Log("첫번째 Argument 값이 숫자가 아닙니다.", Logger.Level.Error);
            }
            catch (OverflowException e)
            {
                Logger.Log("허용가능 Port 범위를 초과하였습니다.", Logger.Level.Error);
            }
        }

        return port;
    }
    public static void CountConnections(int port)
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
        Logger.Log($"Active TCP Connections: {count}");
    }
}
