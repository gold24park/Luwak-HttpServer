using System.Text;

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
    private List<byte> buffer = new();
    public byte[] content = new byte[0];
    private int contentLength = 0;

    private const int NEW_LINE = 10;

    public bool Receive(byte[] chunk)
    {
        buffer.AddRange(chunk.ToList());
        int idx;
        while ((idx = buffer.IndexOf(NEW_LINE)) > -1 && _readFlag < ReadFlag.Body)
        {
            var line = Encoding.UTF8.GetString(buffer.GetRange(0, idx).ToArray());

            buffer.RemoveRange(0, idx + 1);
            switch (_readFlag) 
            {
                case ReadFlag.StartLine:
                    ReadStartLine(line);
                    break;
                
                case ReadFlag.Header:
                    ReadHeaders(line);
                    break;
            }
        }
        if (_readFlag == ReadFlag.Body)
        {
            ReadBody();
        }
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
        else
        {
            throw new Exception("Startline 형식이 잘못되었습니다.");
        }
    }

    private void ReadHeaders(string line)
    {
        headers.Add(line);
        if (!line.Contains(":"))
        {
            contentLength = Util.ToInt(headers.Get("Content-Length"));
            _readFlag = contentLength == 0 ? ReadFlag.End : ReadFlag.Body;
        }
    }
    private void ReadBody()
    {
        if (buffer.Count < contentLength)
            return;
        content = buffer.GetRange(0, contentLength).ToArray().Reverse().ToArray();
        _readFlag++;
    }
}