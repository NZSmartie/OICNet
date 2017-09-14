using System.Threading.Tasks;

namespace OICNet.Server.Hosting
{
    public delegate Task RequestDelegate(OicContext context);
}