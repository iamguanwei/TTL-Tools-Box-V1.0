# TTL引擎重构计划

## 任务概述
1. 将 `Core.SystemOption.TtlEngine` 命名空间上提一级为 `Core.TtlEngine`
2. 将 `Core.SystemOption.Helper` 移动到 `Core.TtlEngine` 中
3. 更新所有相关引用
4. 为朗读者添加 Speed 属性
5. 将 Speed 存储到系统配置
6. 在 UI 朗读者表格中添加"速度"列

---

## 一、命名空间重构

### 1.1 移动 TtlEngine 文件夹
**源路径**: `GW.TTLtoolsBox.Core\SystemOption\TtlEngine\`
**目标路径**: `GW.TTLtoolsBox.Core\TtlEngine\`

需要移动的文件：
- `ITtlEngineConnector.cs`
- `ATtlEngineConnector.cs`
- `ANetworkTtlEngineConnector.cs`
- `CosyVoiceV3LYttlEngineConnector.cs`
- `IndexTTLv2LYttlEngineConnector.cs`
- `SpeakerInfo.cs`
- `TtlEngineParameters.cs`
- `TtlEngineTask.cs`
- `TtlEngineFeatureDefinition.cs`
- `FeatureOptionAttribute.cs`
- `ConnectionStatus.cs`
- `PreviewVoiceManager.cs`
- `Events/` 子目录下的所有事件参数类
- `Features/` 子目录下的所有特性枚举

### 1.2 移动 Helper 文件夹到 TtlEngine 下
**源路径**: `GW.TTLtoolsBox.Core\SystemOption\Helper\MD5Helper.cs`
**目标路径**: `GW.TTLtoolsBox.Core\TtlEngine\Helper\MD5Helper.cs`

### 1.3 修改命名空间声明

#### TtlEngine 文件夹下的文件
**旧命名空间**: `GW.TTLtoolsBox.Core.SystemOption.TtlEngine`
**新命名空间**: `GW.TTLtoolsBox.Core.TtlEngine`

需要修改的文件（共12个主文件 + Events子目录5个 + Features子目录3个）：
1. `ITtlEngineConnector.cs`
2. `ATtlEngineConnector.cs`
3. `ANetworkTtlEngineConnector.cs`
4. `CosyVoiceV3LYttlEngineConnector.cs`
5. `IndexTTLv2LYttlEngineConnector.cs`
6. `SpeakerInfo.cs`
7. `TtlEngineParameters.cs`
8. `TtlEngineTask.cs`
9. `TtlEngineFeatureDefinition.cs`
10. `FeatureOptionAttribute.cs`
11. `ConnectionStatus.cs`
12. `PreviewVoiceManager.cs`
13. `Events/TtlEngineConnectionEventArgs.cs`
14. `Events/TtlEngineCompletedEventArgs.cs`
15. `Events/TtlEngineProgressEventArgs.cs`
16. `Events/TtlEngineQueuedEventArgs.cs`
17. `Events/TtlEngineReconnectEventArgs.cs`
18. `Features/DialectFeature.cs`
19. `Features/EmotionFeature.cs`
20. `Features/SceneFeature.cs`

#### MD5Helper.cs
**旧命名空间**: `GW.TTLtoolsBox.Core.SystemOption.Helper`
**新命名空间**: `GW.TTLtoolsBox.Core.TtlEngine.Helper`

### 1.4 更新引用命名空间的文件

#### 更新 `using GW.TTLtoolsBox.Core.SystemOption.TtlEngine` 的文件（12个）
1. `VoiceGenerationPanel.cs`
2. `VoiceGenerationTaskQueue.cs`
3. `MainForm.cs`
4. `RoleEmotionPanel.cs`
5. `RoleMappingPanel.cs`
6. `TtlSchemePanel.cs`
7. `TtlSchemeController.cs`
8. `CosyVoiceV3LYttlEngineConnector.cs`（内部引用）
9. `ATtlEngineConnector.cs`（内部引用）
10. `VoicePreprocessPanel.cs`
11. `VoiceGenerationTaskItem.cs`
12. `ITtlEngineConnector.cs`（内部引用）

#### 更新 `using GW.TTLtoolsBox.Core.SystemOption.Helper` 的文件（4个）
1. `VoiceGenerationPanel.cs`
2. `MainForm.cs`
3. `TtlSchemeController.cs`
4. `VoiceGenerationTask.cs`

---

## 二、为 SpeakerInfo 添加 Speed 属性

### 2.1 修改 SpeakerInfo.cs
- 添加 `Speed` 属性，类型为 `int`，默认值 `100`
- 修改 `TryFromString` 方法以支持 Speed 的序列化/反序列化
- 修改 `Clone` 方法以复制 Speed 属性
- 修改构造函数以支持 Speed 参数

### 2.2 Speed 属性设计
```csharp
/// <summary>
/// 获取或设置朗读速度（百分比，默认100）。
/// </summary>
public int Speed { get; set; } = 100;
```

### 2.3 序列化格式
字符串格式从 `SourceName|VoiceSamplePath` 改为 `SourceName|VoiceSamplePath|Speed`

---

## 三、系统配置存储

### 3.1 修改 TtlSchemeController.cs

#### SaveSpeakerParameters 方法
- 保存时将 Speed 包含在序列化字符串中

#### LoadSpeakerSettings 方法
- 加载时解析 Speed 值
- 兼容旧格式（没有 Speed 时使用默认值 100）

---

## 四、UI 修改

### 4.1 修改 TtlSchemePanel.cs

#### setupSpeakerGridColumns 方法
- 在"源名称"列后添加"速度"列（插入到第二列位置）
- 列配置：
  - 名称：`Speed`
  - 标题：`速度`
  - 数据类型：`int`
  - 可编辑：是（仅在编辑模式下）

#### 保存和还原逻辑
- 保存时：将 Speed 值保存到配置
- 还原时：从配置重新加载 Speed 值

---

## 五、实施步骤

### 步骤 1：创建新的文件夹结构
1. 创建 `GW.TTLtoolsBox.Core\TtlEngine\` 目录
2. 创建 `GW.TTLtoolsBox.Core\TtlEngine\Helper\` 目录
3. 创建 `GW.TTLtoolsBox.Core\TtlEngine\Events\` 目录
4. 创建 `GW.TTLtoolsBox.Core\TtlEngine\Features\` 目录

### 步骤 2：移动并修改 TtlEngine 文件
1. 移动所有 TtlEngine 相关文件到新目录
2. 修改每个文件的命名空间声明
3. 更新 Events 和 Features 子目录中的文件

### 步骤 3：移动并修改 MD5Helper
1. 移动 MD5Helper.cs 到 `TtlEngine\Helper\` 目录
2. 修改命名空间声明

### 步骤 4：更新所有引用
1. 更新 WinFormUi 项目中所有引用文件的 using 语句
2. 更新 Core 项目内部文件的 using 语句

### 步骤 5：修改 SpeakerInfo.cs
1. 添加 Speed 属性
2. 修改序列化/反序列化方法
3. 修改 Clone 方法

### 步骤 6：修改 TtlSchemeController.cs
1. 更新 SaveSpeakerParameters 方法
2. 更新 LoadSpeakerSettings 方法

### 步骤 7：修改 TtlSchemePanel.cs
1. 更新 setupSpeakerGridColumns 方法添加速度列
2. 确保编辑/保存/还原逻辑正确处理 Speed

### 步骤 8：删除旧的空目录
1. 确认 `GW.TTLtoolsBox.Core\SystemOption\` 目录已清空后删除

### 步骤 9：验证
1. 编译项目确保无错误
2. 测试功能确保正常运行

---

## 六、文件变更清单

### 需要移动的文件（21个）
| 原路径 | 新路径 |
|--------|--------|
| Core\SystemOption\TtlEngine\*.cs | Core\TtlEngine\*.cs |
| Core\SystemOption\TtlEngine\Events\*.cs | Core\TtlEngine\Events\*.cs |
| Core\SystemOption\TtlEngine\Features\*.cs | Core\TtlEngine\Features\*.cs |
| Core\SystemOption\Helper\MD5Helper.cs | Core\TtlEngine\Helper\MD5Helper.cs |

### 需要修改命名空间的文件（21个 Core 文件 + 12个 WinFormUi 文件）

### 需要修改业务逻辑的文件
1. `SpeakerInfo.cs` - 添加 Speed 属性
2. `TtlSchemeController.cs` - 更新保存/加载逻辑
3. `TtlSchemePanel.cs` - 添加速度列

---

## 七、注意事项

1. **兼容性**：Speed 的序列化格式需要兼容旧版本配置，旧配置没有 Speed 时使用默认值 100
2. **编辑模式**：速度列仅在点击"编辑"按钮后可修改
3. **保存逻辑**：点击"保存"按钮立即存入系统配置
4. **还原逻辑**：点击"取消"还原未保存的修改
