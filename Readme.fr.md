# NewLife.Panasonic - Protocole PLC Panasonic

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/NewLife.Panasonic?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/NewLife.Panasonic?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/NewLife.Panasonic?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/NewLife.Panasonic?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/NewLife.Panasonic?label=dev%20nuget&logo=nuget)

> **🌐 Language**: [English](Readme.en.md) | [Español](Readme.es.md) | [Deutsch](Readme.de.md) | [Français](Readme.fr.md) | [Português](Readme.pt-BR.md) | [Русский](Readme.ru.md) | [العربية](Readme.ar.md) | [한국어](Readme.ko.md) | [日本語](Readme.ja-JP.md) | [中文](Readme.MD)

Bibliothèque de protocole de communication PLC Panasonic pour .NET. Construite sur le framework standard NewLife.IoT, elle fournit plusieurs pilotes de communication (Modbus TCP / Modbus RTU / Mewtocol TCP) pour les automates Panasonic à partir d'applications .NET. **Toutes les fonctionnalités sont implémentées et vérifiées par des tests.**

## Points Forts

- **Trois protocoles en un** : Un point d'entrée unifié qui sélectionne automatiquement entre Modbus TCP / Modbus RTU, avec un pilote Mewtocol indépendant couvrant les scénarios de communication principaux des automates Panasonic
- **Connexion fiable** : Détection de heartbeat et reconnexion automatique intégrées, adaptées aux environnements d'automatisation industrielle
- **Synergie d'écosystème** : Basé sur les interfaces standard NewLife.IoT, plug-and-play avec les plateformes IoTEdge et ZeroIoT
- **Licence MIT** : La licence open source la plus permissive, utilisation commerciale sans restriction

## Démarrage Rapide

```xml
<PackageReference Include="NewLife.Panasonic" Version="1.0.*" />
```

### Modbus TCP (via Passerelle Modbus)

```csharp
// Créer un pilote (avec heartbeat et reconnexion automatique intégrés)
var driver = new PanasonicDriver
{
    AutoReconnect = true,
    HeartbeatInterval = 30,
};

// Configurer les paramètres
var param = new PanasonicParameter
{
    Server = "192.168.1.100:502",
    Host = 1,
};

// Ouvrir le canal
var node = driver.Open(null, param);
```

### Modbus RTU (Port Série, Détection Automatique)

```csharp
// Même pilote, il suffit de remplir les paramètres du port série
var driver = new PanasonicDriver();
var param = new PanasonicParameter
{
    PortName = "COM3",
    Baudrate = 9600,
    Host = 1,
};

// Ouvrir le canal (détection automatique : PortName non vide → Modbus RTU)
var node = driver.Open(null, param);
```

### Mewtocol Natif (Connexion TCP Directe)

```csharp
// Créer un pilote Mewtocol (pour les automates Panasonic qui ne supportent que Mewtocol)
var driver = new MewtocolDriver();

// Configurer les paramètres (port par défaut 9094)
var param = new MewtocolParameter
{
    Server = "192.168.1.100:9094",
    Station = 1,
};

// Ouvrir le canal
var node = await driver.OpenAsync(null, param);
```

## Aperçu des Pilotes

| Pilote | Protocole | Connexion | Nom d'Enregistrement |
|--------|-----------|-----------|----------------------|
| `PanasonicDriver` | Modbus TCP / Modbus RTU | Ethernet ou Série (sélection automatique) | `PanasonicPLC` |
| `MewtocolDriver` | Mewtocol Natif | Ethernet (connexion directe automate) | `PanasonicMewtocol` |

## Documentation

| Document | Description |
|----------|-------------|
| [Exigences](Doc/需求文档.md) | Vision, fonctionnalités principales et limites (v3.0) |
| [Liste des Fonctions](Doc/功能清单.md) | Les 8 points de fonction terminés (v3.0) |
| [Architecture](Doc/架构设计.md) | Couches, composants, flux et décisions de conception (v3.0) |
| [Analyse Concurrentielle](Doc/竞品分析报告.md) | Comparaison, matrice fonctionnelle, analyse des écarts (v3.0) |

Code Source : https://github.com/NewLifeX/NewLife.Panasonic  
NuGet : NewLife.Panasonic  

---

## NewLife Development Team

![NewLife](https://newlifex.com/logo.png)

Fondée en 2002, l'équipe NewLife est un fournisseur de solutions IoT, offrant du conseil en applications logicielles/matérielles, de la planification d'architecture système et des services de développement.

L'équipe a initié plus de 80 projets open source largement utilisés dans diverses industries, avec plus de 4 millions de téléchargements NuGet.

Site web : https://newlifex.com  
Open Source : https://github.com/newlifex  
Groupes QQ : 1600800/1600838  
