# RequestCore

Library for handling requests in Asp.net Core applications.

## Getting Started

```posh
Install-Package RequestCore
```

## Basic Usage - CRUD operations

Initialize `RequestHandler`. Use `Newtonsoft.Json` to convert returned objects to strings:

```csharp
var handler = new RequestHandler()
{
    ObjectConverter = obj => JsonConvert.SerializeObject(obj)
};
```

Add the request handler middleware to Asp.net core pipline:

```csharp
void Configure(IApplicationBuilder appBuilder)
{
  appBuilder.UseRequestHandler(handler);
}
```

Return list of objects:

```csharp
handler.Get["/notice"].With(async _ => await _repository.GetNoticesAsync());
```

Return single object:

```csharp
handler.Get[@"/notice/(\d+)"].With(async (_, idString) =>
{
    int id = int.Parse(idString);
    return (await _repository.GetNoticesAsync()).First(notice => notice.Id == id);
});
```

Add new object:

```csharp
handler.Post["/notice"].With(noticeJson =>
{
    _repository.AddNotice(JsonConvert.DeserializeObject<Notice>(noticeJson));
});
```

### Full example:

Following class can used to run the service. It uses `Microsoft.AspNetCore.Server.Kestrel` and `Newtonsoft.Json`:

```csharp
public class NoticeService
{
    private readonly INoticeRepository _repository;

    public NoticeService(INoticeRepository repository)
    {
        _repository = repository;
    }

    public void Start(int port)
    {
        new WebHostBuilder()
            .UseKestrel()
            .UseUrls($"http://+:{port}")
            .Configure(Configure)
            .Build()
            .Start(); ;
    }

    private void Configure(IApplicationBuilder appBuilder)
    {
        var handler = new RequestHandler()
        {
            ObjectCoverter = obj => JsonConvert.SerializeObject(obj)
        };

        handler.Get["/notice"].With(async _ => await _repository.GetNoticesAsync());

        handler.Get[@"/notice/(\d+)"].With(async (_, idString) =>
        {
            int id = int.Parse(idString);
            return (await _repository.GetNoticesAsync()).First(notice => notice.Id == id);
        });

        handler.Post["/notice"].With(noticeJson =>
        {
            _repository.AddNotice(JsonConvert.DeserializeObject<Notice>(noticeJson));
        });

        appBuilder.UseRequestHandler(handler);
    }
}
```
