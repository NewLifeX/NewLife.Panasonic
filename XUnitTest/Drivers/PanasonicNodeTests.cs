using System.ComponentModel;
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
        };
        node.Modbus = modbus;

        Assert.NotNull(node.Modbus);
        Assert.Same(modbus, node.Modbus);
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
    [DisplayName("设置Device_可设为null")]
    public void SetDevice_CanBeNull()
    {
        var node = new PanasonicNode();

        // Device 属性可接受 null 值（未关联设备时）
        node.Device = null;
        Assert.Null(node.Device);
    }

    [Fact]
    [DisplayName("Modbus赋值_支持null清除")]
    public void Modbus_CanSetToNull()
    {
        var node = new PanasonicNode
        {
            Modbus = new ModbusTcp(),
        };

        Assert.NotNull(node.Modbus);

        // 可以重新设置为 null
        node.Modbus = null;
        Assert.Null(node.Modbus);
    }
}