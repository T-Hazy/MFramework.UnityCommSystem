using System;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class TestUdpClient : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public InputField inputField_IpHost;
    public InputField inputField_msg;

    private UdpSession m_udpSession;

    public void Send()
    {
        try
        {
            this.m_udpSession.Send(inputField_msg.text);
        }
        catch (Exception ex)
        {
            UnityLog.Logger.Exception(ex);
        }
    }

    public void InitUdp()
    {
        try
        {
            m_udpSession.SafeDispose();//提前释放
            m_udpSession = new UdpSession();
            m_udpSession.Received += (remote, e) =>
            {
                UnityLog.Logger.Info($"Udp收到：{e.ByteBlock.Span.ToString(Encoding.UTF8)}");
                return Task.CompletedTask;
            };

            IPHost iPHost = new IPHost(this.inputField_IpHost.text);//配置默认原创地址，不设置时，需要在send时指定
            m_udpSession.Setup(new TouchSocketConfig()
                .SetUdpDataHandlingAdapter(() => new NormalUdpDataHandlingAdapter())//常规udp
                                                                                    //.SetUdpDataHandlingAdapter(() => new UdpPackageAdapter())//Udp包模式，支持超过64k数据。
                .SetBindIPHost(new IPHost(0))//0端口即为随机端口
                .SetRemoteIPHost(iPHost));

            m_udpSession.Start();
            UnityLog.Logger.Info($"Udp初始化完成");
        }
        catch (Exception ex)
        {
            UnityLog.Logger.Exception(ex);
        }
    }
}