using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MFramework.CommSystem.Samples
{
    public class NetworkAdaptorSample : MonoBehaviour
    {
        [TextArea(40,60)]public string networkAdaptorInformation;

        private void Start()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var networkAdapter in NetworkUtility.GetNetworkAdapters())
            {
                builder.AppendLine($"适配器名称：{networkAdapter.Name}");
                builder.AppendLine($"描述：{networkAdapter.Description}");
                builder.AppendLine($"MAC地址：{networkAdapter.MACAddress}");
                builder.AppendLine($"是否为物理适配器：{networkAdapter.IsPhysicalAdapter}");
                builder.AppendLine($"是否为无线网络适配器：{networkAdapter.IsWirelessAdapter}");
                builder.AppendLine($"是否为标准以太网适配器：{networkAdapter.isEthernetAdapter}");
                builder.AppendLine($"IPv6地址：{networkAdapter.IPv6Address}");
                builder.AppendLine($"IPv4地址：{networkAdapter.IPv4Address}");
                builder.AppendLine($"子网掩码：{networkAdapter.SubnetMask}");
                builder.AppendLine($"默认网关：{networkAdapter.DefaultGateway}");
                builder.AppendLine($"首选DNS服务器：{networkAdapter.PreferredDNSServer}");
                builder.AppendLine();
            }

            networkAdaptorInformation = builder.ToString();
        }
    }
}