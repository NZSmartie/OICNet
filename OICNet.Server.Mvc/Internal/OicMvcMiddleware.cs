using System.Threading.Tasks;
using OICNet.Server.Hosting;

namespace OICNet.Server.Mvc.Internal
{
    public class OicMvcMiddleware
    {
        private readonly RequestDelegate _next;

        public OicMvcMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(OicContext oicContext)
        {
            await _next.Invoke(oicContext);
        }
    }
}