# Luwak-HttpServer
HTTP 프로토콜 공부 & C# 학습을 위해 외부 라이브러리 사용없이 만든 웹 서버입니다. 

Luwak은 이 프로그램이 단순히 연습용 웹 서버가 아닌, 내가 node.js의 express나 python의 falcon 같은 프레임워크처럼 쉽게 쓰도록 만들자는 생각으로 붙인 이름입니다.

### 실행하기
아래 코드는 8080 포트에서 Luwak 웹 서버를 실행하는 예제입니다.
```
var luwak = new Luwak();
luwak.Listen(8080).Wait();
```

### Router 등록하기
GET/POST/PUT/DELETE를 지원하는 RouteHandler를 상속하여 만든 클래스를 등록합니다.
```
luwak.RegisterRoute("/index", new FileIndexRouteHandler());
```

### Luwak 구조
TCP 연결이 수립되면 새로운 스레드에서 데이터 스트림을 주고 받습니다.
```
client = await listener.AcceptTcpClientAsync();
Task.Factory.StartNew(CreateConnection, client);
```

데이터가 들어오면 `HttpRequestParser`에서 처리됩니다. `HttpRequestParser`는 ByteArray에서 `\r`를 찾아 Line단위로 읽는데, 

읽어들인 Request의 상태에 따라 ReadFlag가 변화합니다.
- StartLine `HttpRequest 시작줄 파싱중` 
- Header 
- Body `content-length 헤더를 확인하고 body를 읽어야하는 상태`
- End `Request를 모두 읽은 상태`
