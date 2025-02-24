using System;
using System.Collections.Generic;
using MFramework.CommSystem.Extensions;
using TouchSocket.Core;
using TouchSocket.Sockets;
using UnityEngine;
using UnityEngine.Events;

namespace MFramework.CommSystem.TouchNetworker
{
    public class TCPClientNetworker : MonoBehaviour
    {
        [Header("目标主机(Target Host)")] public string hostIPAddress = "127.0.0.1";
        public int hostPort = 5001;

        [Header("重连机制(Reconnect Mechanism)"), Tooltip("断线重连模式")]
        public ReconnectionMode reconnectionMode = ReconnectionMode.Polling;

        [Tooltip("重连轮训间隔，只在重连模式为Polling时生效。")] public int reconnectPollingTime = 3;

        [Header("粘包与分包"), Tooltip("数据处理适配器类型")]
        public DataHandlingAdapterType dataHandlingAdapterType = DataHandlingAdapterType.Normal;

        public int fixedSize = 256;
        public string terminator = "\r\n";

        [Header("数据打印到控制台(Print Data To Console)")]
        public bool useDataPrintPlugin = true;

        public int maxPrintLength = 64;

        [Header("保活机制(Keep Connection)"), Tooltip("启用客户端活性检查")]
        public bool useCheckClear;

        [Tooltip("活性检查清理类型")] public CheckClearType checkClearType = CheckClearType.All;
        [Tooltip("检查清理时间")] public int checkClearTime = 60;
        public UnityEvent<(ITcpSession, CheckClearType)> onCheckClearClose;
        public DataHandler<ReceivedDataEventArgs> dataHandler { get; set; }
        public DataHandler<SerializableObject> serializableObjectHandler { get; set; }
        public StringHandler<StringContainer> stringHandler { get; set; }
        private TCPCommClient sessionClient;
        private readonly TouchSocketConfig config = new TouchSocketConfig();

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }


        private async void Start()
        {
            sessionClient.SafeDispose();
            sessionClient = new TCPCommClient();
            if (!ValidateIPHost()) return;
            config.SetRemoteIPHost($"{hostIPAddress}:{hostPort}");
            switch (dataHandlingAdapterType)
            {
                case DataHandlingAdapterType.Normal:
                    break;
                case DataHandlingAdapterType.FixedHeader:
                    config.SetTcpDataHandlingAdapter(() => new FixedHeaderPackageAdapter());
                    break;
                case DataHandlingAdapterType.FixedSize:
                    config.SetTcpDataHandlingAdapter(() => new FixedSizePackageAdapter(fixedSize));
                    break;
                case DataHandlingAdapterType.Terminator:
                    config.SetTcpDataHandlingAdapter(() => new TerminatorPackageAdapter(terminator));
                    break;
                case DataHandlingAdapterType.Period:
                    config.SetTcpDataHandlingAdapter(() => new PeriodPackageAdapter());
                    break;
                case DataHandlingAdapterType.Json:
                    //TODO 找到JsonPackageAdapter
                    //config.SetTcpDataHandlingAdapter(() => new JsonPackageAdapter(Encoding.UTF8));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            config.ConfigureContainer(registrar => { registrar.AddConsoleLogger(); });
            config.ConfigurePlugins(manager =>
            {
                if (useDataPrintPlugin) manager.Add<DataPrintingPlugin>().maxPrintLength = maxPrintLength;
                //重连机制
                switch (reconnectionMode)
                {
                    case ReconnectionMode.None:
                        break;
                    case ReconnectionMode.Trigger:
                        manager.UseTcpReconnection();
                        break;
                    case ReconnectionMode.Polling:
                        manager.UseTcpReconnection().UsePolling(TimeSpan.FromSeconds(reconnectPollingTime));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                //活性检测机制(心跳检测)
                if (useCheckClear)
                {
                    manager.UseCheckClear().SetCheckClearType(checkClearType)
                        .SetTick(TimeSpan.FromSeconds(checkClearTime))
                        .SetOnClose((session, type) => onCheckClearClose.Invoke((session, type)));
                }
            });
            await sessionClient.SetupAsync(config);
            await sessionClient.TryConnectAsync();
        }


        private void Update()
        {
            if (sessionClient.dataHandlerEventArgs.TryDequeue(out var dataHandlerArgs))
                dataHandler?.Invoke(dataHandlerArgs.dataArgs);
            if (sessionClient.serializableObjectHandleEventArgs.TryDequeue(out var serializableObjectHandleArgs))
                serializableObjectHandler?.Invoke(serializableObjectHandleArgs.serializableObject);
            if (sessionClient.stringHandlerEventArgs.TryDequeue(out var stringHandlerArgs))
                stringHandler?.Invoke(stringHandlerArgs.container);
        }

        private async void OnApplicationQuit()
        {
            await sessionClient?.CloseAsync()!;
            sessionClient?.Dispose();
        }

        public void Send(ReadOnlyMemory<byte> memory) => sessionClient.Send(memory);
        public void Send(List<ArraySegment<byte>> transferBytes) => sessionClient.Send(transferBytes);
        public void Send(IRequestInfo requestInfo) => sessionClient.Send(requestInfo);
        public void Send(string content) => sessionClient.Send(content);
        public void Send(string content, EncodingType encodingType) => Send(content.EncodingToBytes(encodingType));
        public void SendAsync(ReadOnlyMemory<byte> memory) => sessionClient.SendAsync(memory);
        public void SendAsync(List<ArraySegment<byte>> transferBytes) => sessionClient.SendAsync(transferBytes);
        public void SendAsync(IRequestInfo requestInfo) => sessionClient.SendAsync(requestInfo);
        public void SendAsync(string content) => sessionClient.SendAsync(content);

        public void SendAsync(string content, EncodingType encodingType) =>
            SendAsync(content.EncodingToBytes(encodingType));


        private bool ValidateIPHost()
        {
            if (!NetworkUtility.ValidateIPv4Address(hostIPAddress))
            {
                Debug.LogError($"【{DateTime.Now}】会话客户端(TCP)：IPv4地址{hostIPAddress}不合法.");
                return false;
            }

            if (!NetworkUtility.ValidateRegisteredPort(hostPort))
            {
                Debug.LogWarning($"【{DateTime.Now}】会话客户端(TCP)：目标端口号{hostPort}不属于注册端口范围(1024~49151)，可能发生错误、冲突或占用！");
            }

            return true;
        }
    }
}