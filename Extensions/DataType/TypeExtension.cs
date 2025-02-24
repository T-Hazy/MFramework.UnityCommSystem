using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MFramework.CommSystem.Extensions
{
    public static partial class TypeExtension
    {
        public static byte[] EncodingToBytes(this Type type)
        {
            using var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, type);
            return stream.ToArray();
        }
    }
}