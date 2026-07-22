# NewLife.Panasonic - بروتوكول PLC باناسونيك

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/NewLife.Panasonic?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/NewLife.Panasonic?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/NewLife.Panasonic?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/NewLife.Panasonic?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/NewLife.Panasonic?label=dev%20nuget&logo=nuget)

> **🌐 Language**: [English](Readme.en.md) | [Español](Readme.es.md) | [Deutsch](Readme.de.md) | [Français](Readme.fr.md) | [Português](Readme.pt-BR.md) | [Русский](Readme.ru.md) | [العربية](Readme.ar.md) | [한국어](Readme.ko.md) | [日本語](Readme.ja-JP.md) | [中文](Readme.MD)

مكتبة بروتوكول اتصال PLC باناسونيك لـ .NET. مبنية على إطار عمل NewLife.IoT القياسي، توفر برامج تشغيل اتصال متعددة (Modbus TCP / Modbus RTU / Mewtocol TCP) لأجهزة PLC باناسونيك من تطبيقات .NET. **جميع الميزات منفذة وموثقة من خلال الاختبارات.**

## المميزات

- **ثلاثة بروتوكولات في واحد**: نقطة دخول موحدة تختار تلقائياً بين Modbus TCP / Modbus RTU، مع برنامج تشغيل Mewtocol مستقل يغطي سيناريوهات الاتصال الرئيسية لـ PLC باناسونيك
- **اتصال موثوق**: كشف نبضات القلب وإعادة الاتصال التلقائي مضمنة، مناسبة لبيئات الأتمتة الصناعية
- **تكامل بيئي**: مبني على واجهات NewLife.IoT القياسية، تشغيل فوري مع منصات IoTEdge و ZeroIoT
- **رخصة MIT**: أكثر رخص المصدر المفتوح تسامحاً، استخدام تجاري بدون قيود

## بداية سريعة

```xml
<PackageReference Include="NewLife.Panasonic" Version="1.0.*" />
```

### Modbus TCP (عبر بوابة Modbus)

```csharp
// إنشاء برنامج تشغيل (مع كشف نبضات القلب وإعادة الاتصال التلقائي)
var driver = new PanasonicDriver
{
    AutoReconnect = true,
    HeartbeatInterval = 30,
};

// تكوين المعلمات
var param = new PanasonicParameter
{
    Server = "192.168.1.100:502",
    Host = 1,
};

// فتح القناة
var node = driver.Open(null, param);
```

### Modbus RTU (منفذ تسلسلي، كشف تلقائي)

```csharp
// نفس برنامج التشغيل، فقط املأ معلمات المنفذ التسلسلي
var driver = new PanasonicDriver();
var param = new PanasonicParameter
{
    PortName = "COM3",
    Baudrate = 9600,
    Host = 1,
};

// فتح القناة (كشف تلقائي: PortName غير فارغ → Modbus RTU)
var node = driver.Open(null, param);
```

### Mewtocol الأصلي (اتصال TCP مباشر)

```csharp
// إنشاء برنامج تشغيل Mewtocol (لـ PLC باناسونيك التي تدعم Mewtocol فقط)
var driver = new MewtocolDriver();

// تكوين المعلمات (المنفذ الافتراضي 9094)
var param = new MewtocolParameter
{
    Server = "192.168.1.100:9094",
    Station = 1,
};

// فتح القناة
var node = await driver.OpenAsync(null, param);
```

## نظرة عامة على برامج التشغيل

| برنامج التشغيل | البروتوكول | الاتصال | اسم التسجيل |
|---------------|-----------|---------|-------------|
| `PanasonicDriver` | Modbus TCP / Modbus RTU | إيثرنت أو تسلسلي (اختيار تلقائي) | `PanasonicPLC` |
| `MewtocolDriver` | Mewtocol أصلي | إيثرنت (اتصال مباشر بـ PLC) | `PanasonicMewtocol` |

## التوثيق

| المستند | الوصف |
|---------|-------|
| [المتطلبات](Doc/需求文档.md) | الرؤية، الوظائف الأساسية وحدود القدرات (v3.0) |
| [قائمة الميزات](Doc/功能清单.md) | جميع نقاط الميزات الثمانية مكتملة (v3.0) |
| [الهندسة المعمارية](Doc/架构设计.md) | الطبقات، المكونات، التدفقات وقرارات التصميم (v3.0) |
| [تحليل المنافسين](Doc/竞品分析报告.md) | مقارنة، مصفوفة الميزات، تحليل الفجوات (v3.0) |

الكود المصدري: https://github.com/NewLifeX/NewLife.Panasonic  
NuGet: NewLife.Panasonic  

---

## NewLife Development Team

![NewLife](https://newlifex.com/logo.png)

تأسس فريق NewLife في عام 2002، وهو مزود حلول إنترنت الأشياء، يقدم استشارات تطبيقات البرامج/الأجهزة، تخطيط هندسة الأنظمة وخدمات التطوير.

أطلق الفريق أكثر من 80 مشروع مفتوح المصدر مستخدمة على نطاق واسع في مختلف الصناعات، مع أكثر من 4 ملايين تحميل على NuGet.

الموقع: https://newlifex.com  
المصدر المفتوح: https://github.com/newlifex  
مجموعات QQ: 1600800/1600838  
