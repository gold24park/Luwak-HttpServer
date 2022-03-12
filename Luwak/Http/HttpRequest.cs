using System.Text;
using System.Collections.Generic;
using System;
using System.IO;

namespace WebServerProgram.Http;

public class HttpRequest
{
    public enum Method { 
        GET, POST, PUT, DELETE, 
        HEAD, CONNECT, OPTIONS, TRACE, PATCH, 
        UNKNOWN 
    }
    
    public enum ReadFlag { StartLine, Header, Body, End }

    public ReadFlag readFlag
    {
        get; protected set;
    }
    
    public Method method = Method.GET;
    public string path;
    public string protocolVersion;
    public HttpHeader headers = new();
    private List<byte> buffer = new();
    public byte[] content = new byte[0];
    private int contentLength = -1;
    private Dictionary<string, string> body;

    public HttpRequest() {
        this.readFlag = ReadFlag.StartLine;
    }

    public void Receive(byte[] chunk)
    {
        buffer.AddRange(chunk.ToList());

        bool next = true;
        while (next) {
            switch (readFlag) {
                case ReadFlag.StartLine:
                    next = ReadStartLine(getNextLineFromBuffer());
                    break;
                case ReadFlag.Header:
                    next = ReadHeaders(getNextLineFromBuffer());
                    break;
                case ReadFlag.Body:
                    next = ReadBody();
                    break;
            }
        }
    }

    private bool ReadStartLine(string line)
    {
        if (line == null) return false;

        var splits = line.Split(" ");
        if (splits.Length == 3)
        {
            if (!Method.TryParse(splits[0], true, out method))
                method = Method.UNKNOWN;
            path = splits[1];
            protocolVersion = splits[2];
            readFlag = ReadFlag.Header;
            return true;
        }
        else
        {
            throw new Exception("Startline 형식이 잘못되었습니다.");
        }
    }

    private bool ReadHeaders(string line)
    {
        if (line == null) return false;

        if (!line.Contains(":")) {
            readFlag = ReadFlag.Body;
        }
        headers.Add(line);
        return true;
    }
    private bool ReadBody()
    {
        if (contentLength == -1) {
            contentLength = Util.ToInt(headers.Get("Content-Length"));
        }
        if (buffer.Count >= contentLength) {
            content = buffer.GetRange(0, contentLength).ToArray();
            readFlag = ReadFlag.End;
        }
        return false;
    }

    private string getNextLineFromBuffer() {
        // \r = 0x0D, \n = 0x0A
        int index = buffer.IndexOf(0x0D);
        if (index == -1)
            return null;
        string line = Encoding.UTF8.GetString(buffer.GetRange(0, index).ToArray());
        // \r\n 다 지워줌
        int removeIndex = index + 1 < buffer.Count && buffer[index + 1] == 0x0A ? index + 2 : index + 1;
        buffer.RemoveRange(0, removeIndex);
        return line;
    }
}
