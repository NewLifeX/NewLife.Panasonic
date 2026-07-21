using System.ComponentModel;
using NewLife.IoT.Drivers;
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
}