using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Sockets;
using UnityEngine;

namespace MFramework.CommSystem.TouchNetworker
{
    public class TCPCommClient : TcpClient
    {
        internal ConcurrentQueue<DataHandlerEventArgs> dataHandlerEventArgs { get; }
        internal ConcurrentQueue<SerializableObjectHandleEventArgs> serializableObjectHandleEventArgs { get; }
        internal ConcurrentQueue<StringHandlerEventArgs> stringHandlerEventArgs { get; }

        public TCPCommClient()
        {
            dataHandlerEventArgs = new ConcurrentQueue<DataHandlerEventArgs>();
            serializableObjectHandleEventArgs = new ConcurrentQueue<SerializableObjectHandleEventArgs>();
            stringHandlerEventArgs = new ConcurrentQueue<StringHandlerEventArgs>();
        }


        /// <summary>
        /// 即将连接到服务器：此时已经创建Socket，但是还未建立tcp。
        /// </summary>
        /// <param name="args">连接事件参数</param>
        protected override Task OnTcpConnecting(ConnectingEventArgs args)
        {
            Debug.Log($"【{DateTime.Now}】通信客户端(TCP)：正在连接目标主机{RemoteIPHost.EndPoint}");
            return base.OnTcpConnecting(args);
        }

        /// <summary>
        /// 成功连接到服务器
        /// </summary>
        /// <param name="args">连接事件参数</param>
        protected override Task OnTcpConnected(ConnectedEventArgs args)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"【{DateTime.Now}】通信客户端(TCP)：成功连接目标主机{RemoteIPHost.EndPoint}");
            builder.AppendLine($"本地端点(LocalEndPoint):{MainSocket.LocalEndPoint}");
            builder.AppendLine($"远程端点(RemoteEndPoint):{MainSocket.RemoteEndPoint}");
            Debug.Log(builder);
            return base.OnTcpConnected(args);
        }

        /// <summary>
        /// 从服务器收到信息。但是一般byteBlock和requestInfo会根据适配器呈现不同的值。
        /// </summary>
        /// <param name="args">接收数据事件参数</param>
        protected override Task OnTcpReceived(ReceivedDataEventArgs args)
        {
            dataHandlerEventArgs.Enqueue(new DataHandlerEventArgs(this, args));
            var data = args.ByteBlock.Span.ToArray();
            var container = new StringContainer(data);
            stringHandlerEventArgs.Enqueue(new StringHandlerEventArgs(this, args, container));
            var serializableObject = new SerializableObject(data);
            serializableObjectHandleEventArgs.Enqueue(
                new SerializableObjectHandleEventArgs(this, args, serializableObject));
            return base.OnTcpReceived(args);
        }

        protected override Task OnTcpClosing(ClosingEventArgs e)
        {
            Debug.Log($"【{DateTime.Now}】通信客户端(TCP)：正在断开目标主机{RemoteIPHost.EndPoint}的连接...");
            return base.OnTcpClosing(e);
        }

        /// <summary>
        /// 从服务器断开连接，当连接不成功时不会触发。
        /// </summary>
        /// <param name="args">关闭事件参数</param>
        protected override Task OnTcpClosed(ClosedEventArgs args)
        {
            Debug.Log($"【{DateTime.Now}】通信客户端(TCP)：与目标主机{RemoteIPHost.EndPoint}断开连接！");
            return base.OnTcpClosed(args);
        }
    }
}