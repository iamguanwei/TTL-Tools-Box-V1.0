# CosyVoiceV3 引擎添加语速、音量参数及方言扩展方案

## 一、需求分析

为 CosyVoiceV3 TTL引擎进行以下扩展：
1. **语速参数**：快速、慢速
2. **音量参数**：大声、轻声
3. **方言扩展**：新增13种方言选项

## 二、当前架构分析

### 2.1 现有方言选项
| 序号 | 方言 | 状态 |
|------|------|------|
| 1 | 上海话 | ✓ 已存在 |
| 2 | 粤语 | ✓ 已存在 |
| 3 | 四川话 | ✓ 已存在 |
| 4 | 东北话 | ✓ 已存在 |

### 2.2 需新增方言
| 序号 | 方言 | 枚举值 |
|------|------|--------|
| 1 | 天津话 | 5 |
| 2 | 山东话 | 6 |
| 3 | 河南话 | 7 |
| 4 | 陕西话 | 8 |
| 5 | 山西话 | 9 |
| 6 | 甘肃话 | 10 |
| 7 | 云南话 | 11 |
| 8 | 贵州话 | 12 |
| 9 | 湖北话 | 13 |
| 10 | 湖南话 | 14 |
| 11 | 江西话 | 15 |
| 12 | 闽南话 | 16 |
| 13 | 宁夏话 | 17 |

### 2.3 特性传递机制
所有特性统一通过 `instruct` 参数传递，格式：`请用{选项}表达。`

**URL格式**：
```
{baseUrl}/?text={text}&speaker={speaker}&instruct={instruct}
```

**instruct示例**：
```
请用上海话表达。请用温柔的语气表达。请用快速的语速表达。请用大声的音量表达。
```

## 三、方案设计

### 3.1 实现步骤

#### 步骤1：创建语速枚举类
**文件**：`GW.TTLtoolsBox.Core/TtlEngine/Features/SpeedFeature.cs`

```csharp
namespace GW.TTLtoolsBox.Core.TtlEngine.Features;

/// <summary>
/// 语速特性枚举，定义TTL引擎支持的语速选项
/// </summary>
public enum SpeedFeature
{
    [Description("正常")]
    [FeatureOption("")]
    正常 = 0,

    [Description("快速")]
    [FeatureOption("快速的语速")]
    快速 = 1,

    [Description("慢速")]
    [FeatureOption("慢速的语速")]
    慢速 = 2
}
```

#### 步骤2：创建音量枚举类
**文件**：`GW.TTLtoolsBox.Core/TtlEngine/Features/VolumeFeature.cs`

```csharp
namespace GW.TTLtoolsBox.Core.TtlEngine.Features;

/// <summary>
/// 音量特性枚举，定义TTL引擎支持的音量选项
/// </summary>
public enum VolumeFeature
{
    [Description("正常")]
    [FeatureOption("")]
    正常 = 0,

    [Description("大声")]
    [FeatureOption("大声的音量")]
    大声 = 1,

    [Description("轻声")]
    [FeatureOption("轻声的音量")]
    轻声 = 2
}
```

#### 步骤3：扩展方言枚举类
**文件**：`GW.TTLtoolsBox.Core/TtlEngine/Features/DialectFeature.cs`

在现有枚举基础上添加13种新方言（枚举值从5开始）。

#### 步骤4：修改CosyVoiceV3LYttlEngineConnector类
**文件**：`GW.TTLtoolsBox.Core/TtlEngine/CosyVoiceV3LYttlEngineConnector.cs`

修改特性定义数组，添加语速和音量：
```csharp
private static readonly TtlEngineFeatureDefinition[] _featureDefinitions = new[]
{
    new TtlEngineFeatureDefinition("方言", typeof(DialectFeature), "请用{0}表达。"),
    new TtlEngineFeatureDefinition("情感风格", typeof(EmotionFeature), "请用{0}表达。"),
    new TtlEngineFeatureDefinition("场景", typeof(SceneFeature), "请用{0}表达。"),
    new TtlEngineFeatureDefinition("语速", typeof(SpeedFeature), "请用{0}表达。"),
    new TtlEngineFeatureDefinition("音量", typeof(VolumeFeature), "请用{0}表达。")
};
```

更新类描述，添加语速和音量特性说明。

## 四、文件变更清单

| 操作 | 文件路径 | 说明 |
|------|----------|------|
| 新增 | `Features/SpeedFeature.cs` | 语速枚举类 |
| 新增 | `Features/VolumeFeature.cs` | 音量枚举类 |
| 修改 | `Features/DialectFeature.cs` | 添加13种新方言 |
| 修改 | `CosyVoiceV3LYttlEngineConnector.cs` | 添加语速和音量特性定义 |

## 五、优势

此方案的优势：
1. **代码改动最小**：无需修改 `TtlEngineFeatureDefinition` 类，复用现有机制
2. **架构一致性**：所有特性统一通过 `instruct` 参数传递
3. **易于扩展**：未来新增特性只需添加枚举类和特性定义

---

*请确认以上方案是否符合需求。*
