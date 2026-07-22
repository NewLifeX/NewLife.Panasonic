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

        Assert.Contains("Server", ex.Message);
    }

    [Fact]
    [DisplayName("打开通道_参数Server为空_抛出ArgumentException")]
    public async Task OpenAsync_EmptyServer_ThrowsArgumentException()
    {
        var driver = new MewtocolDriver();
        var param = new MewtocolParameter
        {
            Server = "",
            Station = 1,
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            driver.OpenAsync(null, param, default));

        Assert.Contains("Server", ex.Message);
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
}
