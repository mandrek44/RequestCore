namespace RequestCore
{
    public interface IHttpService
    {
        HttpHandlerCollection Delete { get; }

        HttpHandlerCollection Post { get; }

        HttpHandlerCollection Get { get; }
    }
}