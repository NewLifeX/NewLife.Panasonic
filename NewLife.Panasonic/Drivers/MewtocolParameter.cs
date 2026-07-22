using System.ComponentModel;
using System.IO.Ports;

namespace NewLife.IoT.Drivers;

/// <summary>
/// Mewtocol 协议参数。配置松下 PLC Mewtocol 原生协议连接参数，
/// 支持 TCP 和串口两种传输方式：
/// TCP 模式：设置 <see cref="Server"/>（如 "192.168.1.100:9094"）
/// 串口模式：设置 <see cref="PortName"/>（如 "COM3"）和 <see cref="Baudrate"/>
/// 驱动根据已填字段自动选择传输方式。
/// </summary>
public class MewtocolParameter : IDriverParameter, IDriverParameterKey
{
    /// <summary>服务器地址。tcp地址如 192.168.1.100:9094，填写此字段表示使用 TCP 模式</summary>
    [Description("服务器地址。tcp地址如 192.168.1.100:9094，填写此字段表示使用 TCP 模式")]
    public String Server { get; set; }

    /// <summary>站号（0~99）。默认 1</summary>
    [Description("站号（0~99）。默认 1")]
    public Byte Station { get; set; } = 1;

    /// <summary>块号。默认 01</summary>
    [Description("块号。默认 01")]
    public String Block { get; set; } = "01";

    /// <summary>超时时间（毫秒）。默认 5000ms</summary>
    [Description("超时时间（毫秒）。默认 5000ms")]
    public Int32 Timeout { get; set; } = 5000;

    /// <summary>串口名。如 COM1（Windows）或 /dev/ttyS0（Linux），填写此字段表示使用串口模式</summary>
    [Description("串口名。如 COM1（Windows）或 /dev/ttyS0（Linux），填写此字段表示使用串口模式")]
    public String PortName { get; set; }

    /// <summary>波特率。串口模式有效，默认 9600</summary>
    [Description("波特率。串口模式有效，默认 9600")]
    public Int32 Baudrate { get; set; } = 9600;

    /// <summary>数据位。串口模式有效，默认 8</summary>
    [Description("数据位。串口模式有效，默认 8")]
    public Int32 DataBits { get; set; } = 8;

    /// <summary>奇偶校验位。串口模式有效，默认 None 无校验</summary>
    [Description("奇偶校验位。串口模式有效，默认 None 无校验")]
    public Parity Parity { get; set; } = Parity.None;

    /// <summary>停止位。串口模式有效，默认 One</summary>
    [Description("停止位。串口模式有效，默认 One")]
    public StopBits StopBits { get; set; } = StopBits.One;

    /// <summary>
    /// 获取唯一标识，用于驱动工厂判断是否可复用已有驱动实例。
    /// TCP 模式返回 Server 地址，串口模式返回 PortName 串口名。
    /// </summary>
    /// <returns>Server 地址（TCP）或 PortName 串口名（串口）；两者均为空时返回 null</returns>
    public String GetKey() => !Server.IsNullOrEmpty() ? Server : PortName;
}
