using System.IO;

namespace WebServerProgram.Http;

public class HttpRequest
{
    enum Method { GET, POST, PUT, DELETE, UNKNOWN }
    
    enum ParseStage { StartLine, Header, Body }

    private ParseStage stage = ParseStage.StartLine;
    
    private Method method = Method.GET;
    private string path;
    private string protocolVersion;
    private HttpHeader headers = new HttpHeader();
    public HttpRequest(string request)
    {
        string boundary = null;
        string line;
        var reader = new StringReader(request);
        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();
            switch (stage)
            {
                case ParseStage.StartLine:
                    var splits = line.Split(" ");
                    if (splits.Length == 3)
                    {
                        if (!Method.TryParse(splits[0], true, out method))
                            method = Method.UNKNOWN;
                        path = splits[1];
                        protocolVersion = splits[2];
                        stage += 1;
                    }
                    break;
                case ParseStage.Header:
                    headers.Add(line);
                    if (line.Length == 0 && !headers.isEmpty())
                        stage += 1;
                    break;
                case ParseStage.Body:
                    if (boundary == null)
                    {
                        var contentType = headers.Get("Content-Type");
                        if (contentType.Contains("multipart/form-data;"))
                        {
                            boundary = contentType.Substring(contentType.IndexOf("boundary=") + "boundary=".Length);
                        }
                        // TODO; 어떻게? body도 Dictinary?
                    }
                    break;
            }
        }
        // Console.WriteLine(method);
        // Console.WriteLine(path);
        // Console.WriteLine(protocolVersion);
        // Console.WriteLine(stage);
        // Console.WriteLine(boundary);
    }
}