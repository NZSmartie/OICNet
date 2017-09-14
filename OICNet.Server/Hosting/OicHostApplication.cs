using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OICNet.Server.Hosting
{
    public class OicHostApplication
    {
        private readonly Dictionary<Type, IOicContextFactory> _contextFactories = new Dictionary<Type, IOicContextFactory>();
        private readonly ILogger<OicHostApplication> _logger;

        private readonly string _debugBeginRequestTag = $"{typeof(OicHostApplication).Namespace}.BeginRequest";
        private readonly string _debugEndRequestTag = $"{typeof(OicHostApplication).Namespace}.EndRequest";
        private readonly string _debugUnhandledExceptionTag = $"{typeof(OicHostApplication).Namespace}.UnhandledException";
        private readonly RequestDelegate _application;

        public OicHostApplication(RequestDelegate application, IEnumerable<IOicContextFactory> contextFactories, ILogger<OicHostApplication> logger)
        {
            _logger = logger;
            _application = application ?? throw new ArgumentNullException(nameof(application));

            foreach (var contextFactory in contextFactories)
            {
                var factoryType = contextFactory.GetType().GetInterfaces().FirstOrDefault(t => t.IsConstructedGenericType);
                if (factoryType == null)
                    continue;

                _contextFactories.Add(factoryType.GenericTypeArguments.First(), contextFactory);
            }
        }

        protected IOicContextFactory<TSource> GetContextFactory<TSource>()
        {
            if (!_contextFactories.TryGetValue(typeof(TSource), out var factory))
                throw new ArgumentOutOfRangeException(nameof(TSource), $"No registered {nameof(IOicContextFactory)} for type {typeof(TSource)}");

            return factory as IOicContextFactory<TSource>
                              ?? throw new InvalidCastException($"Could not cast {factory.GetType()} to {typeof(IOicContextFactory<TSource>)}");
        }

        public OicContext CreateContext<TSource>(TSource contextFeatures)
        {
            var oicContextFactory = GetContextFactory<TSource>();
            var oicContext = oicContextFactory.CreateContext(contextFeatures);

            //var diagnoticsEnabled = _diagnosticSource.IsEnabled(_debugBeginRequestTag);
            //var startTimestamp = (diagnoticsEnabled || _logger.IsEnabled(LogLevel.Information)) ? Stopwatch.GetTimestamp() : 0;

            //if (diagnoticsEnabled)
            //    _diagnosticSource.Write(_debugBeginRequestTag, new { oicContext = oicContext, timestamp = startTimestamp });

            return oicContext;
        }

        public void DisposeContext<TSource>(OicContext oicContext, Exception exception)
        {
            var oicContextFactory = GetContextFactory<TSource>();

            //if (exception == null)
            //{
            //    var diagnoticsEnabled = _diagnosticSource.IsEnabled(_debugEndRequestTag);
            //    var currentTimestamp = (diagnoticsEnabled) ? Stopwatch.GetTimestamp() : 0;

            //    if (diagnoticsEnabled)
            //        _diagnosticSource.Write(_debugEndRequestTag, new { oicContext = oicContext, timestamp = currentTimestamp });
            //}
            //else
            if(exception != null)
                _logger.LogError(exception, "Exception occured during handling request in application stack");

            //{
            //    var diagnoticsEnabled = _diagnosticSource.IsEnabled(_debugUnhandledExceptionTag);
            //    var currentTimestamp = diagnoticsEnabled ? Stopwatch.GetTimestamp() : 0;

            //    if (diagnoticsEnabled)
            //        _diagnosticSource.Write(_debugUnhandledExceptionTag, new { oicContext = oicContext, timestamp = currentTimestamp, exception = exception });
            //}

            oicContextFactory.DisposeContext(oicContext);
        }

        public Task ProcessRequestAsync(OicContext oicContext)
        {
            return _application(oicContext);
        }
    }
}