# NewLife.Panasonic - パナソニックPLCプロトコル

![GitHub top language](https://img.shields.io/github/languages/top/newlifex/NewLife.Panasonic?logo=github)
![GitHub License](https://img.shields.io/github/license/newlifex/NewLife.Panasonic?logo=github)
![Nuget Downloads](https://img.shields.io/nuget/dt/NewLife.Panasonic?logo=nuget)
![Nuget](https://img.shields.io/nuget/v/NewLife.Panasonic?logo=nuget)
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/NewLife.Panasonic?label=dev%20nuget&logo=nuget)

> **🌐 Language**: [English](Readme.en.md) | [Español](Readme.es.md) | [Deutsch](Readme.de.md) | [Français](Readme.fr.md) | [Português](Readme.pt-BR.md) | [Русский](Readme.ru.md) | [العربية](Readme.ar.md) | [한국어](Readme.ko.md) | [日本語](Readme.ja-JP.md) | [中文](Readme.MD)

.NET向けパナソニックPLC通信プロトコルライブラリ。NewLife.IoT標準フレームワークに基づき、.NETアプリケーションにパナソニックPLCデバイス向けの複数の通信ドライバ（Modbus TCP / Modbus RTU / Mewtocol TCP）を提供します。**全機能が実装され、テストで検証済みです。**

## 特長

- **3つのプロトコルを一元化**：同一ドライバエントリがModbus TCP / Modbus RTUを自動選択、Mewtocol独立ドライバでパナソニックPLCの主流通信シナリオをカバー
- **信頼性の高い接続**：ハートビート検出と自動再接続を内蔵、産業オートメーション環境に最適
- **エコシステム連携**：NewLife.IoT標準インターフェースに基づき、IoTEdge、ZeroIoTプラットフォームとプラグアンドプレイ
- **MITライセンス**：最も緩和なオープンソースライセンス、商用利用に制限なし

## クイックスタート

```xml
<PackageReference Include="NewLife.Panasonic" Version="1.0.*" />
```

### Modbus TCP（Modbusゲートウェイ経由）

```csharp
// ドライバを作成（ハートビート検出と自動再接続を内蔵）
var driver = new PanasonicDriver
{
    AutoReconnect = true,
    HeartbeatInterval = 30,
};

// パラメータを設定
var param = new PanasonicParameter
{
    Server = "192.168.1.100:502",
    Host = 1,
};

// チャネルを開く
var node = driver.Open(null, param);
```

### Modbus RTU（シリアルポート接続、同一ドライバで自動認識）

```csharp
// 同一ドライバ、シリアルポートパラメータを入力するだけで自動的にRTUプロトコルを使用
var driver = new PanasonicDriver();
var param = new PanasonicParameter
{
    PortName = "COM3",
    Baudrate = 9600,
    Host = 1,
};

// チャネルを開く（ドライバが自動判定：PortNameが空でない → Modbus RTU）
var node = driver.Open(null, param);
```

### Mewtocolネイティブプロトコル（TCP直接接続）

```csharp
// Mewtocolドライバを作成（Mewtocolプロトコルのみ対応のパナソニックPLC向け）
var driver = new MewtocolDriver();

// パラメータを設定（デフォルトポート9094）
var param = new MewtocolParameter
{
    Server = "192.168.1.100:9094",
    Station = 1,
};

// チャネルを開く
var node = await driver.OpenAsync(null, param);
```

## ドライバ一覧

| ドライバ | プロトコル | 接続方式 | 登録名 |
|----------|-----------|---------|--------|
| `PanasonicDriver` | Modbus TCP / Modbus RTU | イーサネットまたはシリアル（パラメータ自動選択） | `PanasonicPLC` |
| `MewtocolDriver` | Mewtocolネイティブ | イーサネット（PLC直接接続） | `PanasonicMewtocol` |

## ドキュメント

| ドキュメント | 説明 |
|-------------|------|
| [要件定義](Doc/需求文档.md) | ビジョン、コア機能、能力範囲（v3.0） |
| [機能一覧](Doc/功能清单.md) | 全8機能完了（v3.0） |
| [アーキテクチャ](Doc/架构设计.md) | 階層、コンポーネント、フロー、設計判断（v3.0） |
| [競合分析](Doc/竞品分析报告.md) | 競合比較、機能マトリックス、ギャップ分析（v3.0） |

ソースコード： https://github.com/NewLifeX/NewLife.Panasonic  
NuGet： NewLife.Panasonic  

---

## 新生命開発チーム

![NewLife](https://newlifex.com/logo.png)

2002年設立のNewLifeチームは、IoT業界のソリューション提供者であり、ソフトウェア/ハードウェアアプリケーションコンサルティング、システムアーキテクチャ計画、開発サービスを提供しています。

チームは80以上のオープンソースプロジェクトを立ち上げ、様々な業界で広く活用され、NuGetダウンロード数は400万を超えています。

Webサイト： https://newlifex.com  
オープンソース： https://github.com/newlifex  
QQグループ： 1600800/1600838  
