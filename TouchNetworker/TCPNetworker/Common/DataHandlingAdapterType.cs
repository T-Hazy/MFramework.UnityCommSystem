namespace MFramework.CommSystem.TouchNetworker
{
    /// <summary>
    /// 数据处理适配器类型：https://touchsocket.net/docs/current/packageadapter
    /// </summary>
    public enum DataHandlingAdapterType
    {
        /// <summary>
        /// 正常数据处理适配器(不对数据做适配处理)
        /// </summary>
        Normal,

        /// <summary>
        /// 固定包头数据处理适配器
        /// </summary>
        FixedHeader,

        /// <summary>
        /// 固定长度数据处理适配器
        /// </summary>
        FixedSize,

        /// <summary>
        /// 终止因子数据处理适配器
        /// </summary>
        Terminator,

        /// <summary>
        /// 周期数据处理适配器
        /// </summary>
        Period,

        /// <summary>
        /// Json格式数据处理适配器
        /// </summary>
        Json
    }
}