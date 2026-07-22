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

    /// <summary>
    /// 是否已连接到 PLC。底层基于 TcpClient.Connected 状态，
    /// true 表示 TCP 连接已建立，false 表示未连接或已断开。
    /// </summary>
    public Boolean IsConnected => Client?.Connected ?? false;

    /// <summary>关联的驱动</summary>
    public IDriver Driver { get; set; }

    /// <summary>逻辑设备</summary>
    public IDevice Device { get; set; }

    /// <summary>连接参数</summary>
    public IDriverParameter Parameter { get; set; }

    /// <summary>
    /// 释放 TCP 客户端和网络流资源。安全可重入，不会抛出异常。
    /// </summary>
    public void Dispose()
    {
        Stream.TryDispose();
        Client.TryDispose();
    }
}
