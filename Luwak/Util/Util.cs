using System;
using System.Net.NetworkInformation;

namespace WebServerProgram;

public static class Util
{
    private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB" };

    public static string GetFileSize(int value) {
        if (value < 0) { return "-" + GetFileSize(-value); }

        int i = 0;
        decimal dValue = (decimal)value;
        while (Math.Round(dValue / 1024) >= 1)
        {
            dValue /= 1024;
            i++;
        }
        return string.Format("{0:n1}{1}", dValue, SizeSuffixes[i]);
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
    
    public static string ByteArrayToString(byte[] ba)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(ba);

        string hex = BitConverter.ToString(ba);
        return hex.Replace("-", "");
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
