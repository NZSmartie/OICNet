using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OICNet
{
    public interface IOicResource
    {
        string RelativeUri { get; set; }

        string Id { get; set; }

        IList<OicResourceInterface> Interfaces { get; }

        string Name { get; set; }

        IList<string> ResourceTypes { get; }

        void UpdateFields(IOicResource source);
    }
}