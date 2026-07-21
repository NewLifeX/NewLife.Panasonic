using System.ComponentModel;
using NewLife.IoT.Drivers;

namespace NewLife.Panasonic.Drivers;

/// <summary>
/// 松下PLC串口参数。配置 Modbus RTU 串口连接参数，继承 ModbusParameter 以获得标准 Modbus 参数属性（Host/ReadCode/WriteCode/Timeout）
/// </summary>
public class PanasonicRtuParameter : ModbusParameter
{
    /// <summary>串口名。如 COM1（Windows）或 /dev/ttyS0（Linux）</summary>
    [Description("串口名。如 COM1（Windows）或 /dev/ttyS0（Linux）")]
    public String PortName { get; set; }

    /// <summary>波特率。默认 9600</summary>
    [Description("波特率。默认 9600")]
    public Int32 Baudrate { get; set; } = 9600;

    /// <summary>数据位。默认 8</summary>
    [Description("数据位。默认 8")]
    public Int32 DataBits { get; set; } = 8;
}
