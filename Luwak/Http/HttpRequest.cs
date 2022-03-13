using System.Text;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace WebServerProgram.Http;

public class HttpRequest
{
    public Method method = Method.GET;
    public string path;
    public string protocolVersion;
    public HttpHeader headers = new();
    public byte[] content = new byte[0];
    private Dictionary<string, string> body;
}
