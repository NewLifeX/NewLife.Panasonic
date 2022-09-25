using System.ComponentModel;
using NewLife.IoT.Drivers;
using NewLife.IoT.Protocols;

namespace NewLife.Panasonic.Drivers;

/// <summary>松下PLC参数</summary>
public class PanasonicParameter : IDriverParameter
{
    /// <summary>主机/站号</summary>
    [Description("主机/站号")]
    public Byte Host { get; set; }

    /// <summary>读取功能码。若点位地址未指定区域，则采用该功能码</summary>
    [Description("读取功能码。若点位地址未指定区域，则采用该功能码")]
    public FunctionCodes ReadCode { get; set; }

    /// <summary>写入功能码。若点位地址未指定区域，则采用该功能码</summary>
    [Description("写入功能码。若点位地址未指定区域，则采用该功能码")]
    public FunctionCodes WriteCode { get; set; }

    /// <summary>地址。tcp地址如127.0.0.1:502</summary>
    [Description("地址。tcp地址如127.0.0.1:502")]
    public String Server { get; set; }

    /// <summary>协议标识。默认0</summary>
    [Description("协议标识。默认0")]
    public UInt16 ProtocolId { get; set; }
}
