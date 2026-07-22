using System.IO.Ports;
using System.Net.Sockets;

namespace NewLife.IoT.Drivers;

/// <summary>
/// Mewtocol 设备节点。表示一台通过 Mewtocol 协议连接的松下 PLC 设备，
/// 支持 TCP 和串口两种传输方式。
/// </summary>
public class MewtocolNode : INode
{
    /// <summary>TCP 客户端连接（TCP 模式）</summary>
    public TcpClient Client { get; set; }

    /// <summary>串口（串口模式）</summary>
    public SerialPort Serial { get; set; }

    /// <summary>数据流。TCP 模式下指向 <see cref="NetworkStream"/>，串口模式下指向 <see cref="SerialPort.BaseStream"/></summary>
    public Stream Stream { get; set; }

    /// <summary>
    /// 是否已连接到 PLC。TCP 模式基于 TcpClient.Connected 状态，
    /// 串口模式基于 SerialPort.IsOpen 状态。
    /// </summary>
    public Boolean IsConnected => Client?.Connected ?? Serial?.IsOpen ?? false;

    /// <summary>关联的驱动</summary>
    public IDriver Driver { get; set; }

    /// <summary>逻辑设备</summary>
    public IDevice Device { get; set; }

    /// <summary>连接参数</summary>
    public IDriverParameter Parameter { get; set; }

    /// <summary>
    /// 释放 TCP 客户端、串口和流资源。安全可重入，不会抛出异常。
    /// </summary>
    public void Dispose()
    {
        Stream.TryDispose();
        Client.TryDispose();
        Serial.TryDispose();
    }
}
