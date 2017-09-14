namespace OICNet.Server.Hosting.Internal
{
    public class HostingEnvironment : IHostingEnvironment
    {
        public string EnvironmentName { get; set; } = Hosting.EnvironmentName.Production;

        public string ApplicationName { get; set; }

        public string WebRootPath { get; set; }

        public string ContentRootPath { get; set; }
    }
}
