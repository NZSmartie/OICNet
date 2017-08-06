namespace OICNet
{
    public class OicConfiguration
    {
        public OicMessageSerialiser Serialiser { get; set; }

        private static OicConfiguration _default;

        public static OicConfiguration Default => _default ?? (_default = new OicConfiguration
        {
            Serialiser = new OicMessageSerialiser(new OicResolver())
        });
    }
}