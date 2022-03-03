namespace WebServerProgram.Http;

public class HttpHeader : Dictionary<string, string>
{

    public void Add(string line)
    {
        var splits = line.Split(":");
        if (splits.Length == 2)
        {
            var key = splits[0].ToLower().Trim();
            if (ContainsKey(key))
                this[key] = splits[1].Trim();
            else
                this.Add(key, splits[1].Trim());
        }
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