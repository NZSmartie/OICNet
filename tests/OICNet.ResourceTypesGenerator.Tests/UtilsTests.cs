using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace OICNet.ResourceTypesGenerator.Tests
{
    [TestFixture]
    public class UtilsTests
    {
        [TestCase("oic.r.audio", "OicRAudio")]
        public void ToCapitalCase(string input, string expected)
        {
            var actual = input.ToCapitalCase();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("oic.r.audio", "oicRAudio")]
        public void ToCamelCase(string input, string expected)
        {
            var actual = input.ToCamelCase();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("OicRAudio", "_oicRAudio")]
        public void ToPrivateName(string input, string expected)
        {
            var actual = input.ToPrivateName();

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
