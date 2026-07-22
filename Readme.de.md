# NewLife.Panasonic - Panasonic-SPS-Protokoll

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/NewLife.Panasonic?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/NewLife.Panasonic?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/NewLife.Panasonic?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/NewLife.Panasonic?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/NewLife.Panasonic?label=dev%20nuget&logo=nuget)

> **🌐 Language**: [English](Readme.en.md) | [Español](Readme.es.md) | [Deutsch](Readme.de.md) | [Français](Readme.fr.md) | [Português](Readme.pt-BR.md) | [Русский](Readme.ru.md) | [العربية](Readme.ar.md) | [한국어](Readme.ko.md) | [日本語](Readme.ja-JP.md) | [中文](Readme.MD)

Panasonic-SPS-Kommunikationsprotokollbibliothek für .NET. Basierend auf dem NewLife.IoT-Standardframework bietet sie mehrere Kommunikationstreiber (Modbus TCP / Modbus RTU / Mewtocol TCP) für Panasonic-SPS-Geräte aus .NET-Anwendungen. **Alle Funktionen sind implementiert und durch Tests verifiziert.**

## Highlights

- **Drei Protokolle in einem**: Ein einheitlicher Treibereintrag, der automatisch zwischen Modbus TCP / Modbus RTU wählt, mit einem unabhängigen Mewtocol-Treiber für gängige Panasonic-SPS-Kommunikationsszenarien
- **Zuverlässige Verbindung**: Integrierte Heartbeat-Erkennung und automatische Wiederverbindung, geeignet für industrielle Automatisierungsumgebungen
- **Ökosystem-Synergie**: Basierend auf NewLife.IoT-Standardschnittstellen, Plug-and-Play mit IoTEdge- und ZeroIoT-Plattformen
- **MIT-Lizenz**: Die freizügigste Open-Source-Lizenz, uneingeschränkte kommerzielle Nutzung

## Schnellstart

```xml
<PackageReference Include="NewLife.Panasonic" Version="1.0.*" />
```

### Modbus TCP (über Modbus-Gateway)

```csharp
// Treiber erstellen (mit integrierter Heartbeat-Erkennung und automatischer Wiederverbindung)
var driver = new PanasonicDriver
{
    AutoReconnect = true,
    HeartbeatInterval = 30,
};

// Parameter konfigurieren
var param = new PanasonicParameter
{
    Server = "192.168.1.100:502",
    Host = 1,
};

// Kanal öffnen
var node = driver.Open(null, param);
```

### Modbus RTU (Serielle Schnittstelle, Automatische Erkennung)

```csharp
// Gleicher Treiber, einfach serielle Parameter ausfüllen
var driver = new PanasonicDriver();
var param = new PanasonicParameter
{
    PortName = "COM3",
    Baudrate = 9600,
    Host = 1,
};

// Kanal öffnen (automatische Erkennung: PortName nicht leer → Modbus RTU)
var node = driver.Open(null, param);
```

### Mewtocol Nativ (Direkte TCP-Verbindung)

```csharp
// Mewtocol-Treiber erstellen (für Panasonic-SPS, die nur Mewtocol unterstützen)
var driver = new MewtocolDriver();

// Parameter konfigurieren (Standardport 9094)
var param = new MewtocolParameter
{
    Server = "192.168.1.100:9094",
    Station = 1,
};

// Kanal öffnen
var node = await driver.OpenAsync(null, param);
```

## Treiberübersicht

| Treiber | Protokoll | Verbindung | Registrierungsname |
|---------|-----------|------------|--------------------|
| `PanasonicDriver` | Modbus TCP / Modbus RTU | Ethernet oder Seriell (automatische Auswahl) | `PanasonicPLC` |
| `MewtocolDriver` | Mewtocol Nativ | Ethernet (direkte SPS-Verbindung) | `PanasonicMewtocol` |

## Dokumentation

| Dokument | Beschreibung |
|----------|--------------|
| [Anforderungen](Doc/需求文档.md) | Vision, Kernfunktionen und Fähigkeitsgrenzen (v3.0) |
| [Funktionsliste](Doc/功能清单.md) | Alle 8 Funktionen abgeschlossen (v3.0) |
| [Architektur](Doc/架构设计.md) | Schichten, Komponenten, Abläufe und Entwurfsentscheidungen (v3.0) |
| [Wettbewerbsanalyse](Doc/竞品分析报告.md) | Vergleich, Funktionsmatrix, Lückenanalyse (v3.0) |

Quellcode: https://github.com/NewLifeX/NewLife.Panasonic  
NuGet: NewLife.Panasonic  

---

## NewLife Development Team

![NewLife](https://newlifex.com/logo.png)

Gegründet im Jahr 2002, ist das NewLife-Team ein IoT-Lösungsanbieter, der Beratung für Software-/Hardware-Anwendungen, Systemarchitekturplanung und Entwicklungsdienstleistungen anbietet.

Das Team hat über 80 Open-Source-Projekte initiiert, die in verschiedenen Branchen weit verbreitet sind, mit über 4 Millionen NuGet-Downloads.

Webseite: https://newlifex.com  
Open Source: https://github.com/newlifex  
QQ-Gruppen: 1600800/1600838  
