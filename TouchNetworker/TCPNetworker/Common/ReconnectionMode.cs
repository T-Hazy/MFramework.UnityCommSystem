namespace MFramework.CommSystem.TouchNetworker
{
    public enum ReconnectionMode
    {
        /// <summary>
        /// 不使用断线重连
        /// </summary>
        None,

        /// <summary>
        /// 触发型重连:依靠的是Tcp断开事件（Closed）发生时，再次尝试连接。
        /// 1.必须完成第一次连接。
        /// 2.必须是被动断开，如果是客户端主动调用Close、Disposed等方法主动断开的话，一般不会生效。
        /// 3.必须有显式的断开信息，也就是说，直接拔网线的话，不会立即生效，会等tcp保活到期后再生效。
        /// </summary>
        Trigger,

        /// <summary>
        /// 轮询式断线重连，是一种无人值守的连接方式，它不要求首次连接。
        /// 必须有显式的断开信息，也就是说，直接拔网线的话，也不会立即生效，会等tcp保活到期后再生效。
        /// </summary>
        Polling
    }
}