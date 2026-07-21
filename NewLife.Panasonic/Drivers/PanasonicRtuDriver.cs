using System.ComponentModel;
using NewLife.IoT.Protocols;
using NewLife.Panasonic.Drivers;
using NewLife.Serial.Protocols;

namespace NewLife.IoT.Drivers;

/// <summary>
/// 松下PLC串口驱动。通过 Modbus RTU 协议实现串口连接松下 PLC Modbus 网关
/// </summary>
[Driver("PanasonicRTU")]
[DisplayName("松下PLC串口")]
public class PanasonicRtuDriver : ModbusDriver, IDriver
{
    #region 方法
    /// <summary>
    /// 创建 Modbus RTU 串口通道
    /// </summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="node">设备节点</param>
    /// <param name="parameter">参数</param>
    /// <returns>Modbus 实例</returns>
    protected override Modbus CreateModbus(IDevice device, ModbusNode node, ModbusParameter parameter)
    {
        var p = parameter as PanasonicRtuParameter;
        if (p == null) throw new ArgumentNullException(nameof(parameter));
        if (p.PortName.IsNullOrEmpty()) throw new ArgumentException("参数中未指定串口名PortName");

        node.Parameter = p;

        var modbus = new ModbusRtu
        {
            PortName = p.PortName,
            Baudrate = p.Baudrate,
            DataBits = p.DataBits,
            Timeout = p.Timeout,

            Tracer = Tracer,
            Log = Log,
        };

        return modbus;
    }
    #endregion
}
