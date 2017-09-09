using System.Threading.Tasks;

namespace OICNet.Server.Hosting
{
    public interface IMiddleware
    {
        Task Invoke(OicContext oicContext, RequestDelegate next);
    }
}