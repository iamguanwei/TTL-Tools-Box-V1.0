# TTL引擎特性扩展实现计划

## 一、需求分析

### 1.1 业务需求
- 每个TTL引擎可以具备不同的扩展参数（称为"特性"），如语气、方言、朗读情境等
- 这些特性在TTL引擎连接器实现类中通过**枚举**定义
- 有些引擎支持多个特性，有些一个也不支持
- 特性将在"角色和情绪指定"表格中动态显示为下拉列表列
- 特性值将在"语音生成预处理"和"语音生成"中传递使用

### 1.2 CosyVoice引擎特性示例

CosyVoice引擎支持3类特性：

#### 方言类指令
| 指令内容 | 效果 |
|---------|------|
| 请用上海话表达。 | 生成上海方言语音 |
| 请用粤语表达。 | 生成粤语（广东话）语音 |
| 请用四川话表达。 | 生成四川方言语音 |
| 请用东北话表达。 | 生成东北方言语音 |

#### 情感风格类指令
| 指令内容 | 效果 |
|---------|------|
| 请用温柔的语气表达。 | 语音轻柔、温和 |
| 请用开心的语气表达。 | 语音欢快、有活力 |
| 请用严肃的语气表达。 | 语音沉稳、正式 |
| 请用悲伤的语气表达。 | 语音低沉、带伤感 |
| 请用生气的语气表达。 | 语音急促、带愤怒 |

#### 场景类指令
| 指令内容 | 效果 |
|---------|------|
| 请用新闻播报的语气表达。 | 语音正式、清晰、平稳 |
| 请用讲故事的语气表达。 | 语音生动、有节奏感 |
| 请用客服的语气表达。 | 语音亲切、耐心 |
| 请用朗诵的语气表达。 | 语音抑扬顿挫、有感情 |

### 1.3 调用方式
在原有URL基础上增加`instruct`参数：
- 单个特性：`instruct=使用四川话。`
- 多个特性组合：`instruct=请用上海话、开心的语气表达。`
- 结尾必须以句号结束
- 多个特性之间用顿号分割

---

## 二、设计方案

### 2.1 核心设计思路
采用**枚举 + 特性标注**的方案：
- 每个特性类别定义为一个枚举类型
- 使用`[Description]`标注显示名称
- 使用自定义`[FeatureOption]`特性标注指令片段
- `TtlEngineFeatureDefinition`类通过反射读取枚举信息

### 2.2 为什么选择枚举方案？

| 对比项 | 枚举方案 | 纯类方案 |
|-------|---------|---------|
| 类型安全 | ✅ 编译时检查 | ⚠️ 运行时检查 |
| 代码简洁 | ✅ 定义简单 | ❌ 需要更多代码 |
| switch支持 | ✅ 原生支持 | ❌ 需要if-else |
| IDE支持 | ✅ 智能提示、跳转 | ⚠️ 一般 |
| 额外信息 | ⚠️ 需要特性标注 | ✅ 直接存储 |
| 动态创建 | ❌ 编译时确定 | ✅ 可运行时创建 |

**结论**：对于固定选项的特性，枚举方案更合适。

### 2.3 新增类型

#### 2.3.1 `FeatureOptionAttribute` 特性类
```csharp
namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine
{
    /// <summary>
    /// 特性选项特性，用于标注枚举项的指令片段
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class FeatureOptionAttribute : Attribute
    {
        /// <summary>
        /// 获取指令文本片段
        /// </summary>
        /// <remarks>
        /// 例如："上海话"、"开心的语气"
        /// </remarks>
        public string InstructionFragment { get; }

        /// <summary>
        /// 初始化FeatureOptionAttribute类的新实例
        /// </summary>
        /// <param name="instructionFragment">指令文本片段</param>
        public FeatureOptionAttribute(string instructionFragment)
        {
            InstructionFragment = instructionFragment;
        }
    }
}
```

#### 2.3.2 `TtlEngineFeatureDefinition` 类
```csharp
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine
{
    /// <summary>
    /// TTL引擎特性定义类，描述一个特性类别及其所有可选值
    /// </summary>
    /// <remarks>
    /// 通过反射读取枚举类型的选项信息
    /// </remarks>
    public class TtlEngineFeatureDefinition
    {
        #region public

        #region 构造函数

        /// <summary>
        /// 初始化TtlEngineFeatureDefinition类的新实例
        /// </summary>
        /// <param name="name">特性名称</param>
        /// <param name="enumType">枚举类型</param>
        /// <param name="instructionTemplate">指令模板</param>
        public TtlEngineFeatureDefinition(string name, Type enumType, string instructionTemplate = null)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("enumType必须是枚举类型", nameof(enumType));
            }

            Name = name;
            EnumType = enumType;
            InstructionTemplate = instructionTemplate;
            EnumValues = Enum.GetValues(enumType);
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取特性的显示名称
        /// </summary>
        /// <remarks>
        /// 例如："方言"、"情感风格"、"场景"
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// 获取枚举类型
        /// </summary>
        public Type EnumType { get; }

        /// <summary>
        /// 获取指令模板
        /// </summary>
        /// <remarks>
        /// 例如："请用{0}表达。" 其中{0}会被选项的指令片段替换
        /// </remarks>
        public string InstructionTemplate { get; }

        /// <summary>
        /// 获取枚举的所有值
        /// </summary>
        public Array EnumValues { get; }

        #endregion

        #region 方法

        /// <summary>
        /// 获取枚举值的显示名称
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns>显示名称</returns>
        public string GetDisplayName(object enumValue)
        {
            var field = EnumType.GetField(enumValue.ToString());
            if (field == null)
            {
                return enumValue.ToString();
            }

            var attr = field.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? enumValue.ToString();
        }

        /// <summary>
        /// 获取枚举值的指令片段
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns>指令片段</returns>
        public string GetInstructionFragment(object enumValue)
        {
            var field = EnumType.GetField(enumValue.ToString());
            if (field == null)
            {
                return string.Empty;
            }

            var attr = field.GetCustomAttribute<FeatureOptionAttribute>();
            return attr?.InstructionFragment ?? string.Empty;
        }

        /// <summary>
        /// 获取枚举值的显示名称数组（用于UI绑定）
        /// </summary>
        /// <returns>显示名称数组</returns>
        public string[] GetDisplayNames()
        {
            return EnumValues.Cast<object>()
                .Select(v => GetDisplayName(v))
                .ToArray();
        }

        /// <summary>
        /// 构建指令文本
        /// </summary>
        /// <param name="enumValue">选中的枚举值</param>
        /// <returns>完整的指令文本，如果指令片段为空则返回空字符串</returns>
        public string BuildInstruction(object enumValue)
        {
            string fragment = GetInstructionFragment(enumValue);
            if (string.IsNullOrEmpty(fragment))
            {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(InstructionTemplate))
            {
                return string.Format(InstructionTemplate, fragment);
            }

            return fragment;
        }

        /// <summary>
        /// 根据显示名称获取枚举值
        /// </summary>
        /// <param name="displayName">显示名称</param>
        /// <returns>枚举值，未找到则返回枚举的第一个值</returns>
        public object GetEnumValueByDisplayName(string displayName)
        {
            foreach (var value in EnumValues)
            {
                if (GetDisplayName(value) == displayName)
                {
                    return value;
                }
            }
            return EnumValues.GetValue(0);
        }

        #endregion

        #endregion
    }
}
```

### 2.4 接口修改

#### 2.4.1 `ITtlEngineConnector` 接口新增属性
```csharp
/// <summary>
/// 获取引擎支持的特性定义列表
/// </summary>
/// <remarks>
/// 如果引擎不支持任何特性，返回空数组
/// </remarks>
TtlEngineFeatureDefinition[] FeatureDefinitions { get; }
```

#### 2.4.2 `ATtlEngineConnector` 抽象类实现
```csharp
/// <summary>
/// 获取引擎支持的特性定义列表
/// </summary>
public virtual TtlEngineFeatureDefinition[] FeatureDefinitions
{
    get { return Array.Empty<TtlEngineFeatureDefinition>(); }
}
```

### 2.5 参数类修改

#### 2.5.1 `TtlEngineParameters` 类新增属性
```csharp
/// <summary>
/// 获取或设置特性选择字典
/// </summary>
/// <remarks>
/// 键为特性名称，值为选中的枚举值
/// </remarks>
public Dictionary<string, object> FeatureSelections { get; set; }
```

### 2.6 CosyVoice实现示例

#### 2.6.1 定义特性枚举
```csharp
using System.ComponentModel;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine.Features
{
    /// <summary>
    /// 方言特性枚举
    /// </summary>
    public enum DialectFeature
    {
        [Description("不选择")]
        [FeatureOption("")]
        不选择,

        [Description("上海话")]
        [FeatureOption("上海话")]
        上海话,

        [Description("粤语")]
        [FeatureOption("粤语")]
        粤语,

        [Description("四川话")]
        [FeatureOption("四川话")]
        四川话,

        [Description("东北话")]
        [FeatureOption("东北话")]
        东北话
    }

    /// <summary>
    /// 情感风格特性枚举
    /// </summary>
    public enum EmotionFeature
    {
        [Description("不选择")]
        [FeatureOption("")]
        不选择,

        [Description("温柔")]
        [FeatureOption("温柔的语气")]
        温柔,

        [Description("开心")]
        [FeatureOption("开心的语气")]
        开心,

        [Description("严肃")]
        [FeatureOption("严肃的语气")]
        严肃,

        [Description("悲伤")]
        [FeatureOption("悲伤的语气")]
        悲伤,

        [Description("生气")]
        [FeatureOption("生气的语气")]
        生气
    }

    /// <summary>
    /// 场景特性枚举
    /// </summary>
    public enum SceneFeature
    {
        [Description("不选择")]
        [FeatureOption("")]
        不选择,

        [Description("新闻播报")]
        [FeatureOption("新闻播报的语气")]
        新闻播报,

        [Description("讲故事")]
        [FeatureOption("讲故事的语气")]
        讲故事,

        [Description("客服")]
        [FeatureOption("客服的语气")]
        客服,

        [Description("朗诵")]
        [FeatureOption("朗诵的语气")]
        朗诵
    }
}
```

#### 2.6.2 CosyVoice连接器实现
```csharp
public class CosyVoiceV3LYttlEngineConnector : ANetworkTtlEngineConnector
{
    private static readonly TtlEngineFeatureDefinition[] _featureDefinitions = new[]
    {
        new TtlEngineFeatureDefinition("方言", typeof(DialectFeature), "请用{0}表达。"),
        new TtlEngineFeatureDefinition("情感风格", typeof(EmotionFeature), "请用{0}表达。"),
        new TtlEngineFeatureDefinition("场景", typeof(SceneFeature), "请用{0}表达。")
    };

    public override TtlEngineFeatureDefinition[] FeatureDefinitions => _featureDefinitions;

    protected override string BuildRequestUrl(TtlEngineParameters parameters)
    {
        // ... 原有代码构建基础URL ...

        // 构建instruct参数
        var instructParts = new List<string>();
        foreach (var featureDef in FeatureDefinitions)
        {
            if (parameters.FeatureSelections != null &&
                parameters.FeatureSelections.TryGetValue(featureDef.Name, out var selectedValue))
            {
                string instruction = featureDef.BuildInstruction(selectedValue);
                if (!string.IsNullOrEmpty(instruction))
                {
                    instructParts.Add(instruction);
                }
            }
        }

        if (instructParts.Count > 0)
        {
            string instruct = string.Join("", instructParts);
            url += $"&instruct={Uri.EscapeDataString(instruct)}";
        }

        return url;
    }
}
```

---

## 三、UI层修改

### 3.1 `_RoleTextItem` 类修改
```csharp
private class _RoleTextItem
{
    /// <summary>
    /// 角色
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// 文本
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// 特性选择字典，键为特性名称，值为选中的枚举值（存储为int便于序列化）
    /// </summary>
    public Dictionary<string, int> FeatureSelections { get; set; } = new();
}
```

### 3.2 `setup角色和情绪指定表格` 方法修改
```csharp
private void setup角色和情绪指定表格()
{
    // ... 现有代码（角色列、文本列）...

    // 动态添加特性列
    var ttl = GetCurrentTtlEngineConnector();
    if (ttl != null)
    {
        foreach (var featureDef in ttl.FeatureDefinitions)
        {
            var featureColumn = new DataGridViewComboBoxColumn();
            featureColumn.Name = $"Feature_{featureDef.Name}";
            featureColumn.HeaderText = featureDef.Name;
            featureColumn.Tag = featureDef; // 存储特性定义

            // 添加选项（显示名称）
            foreach (var value in featureDef.EnumValues)
            {
                featureColumn.Items.Add(featureDef.GetDisplayName(value));
            }

            dgv_角色和情绪指定.Columns.Add(featureColumn);
        }
    }

    // ... 其余代码 ...
}
```

### 3.3 数据绑定处理
由于DataGridViewComboBoxColumn绑定的是显示名称，需要处理单元格格式化和解析：
```csharp
private void dgv_角色和情绪指定_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
{
    var column = dgv_角色和情绪指定.Columns[e.ColumnIndex];
    if (column.Tag is TtlEngineFeatureDefinition featureDef)
    {
        var item = dgv_角色和情绪指定.Rows[e.RowIndex].DataBoundItem as _RoleTextItem;
        if (item != null && item.FeatureSelections.TryGetValue(featureDef.Name, out int enumValue))
        {
            e.Value = featureDef.GetDisplayName(Enum.ToObject(featureDef.EnumType, enumValue));
            e.FormattingApplied = true;
        }
    }
}

private void dgv_角色和情绪指定_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
{
    var column = dgv_角色和情绪指定.Columns[e.ColumnIndex];
    if (column.Tag is TtlEngineFeatureDefinition featureDef)
    {
        var item = dgv_角色和情绪指定.Rows[e.RowIndex].DataBoundItem as _RoleTextItem;
        if (item != null && e.Value is string displayName)
        {
            var enumValue = featureDef.GetEnumValueByDisplayName(displayName);
            item.FeatureSelections[featureDef.Name] = Convert.ToInt32(enumValue);
            e.Value = displayName;
            e.ParsingApplied = true;
        }
    }
}
```

---

## 四、数据持久化

### 4.1 存储格式
```json
{
    "RoleTextItems": [
        {
            "Role": "角色A",
            "Text": "文本内容",
            "FeatureSelections": {
                "方言": 2,
                "情感风格": 3,
                "场景": 1
            }
        }
    ]
}
```

### 4.2 兼容性处理
- 加载旧版本项目文件时，`FeatureSelections`为空，使用默认值
- 切换TTL引擎时，保留兼容的特性选择，不兼容的丢弃

---

## 五、实现步骤

### 步骤1：Core层基础设施
1. 创建 `FeatureOptionAttribute` 特性类
2. 创建 `TtlEngineFeatureDefinition` 类

### 步骤2：接口和基类修改
1. 在 `ITtlEngineConnector` 接口中添加 `FeatureDefinitions` 属性
2. 在 `ATtlEngineConnector` 抽象类中实现默认返回空数组

### 步骤3：参数类修改
1. 在 `TtlEngineParameters` 类中添加 `FeatureSelections` 属性

### 步骤4：CosyVoice引擎实现
1. 创建特性枚举文件（DialectFeature、EmotionFeature、SceneFeature）
2. 在 `CosyVoiceV3LYttlEngineConnector` 中定义特性定义数组
3. 修改 `BuildRequestUrl` 方法，添加instruct参数

### 步骤5：UI层修改
1. 修改 `_RoleTextItem` 类，添加特性选择支持
2. 修改 `setup角色和情绪指定表格` 方法，动态添加特性列
3. 添加单元格格式化和解析事件处理
4. 修改 `refresh角色和情绪指定角色清单` 方法

### 步骤6：数据持久化
1. 修改 `save角色和情绪指定Data` 方法
2. 修改 `load角色和情绪指定` 方法

### 步骤7：语音生成流程适配
1. 修改语音生成预处理逻辑
2. 在创建 `TtlEngineParameters` 时设置特性选择

---

## 六、文件清单

### 新增文件
| 文件路径 | 说明 |
|---------|------|
| `GW.TTLtoolsBox.Core\SystemOption\TtlEngine\FeatureOptionAttribute.cs` | 特性选项特性类 |
| `GW.TTLtoolsBox.Core\SystemOption\TtlEngine\TtlEngineFeatureDefinition.cs` | 特性定义类 |
| `GW.TTLtoolsBox.Core\SystemOption\TtlEngine\Features\DialectFeature.cs` | 方言特性枚举 |
| `GW.TTLtoolsBox.Core\SystemOption\TtlEngine\Features\EmotionFeature.cs` | 情感风格特性枚举 |
| `GW.TTLtoolsBox.Core\SystemOption\TtlEngine\Features\SceneFeature.cs` | 场景特性枚举 |

### 修改文件
| 文件路径 | 修改内容 |
|---------|---------|
| `ITtlEngineConnector.cs` | 添加 `FeatureDefinitions` 属性 |
| `ATtlEngineConnector.cs` | 实现 `FeatureDefinitions` 默认值 |
| `TtlEngineParameters.cs` | 添加 `FeatureSelections` 属性 |
| `CosyVoiceV3LYttlEngineConnector.cs` | 定义特性，修改URL构建 |
| `MainForm.cs` | UI层动态特性列支持 |

---

## 七、注意事项

1. **向后兼容**：现有的不使用特性的引擎无需修改，基类默认返回空数组
2. **UI刷新**：切换TTL引擎时需要重新设置表格列
3. **空值处理**：枚举第一个值通常为"不选择"，对应空指令片段
4. **指令组合**：多个特性的指令需要正确拼接
5. **枚举值存储**：序列化时存储为int，便于跨引擎兼容
