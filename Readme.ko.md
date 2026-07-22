# NewLife.Panasonic - 파나소닉 PLC 프로토콜

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/NewLife.Panasonic?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/NewLife.Panasonic?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/NewLife.Panasonic?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/NewLife.Panasonic?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/NewLife.Panasonic?label=dev%20nuget&logo=nuget)

> **🌐 Language**: [English](Readme.en.md) | [Español](Readme.es.md) | [Deutsch](Readme.de.md) | [Français](Readme.fr.md) | [Português](Readme.pt-BR.md) | [Русский](Readme.ru.md) | [العربية](Readme.ar.md) | [한국어](Readme.ko.md) | [日本語](Readme.ja-JP.md) | [中文](Readme.MD)

.NET용 파나소닉 PLC 통신 프로토콜 라이브러리입니다. NewLife.IoT 표준 프레임워크를 기반으로 .NET 애플리케이션에서 파나소닉 PLC 장치를 위한 다양한 통신 드라이버(Modbus TCP / Modbus RTU / Mewtocol TCP)를 제공합니다. **모든 기능이 구현되었으며 테스트를 통해 검증되었습니다.**

## 주요 특징

- **세 가지 프로토콜을 하나로**: Modbus TCP / Modbus RTU를 자동으로 선택하는 통합 드라이버 진입점과 파나소닉 PLC 주요 통신 시나리오를 포괄하는 독립적인 Mewtocol 드라이버
- **안정적인 연결**: 내장된 하트비트 감지 및 자동 재연결로 산업 자동화 환경에 적합
- **에코시스템 시너지**: NewLife.IoT 표준 인터페이스 기반, IoTEdge 및 ZeroIoT 플랫폼과 플러그앤플레이
- **MIT 라이선스**: 가장 관대한 오픈소스 라이선스, 상업적 사용에 제한 없음

## 빠른 시작

```xml
<PackageReference Include="NewLife.Panasonic" Version="1.0.*" />
```

### Modbus TCP (Modbus 게이트웨이를 통한 연결)

```csharp
// 드라이버 생성 (하트비트 감지 및 자동 재연결 내장)
var driver = new PanasonicDriver
{
    AutoReconnect = true,
    HeartbeatInterval = 30,
};

// 매개변수 설정
var param = new PanasonicParameter
{
    Server = "192.168.1.100:502",
    Host = 1,
};

// 채널 열기
var node = driver.Open(null, param);
```

### Modbus RTU (시리얼 포트 연결, 동일 드라이버에서 자동 인식)

```csharp
// 동일한 드라이버, 시리얼 포트 매개변수만 입력하면 자동으로 RTU 프로토콜 사용
var driver = new PanasonicDriver();
var param = new PanasonicParameter
{
    PortName = "COM3",
    Baudrate = 9600,
    Host = 1,
};

// 채널 열기 (드라이버 자동 판단: PortName이 비어있지 않음 → Modbus RTU)
var node = driver.Open(null, param);
```

### Mewtocol 네이티브 프로토콜 (TCP 직접 연결)

```csharp
// Mewtocol 드라이버 생성 (Mewtocol 프로토콜만 지원하는 파나소닉 PLC용)
var driver = new MewtocolDriver();

// 매개변수 설정 (기본 포트 9094)
var param = new MewtocolParameter
{
    Server = "192.168.1.100:9094",
    Station = 1,
};

// 채널 열기
var node = await driver.OpenAsync(null, param);
```

## 드라이버 개요

| 드라이버 | 프로토콜 | 연결 방식 | 등록 이름 |
|---------|---------|----------|----------|
| `PanasonicDriver` | Modbus TCP / Modbus RTU | 이더넷 또는 시리얼 (매개변수 자동 선택) | `PanasonicPLC` |
| `MewtocolDriver` | Mewtocol 네이티브 | 이더넷 (PLC 직접 연결) | `PanasonicMewtocol` |

## 문서

| 문서 | 설명 |
|------|------|
| [요구사항](Doc/需求文档.md) | 비전, 핵심 기능 및 기능 경계 (v3.0) |
| [기능 목록](Doc/功能清单.md) | 전체 8개 기능 포인트 완료 (v3.0) |
| [아키텍처](Doc/架构设计.md) | 계층, 구성 요소, 흐름 및 설계 결정 (v3.0) |
| [경쟁사 분석](Doc/竞品分析报告.md) | 경쟁사 비교, 기능 매트릭스, 격차 분석 (v3.0) |

소스 코드: https://github.com/NewLifeX/NewLife.Panasonic  
NuGet: NewLife.Panasonic  

---

## NewLife Development Team

![NewLife](https://newlifex.com/logo.png)

2002년에 설립된 NewLife 팀은 IoT 업계 솔루션 제공업체로, 소프트웨어/하드웨어 애플리케이션 컨설팅, 시스템 아키텍처 계획 및 개발 서비스를 제공합니다.

팀은 80개 이상의 오픈소스 프로젝트를 시작하여 다양한 산업에서 널리 사용되고 있으며, NuGet 다운로드 수는 400만 회를 초과합니다.

웹사이트: https://newlifex.com  
오픈소스: https://github.com/newlifex  
QQ 그룹: 1600800/1600838  
