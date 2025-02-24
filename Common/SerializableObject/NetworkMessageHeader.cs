using MFramework.CommSystem;
using UnityEngine;

namespace MFramework.Network.Core
{
    public class NetworkMessageHeader : SerializableObject
    {
        public NetworkMessageHeader(byte[] data) : base(data) {
        }

        public NetworkMessageHeader(int length, int type, int id, int checksum, long timestamp, int priority) {
            BodyLength = length;
            Type = type;
            ID = id;
            Checksum = checksum;
            Timestamp = timestamp;
            Priority = priority;
        }

        /// <summary>
        /// 消息体长度
        /// </summary>
        public int BodyLength { get; private set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// 消息ID
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// 消息校验和
        /// </summary>
        public int Checksum { get; private set; }

        /// <summary>
        /// 消息时间戳
        /// </summary>
        public long Timestamp { get; private set; }

        /// <summary>
        /// 消息优先级
        /// </summary>
        public int Priority { get; private set; }

        protected override int GetFieldsContainerSize() {
            return sizeof(int) * 5 + sizeof(long);
        }

        protected override void SerializeFields() {
            SerializeInt(BodyLength);
            SerializeInt(Type);
            SerializeInt(ID);
            SerializeInt(Checksum);
            SerializeLong(Timestamp);
            SerializeInt(Priority);
        }

        protected override void DeserializeFields() {
            BodyLength = DeserializeInt();
            Type = DeserializeInt();
            ID = DeserializeInt();
            Checksum = DeserializeInt();
            Timestamp = DeserializeLong();
            Priority = DeserializeInt();
        }
    }
}