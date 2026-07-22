using System.ComponentModel;
using NewLife.IoT.Drivers;
using Xunit;

namespace XUnitTest.Drivers;

/// <summary>MewtocolParameter 单元测试</summary>
public class MewtocolParameterTests
{
    [Fact]
    [DisplayName("默认参数_属性具有正确默认值")]
    public void DefaultValues_AreCorrect()
    {
        var param = new MewtocolParameter();

        Assert.Null(param.Server);
        Assert.Equal(1, param.Station);
        Assert.Equal("01", param.Block);
        Assert.Equal(5000, param.Timeout);
    }

    [Fact]
    [DisplayName("设置属性_值正确保留")]
    public void SetProperties_ValuesArePreserved()
    {
        var param = new MewtocolParameter
        {
            Server = "192.168.1.100:9094",
            Station = 2,
            Block = "02",
            Timeout = 3000,
        };

        Assert.Equal("192.168.1.100:9094", param.Server);
        Assert.Equal(2, param.Station);
        Assert.Equal("02", param.Block);
        Assert.Equal(3000, param.Timeout);
    }

    [Fact]
    [DisplayName("未设置Server_返回Null")]
    public void Server_NotSet_ReturnsNull()
    {
        var param = new MewtocolParameter();
        Assert.Null(param.Server);
    }

    [Fact]
    [DisplayName("Station_默认值为1")]
    public void Station_DefaultIsOne()
    {
        var param = new MewtocolParameter();
        Assert.Equal((Byte)1, param.Station);
    }

    [Fact]
    [DisplayName("Station_支持0-99范围")]
    public void Station_ValidRange()
    {
        Assert.Equal((Byte)0, new MewtocolParameter { Station = 0 }.Station);
        Assert.Equal((Byte)99, new MewtocolParameter { Station = 99 }.Station);
    }

    [Fact]
    [DisplayName("GetKey_返回Server")]
    public void GetKey_ReturnsServer()
    {
        var param = new MewtocolParameter { Server = "10.0.0.1:9094" };
        Assert.Equal("10.0.0.1:9094", param.GetKey());
    }

    [Fact]
    [DisplayName("GetKey_Server未设置_返回Null")]
    public void GetKey_ServerNull_ReturnsNull()
    {
        var param = new MewtocolParameter();
        Assert.Null(param.GetKey());
    }

    [Fact]
    [DisplayName("实现_IDriverParameterKey接口")]
    public void Implements_IDriverParameterKey()
    {
        var param = new MewtocolParameter();
        Assert.IsAssignableFrom<IDriverParameterKey>(param);
    }

    #region 串口参数
    [Fact]
    [DisplayName("串口参数_默认值正确")]
    public void SerialDefaults_AreCorrect()
    {
        var param = new MewtocolParameter();

        Assert.Null(param.PortName);
        Assert.Equal(9600, param.Baudrate);
        Assert.Equal(8, param.DataBits);
        Assert.Equal(System.IO.Ports.Parity.None, param.Parity);
        Assert.Equal(System.IO.Ports.StopBits.One, param.StopBits);
    }

    [Fact]
    [DisplayName("串口参数_设置值正确保留")]
    public void SerialProperties_ValuesArePreserved()
    {
        var param = new MewtocolParameter
        {
            PortName = "COM3",
            Baudrate = 19200,
            DataBits = 7,
            Parity = System.IO.Ports.Parity.Even,
            StopBits = System.IO.Ports.StopBits.Two,
        };

        Assert.Equal("COM3", param.PortName);
        Assert.Equal(19200, param.Baudrate);
        Assert.Equal(7, param.DataBits);
        Assert.Equal(System.IO.Ports.Parity.Even, param.Parity);
        Assert.Equal(System.IO.Ports.StopBits.Two, param.StopBits);
    }

    [Fact]
    [DisplayName("GetKey_串口模式_返回PortName")]
    public void GetKey_SerialMode_ReturnsPortName()
    {
        var param = new MewtocolParameter { PortName = "COM1" };
        Assert.Equal("COM1", param.GetKey());
    }

    [Fact]
    [DisplayName("GetKey_Server优先_同时设置Server和PortName时返回Server")]
    public void GetKey_ServerPriority_ReturnsServer()
    {
        var param = new MewtocolParameter
        {
            Server = "192.168.1.100:9094",
            PortName = "COM1",
        };
        Assert.Equal("192.168.1.100:9094", param.GetKey());
    }
    #endregion
}
