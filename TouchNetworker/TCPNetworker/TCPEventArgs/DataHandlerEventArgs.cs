using System;
using TouchSocket.Sockets;

namespace MFramework.CommSystem
{
    public class DataHandlerEventArgs : EventArgs
    {
        public IClient client { get; }
        public ReceivedDataEventArgs dataArgs { get; }

        protected DataHandlerEventArgs()
        {
        }

        public DataHandlerEventArgs(IClient client, ReceivedDataEventArgs dataArgs)
        {
            this.client = client;
            this.dataArgs = dataArgs;
        }
    }
}