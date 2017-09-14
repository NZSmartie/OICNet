using System;
using System.Collections.Generic;
using System.Text;

namespace OICNet.Server.Example
{
    public class MyResourceResolver : OicResolver
    {
        public MyResourceResolver()
        {
            _resourceTypes.Add("oicnet.hello", typeof(OicBaseResouece<string>));
        }
    }
}
