using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace MFramework.CommSystem.Extensions
{
    public static partial class BytesExtension
    {
        public static Type EncodingToType(this byte[] data)
        {
            using var stream = new MemoryStream(data);
            var formatter = new BinaryFormatter();
            return (Type)formatter.Deserialize(stream);
        }

        public static string EncodingToString(this byte[] data, EncodingType encodingType)
        {
            switch (encodingType)
            {
                case EncodingType.UTF7:
                    return Encoding.UTF7.GetString(data);
                case EncodingType.UTF8:
                    return Encoding.UTF8.GetString(data);
                case EncodingType.Unicode:
                    return Encoding.Unicode.GetString(data);
                case EncodingType.BigEndianUnicode:
                    return Encoding.BigEndianUnicode.GetString(data);
                case EncodingType.GBK:
                    return Encoding.GetEncoding("GBK").GetString(data);
                case EncodingType.UTF32:
                    return Encoding.UTF32.GetString(data);
                case EncodingType.ASCII:
                    return Encoding.ASCII.GetString(data);
                default:
                    throw new ArgumentOutOfRangeException(nameof(encodingType), encodingType, null);
            }
        }

        public static char[] EncodingToChar(this byte[] data, EncodingType encodingType)
        {
            Decoder decoder = null;
            switch (encodingType)
            {
                case EncodingType.UTF7:
                    decoder = Encoding.UTF7.GetDecoder();
                    break;
                case EncodingType.UTF8:
                    decoder = Encoding.UTF8.GetDecoder();
                    break;
                case EncodingType.Unicode:
                    decoder = Encoding.Unicode.GetDecoder();
                    break;
                case EncodingType.BigEndianUnicode:
                    decoder = Encoding.BigEndianUnicode.GetDecoder();
                    break;
                case EncodingType.GBK:
                    decoder = Encoding.GetEncoding("GBK").GetDecoder();
                    break;
                case EncodingType.UTF32:
                    decoder = Encoding.UTF32.GetDecoder();
                    break;
                case EncodingType.ASCII:
                    decoder = Encoding.ASCII.GetDecoder();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(encodingType), encodingType, null);
            }

            var chars = new char[decoder.GetCharCount(data, 0, data.Length)];
            decoder.GetChars(data, 0, data.Length, chars, 0);
            return chars;
        }
    }
}