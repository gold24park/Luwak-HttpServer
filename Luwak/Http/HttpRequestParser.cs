using System.Text;
using System.Text.RegularExpressions;

namespace WebServerProgram.Http;

public enum Method { 
    GET, POST, PUT, DELETE, 
    HEAD, CONNECT, OPTIONS, TRACE, PATCH, 
    UNKNOWN 
}
public class HttpRequestParser
{
    private HttpRequest req;
    private List<byte> buffer = new();
    
    public enum ReadFlag { StartLine, Header, Body, End }

    public ReadFlag readFlag
    {
        get; protected set;
    }

    private int contentLength = -1;

    public HttpRequestParser() {
        this.readFlag = ReadFlag.StartLine;
    }

    public HttpRequest GetRequest()
    {
        return req;
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
        
        req = new HttpRequest();

        string regex = @"^(?<method>[^ ]+) (?<path>[^ ]+) (?<protocolVersion>HTTP\/.*)";

        foreach (Match match in Regex.Matches(line, regex)) {
            if (!Method.TryParse(match.Groups["method"].Value, true, out req.method))
                req.method = Method.UNKNOWN;
            req.path = match.Groups["path"].Value;
            req.protocolVersion = match.Groups["protocolVersion"].Value;
            readFlag = ReadFlag.Header;
            return true;
        }

        return false;
    }

    private bool ReadHeaders(string line)
    {
        if (line == null) return false;

        string regex = @"^(?<headerKey>.*?):[ ]?(?<headerValue>.*)";

        foreach (Match match in Regex.Matches(line, regex)) {
            string key = match.Groups["headerKey"].Value.ToLower();
            string value = match.Groups["headerValue"].Value;
            req.headers[key] = value;
        }

        if (!Regex.IsMatch(line, regex)) readFlag = ReadFlag.Body;

        return true;
    }

    private bool ReadBody()
    {
        if (contentLength == -1) {
            contentLength = Util.ToInt(req.headers.Get("Content-Length"));
        }
        if (buffer.Count >= contentLength) {
            req.content = buffer.GetRange(0, contentLength).ToArray();
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