using System;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;
using UnityEngine;

namespace MFramework.CommSystem.TouchNetworker
{
    /// <summary>
    /// 将接收数据的指定长度解析为字符串并输出到控制台插件
    /// </summary>
    public class DataPrintingPlugin : PluginBase, ITcpReceivingPlugin, IUdpReceivedPlugin
    {
        public int maxPrintLength { get; set; } = 64;

        public Task OnTcpReceiving(ITcpSession client, ByteBlockEventArgs args)
        {
            var data = args.ByteBlock.Span.ToArray();
            var copyLength = Math.Min(data.Length, maxPrintLength);
            var tempData = new byte[copyLength];
            Array.Copy(data, tempData, copyLength);
            var tempContainer = new StringContainer(tempData);
            var builder = new StringBuilder();
            builder.AppendLine($"【{DateTime.Now}】会话客户端(TCP)：接收到来自{client.GetIPPort()}的{data.Length}字节数据！");
            builder.AppendLine($"字符串解析结果(最大长度{maxPrintLength}字节)：");
            builder.AppendLine($"UTF8Result：{tempContainer.UTF8Result}");
            builder.AppendLine($"UTF7Result：{tempContainer.UTF7Result}");
            builder.AppendLine($"UTF32Result：{tempContainer.UTF32Result}");
            builder.AppendLine($"GBKResult：{tempContainer.GBKResult}");
            builder.AppendLine($"ASCIIResult：{tempContainer.ASCIIResult}");
            builder.AppendLine($"UnicodeResult：{tempContainer.UnicodeResult}");
            builder.AppendLine($"BigEndianUnicodeResult：{tempContainer.BigEndianUnicodeResult}");
            Debug.Log(builder);
            return Task.CompletedTask;
        }

        public Task OnUdpReceived(IUdpSessionBase client, UdpReceivedDataEventArgs args)
        {
            var data = args.ByteBlock.Span.ToArray();
            var copyLength = Math.Min(data.Length, maxPrintLength);
            var tempData = new byte[copyLength];
            Array.Copy(data, tempData, copyLength);
            var tempContainer = new StringContainer(tempData);
            var builder = new StringBuilder();
            builder.AppendLine($"【{DateTime.Now}】会话客户端(UDP)：接收到{data.Length}字节数据！");
            builder.AppendLine($"字符串解析结果(最大长度{maxPrintLength}字节)：");
            builder.AppendLine($"UTF8Result：{tempContainer.UTF8Result}");
            builder.AppendLine($"UTF7Result：{tempContainer.UTF7Result}");
            builder.AppendLine($"UTF32Result：{tempContainer.UTF32Result}");
            builder.AppendLine($"GBKResult：{tempContainer.GBKResult}");
            builder.AppendLine($"ASCIIResult：{tempContainer.ASCIIResult}");
            builder.AppendLine($"UnicodeResult：{tempContainer.UnicodeResult}");
            builder.AppendLine($"BigEndianUnicodeResult：{tempContainer.BigEndianUnicodeResult}");
            Debug.Log(builder);
            return Task.CompletedTask;
        }
    }
}