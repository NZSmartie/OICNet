using System.Threading.Tasks;

namespace OICNet.Server.Mvc
{
    public interface IActionResult
    {
        Task ExecuteResultAsync(ActionContext context);
    }

    public class ActionResult : IActionResult
    {
        public virtual Task ExecuteResultAsync(ActionContext context)
        {
            ExecuteResult(context);
            return Task.FromResult<object>(null);
        }

        public virtual void ExecuteResult(ActionContext context)
        {
            
        }
    }
}