using System;
using OICNet.Server.Builder;

namespace OICNet.Server.ResourceRepository
{
    public class ResourceRepositoryOptions
    {
        public string RequestPath { get; set; }

        public IOicResourceRepository ResourceRepository { get; set; }
    }
}