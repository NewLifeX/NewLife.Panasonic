using NewLife.IoT.Drivers;
using NewLife.Panasonic.Drivers;
using Xunit;

namespace XUnitTest.Drivers;

/// <summary>
/// PanasonicDriver 单元测试
/// </summary>
public class PanasonicDriverTests
{
    [Fact]
    [DisplayName("驱动类型_具有正确的Driver特性")]
    public void DriverAttribute_IsCorrect()
    {
        var driver = new PanasonicDriver();

        Assert.NotNull(driver);
        Assert.IsAssignableFrom<ModbusDriver>(driver);
        Assert.IsAssignableFrom<IDriver>(driver);
    }

    [Fact]
    [DisplayName("驱动名称_与Driver特性一致")]
    public void DriverName_MatchesAttribute()
    {
        var attr = typeof(PanasonicDriver).GetCustomAttributes(typeof(DriverAttribute), false);
        Assert.NotEmpty(attr);

        var driverAttr = attr[0] as DriverAttribute;
        Assert.NotNull(driverAttr);
        Assert.Equal("PanasonicPLC", driverAttr.Name);
    }

    [Fact]
    [DisplayName("显示名称_与DisplayName特性一致")]
    public void DisplayName_MatchesAttribute()
    {
        var attr = typeof(PanasonicDriver).GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), false);
        Assert.NotEmpty(attr);

        var displayNameAttr = attr[0] as System.ComponentModel.DisplayNameAttribute;
        Assert.NotNull(displayNameAttr);
        Assert.Equal("松下PLC", displayNameAttr.DisplayName);
    }

    [Fact]
    [DisplayName("创建驱动_多次实例化不共享状态")]
    public void MultipleInstances_AreIndependent()
    {
        var driver1 = new PanasonicDriver();
        var driver2 = new PanasonicDriver();

        Assert.NotSame(driver1, driver2);
        Assert.NotNull(driver1);
        Assert.NotNull(driver2);
    }

    [Fact]
    [DisplayName("驱动类型_具有ModbusDriver基类的所有公共方法")]
    public void Driver_HasBaseClassMethods()
    {
        var methods = typeof(PanasonicDriver).GetMethods();

        // 确认继承了关键方法
        Assert.Contains(methods, m => m.Name == "OpenAsync");
        Assert.Contains(methods, m => m.Name == "CloseAsync");
        Assert.Contains(methods, m => m.Name == "ReadAsync");
        Assert.Contains(methods, m => m.Name == "WriteAsync");
        Assert.Contains(methods, m => m.Name == "Dispose");
    }

    [Fact]
    [DisplayName("驱动_初始状态未连接")]
    public void Driver_InitialState_NotConnected()
    {
        var driver = new PanasonicDriver();

        // Modbus 通道属性初始为 null
        Assert.Null(driver.Modbus);
    }

    [Fact]
    [DisplayName("打开通道_参数为null_抛出ArgumentNullException")]
    public async Task OpenAsync_NullParameter_ThrowsArgumentNullException()
    {
        var driver = new PanasonicDriver();

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
            driver.OpenAsync(null, null, default));

        Assert.Contains("Open", ex.Message);
    }

    [Fact]
    [DisplayName("打开通道_参数Server为空_抛出ArgumentException")]
    public async Task OpenAsync_EmptyServer_ThrowsArgumentException()
    {
        var driver = new PanasonicDriver();
        var param = new PanasonicParameter
        {
            Server = "",
            Host = 1,
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            driver.OpenAsync(null, param, default));

        Assert.NotNull(ex);
    }

    [Fact]
    [DisplayName("关闭通道_节点为null_不抛出异常")]
    public async Task CloseAsync_NullNode_DoesNotThrow()
    {
        var driver = new PanasonicDriver();

        // 关闭 null 节点应该安全（基类容错）
        var exception = await Record.ExceptionAsync(() =>
            driver.CloseAsync(null, default));

        Assert.Null(exception);
    }
}