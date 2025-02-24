using System.Collections.Concurrent;
using System.Threading.Tasks;
using TouchSocket.Sockets;

namespace MFramework.CommSystem.TouchNetworker
{
    public sealed class TCPSessionClient : TcpSessionClient
    {
        internal ConcurrentQueue<DataHandlerEventArgs> dataHandlerEventArgs { get; }
        internal ConcurrentQueue<SerializableObjectHandleEventArgs> serializableObjectHandleEventArgs { get; }
        internal ConcurrentQueue<StringHandlerEventArgs> stringHandlerEventArgs { get; }

        public string IPPort => $"{IP}:{Port}";

        public TCPSessionClient()
        {
            dataHandlerEventArgs = new ConcurrentQueue<DataHandlerEventArgs>();
            serializableObjectHandleEventArgs = new ConcurrentQueue<SerializableObjectHandleEventArgs>();
            stringHandlerEventArgs = new ConcurrentQueue<StringHandlerEventArgs>();
        }

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
    }
}