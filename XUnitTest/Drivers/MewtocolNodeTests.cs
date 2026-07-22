using System.ComponentModel;
using NewLife.IoT.Drivers;
using Xunit;

namespace XUnitTest.Drivers;

/// <summary>MewtocolNode 单元测试</summary>
public class MewtocolNodeTests
{
    [Fact]
    [DisplayName("MewtocolNode_实现INode接口")]
    public void Implements_INode()
    {
        var node = new MewtocolNode();
        Assert.IsAssignableFrom<INode>(node);
    }

    [Fact]
    [DisplayName("新节点_默认属性为空")]
    public void NewNode_DefaultProperties()
    {
        var node = new MewtocolNode();

        Assert.Null(node.Client);
        Assert.Null(node.Stream);
        Assert.False(node.IsConnected);
        Assert.Null(node.Driver);
        Assert.Null(node.Device);
        Assert.Null(node.Parameter);
    }

    [Fact]
    [DisplayName("设置Client_IsConnected为False")]
    public void Client_NotSet_IsConnectedFalse()
    {
        var node = new MewtocolNode();
        Assert.False(node.IsConnected);
    }

    [Fact]
    [DisplayName("设置Driver_值正确保留")]
    public void SetDriver_ValueIsPreserved()
    {
        var node = new MewtocolNode();
        var driver = new MewtocolDriver();
        node.Driver = driver;

        Assert.NotNull(node.Driver);
        Assert.Same(driver, node.Driver);
    }

    [Fact]
    [DisplayName("设置Device_可设为null")]
    public void SetDevice_CanBeNull()
    {
        var node = new MewtocolNode();
        node.Device = null;
        Assert.Null(node.Device);
    }

    [Fact]
    [DisplayName("设置Parameter_值正确保留")]
    public void SetParameter_ValueIsPreserved()
    {
        var node = new MewtocolNode();
        var param = new MewtocolParameter
        {
            Server = "192.168.1.100:9094",
        };
        node.Parameter = param;

        Assert.NotNull(node.Parameter);
        Assert.Same(param, node.Parameter);
    }

    [Fact]
    [DisplayName("Dispose_Client为null_不抛出异常")]
    public void Dispose_NullClient_DoesNotThrow()
    {
        var node = new MewtocolNode();
        var exception = Record.Exception(() => node.Dispose());
        Assert.Null(exception);
    }

    [Fact]
    [DisplayName("新节点_Serial属性默认null")]
    public void NewNode_SerialDefaultNull()
    {
        var node = new MewtocolNode();
        Assert.Null(node.Serial);
    }

    [Fact]
    [DisplayName("新节点_未设置传输方式_IsConnected为False")]
    public void NewNode_NoTransport_IsConnectedFalse()
    {
        var node = new MewtocolNode();
        Assert.False(node.IsConnected);
    }
}
