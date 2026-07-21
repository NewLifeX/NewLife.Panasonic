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
}