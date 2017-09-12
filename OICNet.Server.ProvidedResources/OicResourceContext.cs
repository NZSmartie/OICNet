using System;

namespace OICNet.Server.ProvidedResources
{
    public class OicResourceContext
    {
        private readonly IOicResourceProvider _resourceProvider;
        private readonly OicContext _context;
        private readonly ProvidedResourceOptions _options;

        private readonly string _requestPath;

        public OicResourceContext(OicContext context, ProvidedResourceOptions options, IOicResourceProvider resourceProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _resourceProvider = resourceProvider ?? throw new ArgumentNullException(nameof(resourceProvider));

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

        public string GetPath()
        {
            var requestedPath = _context.Request.ToUri?.AbsolutePath 
                ?? throw new InvalidOperationException();

            return requestedPath.Substring(_requestPath.Length - 1);
        }
    }
}