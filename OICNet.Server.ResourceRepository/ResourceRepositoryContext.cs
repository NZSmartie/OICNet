using System;

namespace OICNet.Server.ResourceRepository
{
    public class ResourceRepositoryContext
    {
        private readonly IOicResourceRepository _resourceProvider;
        private readonly OicContext _context;
        private readonly ResourceRepositoryOptions _options;

        private readonly string _requestPath;

        public ResourceRepositoryContext(OicContext context, ResourceRepositoryOptions options, IOicResourceRepository resourceProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _resourceProvider = resourceProvider ?? throw new ArgumentNullException(nameof(resourceProvider));

            // Since request path is a segment, ensure it is surrounded with slashes.
            _requestPath = _options.RequestPath ?? "/";
            if (!_requestPath.EndsWith("/"))
                _requestPath += "/";
            if (!_requestPath.StartsWith("/"))
                _requestPath = "/" + _requestPath;
        }

        public bool ValidatePath()
        {
            if (_context.Request.ToUri == null)
                return false;

            var search = "";
            foreach (var segmentt in _context.Request.ToUri.Segments)
            {
                search += segmentt;
                if (search.Equals(_requestPath, StringComparison.Ordinal))
                    return true;
                if (search.Length > _requestPath.Length)
                    return false;
            }
            return false;
        }

        public Uri GetPath()
        {
            var requestedPath = _context.Request.ToUri?.AbsolutePath 
                ?? throw new InvalidOperationException();

            return new UriBuilder(_context.Request.ToUri)
            {
                Path = requestedPath.Substring(_requestPath.Length - 1)
            }.Uri;
        }
    }
}