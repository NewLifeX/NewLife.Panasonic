using System.ComponentModel;
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

        Assert.Equal("parameter", ex.ParamName);
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

    [Fact]
    [DisplayName("打开通道_RTU参数_PortName为空_抛出ArgumentException")]
    public async Task OpenAsync_RtuParameter_EmptyPortName_ThrowsArgumentException()
    {
        var driver = new PanasonicDriver();
        var param = new PanasonicParameter
        {
            PortName = "",
            Baudrate = 9600,
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            driver.OpenAsync(null, param, default));

        Assert.NotNull(ex);
    }

    [Fact]
    [DisplayName("打开通道_RTU参数_参数校验通过")]
    public async Task OpenAsync_RtuParameter_ValidationPasses()
    {
        var driver = new PanasonicDriver();
        var param = new PanasonicParameter
        {
            PortName = "COM_NONEXISTENT",
            Baudrate = 9600,
            Host = 1,
        };

        // 参数校验通过（PortName 非空），不抛出 ArgumentException
        // 串口不存在时基类可能返回节点或抛出 IO 异常，不是参数校验层面的问题
        var ex = await Record.ExceptionAsync(() =>
            driver.OpenAsync(null, param, default));

        // 如果抛出异常，不能是参数校验异常（表示分支逻辑正确）
        if (ex != null)
            Assert.IsNotType<ArgumentException>(ex);
    }

    #region 心跳与重连测试
    [Fact]
    [DisplayName("心跳_AutoReconnect默认true")]
    public void AutoReconnect_DefaultIsTrue()
    {
        var driver = new PanasonicDriver();
        Assert.True(driver.AutoReconnect);
    }

    [Fact]
    [DisplayName("心跳_HeartbeatInterval默认30秒")]
    public void HeartbeatInterval_DefaultIs30()
    {
        var driver = new PanasonicDriver();
        Assert.Equal(30, driver.HeartbeatInterval);
    }

    [Fact]
    [DisplayName("重连_MaxRetries默认3")]
    public void MaxRetries_DefaultIs3()
    {
        var driver = new PanasonicDriver();
        Assert.Equal(3, driver.MaxRetries);
    }

    [Fact]
    [DisplayName("重连_RetryInterval默认1000ms")]
    public void RetryInterval_DefaultIs1000()
    {
        var driver = new PanasonicDriver();
        Assert.Equal(1000, driver.RetryInterval);
    }

    [Fact]
    [DisplayName("心跳_属性可设置后保留")]
    public void HeartbeatProperties_CanBeSet()
    {
        var driver = new PanasonicDriver
        {
            AutoReconnect = false,
            HeartbeatInterval = 60,
            MaxRetries = 5,
            RetryInterval = 2000,
        };

        Assert.False(driver.AutoReconnect);
        Assert.Equal(60, driver.HeartbeatInterval);
        Assert.Equal(5, driver.MaxRetries);
        Assert.Equal(2000, driver.RetryInterval);
    }
    #endregion
}