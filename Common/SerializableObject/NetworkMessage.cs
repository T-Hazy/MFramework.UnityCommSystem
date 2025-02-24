using System;
using MFramework.CommSystem;
using Unity.VisualScripting;

namespace MFramework.Network.Core
{
    public class NetworkMessage : SerializableObject
    {
        public NetworkMessageHeader Header { get; private set; }
        public byte[] Body { get; private set; }

        /// <summary>
        /// 验证消息是否完整 TODO 判断逻辑完善
        /// </summary>
        public bool IsComplete => IsDeserialized && Header.BodyLength == Body.Length &&
                                  (Body.Length > 0 && Header.BodyLength > 0);


        public NetworkMessage(byte[] data) : base(data) {
        }

        public NetworkMessage(NetworkMessageHeader header, byte[] body) {
            Header = header == null ? new NetworkMessageHeader(body.Length, 0, 0, 0, GenerationTimestamp(), 0) : header;
            Body = body;
        }

        // /// <summary>
        // /// 创建消息容器：用于接收端将接收到的消息封装成消息容器，然后进行反序列化
        // /// </summary>
        // /// <returns></returns>
        // public static NetworkMessage CreateContainer(byte[] data) {
        //     return new NetworkMessage(true, data);
        // }
        //
        // /// <summary>
        // /// 创建消息实例：用于发送端将需要发送的内容封装成消息实例
        // /// </summary>
        // public static NetworkMessage CreateMessage(byte[] data) {
        //     return new NetworkMessage(data);
        // }

        private long GenerationTimestamp() {
            return long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        protected override int GetFieldsContainerSize() {
            return Header.GetDataContainerSize() + sizeof(int) + Body.Length;
        }

        protected override void SerializeFields() {
            //SerializeSerializableObject(Header);
            SerializeBytes(Header.Serialize());
            SerializeBytes(Body);
        }

        protected override void DeserializeFields() {
            //Header = DeserializeSerializableObject<NetworkMessageHeader>();
            var header = new NetworkMessageHeader(DeserializeBytes());
            header.Deserialize();
            Header = header;
            Body = DeserializeBytes();
        }
    }
}