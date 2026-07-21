using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using NewLife.IoT.ThingModels;

namespace NewLife.IoT.Drivers;

/// <summary>
/// 松下 Mewtocol 协议驱动。通过 Mewtocol 原生协议 TCP 通道与松下 PLC 通信，
/// 覆盖仅支持 Mewtocol 协议的松下 PLC 型号（FP 系列等）
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
    /// 创建驱动参数
    /// </summary>
    protected override IDriverParameter OnCreateParameter() => new MewtocolParameter();

    /// <summary>
    /// 打开通道，建立 TCP 连接
    /// </summary>
    /// <param name="device">逻辑设备</param>
    /// <param name="parameter">连接参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设备节点</returns>
    public override async Task<INode> OpenAsync(IDevice device, IDriverParameter parameter, CancellationToken cancellationToken)
    {
        var p = parameter as MewtocolParameter;
        if (p == null || p.Server.IsNullOrEmpty())
            throw new ArgumentException("参数中未指定地址 Server");

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

        WriteLog("Mewtocol 连接已建立：{0}:{1}", addr, port);

        return node;
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
    /// 读取数据
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
                var value = await ReadRegisterAsync(mn, address, cancellationToken);
                values[i] = value;
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
    /// <param name="requests">写入请求</param>
    /// <param name="cancellationToken">取消令牌</param>
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
    public async Task<UInt16> ReadRegisterAsync(MewtocolNode node, String address, CancellationToken cancellationToken)
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
    /// <param name="value">写入值</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task WriteRegisterAsync(MewtocolNode node, String address, UInt16 value, CancellationToken cancellationToken)
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
    /// <param name="node">节点</param>
    /// <param name="address">触点地址，如 R0</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否导通</returns>
    public async Task<Boolean> ReadContactAsync(MewtocolNode node, String address, CancellationToken cancellationToken)
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
    /// <param name="node">节点</param>
    /// <param name="address">触点地址，如 R0</param>
    /// <param name="value">是否导通</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task WriteContactAsync(MewtocolNode node, String address, Boolean value, CancellationToken cancellationToken)
    {
        // WCS 命令：写入触点状态
        var cmd = $"WCS{address}0001{(value ? "1" : "0")}";
        var response = await SendCommandAsync(node, cmd, cancellationToken);

        if (response.Length < 1) throw new InvalidOperationException("Mewtocol 写入无响应");

        var code = response.Substring(0, 1);
        if (code != "$") throw new InvalidOperationException($"Mewtocol 写入错误码: {code}");
    }

    /// <summary>
    /// 发送 Mewtocol 命令并接收响应
    /// </summary>
    /// <param name="node">节点</param>
    /// <param name="command">命令体（不含帧头帧尾）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应数据（不含帧头帧尾和校验码）</returns>
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
    /// 计算 Mewtocol 校验和（XOR 所有字节，格式化为 2 字符十六进制大写）
    /// </summary>
    /// <param name="data">要计算校验和的数据</param>
    /// <returns>2 字符校验和</returns>
    private static String CalcChecksum(String data)
    {
        Byte xor = 0;
        foreach (var ch in data)
            xor ^= (Byte)ch;

        return xor.ToString("X2");
    }
    #endregion

    #region 日志
    // WriteLog 继承自 DriverBase
    #endregion
}
