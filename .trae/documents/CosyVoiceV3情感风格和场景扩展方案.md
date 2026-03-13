# CosyVoiceV3 情感风格和场景扩展方案

## 一、需求分析

为 CosyVoiceV3 TTL引擎扩展：
1. **情感风格**：新增"害怕"、"沮丧"
2. **场景**：新增"谈话"

## 二、当前枚举状态

### 2.1 情感风格（EmotionFeature）
| 序号 | 选项 | 枚举值 | 状态 |
|------|------|--------|------|
| 1 | 不选择 | 0 | 已存在 |
| 2 | 温柔 | 1 | 已存在 |
| 3 | 开心 | 2 | 已存在 |
| 4 | 严肃 | 3 | 已存在 |
| 5 | 悲伤 | 4 | 已存在 |
| 6 | 生气 | 5 | 已存在 |
| 7 | 害怕 | 6 | **新增** |
| 8 | 沮丧 | 7 | **新增** |

### 2.2 场景（SceneFeature）
| 序号 | 选项 | 枚举值 | 状态 |
|------|------|--------|------|
| 1 | 不选择 | 0 | 已存在 |
| 2 | 新闻播报 | 1 | 已存在 |
| 3 | 讲故事 | 2 | 已存在 |
| 4 | 客服 | 3 | 已存在 |
| 5 | 朗诵 | 4 | 已存在 |
| 6 | 谈话 | 5 | **新增** |

## 三、实现步骤

### 步骤1：修改情感风格枚举类
**文件**：`GW.TTLtoolsBox.Core/TtlEngine/Features/EmotionFeature.cs`

在 `生气 = 5` 后添加：
```csharp
/// <summary>
/// 害怕
/// </summary>
[Description("害怕")]
[FeatureOption("害怕的语气")]
害怕 = 6,

/// <summary>
/// 沮丧
/// </summary>
[Description("沮丧")]
[FeatureOption("沮丧的语气")]
沮丧 = 7
```

### 步骤2：修改场景枚举类
**文件**：`GW.TTLtoolsBox.Core/TtlEngine/Features/SceneFeature.cs`

在 `朗诵 = 4` 后添加：
```csharp
/// <summary>
/// 谈话
/// </summary>
[Description("谈话")]
[FeatureOption("谈话的语气")]
谈话 = 5
```

## 四、文件变更清单

| 操作 | 文件路径 | 说明 |
|------|----------|------|
| 修改 | `Features/EmotionFeature.cs` | 添加"害怕"、"沮丧" |
| 修改 | `Features/SceneFeature.cs` | 添加"谈话" |

## 五、instruct 参数示例

选择新增选项后，生成的 instruct 参数格式：
```
请用害怕的语气表达。
请用沮丧的语气表达。
请用谈话的语气表达。
```
