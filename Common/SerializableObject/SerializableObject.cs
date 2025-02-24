using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MFramework.CommSystem.Extensions;

namespace MFramework.CommSystem
{
    public class SerializableObject : SerializableCore
    {
        protected SerializableObject() {
            SerializableObjectType = GetType();
        }

        public SerializableObject(byte[] data) {
            SerializedData = data;
            SerializableObjectType = DeserializeType();
        }

        public static List<Type> GetDerivedTypes<T>() {
            return Assembly.GetAssembly(typeof(T))
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T)))
                .ToList();
        }

        public static T CreateInstance<T>(byte[] data) where T : SerializableObject {
            var serializableObject = new SerializableObject(data);
            var serializableObjectInstance = Activator.CreateInstance(
                serializableObject.SerializableObjectType,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null, new object[] { data }, null) as T;
            if (serializableObjectInstance != null) {
                serializableObjectInstance.Deserialize();
            }

            return serializableObjectInstance;
        }

        protected override int GetFieldsContainerSize() {
            return 0;
        }

        protected override void SerializeFields() {
        }

        protected override void DeserializeFields() {
        }

        public override byte[] Serialize() {
            if (IsSerialized) return SerializedData;
            SerializeType();
            SerializeFields();
            MarkAsSerialized();
            return SerializedData;
        }

        public override int Deserialize(int startIndex = 0) {
            if (IsDeserialized) return DeserializeIndex;
            DeserializeIndex += startIndex;
            DeserializeFields();
            MarkAsDeserialized();
            return DeserializeIndex - startIndex;
        }

        public override int GetDataContainerSize() {
            //序列化存SerializableType的int
            //序列化SerializableType的长度
            return sizeof(int) + SerializableObjectType.EncodingToBytes().Length + GetFieldsContainerSize();
        }

        protected void SerializeType() {
            var data = SerializableObjectType.EncodingToBytes();
            SerializeInt(data.Length);
            data.CopyTo(SerializedData, SerializeIndex);
            SerializeIndex += data.Length;
        }

        protected Type DeserializeType() {
            var length = DeserializeInt();
            var typeData = new byte[length];
            Array.Copy(SerializedData, DeserializeIndex, typeData, 0, length);
            DeserializeIndex += length;
            return typeData.EncodingToType();
        }
    }
}