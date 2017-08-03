using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OICNet.Tests
{
    public class TestFixtureBase
    {
        protected static byte[] HexToBytes(string value)
        {
            return value.Split('-').Select(b => Convert.ToByte(b, 16)).ToArray();
        }
    }
}
