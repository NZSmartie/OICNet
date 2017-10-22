using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OICNet.ResourceTypesGenerator
{
    public static class Utils
    {
        public static string WithoutAttributeSuffix(this string input)
        {
            if (!input.EndsWith("Attribute"))
                return input;

            return input.Substring(0, input.Length - "Attribute".Length);
        }

        public static string ToCapitalCase(this string input)
        {
            return Capitalise(input, true);
        }

        public static string ToCamelCase(this string input)
        {
            return Capitalise(input, false);
        }

        public static string ToPrivateName(this string input)
        {
            return $"_{Char.ToLowerInvariant(input[0])}{input.Substring(1)}";
        }

        private static string Capitalise(string input, bool wordBreak)
        {
            var result = new List<char>();

            foreach (var c in input)
            {
                if (result.Count == 0 && !Char.IsLetter(c))
                    continue;

                if (!Char.IsLetterOrDigit(c))
                {
                    wordBreak = true;
                    continue;
                }

                if (wordBreak)
                    result.Add(Char.ToUpperInvariant(c));
                else
                    result.Add(Char.ToLowerInvariant(c));

                wordBreak = false;
            }

            return new string(result.ToArray());
        }
    }
}
