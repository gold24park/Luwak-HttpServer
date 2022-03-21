using WebServerProgram.Http;

namespace WebServerProgram
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var luwak = new Luwak();
            luwak.RegisterRoute("/index", new FileIndexRouteHandler());
            luwak.RegisterRoute("/download", new FileDownloadRouteHandler());
            luwak.Listen(Util.GetPort(args)).Wait();
        }
    }
}