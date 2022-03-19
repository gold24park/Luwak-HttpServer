using System.Net;
using System.Text;

namespace WebServerProgram.Http;

public class HttpResponse
{
    public HttpHeader headers = new HttpHeader();
    public int status = (int) HttpStatusCode.OK;
    public string statusMessage = HttpStatusCode.OK.ToString();
    public string content = "";

    public void Init() {
        headers.AddHeader("Server", "Luwak");
        headers.AddHeader("Date", DateTime.Now.ToString());
    }
}
