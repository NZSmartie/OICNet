namespace OICNet.Server.Hosting
{
    public interface IOicContextFactory { }

    public interface IOicContextFactory<TSource> : IOicContextFactory
    {
        OicContext CreateContext(TSource source);

        void DisposeContext(OicContext context);
    }
}