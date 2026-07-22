using System.ComponentModel;
using NewLife.Panasonic.Drivers;
using NewLife.Threading;
using NewLife.IoT.Protocols;
using NewLife.Serial.Protocols;

namespace NewLife.IoT.Drivers;

/// <summary>
/// 松下PLC驱动。统一支持 Modbus TCP 和 Modbus RTU 两种连接方式，
/// 根据 <see cref="PanasonicParameter"/> 中设置的字段自动选择协议。
/// </summary>
/// <remarks>
/// 使用方式：
/// TCP 模式：设置 <c>PanasonicParameter.Server</c>（如 "127.0.0.1:502"）
/// RTU 模式：设置 <c>PanasonicParameter.PortName</c>（如 "COM3"）
/// 驱动内建心跳检测（TimerX）和自动重连机制，仅 TCP 模式生效。
/// </remarks>
/// <example>
/// <code>
/// var driver = new PanasonicDriver();
/// var param = new PanasonicParameter { Server = "127.0.0.1:502", Host = 1 };
/// var node = driver.Open(null, param);
/// </code>
/// </example>
[Driver("PanasonicPLC")]
[DisplayName("松下PLC")]
public class PanasonicDriver : ModbusDriver, IDriver
{
    #region 属性
    /// <summary>是否启用自动重连。默认 true（仅 Modbus TCP 模式有效）</summary>
    [Description("是否启用自动重连。默认 true（仅 Modbus TCP 模式有效）")]
    public Boolean AutoReconnect { get; set; } = true;

    /// <summary>心跳间隔（秒）。定期检查连接状态，默认 30 秒（仅 Modbus TCP 模式有效）</summary>
    [Description("心跳间隔（秒）。定期检查连接状态，默认 30 秒（仅 Modbus TCP 模式有效）")]
    public Int32 HeartbeatInterval { get; set; } = 30;

    /// <summary>最大重试次数。默认 3</summary>
    [Description("最大重试次数。默认 3")]
    public Int32 MaxRetries { get; set; } = 3;

    /// <summary>重试间隔（毫秒）。默认 1000ms</summary>
    [Description("重试间隔（毫秒）。默认 1000ms")]
    public Int32 RetryInterval { get; set; } = 1000;

    private TimerX _timer;
    private Int32 _retryCount;
    private IDevice _currentDevice;
    internal ModbusParameter _currentParameter;

    /// <summary>当前连接是否为 Modbus TCP 模式</summary>
    private Boolean _isTcpMode;
    #endregion

    #region 方法
    /// <summary>
    /// 创建驱动参数对象，可序列化成 Xml/Json 作为该协议的参数模板
    /// </summary>
    /// <returns>包含默认值的 PanasonicParameter 实例</returns>
    protected override IDriverParameter OnCreateParameter() => new PanasonicParameter
    {
        Server = "127.0.0.1:502",
        Host = 1,
        ReadCode = FunctionCodes.ReadRegister,
        WriteCode = FunctionCodes.WriteRegister,
    };

    /// <summary>
    /// 创建 Modbus 通道。根据参数中填写的字段自动选择协议：
    /// Server 不为空 → Modbus TCP；PortName 不为空 → Modbus RTU
    /// </summary>
    /// <param name="device">逻辑设备，可为 null</param>
    /// <param name="node">设备节点，承载站号等运行时状态</param>
    /// <param name="parameter">连接参数，必须是 <see cref="PanasonicParameter"/> 类型</param>
    /// <returns>ModbusTcp 或 ModbusRtu 实例</returns>
    protected override Modbus CreateModbus(IDevice device, ModbusNode node, ModbusParameter parameter)
    {
        if (parameter is not PanasonicParameter p) throw new ArgumentNullException(nameof(parameter));

        // TCP 模式：Server 不为空
        if (!p.Server.IsNullOrEmpty())
        {
            node.Parameter = p;

            var modbus = new ModbusTcp
            {
                Server = p.Server,
                ProtocolId = p.ProtocolId,

                Tracer = Tracer,
                Log = Log,
            };

            // 保存当前连接参数，用于断线重连
            _currentDevice = device;
            _currentParameter = parameter;
            _isTcpMode = true;

            // 初始化心跳定时器（仅 TCP 模式）
            StartHeartbeat();

            return modbus;
        }

        // RTU 模式：PortName 不为空
        if (!p.PortName.IsNullOrEmpty())
        {
            node.Parameter = p;

            var modbus = new ModbusRtu
            {
                PortName = p.PortName,
                Baudrate = p.Baudrate,
                DataBits = p.DataBits,
                Timeout = p.Timeout,

                Tracer = Tracer,
                Log = Log,
            };

            // 高级串口参数（如 Parity/StopBits/ByteTimeout）可通过 ModbusRtu 直接配置

            _isTcpMode = false;

            return modbus;
        }

        throw new ArgumentException("必须指定 Server (Modbus TCP) 或 PortName (Modbus RTU)");
    }

    /// <summary>
    /// 启动心跳检测（仅 TCP 模式）
    /// </summary>
    private void StartHeartbeat()
    {
        _timer.TryDispose();
        _timer = null;

        if (!AutoReconnect || HeartbeatInterval <= 0 || !_isTcpMode) return;

        _timer = new TimerX(DoHeartbeat, null, HeartbeatInterval * 1000, HeartbeatInterval * 1000)
        {
            Async = true,
        };
    }

    /// <summary>
    /// 执行心跳检测。发送 Modbus 读保持寄存器请求验证 TCP 连接是否正常，
    /// 失败时触发自动重连。
    /// </summary>
    /// <param name="state">TimerX 传递的状态参数，未使用</param>
    private async Task DoHeartbeat(Object state)
    {
        var modbus = Modbus;
        if (modbus == null) return;

        try
        {
            // 尝试读取保持寄存器，验证连接是否正常
            var host = _currentParameter?.Host ?? (Byte)1;
            using var source = new CancellationTokenSource(3000);
            await modbus.ReadAsync(FunctionCodes.ReadRegister, host, 0, 1, source.Token);

            // 连接正常，重置重试计数
            _retryCount = 0;
        }
        catch (Exception ex)
        {
            WriteLog("心跳检测失败：{0}", ex.Message);

            if (!AutoReconnect) return;

            // 尝试重连
            await TryReconnect();
        }
    }

    /// <summary>
    /// 尝试重新连接
    /// </summary>
    private async Task TryReconnect()
    {
        if (_retryCount >= MaxRetries)
        {
            WriteLog("自动重连已达最大次数 {0}，停止重试", MaxRetries);
            return;
        }

        _retryCount++;

        WriteLog("正在尝试自动重连（第 {0}/{1} 次）...", _retryCount, MaxRetries);

        try
        {
            // 关闭旧通道
            var oldModbus = Modbus;
            if (oldModbus != null)
            {
                await oldModbus.CloseAsync(default);
                (oldModbus as IDisposable)?.Dispose();
            }

            // 使用保存的参数重新打开
            if (_currentParameter != null)
            {
                var node = await OpenAsync(_currentDevice, _currentParameter, default);
                if (node != null)
                {
                    WriteLog("自动重连成功");
                    _retryCount = 0;
                }
            }
        }
        catch (Exception ex)
        {
            WriteLog("自动重连失败：{0}", ex.Message);

            // 等待重试间隔后再次尝试
            if (_retryCount < MaxRetries)
                await Task.Delay(RetryInterval);
        }
    }
    #endregion
}