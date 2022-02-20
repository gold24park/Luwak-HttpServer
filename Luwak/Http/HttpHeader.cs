using System.Collections.Generic;

namespace WebServerProgram.Http;

public class HttpHeader
{
    private Dictionary<string, string> dict = new Dictionary<string, string>();
    
    public void Add(string key, object value)
    {
        if (value == null || value.ToString().Length == 0) return;
        key = key.ToLower();
        if (dict.ContainsKey(key))
            dict[key] = value.ToString();
        else
            dict.Add(key, value.ToString());
    }

    public void Add(string line)
    {
        var splits = line.Split(": ");
        if (splits.Length == 2)
            Add(splits[0].ToLower(), splits[1]);
    }

    public string Get(string key)
    {
        return dict[key.ToLower()];
    }

    public bool isEmpty()
    {
        return dict.Count == 0;
    }
}