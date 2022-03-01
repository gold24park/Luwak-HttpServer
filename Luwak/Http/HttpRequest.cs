using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace WebServerProgram.Http;

public class HttpRequest
{
    public enum Method { GET, POST, PUT, DELETE, UNKNOWN }
    
    enum ReadFlag { StartLine, Header, Body, End }

    private ReadFlag _readFlag = ReadFlag.StartLine;
    
    public Method method = Method.GET;
    public string path;
    public string protocolVersion;
    public HttpHeader headers = new();
    private Dictionary<String, String> body;
    private StringBuilder buffer = new();
    private string contentType = null;

    public bool Receive(string chunk)
    {
        buffer.Append(chunk);
        Console.WriteLine($"buffer: {buffer}");
        int idx;
        while ((idx = buffer.ToString().IndexOf("\n")) > -1)
        {
            var line = buffer.ToString().Substring(0, idx);
            Console.WriteLine($"line: {line}");
            switch (_readFlag) 
            {
                case ReadFlag.StartLine:
                    ReadStartLine(line);
                    break;
                
                case ReadFlag.Header:
                    ReadHeaders(line);
                    break;
                
                case ReadFlag.Body:
                    ReadBody();
                    break;
            }
            if (_readFlag != ReadFlag.Body) 
                buffer.Remove(0, idx + 1);
            
            Console.WriteLine($"contains: {buffer.ToString().Contains("\n")}");
            Console.WriteLine($"remain: {buffer.ToString().Length}");
        }
        Console.WriteLine($"End of Receive(): {_readFlag} / {idx}");
        return _readFlag == ReadFlag.End;

    }

    private void ReadStartLine(string line)
    {
        var splits = line.Split(" ");
        if (splits.Length == 3)
        {
            if (!Method.TryParse(splits[0], true, out method))
                method = Method.UNKNOWN;
            path = splits[1];
            protocolVersion = splits[2];
            _readFlag++;
        }
    }

    private void ReadHeaders(string line)
    {
        headers.Add(line);
        if (!line.Contains(":"))
        {
            int contentLength = Parser.ToInt(headers.Get("Content-Length"));
            _readFlag = contentLength == 0 ? ReadFlag.End : ReadFlag.Body;
        }
    }
    private void ReadBody()
    {
        if (contentType == null)
            contentType = headers.Get("Content-Type");
        
        switch (contentType.ToLower())
        {
            case "application/x-www-form-urlencoded":
                // percent-encoded 문자열을 풀어줌
                Uri.UnescapeDataString(buffer.ToString()); 
                break;
            case "application/json":
                break;
            default:
                throw new Exception("지원하지 않는 Content-Type입니다.");
        }
        // TODO: 바디 파싱 다되면 의미있게 readFlag 옮기기...
        _readFlag++;
    }
}