# NewLife.Panasonic - Panasonic PLC Protocol

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/NewLife.Panasonic?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/NewLife.Panasonic?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/NewLife.Panasonic?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/NewLife.Panasonic?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/NewLife.Panasonic?label=dev%20nuget&logo=nuget)

> **🌐 Language**: [English](Readme.en.md) | [Español](Readme.es.md) | [Deutsch](Readme.de.md) | [Français](Readme.fr.md) | [Português](Readme.pt-BR.md) | [Русский](Readme.ru.md) | [العربية](Readme.ar.md) | [한국어](Readme.ko.md) | [日本語](Readme.ja-JP.md) | [中文](Readme.MD)

Panasonic PLC communication protocol library for .NET. Built on the NewLife.IoT standard framework, it provides multiple communication drivers (Modbus TCP / Modbus RTU / Mewtocol TCP) for Panasonic PLC devices from .NET applications. **All features are implemented and verified by tests.**

## Highlights

- **Three Protocols in One**: A unified driver entry that automatically selects between Modbus TCP / Modbus RTU, with an independent Mewtocol driver covering mainstream Panasonic PLC communication scenarios
- **Reliable Connection**: Built-in heartbeat detection and auto-reconnect, suitable for industrial automation scenarios
- **Ecosystem Synergy**: Based on NewLife.IoT standard interfaces, plug-and-play with IoTEdge and ZeroIoT platforms
- **MIT License**: The most permissive open-source license, unlimited commercial use

## Quick Start

```xml
<PackageReference Include="NewLife.Panasonic" Version="1.0.*" />
```

### Modbus TCP (via Modbus Gateway)

```csharp
// Create driver (built-in heartbeat and auto-reconnect)
var driver = new PanasonicDriver
{
    AutoReconnect = true,
    HeartbeatInterval = 30,
};

// Configure parameters
var param = new PanasonicParameter
{
    Server = "192.168.1.100:502",
    Host = 1,
};

// Open channel
var node = driver.Open(null, param);
```

### Modbus RTU (Serial Port, Auto-Detected by Same Driver)

```csharp
// Same driver, just fill serial port parameters to automatically use RTU protocol
var driver = new PanasonicDriver();
var param = new PanasonicParameter
{
    PortName = "COM3",
    Baudrate = 9600,
    Host = 1,
};

// Open channel (driver auto-detects: PortName not empty → Modbus RTU)
var node = driver.Open(null, param);
```

### Mewtocol Native Protocol (TCP Direct Connection)

```csharp
// Create Mewtocol driver (for Panasonic PLCs that only support Mewtocol protocol)
var driver = new MewtocolDriver();

// Configure parameters (default port 9094)
var param = new MewtocolParameter
{
    Server = "192.168.1.100:9094",
    Station = 1,
};

// Open channel
var node = await driver.OpenAsync(null, param);
```

## Driver Overview

| Driver | Protocol | Connection | Registration Name |
|--------|----------|-----------|-------------------|
| `PanasonicDriver` | Modbus TCP / Modbus RTU | Ethernet or Serial (auto-select by parameter) | `PanasonicPLC` |
| `MewtocolDriver` | Mewtocol Native | Ethernet (direct PLC connection) | `PanasonicMewtocol` |

## Documentation

| Document | Description |
|----------|-------------|
| [Requirements](Doc/需求文档.md) | Vision, core features, and capability boundaries (v3.0) |
| [Feature List](Doc/功能清单.md) | All 8 feature points completed (v3.0) |
| [Architecture](Doc/架构设计.md) | Layering, components, workflows, and design decisions (v3.0) |
| [Competitive Analysis](Doc/竞品分析报告.md) | Competitor comparison, feature matrix, gap analysis (v3.0) |

Source Code: https://github.com/NewLifeX/NewLife.Panasonic  
NuGet: NewLife.Panasonic  

---

## NewLife Project Matrix

| Project | Year | Description |
|:--------|:----:|:------------|
| [NewLife.Core](https://github.com/NewLifeX/X) | 2002 | Core library: logging, configuration, cache, networking, serialization, APM tracing |
| [NewLife.XCode](https://github.com/NewLifeX/NewLife.XCode) | 2005 | Big-data ORM: MySQL/SQLite/SQL Server/Oracle/PostgreSQL/DaMeng, auto-sharding, read-write separation |
| [NewLife.Net](https://github.com/NewLifeX/NewLife.Net) | 2005 | Networking library: 22.66M TPS throughput, 4M TCP connections |
| [NewLife.Remoting](https://github.com/NewLifeX/NewLife.Remoting) | 2011 | RPC communication framework for client-server applications |
| [NewLife.Cube](https://github.com/NewLifeX/NewLife.Cube) | 2010 | Rapid development platform with user permissions, SSO, OAuth |
| [NewLife.Redis](https://github.com/NewLifeX/NewLife.Redis) | 2017 | Redis client: microsecond latency, million-level throughput, message queues |
| [NewLife.RocketMQ](https://github.com/NewLifeX/NewLife.RocketMQ) | 2018 | Native .NET RocketMQ client supporting Apache RocketMQ and Alibaba Cloud MQ |
| [NewLife.MQTT](https://github.com/NewLifeX/NewLife.MQTT) | 2019 | IoT messaging protocol: MqttClient/MqttServer |
| [NewLife.IoT](https://github.com/NewLifeX/NewLife.IoT) | 2022 | IoT standard library: driver interfaces, device models, controller definitions |
| [NewLife.Modbus](https://github.com/NewLifeX/NewLife.Modbus) | 2022 | Modbus TCP/RTU/ASCII protocol stack |
| [NewLife.Siemens](https://github.com/NewLifeX/NewLife.Siemens) | 2022 | Siemens PLC protocol driver |
| [NewLife.Panasonic](https://github.com/NewLifeX/NewLife.Panasonic) | 2025 | Panasonic PLC protocol driver |
| [Stardust](https://github.com/NewLifeX/Stardust) | 2018 | Distributed service platform: APM monitoring, configuration center, registry |
| [AntJob](https://github.com/NewLifeX/AntJob) | 2019 | Distributed big-data computing platform (real-time/batch) |

## NewLife Development Team

![NewLife](https://newlifex.com/logo.png)

Founded in 2002, the NewLife team is an IoT industry solution provider, offering software/hardware application consulting, system architecture planning, and development services.

The team has initiated 80+ open-source projects widely used across various industries, with NuGet downloads exceeding 4 million.

Website: https://newlifex.com  
Open Source: https://github.com/newlifex  
QQ Groups: 1600800/1600838  
