using System;
using MFramework.CommSystem.Extensions;

namespace MFramework.CommSystem
{
    public abstract class SerializableCore
    {
        public Type SerializableObjectType { get; protected set; }

        /// <summary>
        /// 序列化索引
        /// </summary>
        protected int SerializeIndex { get; set; }

        /// <summary>
        /// 已序列化
        /// </summary>
        protected bool IsSerialized { get; set; }

        /// <summary>
        /// 反序列化索引
        /// </summary>
        protected int DeserializeIndex { get; set; }

        /// <summary>
        /// 已反序列化
        /// </summary>
        protected bool IsDeserialized { get; set; }

        /// <summary>
        /// 序列化数据
        /// </summary>
        private byte[] data;

        /// <summary>
        /// 数据包，创建情况有一下两种：
        /// 1.在子类的构造函数中初始化，此时大小和数据内容由<see cref="SerializableObject"/>派生类型的
        /// GetDataContainerSize()方法和Serialize()方法决定。
        /// 2.在创建SerializableData实例时由构造函数中传入，通常作为接收数据包时的容器，方便反序列化。
        /// </summary>
        protected byte[] SerializedData
        {
            get => data == null ? (data = new byte[GetDataContainerSize()]) : data;
            set => data = value;
        }

        /// <summary>
        /// 获取字段容器的大小
        /// </summary>
        /// <returns></returns>
        protected abstract int GetFieldsContainerSize();

        /// <summary>
        /// 序列化字段成员
        /// </summary>
        protected abstract void SerializeFields();

        /// <summary>
        /// 反序列化字段成员
        /// </summary>
        protected abstract void DeserializeFields();

        public abstract byte[] Serialize();

        public abstract int Deserialize(int startIndex = 0);

        protected void MarkAsDeserialized()
        {
            IsDeserialized = true;
            IsSerialized = false;
            SerializeIndex = 0;
        }

        protected void MarkAsSerialized()
        {
            IsSerialized = true;
            IsDeserialized = false;
            DeserializeIndex = 0;
        }

        /// <summary>
        /// 容量包括父类中的SerializableObjectType，Type类型的存储方法和string相同。
        /// </summary>
        public abstract int GetDataContainerSize();

        #region 序列化值类型和引用类型

        protected void SerializeInt(int value)
        {
            BitConverter.GetBytes(value).CopyTo(SerializedData, SerializeIndex);
            SerializeIndex += sizeof(int);
        }

        protected void SerializeUInt(uint value)
        {
            BitConverter.GetBytes(value).CopyTo(SerializedData, SerializeIndex);
            SerializeIndex += sizeof(uint);
        }

        protected void SerializeShort(short value)
        {
            BitConverter.GetBytes(value).CopyTo(SerializedData, SerializeIndex);
            SerializeIndex += sizeof(short);
        }

        protected void SerializeUShort(ushort value)
        {
            BitConverter.GetBytes(value).CopyTo(SerializedData, SerializeIndex);
            SerializeIndex += sizeof(ushort);
        }

        protected void SerializeLong(long value)
        {
            BitConverter.GetBytes(value).CopyTo(SerializedData, SerializeIndex);
            SerializeIndex += sizeof(long);
        }

        protected void SerializeULong(ulong value)
        {
            BitConverter.GetBytes(value).CopyTo(SerializedData, SerializeIndex);
            SerializeIndex += sizeof(ulong);
        }

        protected void SerializeByte(byte value)
        {
            SerializedData[SerializeIndex] = value;
            SerializeIndex += sizeof(byte);
        }

        protected void SerializeBytes(byte[] value)
        {
            SerializeInt(value.Length);
            value.CopyTo(SerializedData, SerializeIndex);
            SerializeIndex += value.Length;
        }


        protected void SerializeFloat(float value)
        {
            BitConverter.GetBytes(value).CopyTo(SerializedData, SerializeIndex);
            SerializeIndex += sizeof(float);
        }

        protected void SerializeDouble(double value)
        {
            BitConverter.GetBytes(value).CopyTo(SerializedData, SerializeIndex);
            SerializeIndex += sizeof(double);
        }

        protected void SerializeBool(bool value)
        {
            BitConverter.GetBytes(value).CopyTo(SerializedData, SerializeIndex);
            SerializeIndex += sizeof(bool);
        }

        protected void SerializeChar(char value, EncodingType encodingType)
        {
            var bytes = value.EncodingToBytes(encodingType);
            SerializeInt(bytes.Length);
            bytes.CopyTo(SerializedData, SerializeIndex);
            SerializeIndex += bytes.Length;
        }

        protected void SerializeString(string value, EncodingType encodingType)
        {
            var bytes = value.EncodingToBytes(encodingType);
            SerializeInt(bytes.Length);
            bytes.CopyTo(SerializedData, SerializeIndex);
            SerializeIndex += bytes.Length;
        }

        protected void SerializeEnum(Enum value)
        {
            SerializeInt(Convert.ToInt32(value));
        }

        protected void SerializeSerializableObject(SerializableObject serializableObject)
        {
            serializableObject.Serialize().CopyTo(SerializedData, SerializeIndex);
            SerializeIndex += serializableObject.GetDataContainerSize();
        }

        //TODO 数组、列表、字典、堆栈、队列等容器序列化

        #endregion

        #region 反序列化值类型和引用类型

        protected int DeserializeInt()
        {
            var value = BitConverter.ToInt32(SerializedData, DeserializeIndex);
            DeserializeIndex += sizeof(int);
            return value;
        }

        protected short DeserializeShort()
        {
            var value = BitConverter.ToInt16(SerializedData, DeserializeIndex);
            DeserializeIndex += sizeof(short);
            return value;
        }


        protected long DeserializeLong()
        {
            var value = BitConverter.ToInt64(SerializedData, DeserializeIndex);
            DeserializeIndex += sizeof(long);
            return value;
        }

        protected float DeserializeFloat()
        {
            var value = BitConverter.ToSingle(SerializedData, DeserializeIndex);
            DeserializeIndex += sizeof(float);
            return value;
        }

        protected double DeserializeDouble()
        {
            var value = BitConverter.ToDouble(SerializedData, DeserializeIndex);
            DeserializeIndex += sizeof(double);
            return value;
        }

        protected byte DeserializeByte()
        {
            var value = SerializedData[DeserializeIndex];
            DeserializeIndex += sizeof(byte);
            return value;
        }

        protected byte[] DeserializeBytes()
        {
            var length = DeserializeInt();
            var result = new byte[length];
            Array.Copy(SerializedData, DeserializeIndex, result, 0, length);
            DeserializeIndex += length;
            return result;
        }

        protected bool DeserializeBool()
        {
            var value = BitConverter.ToBoolean(SerializedData, DeserializeIndex);
            DeserializeIndex += sizeof(bool);
            return value;
        }

        protected string DeserializeString(EncodingType encodingType)
        {
            var length = DeserializeInt();
            var result = new byte[length];
            Array.Copy(SerializedData, DeserializeIndex, result, 0, length);
            DeserializeIndex += length;
            return result.EncodingToString(encodingType);
        }

        protected T DeserializeEnum<T>() where T : Enum
        {
            return (T)Enum.ToObject(typeof(T), DeserializeInt());
        }

        protected T DeserializeSerializableObject<T>() where T : SerializableObject, new()
        {
            T value = new T();
            DeserializeIndex += value.Deserialize(DeserializeIndex);
            return value;
        }

        //TODO 数组、列表、字典、堆栈、队列等容器序列化

        #endregion
    }
}