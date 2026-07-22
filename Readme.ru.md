# NewLife.Panasonic - Протокол PLC Panasonic

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/NewLife.Panasonic?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/NewLife.Panasonic?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/NewLife.Panasonic?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/NewLife.Panasonic?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/NewLife.Panasonic?label=dev%20nuget&logo=nuget)

> **🌐 Language**: [English](Readme.en.md) | [Español](Readme.es.md) | [Deutsch](Readme.de.md) | [Français](Readme.fr.md) | [Português](Readme.pt-BR.md) | [Русский](Readme.ru.md) | [العربية](Readme.ar.md) | [한국어](Readme.ko.md) | [日本語](Readme.ja-JP.md) | [中文](Readme.MD)

Библиотека протокола связи с PLC Panasonic для .NET. Построенная на стандартном фреймворке NewLife.IoT, она предоставляет несколько драйверов связи (Modbus TCP / Modbus RTU / Mewtocol TCP) для устройств PLC Panasonic из .NET-приложений. **Все функции реализованы и проверены тестами.**

## Особенности

- **Три протокола в одном**: Единая точка входа, автоматически выбирающая между Modbus TCP / Modbus RTU, с независимым драйвером Mewtocol, охватывающим основные сценарии связи с PLC Panasonic
- **Надёжное соединение**: Встроенное обнаружение heartbeat и автоматическое переподключение, подходит для сред промышленной автоматизации
- **Экосистемная синергия**: Основан на стандартных интерфейсах NewLife.IoT, plug-and-play с платформами IoTEdge и ZeroIoT
- **Лицензия MIT**: Самая разрешительная лицензия с открытым исходным кодом, неограниченное коммерческое использование

## Быстрый Старт

```xml
<PackageReference Include="NewLife.Panasonic" Version="1.0.*" />
```

### Modbus TCP (через шлюз Modbus)

```csharp
// Создать драйвер (со встроенным heartbeat и автоматическим переподключением)
var driver = new PanasonicDriver
{
    AutoReconnect = true,
    HeartbeatInterval = 30,
};

// Настроить параметры
var param = new PanasonicParameter
{
    Server = "192.168.1.100:502",
    Host = 1,
};

// Открыть канал
var node = driver.Open(null, param);
```

### Modbus RTU (Последовательный порт, Автоматическое определение)

```csharp
// Тот же драйвер, просто заполните параметры последовательного порта
var driver = new PanasonicDriver();
var param = new PanasonicParameter
{
    PortName = "COM3",
    Baudrate = 9600,
    Host = 1,
};

// Открыть канал (автоматическое определение: PortName не пуст → Modbus RTU)
var node = driver.Open(null, param);
```

### Mewtocol Нативный (Прямое TCP-соединение)

```csharp
// Создать драйвер Mewtocol (для PLC Panasonic, поддерживающих только Mewtocol)
var driver = new MewtocolDriver();

// Настроить параметры (порт по умолчанию 9094)
var param = new MewtocolParameter
{
    Server = "192.168.1.100:9094",
    Station = 1,
};

// Открыть канал
var node = await driver.OpenAsync(null, param);
```

## Обзор Драйверов

| Драйвер | Протокол | Соединение | Имя регистрации |
|---------|----------|------------|-----------------|
| `PanasonicDriver` | Modbus TCP / Modbus RTU | Ethernet или Последовательный порт (автовыбор) | `PanasonicPLC` |
| `MewtocolDriver` | Mewtocol Нативный | Ethernet (прямое подключение к PLC) | `PanasonicMewtocol` |

## Документация

| Документ | Описание |
|----------|----------|
| [Требования](Doc/需求文档.md) | Видение, основные функции и границы возможностей (v3.0) |
| [Список функций](Doc/功能清单.md) | Все 8 пунктов функций завершены (v3.0) |
| [Архитектура](Doc/架构设计.md) | Уровни, компоненты, потоки и проектные решения (v3.0) |
| [Конкурентный анализ](Doc/竞品分析报告.md) | Сравнение, матрица функций, анализ пробелов (v3.0) |

Исходный код: https://github.com/NewLifeX/NewLife.Panasonic  
NuGet: NewLife.Panasonic  

---

## NewLife Development Team

![NewLife](https://newlifex.com/logo.png)

Основанная в 2002 году, команда NewLife является поставщиком решений для IoT, предлагая консультации по программным/аппаратным приложениям, планирование системной архитектуры и услуги разработки.

Команда инициировала более 80 проектов с открытым исходным кодом, широко используемых в различных отраслях, с более чем 4 миллионами загрузок на NuGet.

Веб-сайт: https://newlifex.com  
Открытый код: https://github.com/newlifex  
Группы QQ: 1600800/1600838  
