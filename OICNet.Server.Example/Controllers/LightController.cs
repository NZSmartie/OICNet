using System.Threading.Tasks;

namespace OICNet.Server.Example.Controllers
{
    public class LightController : OicController
    {
        public override IActionResult Get()
        {
            return Valid("Yup, this do gooder is working");
        }
    }
}