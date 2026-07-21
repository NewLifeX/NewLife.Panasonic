using System.ComponentModel;

namespace NewLife.IoT.Drivers;

/// <summary>
/// Mewtocol 协议参数。配置松下 PLC Mewtocol 原生协议 TCP 连接参数
/// </summary>
public class MewtocolParameter : IDriverParameter
{
    /// <summary>服务器地址。tcp地址如 192.168.1.100:9094</summary>
    [Description("服务器地址。tcp地址如 192.168.1.100:9094")]
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
}
