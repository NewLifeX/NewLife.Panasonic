using System.Net.Sockets;

namespace NewLife.IoT.Drivers;

/// <summary>
/// Mewtocol 设备节点。表示一台通过 Mewtocol 协议连接的松下 PLC 设备
/// </summary>
public class MewtocolNode : INode
{
    /// <summary>TCP 客户端连接</summary>
    public TcpClient Client { get; set; }

    /// <summary>网络流</summary>
    public NetworkStream Stream { get; set; }

    /// <summary>是否已连接</summary>
    public Boolean IsConnected => Client?.Connected ?? false;

    /// <summary>关联的驱动</summary>
    public IDriver Driver { get; set; }

    /// <summary>逻辑设备</summary>
    public IDevice Device { get; set; }

    /// <summary>连接参数</summary>
    public IDriverParameter Parameter { get; set; }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Stream.TryDispose();
        Client.TryDispose();
    }
}
