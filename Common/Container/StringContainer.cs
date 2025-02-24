using System;
using MFramework.CommSystem.Extensions;

namespace MFramework.CommSystem
{
    public class StringContainer: DataContainer
    {

        public string UTF8Result { get; }
        public string UTF7Result { get; }
        public string UTF32Result { get; }
        public string GBKResult { get; }
        public string ASCIIResult { get; }
        public string UnicodeResult { get; }
        public string BigEndianUnicodeResult { get; }

        public StringContainer(byte[] data): base(data)
        {
            UTF7Result = data.EncodingToString(EncodingType.UTF7);
            UTF8Result = data.EncodingToString(EncodingType.UTF8);
            UnicodeResult = data.EncodingToString(EncodingType.Unicode);
            BigEndianUnicodeResult = data.EncodingToString(EncodingType.BigEndianUnicode);
            GBKResult = data.EncodingToString(EncodingType.GBK);
            UTF32Result = data.EncodingToString(EncodingType.UTF32);
            ASCIIResult = data.EncodingToString(EncodingType.ASCII);
        }

        public StringContainer(string content, EncodingType encodingType)
        {
            data = content.EncodingToBytes(encodingType);
        }
    }
}