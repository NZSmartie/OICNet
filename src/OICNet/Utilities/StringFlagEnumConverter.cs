using System;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OICNet.Utilities
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class StringFlagEnumConverterOptionsAttribute : Attribute
    {
        public bool ReverseOrder { get; set; }
    }

    public class StringFlagEnumConverter : StringEnumConverter
    {
        public StringFlagEnumConverter()
            : base()
        { }

        public StringFlagEnumConverter(bool camelCaseText)
            : base(camelCaseText)
        { }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Enum e = (Enum)value;
#if !NETSTANDARD1_3
            var options = value.GetType().GetCustomAttribute<StringFlagEnumConverterOptionsAttribute>();
#else
            var options = value.GetType().GetTypeInfo().GetCustomAttribute<StringFlagEnumConverterOptionsAttribute>();
#endif
            writer.WriteStartArray();

            var members = Enum.GetValues(e.GetType());
            if (options?.ReverseOrder ?? false)
                Array.Reverse(members);

            foreach (Enum member in members)
                if(e.HasFlag(member) && Convert.ToInt32(member) != 0)
                    base.WriteJson(writer, member, serializer);

            writer.WriteEndArray();
        }


        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
#if !NETSTANDARD1_3
            bool isGenericType = objectType.IsGenericType;
#else
            bool isGenericType = objectType.GetTypeInfo().IsGenericType;
#endif
            bool isNullable = (isGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>));

            if (reader.TokenType == JsonToken.Null)
            {
                if(!isNullable)
                    throw new OicException($"Cannot convert null value to {objectType}.");
                return null;
            }

            if (reader.TokenType != JsonToken.StartArray)
                throw new OicException($"Expected JsonToken.StartArray, but instead got JsonToken.{reader.TokenType:G}.");
            // Read into the array
            reader.Read();

            Type t = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;
            var result = 0;

            while (reader.TokenType == JsonToken.String || reader.TokenType == JsonToken.Integer)
            {
                var value = (Enum)base.ReadJson(reader, objectType, existingValue, serializer);
                result |= Convert.ToInt32(value);
                reader.Read();
            }

            if(reader.TokenType != JsonToken.EndArray)
                throw new OicException($"Expected JsonToken.EndArray, but instead got JsonToken.{reader.TokenType:G}.");

            return Enum.ToObject(t, result);
        }

        public override bool CanConvert(Type objectType)
        {
            if (!base.CanConvert(objectType))
                return false;

#if !NETSTANDARD1_3
            return objectType.GetCustomAttribute<FlagsAttribute>() != null;
#else
            return objectType.GetTypeInfo().GetCustomAttribute<FlagsAttribute>() != null;
#endif
        }
    }
}
