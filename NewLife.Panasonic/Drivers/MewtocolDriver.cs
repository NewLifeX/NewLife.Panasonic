using System.ComponentModel;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using NewLife.IoT.ThingModels;

namespace NewLife.IoT.Drivers;

/// <summary>
/// 松下 Mewtocol 协议驱动。通过 Mewtocol 原生协议与松下 PLC 通信，
/// 支持 TCP 和串口两种传输方式，覆盖仅支持 Mewtocol 协议的松下 PLC 型号（FP 系列等）。
/// 根据 <see cref="MewtocolParameter"/> 中设置的字段自动选择传输方式：
/// <see cref="MewtocolParameter.Server"/> 非空 → TCP 模式，
/// <see cref="MewtocolParameter.PortName"/> 非空 → 串口模式。
/// </summary>
/// <remarks>
/// Mewtocol 帧格式：
///  发送：%{Block}#{Station}#{Command}{Data}{Checksum}\r
///  响应：%{Block}#{Station}#{ResponseCode}{Data}{Checksum}\r
///  校验和：% 到 \r 之间所有字节的 XOR，格式化为 2 字符十六进制大写
/// 默认端口：9094
/// </remarks>
[Driver("PanasonicMewtocol")]
[DisplayName("松下PLC(Mewtocol)")]
public class MewtocolDriver : DriverBase, IDriver
{
    #region 方法
    /// <summary>
    /// 创建驱动参数对象
    /// </summary>
    /// <returns>默认配置的 MewtocolParameter 实例</returns>
    protected override IDriverParameter OnCreateParameter() => new MewtocolParameter();

    /// <summary>
    /// 打开通道，建立 TCP 或串口连接。根据参数自动选择传输方式：
    /// Server 非空 → TCP 模式；PortName 非空 → 串口模式。
    /// </summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameter">连接参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设备节点</returns>
    /// <exception cref="ArgumentException">Server 和 PortName 均为空时抛出</exception>
    public override async Task<INode> OpenAsync(IDevice device, IDriverParameter parameter, CancellationToken cancellationToken)
    {
        var p = parameter as MewtocolParameter;
        if (p == null)
            throw new ArgumentException("参数类型必须为 MewtocolParameter");

        // 串口模式：PortName 不为空
        if (!p.PortName.IsNullOrEmpty())
        {
            var serial = new SerialPort(p.PortName, p.Baudrate, p.Parity, p.DataBits, p.StopBits);
            serial.Open();

            var node = new MewtocolNode
            {
                Serial = serial,
                Stream = serial.BaseStream,
                Driver = this,
                Device = device,
                Parameter = p,
            };

            WriteLog("Mewtocol 串口已打开：{0} (Baudrate={1}, DataBits={2}, Parity={3}, StopBits={4})",
                p.PortName, p.Baudrate, p.DataBits, p.Parity, p.StopBits);

            return node;
        }

        // TCP 模式：Server 不为空
        if (!p.Server.IsNullOrEmpty())
        {
            // 解析服务器地址和端口
            var addr = p.Server;
            var port = 9094; // Mewtocol 默认端口
            if (addr.Contains(':'))
            {
                var parts = addr.Split(':');
                addr = parts[0];
                if (parts.Length > 1 && Int32.TryParse(parts[1], out var p2))
                    port = p2;
            }

            // 创建 TCP 连接
            var client = new TcpClient();
            await client.ConnectAsync(addr, port);

            var node = new MewtocolNode
            {
                Client = client,
                Stream = client.GetStream(),
                Driver = this,
                Device = device,
                Parameter = p,
            };

            WriteLog("Mewtocol TCP 连接已建立：{0}:{1}", addr, port);

            return node;
        }

        throw new ArgumentException("必须指定 Server (TCP) 或 PortName (串口)");
    }

    /// <summary>
    /// 关闭通道
    /// </summary>
    /// <param name="node">设备节点</param>
    /// <param name="cancellationToken">取消令牌</param>
    public override Task CloseAsync(INode node, CancellationToken cancellationToken)
    {
        if (node is MewtocolNode mn)
        {
            mn.Dispose();
            WriteLog("Mewtocol 连接已关闭");
        }

#if NET45
        return Task.FromResult(0);
#else
        return Task.CompletedTask;
#endif
    }

    /// <summary>
    /// 读取数据。根据地址前缀自动选择 RCP（寄存器）或 RCS（触点）命令。
    /// 触点类地址（R/Y/X/L/T/C/S 开头）走 RCS，其余走 RCP。
    /// </summary>
    /// <param name="node">设备节点</param>
    /// <param name="points">点位集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>读取结果</returns>
    public override async Task<ReadResult> ReadAsync(INode node, IPoint[] points, CancellationToken cancellationToken)
    {
        if (node is not MewtocolNode mn)
            throw new ArgumentNullException(nameof(node));

        var values = new Object[points.Length];

        for (var i = 0; i < points.Length; i++)
        {
            var point = points[i];
            using var span = Tracer?.NewSpan("MewtocolRead", point.Name);

            try
            {
                // 根据点位地址类型选择命令
                var address = point.Address ?? point.Name;
                if (IsContactAddress(address))
                {
                    var contactValue = await ReadContactAsync(mn, address, cancellationToken);
                    values[i] = contactValue;
                }
                else
                {
                    var registerValue = await ReadRegisterAsync(mn, address, cancellationToken);
                    values[i] = registerValue;
                }
            }
            catch (Exception ex)
            {
                WriteLog("读取失败 [{0}]: {1}", point.Name, ex.Message);
                span?.SetError(ex, null);
                return ReadResult.Fail(0, ex.Message);
            }
        }

        return ReadResult.Success(points, values, default, null, null);
    }

    /// <summary>
    /// 写入数据
    /// </summary>
    /// <param name="node">设备节点</param>
    /// <param name="requests">写入请求，支持批量写入</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>写入结果，包含成功写入的点数或错误信息</returns>
    public override async Task<WriteResult> WriteAsync(INode node, WriteRequest[] requests, CancellationToken cancellationToken)
    {
        if (node is not MewtocolNode mn)
            throw new ArgumentNullException(nameof(node));

        var count = 0;

        foreach (var request in requests)
        {
            using var span = Tracer?.NewSpan("MewtocolWrite", request.Point?.Name);

            try
            {
                var address = request.Point?.Address ?? request.Point?.Name;
                if (address.IsNullOrEmpty()) continue;

                var value = request.Value?.ToString();
                if (value == null) continue;

                if (Int16.TryParse(value, out var num))
                    await WriteRegisterAsync(mn, address, (UInt16)num, cancellationToken);
                else
                    await WriteContactAsync(mn, address, value == "1" || value == "true", cancellationToken);

                count++;
            }
            catch (Exception ex)
            {
                WriteLog("写入失败 [{0}]: {1}", request.Point?.Name, ex.Message);
                span?.SetError(ex, null);
                return WriteResult.Fail(0, ex.Message);
            }
        }

        return WriteResult.Success(count);
    }

    /// <summary>
    /// 读取保持寄存器（RCP 命令）
    /// </summary>
    /// <param name="node">节点</param>
    /// <param name="address">寄存器地址，如 DT100</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>寄存器值</returns>
    internal async Task<UInt16> ReadRegisterAsync(MewtocolNode node, String address, CancellationToken cancellationToken)
    {
        // RCP 命令：读取保持寄存器内容
        // 命令格式：%01#RCP{address}{count}\r
        var cmd = $"RCP{address}0001";
        var response = await SendCommandAsync(node, cmd, cancellationToken);

        // 响应格式：%01#${data}{checksum}\r
        // $ 表示正常响应
        if (response.Length < 5) throw new InvalidOperationException($"Mewtocol 响应太短: {response}");

        var code = response.Substring(0, 1);
        if (code != "$") throw new InvalidOperationException($"Mewtocol 响应错误码: {code}");

        var data = response.Substring(1);
        if (data.Length >= 4 && UInt16.TryParse(data.Substring(0, 4), System.Globalization.NumberStyles.HexNumber, null, out var val))
            return val;

        return 0;
    }

    /// <summary>
    /// 写入保持寄存器（WCP 命令）
    /// </summary>
    /// <param name="node">节点</param>
    /// <param name="address">寄存器地址，如 DT100</param>
    /// <param name="value">写入值，范围 0x0000~0xFFFF</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>表示异步操作的任务</returns>
    internal async Task WriteRegisterAsync(MewtocolNode node, String address, UInt16 value, CancellationToken cancellationToken)
    {
        // WCP 命令：写入保持寄存器
        // 命令格式：%01#WCP{address}{count}{data}\r
        var data = value.ToString("X4");
        var cmd = $"WCP{address}0001{data}";
        var response = await SendCommandAsync(node, cmd, cancellationToken);

        if (response.Length < 1) throw new InvalidOperationException("Mewtocol 写入无响应");

        var code = response.Substring(0, 1);
        if (code != "$") throw new InvalidOperationException($"Mewtocol 写入错误码: {code}");
    }

    /// <summary>
    /// 读取线圈/触点（RCS 命令）
    /// </summary>
    /// <param name="node">已连接的 Mewtocol 节点</param>
    /// <param name="address">触点地址，如 R0、Y0、X0 等</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>触点是否导通（true=导通）</returns>
    internal async Task<Boolean> ReadContactAsync(MewtocolNode node, String address, CancellationToken cancellationToken)
    {
        // RCS 命令：读取触点状态
        var cmd = $"RCS{address}0001";
        var response = await SendCommandAsync(node, cmd, cancellationToken);

        if (response.Length < 5) throw new InvalidOperationException($"Mewtocol 响应太短: {response}");

        var code = response.Substring(0, 1);
        if (code != "$") throw new InvalidOperationException($"Mewtocol 响应错误码: {code}");

        var data = response.Substring(1);
        return data == "1";
    }

    /// <summary>
    /// 写入线圈/触点（WCS 命令）
    /// </summary>
    /// <param name="node">已连接的 Mewtocol 节点</param>
    /// <param name="address">触点地址，如 R0、Y0、X0 等</param>
    /// <param name="value">是否导通（true=导通，false=断开）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>表示异步操作的任务</returns>
    internal async Task WriteContactAsync(MewtocolNode node, String address, Boolean value, CancellationToken cancellationToken)
    {
        // WCS 命令：写入触点状态
        var cmd = $"WCS{address}0001{(value ? "1" : "0")}";
        var response = await SendCommandAsync(node, cmd, cancellationToken);

        if (response.Length < 1) throw new InvalidOperationException("Mewtocol 写入无响应");

        var code = response.Substring(0, 1);
        if (code != "$") throw new InvalidOperationException($"Mewtocol 写入错误码: {code}");
    }

    /// <summary>
    /// 读取 PLC 状态/错误码（HELP 命令）。
    /// </summary>
    /// <param name="node">已连接的 Mewtocol 节点</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>PLC 返回的状态码或错误信息字符串</returns>
    /// <exception cref="ArgumentNullException">node 为 null 时抛出</exception>
    public async Task<String> ReadStatusAsync(MewtocolNode node, CancellationToken cancellationToken)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        // HELP 命令（也兼容 H 简写）：读取 PLC 错误状态
        var response = await SendCommandAsync(node, "HELP", cancellationToken);

        // 响应格式：%01#01#${statusCode}{data}{checksum}\r
        if (response.Length < 1) throw new InvalidOperationException("Mewtocol 状态读取无响应");

        var code = response.Substring(0, 1);
        if (code != "$") throw new InvalidOperationException($"Mewtocol 状态读取错误码: {code}");

        // 返回 $ 之后的原始状态数据
        return response.Substring(1);
    }

    /// <summary>
    /// 切换 PLC 至运行模式（RUN 命令）。
    /// </summary>
    /// <param name="node">已连接的 Mewtocol 节点</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>表示异步操作的任务</returns>
    /// <exception cref="ArgumentNullException">node 为 null 时抛出</exception>
    public async Task SetRunModeAsync(MewtocolNode node, CancellationToken cancellationToken)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        // RUN 命令：切换至运行模式
        var response = await SendCommandAsync(node, "RUN", cancellationToken);

        if (response.Length < 1) throw new InvalidOperationException("Mewtocol RUN 命令无响应");

        var code = response.Substring(0, 1);
        if (code != "$") throw new InvalidOperationException($"Mewtocol RUN 命令错误码: {code}");
    }

    /// <summary>
    /// 切换 PLC 至编程模式（PROG 命令）。
    /// </summary>
    /// <param name="node">已连接的 Mewtocol 节点</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>表示异步操作的任务</returns>
    /// <exception cref="ArgumentNullException">node 为 null 时抛出</exception>
    public async Task SetProgramModeAsync(MewtocolNode node, CancellationToken cancellationToken)
    {
        if (node == null) throw new ArgumentNullException(nameof(node));

        // PROG 命令：切换至编程模式
        var response = await SendCommandAsync(node, "PROG", cancellationToken);

        if (response.Length < 1) throw new InvalidOperationException("Mewtocol PROG 命令无响应");

        var code = response.Substring(0, 1);
        if (code != "$") throw new InvalidOperationException($"Mewtocol PROG 命令错误码: {code}");
    }

    /// <summary>
    /// 发送 Mewtocol 命令并接收响应。
    /// 自动构建帧（含头尾标记和校验和）、发送、接收完整响应帧、验证格式和校验和。
    /// </summary>
    /// <param name="node">已连接的 Mewtocol 节点</param>
    /// <param name="command">命令体（不含帧头帧尾和校验码），如 "RCPDT1000001"</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应数据（已去除帧头帧尾和校验码），正常响应首字符为 "$"</returns>
    /// <exception cref="TimeoutException">响应超时（默认 5000ms）</exception>
    /// <exception cref="InvalidOperationException">响应格式错误或校验和不匹配</exception>
    private async Task<String> SendCommandAsync(MewtocolNode node, String command, CancellationToken cancellationToken)
    {
        var p = node.Parameter as MewtocolParameter;
        var block = p?.Block ?? "01";
        var station = p?.Station ?? 1;

        // 构建帧：%{Block}#{Station}#{Command}{Checksum}\r
        var body = $"{block}#{station:D2}#{command}";
        var frame = $"%{body}{CalcChecksum(body)}\r";

        // 发送
        var data = Encoding.ASCII.GetBytes(frame);
        await node.Stream.WriteAsync(data, cancellationToken);
        await node.Stream.FlushAsync(cancellationToken);

        WriteLog("→ {0}", frame.TrimEnd('\r'));

        // 接收响应
        using var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        source.CancelAfter(p?.Timeout ?? 5000);

        var buffer = new List<Byte>();
        var readBuffer = new Byte[1];

        while (!source.Token.IsCancellationRequested)
        {
            var count = await node.Stream.ReadAsync(readBuffer, 0, readBuffer.Length, source.Token);
            if (count <= 0) break;

            buffer.Add(readBuffer[0]);

            // Mewtocol 帧以 \r 结尾
            if (readBuffer[0] == '\r')
            {
                var response = Encoding.ASCII.GetString(buffer.ToArray());

                WriteLog("← {0}", response.TrimEnd('\r'));

                // 验证帧格式：%...\r
                if (!response.StartsWith("%") || !response.EndsWith("\r"))
                    throw new InvalidOperationException($"Mewtocol 响应格式错误: {response.TrimEnd('\r')}");

                // 提取响应体（去掉 % 和 \r）
                var responseBody = response.Substring(1, response.Length - 2);

                // 验证校验和
                var responseData = responseBody.Substring(0, responseBody.Length - 2);
                var responseChecksum = responseBody.Substring(responseBody.Length - 2);
                var expectedChecksum = CalcChecksum(responseData);
                if (responseChecksum != expectedChecksum)
                    WriteLog("校验和不匹配：期望 {0}，实际 {1}", expectedChecksum, responseChecksum);

                // 去掉帧头标识和站号部分，返回命令响应部分
                // 格式：{Block}#{Station}#{ResponseData}{Checksum}
                var hashIndex = responseData.IndexOf('#');
                if (hashIndex >= 0)
                {
                    var afterFirstHash = responseData.Substring(hashIndex + 1);
                    var hashIndex2 = afterFirstHash.IndexOf('#');
                    if (hashIndex2 >= 0)
                        return afterFirstHash.Substring(hashIndex2 + 1);
                }

                return responseData;
            }
        }

        throw new TimeoutException("Mewtocol 响应超时");
    }

    /// <summary>
    /// 计算 Mewtocol 校验和。对输入字符串每个字节做 XOR，结果格式化为 2 字符十六进制大写。
    /// </summary>
    /// <param name="data">要计算校验和的数据（不含帧头 % 和帧尾 \r）</param>
    /// <returns>2 字符十六进制校验和，如 "3F"</returns>
    internal static String CalcChecksum(String data)
    {
        Byte xor = 0;
        foreach (var ch in data)
            xor ^= (Byte)ch;

        return xor.ToString("X2");
    }

    /// <summary>Mewtocol 触点类地址前缀</summary>
    private static readonly String[] _contactPrefixes = ["R", "Y", "X", "L", "T", "C", "S"];

    /// <summary>
    /// 判断地址是否为触点类地址。触点类使用 RCS/WCS 命令，寄存器类使用 RCP/WCP 命令。
    /// </summary>
    /// <param name="address">点位地址，如 "DT100"、"R0"、"Y1"</param>
    /// <returns>true 表示触点类地址，false 表示寄存器类地址</returns>
    internal static Boolean IsContactAddress(String address)
    {
        if (address.IsNullOrEmpty()) return false;

        foreach (var prefix in _contactPrefixes)
        {
            if (address.StartsWith(prefix) && address.Length > prefix.Length && Char.IsDigit(address[prefix.Length]))
                return true;
        }
        return false;
    }
    #endregion

    #region 日志
    // WriteLog 继承自 DriverBase
    #endregion
}
