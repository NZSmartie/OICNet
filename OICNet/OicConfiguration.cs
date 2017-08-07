namespace OICNet
{
    public class OicConfiguration
    {
        public OicMessageSerialiser Serialiser { get; set; }
        public OicResolver Resolver { get; set; }

        private static OicConfiguration _default;

        public static OicConfiguration Default
        {
            get
            {
                if(_default!= null)
                    return _default;

                _default = new OicConfiguration
                {
                    Resolver = new OicResolver()
                };
                _default.Serialiser = new OicMessageSerialiser(_default.Resolver);

                return _default;
            }
        }
    }
}