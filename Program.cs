using WebServerProgram.Http;

namespace WebServerProgram
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var luwak = new Luwak();
            luwak.RegisterRoute("/index", new FileIndex());
            luwak.Listen(Util.GetPort(args)).Wait();
        }
    }
}