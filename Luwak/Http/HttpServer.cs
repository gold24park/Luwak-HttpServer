using System.Net.Sockets;

namespace WebServerProgram.Http;

public class HttpServer
{

    public async Task process(NetworkStream stream)
    {
        // TOOD: Action<Req, Res> 로 미들웨어 구현하자
        // TODO; 여기에서 일어나는 모든 Exception 다 돌리는 처리 필요..
        // var request = new HttpRequest(await StreamIO.read(stream));
        // var response = new HttpResponse();

        // echo
        var message = await StreamIO.read(stream);
        StreamIO.write(stream, message);
    }
}