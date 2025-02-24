using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace MFramework.CommSystem
{
    //TODO 关于物理适配器的方法在存在虚拟适配器时可能会获取到错误的结果
    public static class NetworkUtility
    {
        /// <summary>
        /// 获取所有的网络适配器
        /// </summary>
        /// <returns></returns>
        public static List<NetworkAdapter> GetNetworkAdapters()
        {
            List<NetworkAdapter> adapters = new List<NetworkAdapter>();
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                adapters.Add(new NetworkAdapter(networkInterface));
            }

            return adapters;
        }

        /// <summary>
        /// 获取第一个物理网络适配器
        /// </summary>
        /// <returns></returns>
        public static NetworkAdapter GetDefaultPhysicalNetworkAdapter()
        {
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ValidatePhysicalAdapter(networkInterface))
                {
                    return new NetworkAdapter(networkInterface);
                }
            }

            return null;
        }

        /// <summary>
        /// 获取IP地址列表
        /// </summary>
        /// <param name="ipType">IP地址类型</param>
        /// <returns></returns>
        public static List<string> GetIPAddresses(IPAddressType ipType)
        {
            List<string> result = new List<string>();
            foreach (var networkAdapter in GetNetworkAdapters())
            {
                result.Add(ipType == IPAddressType.IPv4 ? networkAdapter.IPv4Address : networkAdapter.IPv6Address);
            }

            return result;
        }

        /// <summary>
        /// 获取物理适配器的IP地址列表
        /// </summary>
        /// <param name="ipType">IP地址类型</param>
        public static List<string> GetPhysicalIPAddresses(IPAddressType ipType)
        {
            List<string> result = new List<string>();
            foreach (var networkAdapter in GetNetworkAdapters())
            {
                if (!networkAdapter.IsPhysicalAdapter) continue;
                result.Add(ipType == IPAddressType.IPv4 ? networkAdapter.IPv4Address : networkAdapter.IPv6Address);
            }

            return result;
        }

        /// <summary>
        /// 获取第一个物理网卡的IP地址
        /// </summary>
        /// <param name="ipType">IP地址类型</param>
        public static string GetDefaultPhysicalIPAddress(IPAddressType ipType)
        {
            foreach (var networkAdapter in GetNetworkAdapters())
            {
                if (networkAdapter.IsPhysicalAdapter)
                {
                    return ipType == IPAddressType.IPv4 ? networkAdapter.IPv4Address : networkAdapter.IPv6Address;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取第一个无线网卡的IP地址
        /// </summary>
        /// <param name="ipType">IP地址类型</param>
        public static string GetDefaultWirelessIPAddress(IPAddressType ipType)
        {
            foreach (var networkAdapter in GetNetworkAdapters())
            {
                if (networkAdapter.IsWirelessAdapter)
                {
                    return ipType == IPAddressType.IPv4 ? networkAdapter.IPv4Address : networkAdapter.IPv6Address;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 验证物理网卡
        /// </summary>
        /// <param name="networkInterface">网络接口对象</param>
        /// <returns>物理网卡返回true，虚拟网卡返回false </returns>
        public static bool ValidatePhysicalAdapter(NetworkInterface networkInterface)
        {
            var networkInterfaceRegistryKey = GetAdapterRegistryKey(networkInterface);
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(networkInterfaceRegistryKey, false);
            if (registryKey != null)
            {
                // 如果前面有 PCI 就是本机的真实网卡 
                string fPnpInstanceID = registryKey.GetValue("PnpInstanceID", "").ToString();
                if (fPnpInstanceID.Length > 3 && fPnpInstanceID.Substring(0, 3) == "PCI")
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 验证是否为无线网卡
        /// </summary>
        /// <param name="networkInterface">网络接口对象</param>
        /// <returns>是无线网卡true，否则返回false</returns>
        public static bool ValidateWirelessAdapter(NetworkInterface networkInterface)
        {
            return networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211;
        }

        /// <summary>
        /// 验证IPv4地址格式
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <returns>符合格式返回true，否则返回false</returns>
        public static bool ValidateIPv4Address(string ipAddress)
        {
            var validIPRegex =
                new Regex(
                    @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
            return (ipAddress != "" && validIPRegex.IsMatch(ipAddress.Trim())) ? true : false;
        }

        /// <summary>
        /// 验证端口号是否属于保留端口
        /// </summary>
        /// <returns>符合范围返回true，否则返回false</returns>
        public static bool ValidateReservePort(int port)
        {
            return port >= 0 && port <= 1023;
        }

        /// <summary>
        /// 验证端口号是否属于注册端口
        /// </summary>
        /// <returns>符合范围返回true，否则返回false</returns>
        public static bool ValidateRegisteredPort(int port)
        {
            return port >= 1024 && port <= 49151;
        }

        /// <summary>
        /// 验证端口号是否属于动态端口
        /// </summary>
        /// <returns>符合范围返回true，否则返回false</returns>
        public static bool ValidateDynamicPort(int port)
        {
            return port >= 49152 && port <= 65535;
        }

        private static string GetAdapterRegistryKey(NetworkInterface networkInterface)
        {
            return "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" +
                   networkInterface.Id + "\\Connection";
        }
    }
}