using System;
using System.Text;

namespace MFramework.CommSystem.Extensions
{
    public static partial class CharExpansion
    {
        public static byte[] EncodingToBytes(this char data, EncodingType encodingType)
        {
            var chars = new[] { data };
            byte[] encodedData;
            switch (encodingType)
            {
                case EncodingType.UTF7:
                    encodedData = Encoding.UTF7.GetBytes(chars);
                    break;
                case EncodingType.UTF8:
                    encodedData = Encoding.UTF8.GetBytes(chars);
                    break;
                case EncodingType.Unicode:
                    encodedData = Encoding.Unicode.GetBytes(chars);
                    break;
                case EncodingType.BigEndianUnicode:
                    encodedData = Encoding.BigEndianUnicode.GetBytes(chars);
                    break;
                case EncodingType.GBK:
                    encodedData = Encoding.GetEncoding("GBK").GetBytes(chars);
                    break;
                case EncodingType.UTF32:
                    encodedData = Encoding.UTF32.GetBytes(chars);
                    break;
                case EncodingType.ASCII:
                    encodedData = Encoding.ASCII.GetBytes(chars);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(encodingType), encodingType, null);
            }

            return encodedData;
        }
    }
}