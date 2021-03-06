﻿namespace OICNet
{
    public class OicConfiguration
    {
        public OicMessageSerialiser Serialiser { get; set; }
        public OicResolver Resolver { get; set; }

        private static OicConfiguration _default;

        public static OicConfiguration Default => _default ?? (_default = new OicConfiguration());

        public OicConfiguration()
        {
            Resolver = new OicResolver();
            Serialiser = new OicMessageSerialiser(Resolver);
        }

        public OicConfiguration(OicResolver resolver)
        {
            Resolver = resolver;
            Serialiser = new OicMessageSerialiser(resolver);
        }
    }
}