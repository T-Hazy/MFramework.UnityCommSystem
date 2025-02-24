using TouchSocket.Sockets;

namespace MFramework.CommSystem
{
    public class SerializableObjectHandleEventArgs : DataHandlerEventArgs
    {
        public SerializableObject serializableObject { get; }

        protected SerializableObjectHandleEventArgs()
        {
        }

        public SerializableObjectHandleEventArgs(IClient client, ReceivedDataEventArgs dataArgs,
            SerializableObject serializableObject) : base(client, dataArgs)
        {
            this.serializableObject = serializableObject;
        }
    }
}