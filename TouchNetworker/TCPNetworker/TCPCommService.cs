using System;
using System.Threading.Tasks;
using MFramework.CommSystem.TouchNetworker;
using TouchSocket.Sockets;
using UnityEngine;

namespace MFramework.CommSystem
{
    public class TCPCommService : TcpService<TCPSessionClient>
    {
        protected override Task OnTcpConnected(TCPSessionClient sessionClient, ConnectedEventArgs e)
        {
            Debug.Log($"【{DateTime.Now}】通信服务器(TCP)：客户端 {sessionClient.GetIPPort()} ID:{sessionClient.Id} 接入服务器！");
            return base.OnTcpConnected(sessionClient, e);
        }

        protected override Task OnTcpClosed(TCPSessionClient sessionClient, ClosedEventArgs e)
        {
            Debug.Log($"【{DateTime.Now}】通信服务器(TCP)：客户端 {sessionClient.GetIPPort()} ID:{sessionClient.Id} 断开服务器连接！");
            return base.OnTcpClosed(sessionClient, e);
        }

        protected override TCPSessionClient NewClient()
        {
            return new TCPSessionClient();
        }

        public void Send(string id, string content, EncodingType encodingType)
        {
            this.Send(id, new StringContainer(content, encodingType).data);
        }

        public void SendAsync(string id, string content, EncodingType encodingType)
        {
            this.SendAsync(id, new StringContainer(content, encodingType).data);
        }

        public bool TryGetClientByIPPort(string ipPort, out TcpSessionClient sessionClient)
        {
            foreach (var tcpSessionClient in Clients)
            {
                if (!tcpSessionClient.IPPort.Contains(ipPort)) continue;
                sessionClient = tcpSessionClient;
                return true;
            }

            sessionClient = null;
            return false;
        }

        public bool TryGetClientByIP(string ip, out TcpSessionClient sessionClient)
        {
            foreach (var tcpSessionClient in Clients)
            {
                if (!tcpSessionClient.IP.Contains(ip)) continue;
                sessionClient = tcpSessionClient;
                return true;
            }

            sessionClient = null;
            return false;
        }
    }
}