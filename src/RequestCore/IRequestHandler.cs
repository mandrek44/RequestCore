using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RequestCore
{
    public interface IRequestHandler
    {
        Task<bool> HandleAsync(HttpContext context);
    }
}