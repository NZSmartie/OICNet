using System;
using System.Collections.Generic;
using System.Text;

namespace OICNet
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class OicJsonReadOnlyAttribute : Attribute
    {

    }
}
