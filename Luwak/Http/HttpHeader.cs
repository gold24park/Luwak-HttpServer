
using System;
using System.IO;
using System.Collections.Generic;

namespace WebServerProgram.Http;

public class HttpHeader : Dictionary<string, string>
{

    public void AddHeader(string key, string value) {
        this[key.ToLower()] = value;
    }
    
    public string Get(string key)
    {
        try
        {
            return this[key.ToLower()];
        }
        catch (Exception e)
        {
            return null;
        }
    }
}

