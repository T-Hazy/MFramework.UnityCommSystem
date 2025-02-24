using System;
using TouchSocket.Sockets;

namespace MFramework.CommSystem
{
    public class StringHandlerEventArgs : DataHandlerEventArgs
    {
        public StringContainer container { get; }
        protected StringHandlerEventArgs()
        {
        }

        public StringHandlerEventArgs(IClient client, ReceivedDataEventArgs dataArgs, StringContainer container) :
            base(client, dataArgs)
        {
            this.container = container;
        }
    }
}