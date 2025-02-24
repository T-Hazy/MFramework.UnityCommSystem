using System.Text;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace MFramework.CommSystem.Extensions
{
    public static class TCPListenOptionExtension
    {
        /// <summary>
        /// 设置正常数据处理适配器
        /// </summary>
        public static void SetNormalDataHandlingAdapter(this TcpListenOption listenOption) =>
            listenOption.Adapter = null;

        /// <summary>
        /// 设置固定包头适配器
        /// </summary>
        public static void SetFixedHeaderPackageAdapter(this TcpListenOption listenOption) =>
            listenOption.Adapter = () => new FixedHeaderPackageAdapter();

        /// <summary>
        /// 设置固定长度数据处理适配器
        /// </summary>
        public static void SetFixedSizePackageAdapter(this TcpListenOption listenOption, int fixedSize) =>
            listenOption.Adapter = () => new FixedSizePackageAdapter(fixedSize);

        /// <summary>
        /// 设置终止因子数据处理适配器
        /// </summary>
        public static void SetTerminatorPackageAdapter(this TcpListenOption listenOption, int minSize,
            byte[] terminatorCode) =>
            listenOption.Adapter = () => new TerminatorPackageAdapter(minSize, terminatorCode);

        /// <summary>
        /// 设置终止因子数据处理适配器
        /// </summary>
        public static void SetTerminatorPackageAdapter(this TcpListenOption listenOption, string terminator) =>
            listenOption.Adapter = () => new TerminatorPackageAdapter(terminator);

        /// <summary>
        /// 设置终止因子数据处理适配器
        /// </summary>
        public static void SetTerminatorPackageAdapter(this TcpListenOption listenOption, string terminator,
            Encoding encoding) =>
            listenOption.Adapter = () => new TerminatorPackageAdapter(terminator, encoding);

        /// <summary>
        /// 设置周期数据处理适配器
        /// </summary>
        public static void SetPeriodPackageAdapter(this TcpListenOption listenOption) =>
            listenOption.Adapter = () => new PeriodPackageAdapter();

        //设置Json格式数据处理适配器
        //TODO JsonPackageAdapter
    }
}