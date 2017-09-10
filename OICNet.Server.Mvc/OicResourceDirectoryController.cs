using System;
using Microsoft.Extensions.DependencyInjection;
using OICNet.Server.Mvc;

namespace OICNet.Server
{
    public class OicResourceDirectoryController : OicController
    {
        public override IActionResult Get()
        {
            var controllers = OicContext.RequestServices.GetServices<OicController>();
            // TODO: filter incomming requests
            var directory = new CoreResources.OicResourceDirectory
            {
                // TODO: where is the Device Guid stored?
                DeviceId = Guid.NewGuid()
            };


            foreach (var controller in controllers)
            {
                //controller.DiscoverableResources
                //directory.Links.Add
            }

            return Content(directory);
        }
    }
}