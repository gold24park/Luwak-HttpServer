using WebServerProgram.Http;

namespace WebServerProgram;
public class RouteHandler {
    public string? path = null;
    public virtual void OnPost(HttpRequest request, HttpResponse response) {}
    public virtual void OnGet(HttpRequest request, HttpResponse response) {}
    public virtual void OnPut(HttpRequest request, HttpResponse response) {}
    public virtual void OnDelete(HttpRequest request, HttpResponse response) {}

    public void Invoke(HttpRequest request, HttpResponse response) {
        switch (request.method) {
            case Method.POST:
                this.OnPost(request, response);
                break;
            case Method.PUT:
                this.OnPut(request, response);
                break;
            case Method.DELETE:
                this.OnDelete(request, response);
                break;
            default:
                this.OnGet(request, response);
                break;
        }
    }
}