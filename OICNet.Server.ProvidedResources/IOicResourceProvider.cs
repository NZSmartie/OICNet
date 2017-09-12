namespace OICNet.Server.ProvidedResources
{
    public interface IOicResourceProvider
    {
        IOicResource GetResource(string id);
    }
}