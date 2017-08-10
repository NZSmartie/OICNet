using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OICNet.Utilities
{
    public static class ExceptionUtil
    {
        public static Exception CreateResourceCastException<TExpectedType>(string parameter)
        {
            return new InvalidCastException($"Can not cast {parameter} paremeter to {typeof(TExpectedType).Name}");
        }
    }
}
