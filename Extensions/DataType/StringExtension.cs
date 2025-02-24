using System;
using System.Text;

namespace MFramework.CommSystem.Extensions
{
    public static partial class StringExtension
    {
        public static byte[] EncodingToBytes(this string data, EncodingType encodingType)
        {
            byte[] encodedData;
            switch (encodingType)
            {
                case EncodingType.UTF7:
                    encodedData = Encoding.UTF7.GetBytes(data);
                    break;
                case EncodingType.UTF8:
                    encodedData = Encoding.UTF8.GetBytes(data);
                    break;
                case EncodingType.Unicode:
                    encodedData = Encoding.Unicode.GetBytes(data);
                    break;
                case EncodingType.BigEndianUnicode:
                    encodedData = Encoding.BigEndianUnicode.GetBytes(data);
                    break;
                case EncodingType.GBK:
                    encodedData = Encoding.GetEncoding("GBK").GetBytes(data);
                    break;
                case EncodingType.UTF32:
                    encodedData = Encoding.UTF32.GetBytes(data);
                    break;
                case EncodingType.ASCII:
                    encodedData = Encoding.ASCII.GetBytes(data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(encodingType), encodingType, null);
            }

            return encodedData;
        }
    }
}