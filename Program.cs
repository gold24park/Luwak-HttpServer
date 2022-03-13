using WebServerProgram.Http;

namespace WebServerProgram
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            new Luwak().Listen(Util.GetPort(args)).Wait();
        }
    }
}