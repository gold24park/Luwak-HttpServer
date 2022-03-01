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

    public Dictionary<string, string> getDict()
    {
        return dict;
    }

    public void Add(string line)
    {
        var splits = line.Split(":");
        if (splits.Length == 2)
            Add(splits[0].ToLower().Trim(), splits[1].Trim());
    }

    public string Get(string key)
    {
        try
        {
            return dict[key.ToLower()];
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public bool isEmpty()
    {
        return dict.Count == 0;
    }

    public string ToString()
    {
        return dict.ToString();
    }
}