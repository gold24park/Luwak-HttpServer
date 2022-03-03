namespace WebServerProgram;

public static class Util
{
    public static int ToInt(string s)
    {
        try
        {
            return Int32.Parse(s);
        }
        catch (Exception e)
        {
            return 0;
        }
    }
    
    public static int GetPort(string[] args)
    {
        int port = 8080;
        if (args.Length > 0)
        {
            try
            {
                port = Int16.Parse(args[0]);
            }
            catch (FormatException e)
            {
                Logger.Log("첫번째 Argument 값이 숫자가 아닙니다.", Logger.Level.Error);
            }
            catch (OverflowException e)
            {
                Logger.Log("허용가능 Port 범위를 초과하였습니다.", Logger.Level.Error);
            }
        }

        return port;
    }
    
    public static string ByteArrayToString(byte[] ba)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(ba);

        string hex = BitConverter.ToString(ba);
        return hex.Replace("-", "");
    }
}