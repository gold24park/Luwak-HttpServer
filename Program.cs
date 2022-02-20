using System;
using WebServerProgram.Http;

namespace WebServerProgram
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            SetPort(out int port, args);
            var httpServer = new HttpServer();
            new Luwak(httpServer).Start(port).Wait();
            
            // Test();
        }

        private static void SetPort(out int port, string[] args)
        {
            port = 8080;
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
        }

        // curl -X GET http://localhost:8080
        public static void Test()
        {
            string input = @"
            POST / HTTP/1.1
            Host: localhost:8000
            User-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10.9; rv:50.0) Gecko/20100101 Firefox/50.0
            Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8
            Accept-Language: en-US,en;q=0.5
            Accept-Encoding: gzip, deflate
            Connection: keep-alive
            Upgrade-Insecure-Requests: 1
            Content-Type: multipart/form-data; boundary=---------------------------8721656041911415653955004498
            Content-Length: 465

            -----------------------------8721656041911415653955004498
            Content-Disposition: form-data; name=""myTextField""

            Test
                -----------------------------8721656041911415653955004498
            Content-Disposition: form-data; name=""myCheckBox""

            on
                -----------------------------8721656041911415653955004498
            Content-Disposition: form-data; name=""myFile""; filename=""test.txt""
            Content-Type: text/plain

            Simple file.
                -----------------------------8721656041911415653955004498--
            ";
            var request = new HttpRequest(input);
            
        }
    }
}