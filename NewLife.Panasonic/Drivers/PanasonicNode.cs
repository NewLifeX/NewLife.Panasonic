using NewLife.IoT.Protocols;

namespace NewLife.IoT.Drivers;

/// <summary>
/// 松下PLC节点。继承 ModbusNode 获得标准节点功能，附加 Modbus 通信对象引用
/// </summary>
public class PanasonicNode : ModbusNode
{
    /// <summary>Modbus对象</summary>
    public Modbus Modbus { get; set; }
}