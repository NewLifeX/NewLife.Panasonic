# NewLife.Panasonic - Protocolo PLC Panasonic

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/NewLife.Panasonic?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/NewLife.Panasonic?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/NewLife.Panasonic?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/NewLife.Panasonic?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/NewLife.Panasonic?label=dev%20nuget&logo=nuget)

> **🌐 Language**: [English](Readme.en.md) | [Español](Readme.es.md) | [Deutsch](Readme.de.md) | [Français](Readme.fr.md) | [Português](Readme.pt-BR.md) | [Русский](Readme.ru.md) | [العربية](Readme.ar.md) | [한국어](Readme.ko.md) | [日本語](Readme.ja-JP.md) | [中文](Readme.MD)

Biblioteca de protocolo de comunicación PLC Panasonic para .NET. Construida sobre el framework estándar NewLife.IoT, proporciona múltiples controladores de comunicación (Modbus TCP / Modbus RTU / Mewtocol TCP) para dispositivos PLC Panasonic desde aplicaciones .NET. **Todas las funciones están implementadas y verificadas mediante pruebas.**

## Destacados

- **Tres protocolos en uno**: Un punto de entrada unificado que selecciona automáticamente entre Modbus TCP / Modbus RTU, con un controlador Mewtocol independiente que cubre los escenarios principales de comunicación con PLC Panasonic
- **Conexión confiable**: Detección de heartbeat y reconexión automática integradas, ideal para entornos de automatización industrial
- **Sinergia del ecosistema**: Basado en interfaces estándar de NewLife.IoT, plug-and-play con las plataformas IoTEdge y ZeroIoT
- **Licencia MIT**: La licencia de código abierto más permisiva, sin restricciones de uso comercial

## Inicio Rápido

```xml
<PackageReference Include="NewLife.Panasonic" Version="1.0.*" />
```

### Modbus TCP (a través de Gateway Modbus)

```csharp
// Crear controlador (con heartbeat y reconexión automática integrados)
var driver = new PanasonicDriver
{
    AutoReconnect = true,
    HeartbeatInterval = 30,
};

// Configurar parámetros
var param = new PanasonicParameter
{
    Server = "192.168.1.100:502",
    Host = 1,
};

// Abrir canal
var node = driver.Open(null, param);
```

### Modbus RTU (Puerto Serie, Detección Automática)

```csharp
// Mismo controlador, solo complete los parámetros del puerto serie
var driver = new PanasonicDriver();
var param = new PanasonicParameter
{
    PortName = "COM3",
    Baudrate = 9600,
    Host = 1,
};

// Abrir canal (detección automática: PortName no vacío → Modbus RTU)
var node = driver.Open(null, param);
```

### Mewtocol Nativo (Conexión TCP Directa)

```csharp
// Crear controlador Mewtocol (para PLC Panasonic que solo soportan Mewtocol)
var driver = new MewtocolDriver();

// Configurar parámetros (puerto predeterminado 9094)
var param = new MewtocolParameter
{
    Server = "192.168.1.100:9094",
    Station = 1,
};

// Abrir canal
var node = await driver.OpenAsync(null, param);
```

## Resumen de Controladores

| Controlador | Protocolo | Conexión | Nombre de Registro |
|-------------|-----------|----------|--------------------|
| `PanasonicDriver` | Modbus TCP / Modbus RTU | Ethernet o Serie (selección automática) | `PanasonicPLC` |
| `MewtocolDriver` | Mewtocol Nativo | Ethernet (conexión directa PLC) | `PanasonicMewtocol` |

## Documentación

| Documento | Descripción |
|-----------|-------------|
| [Requisitos](Doc/需求文档.md) | Visión, funciones principales y límites de capacidad (v3.0) |
| [Lista de Funciones](Doc/功能清单.md) | Todos los 8 puntos de función completados (v3.0) |
| [Arquitectura](Doc/架构设计.md) | Capas, componentes, flujos y decisiones de diseño (v3.0) |
| [Análisis Competitivo](Doc/竞品分析报告.md) | Comparativa, matriz de funciones, análisis de brechas (v3.0) |

Código Fuente: https://github.com/NewLifeX/NewLife.Panasonic  
NuGet: NewLife.Panasonic  

---

## NewLife Development Team

![NewLife](https://newlifex.com/logo.png)

Fundado en 2002, el equipo NewLife es un proveedor de soluciones IoT, ofreciendo consultoría de aplicaciones de software/hardware, planificación de arquitectura de sistemas y servicios de desarrollo.

El equipo ha iniciado más de 80 proyectos de código abierto ampliamente utilizados en diversas industrias, con descargas de NuGet que superan los 4 millones.

Sitio web: https://newlifex.com  
Código abierto: https://github.com/newlifex  
Grupos QQ: 1600800/1600838  
