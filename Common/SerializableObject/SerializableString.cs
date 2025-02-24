using MFramework.CommSystem;
using MFramework.CommSystem.Extensions;

namespace MFramework.Network.Core
{
    public class SerializableString : SerializableObject
    {
        public EncodingType EncodingType { get; private set; }
        public string Content { get; private set; }

        public SerializableString(byte[] data) : base(data) {
        }

        protected override int GetFieldsContainerSize() {
            return sizeof(int) + sizeof(int) + Content.EncodingToBytes(EncodingType).Length;
        }

        protected override void SerializeFields() {
            // SerializeInt((int)EncodingType);
            SerializeEnum(EncodingType);
            SerializeString(Content, EncodingType);
        }

        protected override void DeserializeFields() {
            EncodingType = DeserializeEnum<EncodingType>();
            Content = DeserializeString(EncodingType);
        }

        public SerializableString(string value, EncodingType encodingType) {
            Content = value;
            EncodingType = encodingType;
        }
    }
}