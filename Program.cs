using WebServerProgram.Http;

namespace WebServerProgram
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            new Luwak().Start(Parser.GetPort(args)).Wait();
        }
    }
}