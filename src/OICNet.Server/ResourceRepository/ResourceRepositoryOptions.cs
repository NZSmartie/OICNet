using System;
using OICNet.Server.Builder;

namespace OICNet.Server.ResourceRepository
{
    public class ResourceRepositoryOptions
    {
        public string RequestPath { get; set; }

        public IOicResourceRepository ResourceRepository { get; set; }

        private Type _resourceRepositoryType;
        public object[] ResourceRepositoryArgs { get; set; } = new object[] { };
        public Type ResourceRepositoryType
        {
            get => _resourceRepositoryType;
            set
            {
                if (!typeof(IOicResourceRepository).IsAssignableFrom(value))
                    throw new ArgumentException($"{ResourceRepositoryType.FullName} can only be assigned types that implement {nameof(IOicResourceRepository)}");
                _resourceRepositoryType = value;
            }
        }

        public void UseResourceRepository<TRepository>(params object[] args) where TRepository : class, IOicResourceRepository
            => UseResourceRepository(typeof(TRepository), args);

        public void UseResourceRepository(Type TRepository, params object[] args)
        {
            ResourceRepositoryType = TRepository;
            ResourceRepositoryArgs = args;
        }
    }
}