using System;
using System.Collections.Generic;

namespace OICNet.Server.ProvidedResources
{
    public interface IOicResourceProvider
    {
        IOicResource GetResource(Uri uri);

        IList<IOicResource> Resources { get; }
    }
}