using System;
using System.Net.NetworkInformation;

namespace MFramework.CommSystem
{
    /// <summary>
    /// 这个类用于获取部分网络适配器信息
    /// </summary>
    public class NetworkAdapter
    {
        /// <summary>
        /// 网络接口
        /// </summary>
        public readonly NetworkInterface networkInterface;

        public NetworkAdapter(NetworkInterface networkInterface)
        {
            this.networkInterface = networkInterface;
        }

        /// <summary>
        /// 网络适配器名称
        /// </summary>
        public string Name => networkInterface.Name;

        /// <summary>
        /// 网络适配器描述
        /// </summary>
        public string Description => networkInterface.Description;

        /// <summary>
        /// 网络适配器物理地址
        /// </summary>
        public string MACAddress => networkInterface.GetPhysicalAddress().ToString();

        /// <summary>
        /// 是否为标准以太网物理适配器(无法区分虚拟网络适配器)
        /// </summary>
        public bool isEthernetAdapter => networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet;

        /// <summary>
        /// 是否为千兆以太网物理适配器(无法区分虚拟网络适配器)
        /// </summary>
        public bool isGigabitEthernetAdapter => networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet;

        /// <summary>
        /// 是否为物理适配器(标准以太网物理适配器、无线网卡(Wi-Fi)物理适配器、千兆以太网物理适配器)(无法区分虚拟网络适配器)
        /// </summary>
        public bool IsPhysicalAdapter => networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                                         networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                                         networkInterface.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet;

        /// <summary>
        /// 是否为无线网卡(Wi-Fi)物理适配器
        /// </summary>
        public bool IsWirelessAdapter => networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211;

        /// <summary>
        /// IPv6地址
        /// </summary>
        public string IPv6Address => GetIPv6Address();

        /// <summary>
        /// IPv4地址
        /// </summary>
        public string IPv4Address => GetIPv4Address();

        /// <summary>
        /// 子网掩码
        /// </summary>
        public string SubnetMask => GetIPv4SubnetMask();

        /// <summary>
        /// 默认网关
        /// </summary>
        public string DefaultGateway => GetDefaultGateway();

        /// <summary>
        /// 首选DNS服务器地址
        /// </summary>
        public string PreferredDNSServer => networkInterface.GetIPProperties().DnsAddresses[0].ToString();

        private string GetIPv6Address()
        {
            foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    return ip.Address.ToString();
                }
            }

            return String.Empty;
        }

        private string GetIPv4Address()
        {
            foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.Address.ToString();
                }
            }

            return String.Empty;
        }

        private string GetIPv4SubnetMask()
        {
            foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.IPv4Mask.ToString();
                }
            }

            return String.Empty;
        }

        private string GetDefaultGateway()
        {
            foreach (GatewayIPAddressInformation gateway in networkInterface.GetIPProperties().GatewayAddresses)
            {
                return gateway.Address.ToString();
            }

            return String.Empty;
        }
    }
}