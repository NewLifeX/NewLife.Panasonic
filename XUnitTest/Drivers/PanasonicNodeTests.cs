using NewLife.IoT.Drivers;
using NewLife.IoT.Protocols;
using Xunit;

namespace XUnitTest.Drivers;

/// <summary>
/// PanasonicNode 单元测试
/// </summary>
public class PanasonicNodeTests
{
    [Fact]
    [DisplayName("PanasonicNode_继承ModbusNode并实现INode")]
    public void Inherits_ModbusNode()
    {
        var node = new PanasonicNode();

        Assert.IsAssignableFrom<ModbusNode>(node);
        Assert.IsAssignableFrom<INode>(node);
    }

    [Fact]
    [DisplayName("新节点_默认属性为空")]
    public void NewNode_DefaultProperties()
    {
        var node = new PanasonicNode();

        Assert.Null(node.Modbus);
        Assert.Null(node.Driver);
        Assert.Null(node.Device);
        Assert.Null(node.Parameter);
        Assert.Equal(0, node.Host);
        Assert.Equal(default, node.ReadCode);
        Assert.Equal(default, node.WriteCode);
    }

    [Fact]
    [DisplayName("设置Host_值正确保留")]
    public void SetHost_ValueIsPreserved()
    {
        var node = new PanasonicNode { Host = 2 };
        Assert.Equal(2, node.Host);
    }

    [Fact]
    [DisplayName("设置Modbus_值正确保留")]
    public void SetModbus_ValueIsPreserved()
    {
        var node = new PanasonicNode();

        // 模拟ModbusTcp对象
        var modbus = new ModbusTcp
        {
            Server = "127.0.0.1:502",
            Host = 1,
        };
        node.Modbus = modbus;

        Assert.NotNull(node.Modbus);
        Assert.Same(modbus, node.Modbus);
        Assert.Equal("127.0.0.1:502", node.Modbus.Server);
        Assert.Equal(1, node.Modbus.Host);
    }

    [Fact]
    [DisplayName("设置ReadCode_值正确保留")]
    public void SetReadCode_ValueIsPreserved()
    {
        var node = new PanasonicNode { ReadCode = FunctionCodes.ReadCoil };
        Assert.Equal(FunctionCodes.ReadCoil, node.ReadCode);
    }

    [Fact]
    [DisplayName("设置WriteCode_值正确保留")]
    public void SetWriteCode_ValueIsPreserved()
    {
        var node = new PanasonicNode { WriteCode = FunctionCodes.WriteCoil };
        Assert.Equal(FunctionCodes.WriteCoil, node.WriteCode);
    }

    [Fact]
    [DisplayName("设置Driver_值正确保留")]
    public void SetDriver_ValueIsPreserved()
    {
        var node = new PanasonicNode();
        var driver = new PanasonicDriver();
        node.Driver = driver;

        Assert.NotNull(node.Driver);
        Assert.Same(driver, node.Driver);
    }

    [Fact]
    [DisplayName("设置Device_值正确保留")]
    public void SetDevice_ValueIsPreserved()
    {
        var node = new PanasonicNode();
        var device = new MockDevice();
        node.Device = device;

        Assert.NotNull(node.Device);
        Assert.Same(device, node.Device);
    }

    [Fact]
    [DisplayName("Modbus为null_释放不抛出异常")]
    public void Dispose_NullModbus_DoesNotThrow()
    {
        var node = new PanasonicNode();

        // Modbus 为 null 时释放应该安全
        var exception = Record.Exception(() => node.Dispose());
        Assert.Null(exception);
    }

    /// <summary>
    /// 模拟设备，用于测试
    /// </summary>
    private class MockDevice : IDevice
    {
        public String Code { get; set; } = "MockDevice";
        public String Name { get; set; } = "Mock Device";
        public Int32 Sort { get; set; }
        public Boolean Enable { get; set; } = true;
        public Boolean IsOnline { get; set; }
        public DateTime LastOpen { get; set; }
        public DateTime LastTime { get; set; }
        public Int32 Total { get; set; }
    }
}