using System.ComponentModel;
using NewLife.IoT.Drivers;

namespace NewLife.Panasonic.Drivers;

/// <summary>
/// 松下PLC连接参数。统一支持 Modbus TCP 和 Modbus RTU 两种连接方式，
/// 驱动根据已填字段自动选择协议（Server → TCP，PortName → RTU）。
/// 实现 <see cref="IDriverParameterKey"/> 以支持驱动工厂的连接复用。
/// </summary>
public class PanasonicParameter : ModbusParameter, IDriverParameterKey
{
    /// <summary>服务器地址。tcp地址如 127.0.0.1:502，填写此字段表示使用 Modbus TCP 协议</summary>
    [Description("服务器地址。tcp地址如 127.0.0.1:502，填写此字段表示使用 Modbus TCP 协议")]
    public String Server { get; set; }

    /// <summary>协议标识。默认0</summary>
    [Description("协议标识。默认0")]
    public UInt16 ProtocolId { get; set; }

    /// <summary>串口名。如 COM1（Windows）或 /dev/ttyS0（Linux），填写此字段表示使用 Modbus RTU 协议</summary>
    [Description("串口名。如 COM1（Windows）或 /dev/ttyS0（Linux），填写此字段表示使用 Modbus RTU 协议")]
    public String PortName { get; set; }

    /// <summary>波特率。默认 9600</summary>
    [Description("波特率。默认 9600")]
    public Int32 Baudrate { get; set; } = 9600;

    /// <summary>数据位。默认 8</summary>
    [Description("数据位。默认 8")]
    public Int32 DataBits { get; set; } = 8;

    /// <summary>
    /// 获取唯一标识，用于驱动工厂判断是否可复用已有驱动实例。
    /// TCP 模式返回 Server 地址（同地址共用一个 TCP 连接），
    /// RTU 模式返回 PortName 串口名（同串口共用一个串口实例）。
    /// </summary>
    /// <returns>Server 地址（TCP）或 PortName 串口名（RTU）；两者均为空时返回 null</returns>
    public String GetKey() => !Server.IsNullOrEmpty() ? Server : PortName;
}
