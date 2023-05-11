using System.ComponentModel;
using NewLife.IoT.Protocols;
using NewLife.Panasonic.Drivers;
using NewLife.Serialization;

namespace NewLife.IoT.Drivers;

/// <summary>
/// 松下PLC协议封装
/// </summary>
[Driver("PanasonicPLC")]
[DisplayName("松下PLC")]
public class PanasonicDriver : ModbusDriver, IDriver
{
    #region 方法
    /// <summary>
    /// 创建Modbus通道
    /// </summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="node">设备节点</param>
    /// <param name="parameters">参数</param>
    /// <returns></returns>
    protected override Modbus CreateModbus(IDevice device, ModbusNode node, IDictionary<String, Object> parameters)
    {
        var p = JsonHelper.Convert<ModbusTcpParameter>(parameters);
        if (p.Server.IsNullOrEmpty()) throw new ArgumentException("参数中未指定地址Server");

        node.Parameter = p;

        var modbus = new ModbusTcp
        {
            Server = p.Server,
            ProtocolId = p.ProtocolId,

            Tracer = Tracer,
            Log = Log,
        };
        //modbus.Init(parameters);

        return modbus;
    }
    #endregion
}