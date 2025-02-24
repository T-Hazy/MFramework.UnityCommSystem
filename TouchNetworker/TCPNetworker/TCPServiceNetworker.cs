using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;
using UnityEngine;

namespace MFramework.CommSystem.TouchNetworker
{
    public class TCPServiceNetworker : MonoBehaviour
    {
        public string listeningIPAddress = "127.0.0.1";
        public int mainListeningPort = 5001;
        public List<int> subListeningPorts = new List<int>();
        public bool useDataPrintPlugin = true;

        public int maxPrintLength = 64;
        public DataHandler<ReceivedDataEventArgs> dataHandler { get; set; }
        public DataHandler<SerializableObject> serializableObjectHandler { get; set; }
        public StringHandler<StringContainer> stringHandler { get; set; }
        private TCPCommService service;
        private readonly TouchSocketConfig config = new TouchSocketConfig();

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private async void Start()
        {
            service?.SafeDispose();
            service = new TCPCommService();
            config.ConfigureContainer(registrar => registrar.AddConsoleLogger());
            config.ConfigurePlugins(manager =>
            {
                if (useDataPrintPlugin) manager.Add<DataPrintingPlugin>().maxPrintLength = maxPrintLength;
            });
            await service.SetupAsync(config);
            await service.StartAsync();
            service.AddListen(AddListenOption($"{listeningIPAddress}:{mainListeningPort}"));
            foreach (var port in subListeningPorts)
            {
                service.AddListen(AddListenOption(new IPHost(port)));
            }

            stringHandler = (conten) =>
            {
                Debug.Log(conten.GBKResult);
                SendByIP("192.168.2.35", conten.GBKResult, EncodingType.GBK);
                return Task.CompletedTask;
            };
        }

        private void Update()
        {
            foreach (var sessionClient in service.Clients)
            {
                if (sessionClient.dataHandlerEventArgs.TryDequeue(out var dataHandlerArgs))
                    dataHandler?.Invoke(dataHandlerArgs.dataArgs);
                if (sessionClient.serializableObjectHandleEventArgs.TryDequeue(out var serializableObjectHandleArgs))
                    serializableObjectHandler?.Invoke(serializableObjectHandleArgs.serializableObject);
                if (sessionClient.stringHandlerEventArgs.TryDequeue(out var stringHandlerArgs))
                    stringHandler?.Invoke(stringHandlerArgs.container);
            }
        }

        private void OnApplicationQuit()
        {
            StopAsyncAndSafeDispose();
        }

        // /// <summary>
        // /// 设置可用主机信息：
        // /// 1.首先验证初始主机信息是否可用.
        // /// 2.初始不可用的情况使用默认物理适配器IP地址.
        // /// 3.均不可用则抛出异常.
        // /// </summary>
        // private bool SetUsableIPHost()
        // {
        //     if (ValidateIPHost()) return true;
        //     listeningIPAddress = NetworkUtility.GetDefaultPhysicalIPAddress(IPAddressType.IPv4);
        //     if (ValidateIPHost()) return true;
        //     throw new Exception("初始主机信息和默认物理适配器均无法进行服务器配置！");
        // }

        // private bool ValidateIPHost()
        // {
        //     if (!NetworkUtility.ValidateIPv4Address(listeningIPAddress))
        //     {
        //         Debug.LogError($"【{DateTime.Now}】通信服务器(TCP)：IPv4地址{listeningIPAddress}不合法.");
        //         return false;
        //     }
        //
        //     if (!NetworkUtility.ValidateRegisteredPort(mainListeningPort))
        //     {
        //         Debug.LogWarning(
        //             $"【{DateTime.Now}】通信服务器(TCP)：目标端口号{mainListeningPort}不属于注册端口范围(1024~49151)，可能发生错误、冲突或占用！");
        //     }
        //
        //     return true;
        // }

        #region Send API

        public void SendByID(string id, ReadOnlyMemory<byte> memory) => service.Send(id, memory);

        public void SendByID(string id, IRequestInfo requestInfo) => service.Send(id, requestInfo);

        public void SendByID(string id, string value) => service.Send(id, value);

        public void SendByID(string id, string content, EncodingType encodingType) =>
            service.Send(id, content, encodingType);

        public void SendAsyncByID(string id, ReadOnlyMemory<byte> memory) => service.SendAsync(id, memory);

        public void SendAsyncByID(string id, IRequestInfo requestInfo) => service.SendAsync(id, requestInfo);

        public void SendAsyncByID(string id, string value) => service.SendAsync(id, value);

        public void SendAsyncByID(string id, string content, EncodingType encodingType) =>
            service.SendAsync(id, content, encodingType);

        /// <summary>
        /// 按IP地址发送
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="memory">数据</param>
        public void SendByIP(string ip, ReadOnlyMemory<byte> memory)
        {
            var clients = service.Clients.Where(client => client.IP.Contains(ip));
            foreach (var client in clients) service.Send(client.Id, memory);
        }

        /// <summary>
        /// 按IP地址发送
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="requestInfo">请求信息数据</param>
        public void SendByIP(string ip, IRequestInfo requestInfo)
        {
            var clients = service.Clients.Where(client => client.IP.Contains(ip));
            foreach (var client in clients) service.Send(client.Id, requestInfo);
        }

        /// <summary>
        /// 按IP地址发送
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="content">内容</param>
        public void SendByIP(string ip, string content)
        {
            var clients = service.Clients.Where(client => client.IP.Contains(ip));
            foreach (var client in clients) service.Send(client.Id, content);
        }

        /// <summary>
        /// 按IP地址发送
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="content">内容</param>
        /// <param name="encodingType">编码方式</param>
        public void SendByIP(string ip, string content, EncodingType encodingType)
        {
            var clients = service.Clients.Where(client => client.IP.Contains(ip));
            foreach (var client in clients) service.Send(client.Id, content, encodingType);
        }

        /// <summary>
        /// 按IP地址异步发送
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="memory">数据</param>
        public void SendAsyncByIP(string ip, ReadOnlyMemory<byte> memory)
        {
            var clients = service.Clients.Where(client => client.IP.Contains(ip));
            foreach (var client in clients) service.SendAsync(client.Id, memory);
        }

        /// <summary>
        /// 按IP地址异步发送
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="requestInfo">请求信息数据</param>
        public void SendAsyncByIP(string ip, IRequestInfo requestInfo)
        {
            var clients = service.Clients.Where(client => client.IP.Contains(ip));
            foreach (var client in clients) service.SendAsync(client.Id, requestInfo);
        }

        /// <summary>
        /// 按IP地址异步发送
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="content">内容</param>
        public void SendAsyncByIP(string ip, string content)
        {
            var clients = service.Clients.Where(client => client.IP.Contains(ip));
            foreach (var client in clients) service.SendAsync(client.Id, content);
        }

        /// <summary>
        /// 按IP地址异步发送
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="content">内容</param>
        /// <param name="encodingType">编码方式</param>
        public void SendAsyncByIP(string ip, string content, EncodingType encodingType)
        {
            var clients = service.Clients.Where(client => client.IP.Contains(ip));
            foreach (var client in clients) service.SendAsync(client.Id, content, encodingType);
        }


        #region 考虑弃用

        // /// <summary>
        // /// 按IP和端口发送
        // /// </summary>
        // /// <param name="ipPort">IP端口示例：127.0.0.1:5001</param>
        // /// <param name="memory">数据</param>
        // public void SendByIPPort(string ipPort, ReadOnlyMemory<byte> memory)
        // {
        //     if (service.TryGetClientByIPPort(ipPort, out var sessionClient)) service.Send(sessionClient.Id, memory);
        // }
        //
        // /// <summary>
        // /// 按IP和端口发送
        // /// </summary>
        // /// <param name="ipPort">IP端口示例：127.0.0.1:5001</param>
        // /// <param name="requestInfo">请求信息数据</param>
        // public void SendByIPPort(string ipPort, IRequestInfo requestInfo)
        // {
        //     if (service.TryGetClientByIPPort(ipPort, out var sessionClient))
        //         service.Send(sessionClient.Id, requestInfo);
        // }
        //
        // /// <summary>
        // /// 按IP和端口发送
        // /// </summary>
        // /// <param name="ipPort">IP端口示例：127.0.0.1:5001</param>
        // /// <param name="content">内容</param>
        // public void SendByIPPort(string ipPort, string content)
        // {
        //     if (service.TryGetClientByIPPort(ipPort, out var sessionClient)) service.Send(sessionClient.Id, content);
        // }
        //
        // /// <summary>
        // /// 按IP和端口发送
        // /// </summary>
        // /// <param name="ipPort">IP端口示例：127.0.0.1:5001</param>
        // /// <param name="content">内容</param>
        // /// <param name="encodingType">编码方式</param>
        // public void SendByIPPort(string ipPort, string content, EncodingType encodingType)
        // {
        //     if (service.TryGetClientByIPPort(ipPort, out var sessionClient))
        //         service.Send(sessionClient.Id, content, encodingType);
        // }
        //
        // /// <summary>
        // /// 按IP和端口异步发送
        // /// </summary>
        // /// <param name="ipPort">IP端口示例：127.0.0.1:5001</param>
        // /// <param name="memory">数据</param>
        // public void SendAsyncByIPPort(string ipPort, ReadOnlyMemory<byte> memory)
        // {
        //     if (service.TryGetClientByIPPort(ipPort, out var sessionClient))
        //         service.SendAsync(sessionClient.Id, memory);
        // }
        //
        // /// <summary>
        // /// 按IP和端口异步发送
        // /// </summary>
        // /// <param name="ipPort">IP端口示例：127.0.0.1:5001</param>
        // /// <param name="requestInfo">请求信息数据</param>
        // public void SendAsyncByIPPort(string ipPort, IRequestInfo requestInfo)
        // {
        //     if (service.TryGetClientByIPPort(ipPort, out var sessionClient))
        //         service.SendAsync(sessionClient.Id, requestInfo);
        // }
        //
        // /// <summary>
        // /// 按IP和端口异步发送
        // /// </summary>
        // /// <param name="ipPort">IP端口示例：127.0.0.1:5001</param>
        // /// <param name="content">内容</param>
        // public void SendAsyncByIPPort(string ipPort, string content)
        // {
        //     if (service.TryGetClientByIPPort(ipPort, out var sessionClient))
        //         service.SendAsync(sessionClient.Id, content);
        // }
        //
        // /// <summary>
        // /// 按IP和端口异步发送
        // /// </summary>
        // /// <param name="ipPort">IP端口示例：127.0.0.1:5001</param>
        // /// <param name="content">内容</param>
        // /// <param name="encodingType">编码方式</param>
        // public void SendAsyncByIPPort(string ipPort, string content, EncodingType encodingType)
        // {
        //     if (service.TryGetClientByIPPort(ipPort, out var sessionClient))
        //         service.SendAsync(sessionClient.Id, content, encodingType);
        // }

        #endregion

        public void Broadcast(ReadOnlyMemory<byte> memory)
        {
            foreach (var sessionClient in service.Clients)
            {
                if (sessionClient.Online) sessionClient.Send(memory);
            }
        }

        public void Broadcast(IRequestInfo requestInfo)
        {
            foreach (var sessionClient in service.Clients)
            {
                if (sessionClient.Online) sessionClient.Send(requestInfo);
            }
        }

        public void Broadcast(string value)
        {
            foreach (var sessionClient in service.Clients)
            {
                if (sessionClient.Online) sessionClient.Send(value);
            }
        }

        public void Broadcast(string content, EncodingType encodingType)
        {
            foreach (var sessionClient in service.Clients)
            {
                if (sessionClient.Online) sessionClient.Send(new StringContainer(content, encodingType).data);
            }
        }

        public void BroadcastAsync(ReadOnlyMemory<byte> memory)
        {
            foreach (var sessionClient in service.Clients)
            {
                if (sessionClient.Online) sessionClient.SendAsync(memory);
            }
        }

        public void BroadcastAsync(IRequestInfo requestInfo)
        {
            foreach (var sessionClient in service.Clients)
            {
                if (sessionClient.Online) sessionClient.SendAsync(requestInfo);
            }
        }

        public void BroadcastAsync(string value)
        {
            foreach (var sessionClient in service.Clients)
            {
                if (sessionClient.Online) sessionClient.SendAsync(value);
            }
        }

        public void BroadcastAsync(string content, EncodingType encodingType)
        {
            foreach (var sessionClient in service.Clients)
            {
                if (sessionClient.Online) sessionClient.SendAsync(new StringContainer(content, encodingType).data);
            }
        }

        #endregion

        private static TcpListenOption AddListenOption(IPHost ipHost)
        {
            Debug.Log($"【{DateTime.Now}】通信服务器(TCP)：已启动服务器 {ipHost.EndPoint}");
            return new TcpListenOption()
            {
                IpHost = ipHost,
            };
        }

        private async void StopAsyncAndSafeDispose()
        {
            await service.StopAsync();
            Debug.Log($"【{DateTime.Now}】通信服务器(TCP)：已关闭服务器 {listeningIPAddress}:{mainListeningPort}");
            service?.SafeDispose();
        }
    }
}