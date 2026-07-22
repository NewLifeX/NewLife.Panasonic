# NewLife.Panasonic - Protocolo PLC Panasonic

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/NewLife.Panasonic?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/NewLife.Panasonic?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/NewLife.Panasonic?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/NewLife.Panasonic?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/NewLife.Panasonic?label=dev%20nuget&logo=nuget)

> **🌐 Language**: [English](Readme.en.md) | [Español](Readme.es.md) | [Deutsch](Readme.de.md) | [Français](Readme.fr.md) | [Português](Readme.pt-BR.md) | [Русский](Readme.ru.md) | [العربية](Readme.ar.md) | [한국어](Readme.ko.md) | [日本語](Readme.ja-JP.md) | [中文](Readme.MD)

Biblioteca de protocolo de comunicação PLC Panasonic para .NET. Construída sobre o framework padrão NewLife.IoT, fornece múltiplos drivers de comunicação (Modbus TCP / Modbus RTU / Mewtocol TCP) para dispositivos PLC Panasonic a partir de aplicações .NET. **Todos os recursos estão implementados e verificados por testes.**

## Destaques

- **Três protocolos em um**: Um ponto de entrada unificado que seleciona automaticamente entre Modbus TCP / Modbus RTU, com um driver Mewtocol independente cobrindo cenários principais de comunicação com PLC Panasonic
- **Conexão confiável**: Detecção de heartbeat e reconexão automática integradas, adequadas para ambientes de automação industrial
- **Sinergia do ecossistema**: Baseado nas interfaces padrão NewLife.IoT, plug-and-play com as plataformas IoTEdge e ZeroIoT
- **Licença MIT**: A licença de código aberto mais permissiva, uso comercial sem restrições

## Início Rápido

```xml
<PackageReference Include="NewLife.Panasonic" Version="1.0.*" />
```

### Modbus TCP (via Gateway Modbus)

```csharp
// Criar driver (com heartbeat e reconexão automática integrados)
var driver = new PanasonicDriver
{
    AutoReconnect = true,
    HeartbeatInterval = 30,
};

// Configurar parâmetros
var param = new PanasonicParameter
{
    Server = "192.168.1.100:502",
    Host = 1,
};

// Abrir canal
var node = driver.Open(null, param);
```

### Modbus RTU (Porta Serial, Detecção Automática)

```csharp
// Mesmo driver, basta preencher os parâmetros da porta serial
var driver = new PanasonicDriver();
var param = new PanasonicParameter
{
    PortName = "COM3",
    Baudrate = 9600,
    Host = 1,
};

// Abrir canal (detecção automática: PortName não vazio → Modbus RTU)
var node = driver.Open(null, param);
```

### Mewtocol Nativo (Conexão TCP Direta)

```csharp
// Criar driver Mewtocol (para PLCs Panasonic que suportam apenas Mewtocol)
var driver = new MewtocolDriver();

// Configurar parâmetros (porta padrão 9094)
var param = new MewtocolParameter
{
    Server = "192.168.1.100:9094",
    Station = 1,
};

// Abrir canal
var node = await driver.OpenAsync(null, param);
```

## Visão Geral dos Drivers

| Driver | Protocolo | Conexão | Nome de Registro |
|--------|-----------|---------|------------------|
| `PanasonicDriver` | Modbus TCP / Modbus RTU | Ethernet ou Serial (seleção automática) | `PanasonicPLC` |
| `MewtocolDriver` | Mewtocol Nativo | Ethernet (conexão direta PLC) | `PanasonicMewtocol` |

## Documentação

| Documento | Descrição |
|-----------|-----------|
| [Requisitos](Doc/需求文档.md) | Visão, funções principais e limites de capacidade (v3.0) |
| [Lista de Funções](Doc/功能清单.md) | Todos os 8 pontos de função concluídos (v3.0) |
| [Arquitetura](Doc/架构设计.md) | Camadas, componentes, fluxos e decisões de design (v3.0) |
| [Análise Concorrencial](Doc/竞品分析报告.md) | Comparação, matriz de funções, análise de lacunas (v3.0) |

Código Fonte: https://github.com/NewLifeX/NewLife.Panasonic  
NuGet: NewLife.Panasonic  

---

## NewLife Development Team

![NewLife](https://newlifex.com/logo.png)

Fundado em 2002, o time NewLife é um fornecedor de soluções IoT, oferecendo consultoria em aplicações de software/hardware, planejamento de arquitetura de sistemas e serviços de desenvolvimento.

O time iniciou mais de 80 projetos de código aberto amplamente utilizados em diversos setores, com mais de 4 milhões de downloads no NuGet.

Site: https://newlifex.com  
Código aberto: https://github.com/newlifex  
Grupos QQ: 1600800/1600838  
