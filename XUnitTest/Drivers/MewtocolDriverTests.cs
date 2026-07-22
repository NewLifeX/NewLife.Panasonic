using System.ComponentModel;
using NewLife.IoT.Drivers;
using Xunit;

namespace XUnitTest.Drivers;

/// <summary>MewtocolDriver 单元测试</summary>
public class MewtocolDriverTests
{
    [Fact]
    [DisplayName("驱动类型_具有正确的Driver特性")]
    public void DriverAttribute_IsCorrect()
    {
        var driver = new MewtocolDriver();

        Assert.NotNull(driver);
        Assert.IsAssignableFrom<DriverBase>(driver);
        Assert.IsAssignableFrom<IDriver>(driver);
    }

    [Fact]
    [DisplayName("驱动名称_与Driver特性一致")]
    public void DriverName_MatchesAttribute()
    {
        var attr = typeof(MewtocolDriver).GetCustomAttributes(typeof(DriverAttribute), false);
        Assert.NotEmpty(attr);

        var driverAttr = attr[0] as DriverAttribute;
        Assert.NotNull(driverAttr);
        Assert.Equal("PanasonicMewtocol", driverAttr.Name);
    }

    [Fact]
    [DisplayName("显示名称_与DisplayName特性一致")]
    public void DisplayName_MatchesAttribute()
    {
        var attrs = typeof(MewtocolDriver).GetCustomAttributes(typeof(DisplayNameAttribute), false);
        Assert.NotEmpty(attrs);

        var displayNameAttr = attrs[0] as DisplayNameAttribute;
        Assert.NotNull(displayNameAttr);
        Assert.Equal("松下PLC(Mewtocol)", displayNameAttr.DisplayName);
    }

    [Fact]
    [DisplayName("打开通道_参数为null_抛出ArgumentException")]
    public async Task OpenAsync_NullParameter_ThrowsArgumentException()
    {
        var driver = new MewtocolDriver();

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            driver.OpenAsync(null, null, default));

        Assert.Contains("MewtocolParameter", ex.Message);
    }

    [Fact]
    [DisplayName("打开通道_参数Server和PortName均为空_抛出ArgumentException")]
    public async Task OpenAsync_EmptyServerAndPortName_ThrowsArgumentException()
    {
        var driver = new MewtocolDriver();
        var param = new MewtocolParameter
        {
            Server = "",
            Station = 1,
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            driver.OpenAsync(null, param, default));

        Assert.True(ex.Message.Contains("Server") || ex.Message.Contains("串口"));
    }

    [Fact]
    [DisplayName("打开通道_仅指定PortName_尝试打开串口_抛出异常_因串口不存在")]
    public async Task OpenAsync_PortNameOnly_ThrowsIOException()
    {
        var driver = new MewtocolDriver();
        var param = new MewtocolParameter
        {
            PortName = "NONEXISTENT_COM",
            Station = 1,
        };

        // 由于串口不存在，应抛出 IOException 或异常
        var ex = await Assert.ThrowsAnyAsync<Exception>(() =>
            driver.OpenAsync(null, param, default));

        // 不应该说"必须指定Server"——因为PortName已指定，只是串口打不开
        Assert.DoesNotContain("必须指定", ex.Message);
    }

    [Fact]
    [DisplayName("关闭通道_节点为null_不抛出异常")]
    public async Task CloseAsync_NullNode_DoesNotThrow()
    {
        var driver = new MewtocolDriver();
        var exception = await Record.ExceptionAsync(() =>
            driver.CloseAsync(null, default));
        Assert.Null(exception);
    }

    [Fact]
    [DisplayName("读取数据_节点类型错误_抛出ArgumentNullException")]
    public async Task ReadAsync_WrongNodeType_ThrowsArgumentNullException()
    {
        var driver = new MewtocolDriver();
        // 传入错误的节点类型（非 MewtocolNode）
        var wrongNode = new PanasonicNode();

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            driver.ReadAsync(wrongNode, [], default));

        Assert.Equal("node", ex.ParamName);
    }

    [Fact]
    [DisplayName("写入数据_节点类型错误_抛出ArgumentNullException")]
    public async Task WriteAsync_WrongNodeType_ThrowsArgumentNullException()
    {
        var driver = new MewtocolDriver();
        var wrongNode = new PanasonicNode();

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            driver.WriteAsync(wrongNode, [], default));

        Assert.Equal("node", ex.ParamName);
    }

    [Fact]
    [DisplayName("创建参数_返回MewtocolParameter实例")]
    public void OnCreateParameter_ReturnsMewtocolParameter()
    {
        // 通过反射调用受保护的 OnCreateParameter
        var driver = new MewtocolDriver();
        var method = typeof(MewtocolDriver).GetMethod("OnCreateParameter",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.NotNull(method);

        var result = method.Invoke(driver, null) as IDriverParameter;

        Assert.NotNull(result);
        Assert.IsType<MewtocolParameter>(result);
    }

    #region 校验和测试
    [Theory]
    [DisplayName("校验和_已知输入_XOR校验和正确")]
    [InlineData("", "00")]
    [InlineData("01#01#RCPDT1000001", "61")]
    [InlineData("01#01#$", "24")]
    [InlineData("01#01#RCSS00001", "20")]
    [InlineData("01#01#WCPDT1000010001", "55")]
    [InlineData("01#01#WCSR00011", "25")]
    public void CalcChecksum_KnownInputs(String input, String expected)
    {
        var method = typeof(MewtocolDriver).GetMethod("CalcChecksum",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Assert.NotNull(method);

        var result = method.Invoke(null, [input]) as String;
        Assert.Equal(expected, result);
    }
    #endregion

    #region 地址类型判断测试
    [Theory]
    [DisplayName("地址判断_触点类地址返回true")]
    [InlineData("R0", true)]
    [InlineData("Y1", true)]
    [InlineData("X10", true)]
    [InlineData("L100", true)]
    [InlineData("T5", true)]
    [InlineData("C99", true)]
    [InlineData("S0", true)]
    [InlineData("DT100", false)]
    [InlineData("D100", false)]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("R", false)]
    [InlineData("XY100", false)]
    public void IsContactAddress_Detection(String address, Boolean expected)
    {
        var method = typeof(MewtocolDriver).GetMethod("IsContactAddress",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Assert.NotNull(method);

        var result = method.Invoke(null, [address]) as Boolean?;
        Assert.Equal(expected, result);
    }
    #endregion

    #region 高级命令测试
    [Fact]
    [DisplayName("ReadStatusAsync_节点为null_抛出ArgumentNullException")]
    public async Task ReadStatusAsync_NullNode_ThrowsArgumentNullException()
    {
        var driver = new MewtocolDriver();

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            driver.ReadStatusAsync(null, default));

        Assert.Equal("node", ex.ParamName);
    }

    [Fact]
    [DisplayName("SetRunModeAsync_节点为null_抛出ArgumentNullException")]
    public async Task SetRunModeAsync_NullNode_ThrowsArgumentNullException()
    {
        var driver = new MewtocolDriver();

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            driver.SetRunModeAsync(null, default));

        Assert.Equal("node", ex.ParamName);
    }

    [Fact]
    [DisplayName("SetProgramModeAsync_节点为null_抛出ArgumentNullException")]
    public async Task SetProgramModeAsync_NullNode_ThrowsArgumentNullException()
    {
        var driver = new MewtocolDriver();

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            driver.SetProgramModeAsync(null, default));

        Assert.Equal("node", ex.ParamName);
    }
    #endregion
}
