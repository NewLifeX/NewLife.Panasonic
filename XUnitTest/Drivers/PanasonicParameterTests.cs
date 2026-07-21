using System.ComponentModel;
using NewLife.IoT.Protocols;
using NewLife.Panasonic.Drivers;
using Xunit;

namespace XUnitTest.Drivers;

/// <summary>
/// PanasonicParameter 单元测试
/// </summary>
public class PanasonicParameterTests
{
    [Fact]
    [DisplayName("默认参数_属性具有正确默认值")]
    public void DefaultValues_AreCorrect()
    {
        var param = new PanasonicParameter();

        Assert.Equal(0, param.Host);
        Assert.Equal(default(FunctionCodes), param.ReadCode);
        Assert.Equal(default(FunctionCodes), param.WriteCode);
        Assert.Null(param.Server);
        Assert.Equal((UInt16)0, param.ProtocolId);
    }

    [Fact]
    [DisplayName("设置属性_值正确保留")]
    public void SetProperties_ValuesArePreserved()
    {
        var param = new PanasonicParameter
        {
            Host = 1,
            ReadCode = FunctionCodes.ReadCoil,
            WriteCode = FunctionCodes.WriteCoil,
            Server = "192.168.1.100:502",
            ProtocolId = 0
        };

        Assert.Equal(1, param.Host);
        Assert.Equal(FunctionCodes.ReadCoil, param.ReadCode);
        Assert.Equal(FunctionCodes.WriteCoil, param.WriteCode);
        Assert.Equal("192.168.1.100:502", param.Server);
        Assert.Equal((UInt16)0, param.ProtocolId);
    }

    [Fact]
    [DisplayName("未设置Server_返回Null")]
    public void Server_NotSet_ReturnsNull()
    {
        var param = new PanasonicParameter();
        Assert.Null(param.Server);
    }

    [Fact]
    [DisplayName("空字符串Server_返回空字符串")]
    public void Server_EmptyString_ReturnsEmpty()
    {
        var param = new PanasonicParameter { Server = "" };
        Assert.Equal("", param.Server);
    }

    [Fact]
    [DisplayName("ProtocolId_默认值为0")]
    public void ProtocolId_DefaultIsZero()
    {
        var param = new PanasonicParameter();
        Assert.Equal((UInt16)0, param.ProtocolId);
    }

    [Fact]
    [DisplayName("ProtocolId_可设置为非零值")]
    public void ProtocolId_CanSetNonZero()
    {
        var param = new PanasonicParameter { ProtocolId = 1 };
        Assert.Equal((UInt16)1, param.ProtocolId);
    }

    [Theory]
    [DisplayName("Host_支持0-255范围内值")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(255)]
    public void Host_ValidRange(Byte host)
    {
        var param = new PanasonicParameter { Host = host };
        Assert.Equal(host, param.Host);
    }
}