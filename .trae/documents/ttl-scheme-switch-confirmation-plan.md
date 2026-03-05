# TTL方案切换时任务确认提示功能实现方案

## 一、需求概述

当用户在TTL方案面板切换当前方案时，如果存在正在执行或排队的语音生成任务（包括正在转换MP3），需要弹出确认提示框，告知用户该操作会停止所有正在执行的任务，部分任务可能需要手动重启，待用户确认后才执行切换。

## 二、涉及文件

| 文件 | 说明 |
|------|------|
| `TtlSchemePanel.cs` | TTL方案面板UI，包含方案切换事件处理 |
| `VoiceGenerationTaskQueue.cs` | 语音生成任务队列管理器 |
| `VoiceGenerationTaskStatus.cs` | 任务状态枚举定义 |

## 三、实现步骤

### 步骤1：在VoiceGenerationTaskQueue中添加任务状态检查方法

在 `VoiceGenerationTaskQueue.cs` 中添加一个公共方法，用于检查是否存在正在执行或排队的任务：

```csharp
/// <summary>
/// 检查是否存在正在执行或排队中的任务（包括转换MP3）。
/// </summary>
/// <returns>如果存在活跃任务返回true，否则返回false。</returns>
public bool HasActiveTasks()
{
    return _tasks.Any(t => t.Status == VoiceGenerationTaskStatus.排队中 ||
                          t.Status == VoiceGenerationTaskStatus.正在生成 ||
                          t.Status == VoiceGenerationTaskStatus.转换MP3);
}
```

### 步骤2：在TtlSchemePanel中添加任务状态检查方法

在 `TtlSchemePanel.cs` 中添加一个私有方法，用于检查是否有活跃任务并返回详细信息：

```csharp
/// <summary>
/// 检查是否存在活跃的语音生成任务。
/// </summary>
/// <returns>如果存在活跃任务返回true，否则返回false。</returns>
private bool hasActiveVoiceGenerationTasks()
{
    return VoiceGenerationTaskQueue?.HasActiveTasks() ?? false;
}
```

### 步骤3：修改方案切换事件处理逻辑

修改 `cb_TTL方案_当前方案名称_SelectedIndexChanged` 方法，在切换方案前添加确认逻辑：

1. 检测是否是真正的方案切换（前后方案不同）
2. 如果存在活跃任务，弹出确认对话框
3. 用户确认后继续切换，取消则恢复原选择

关键修改点：
- 在方法开始处保存当前选择项，用于取消时恢复
- 在执行切换逻辑前添加任务检查和确认对话框
- 根据用户选择决定是否继续执行切换

### 步骤4：实现确认对话框逻辑

确认对话框需要包含以下信息：
- 标题：切换TTL方案
- 内容：当前有正在执行或排队的语音生成任务，切换方案将停止所有正在执行的任务。部分任务可能需要手动重启。是否继续切换？
- 按钮：是/否

## 四、详细实现代码

### 4.1 VoiceGenerationTaskQueue.cs 修改

在 `#region public` 的 `#region 方法` 区域末尾添加：

```csharp
/// <summary>
/// 检查是否存在正在执行或排队中的任务（包括转换MP3）。
/// </summary>
/// <returns>如果存在活跃任务返回true，否则返回false。</returns>
public bool HasActiveTasks()
{
    return _tasks.Any(t => t.Status == VoiceGenerationTaskStatus.排队中 ||
                          t.Status == VoiceGenerationTaskStatus.正在生成 ||
                          t.Status == VoiceGenerationTaskStatus.转换MP3);
}
```

### 4.2 TtlSchemePanel.cs 修改

#### 4.2.1 添加私有方法

在 `#region private` 的 `#region 方法` 区域添加：

```csharp
/// <summary>
/// 检查是否存在活跃的语音生成任务。
/// </summary>
/// <returns>如果存在活跃任务返回true，否则返回false。</returns>
private bool hasActiveVoiceGenerationTasks()
{
    return VoiceGenerationTaskQueue?.HasActiveTasks() ?? false;
}
```

#### 4.2.2 修改 cb_TTL方案_当前方案名称_SelectedIndexChanged 方法

在方法开头添加确认逻辑，主要修改如下：

```csharp
private void cb_TTL方案_当前方案名称_SelectedIndexChanged(object sender, EventArgs e)
{
    // 获取新选择的引擎
    string selectedItem = this.cb_TTL方案_当前方案名称.SelectedItem?.ToString() ?? string.Empty;
    ITtlEngineConnector newEngine = TtlSchemeController?.FindEngineByDisplayText(selectedItem);
    string newEngineId = newEngine?.Id ?? Constants.无_引擎标识;
    
    // 获取当前引擎ID
    string currentEngineId = TtlSchemeController?.GetCurrentEngineId() ?? Constants.无_引擎标识;
    
    // 检查是否是真正的方案切换
    bool isEngineChanging = (currentEngineId != newEngineId);
    
    // 如果是方案切换且存在活跃任务，需要用户确认
    if (isEngineChanging && hasActiveVoiceGenerationTasks())
    {
        // 弹出确认对话框
        DialogResult result = MessageBox.Show(
            "当前有正在执行或排队的语音生成任务。\n\n切换方案将停止所有正在执行的任务，部分任务可能需要手动重启。\n\n是否继续切换？",
            "切换TTL方案",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);
        
        if (result != DialogResult.Yes)
        {
            // 用户取消，恢复原选择
            restoreEngineSelection(currentEngineId);
            return;
        }
    }
    
    // 原有的切换逻辑...
}

/// <summary>
/// 恢复引擎选择下拉框的选中项。
/// </summary>
/// <param name="engineId">要恢复的引擎ID。</param>
private void restoreEngineSelection(string engineId)
{
    if (string.IsNullOrEmpty(engineId) || engineId == Constants.无_引擎标识)
    {
        this.cb_TTL方案_当前方案名称.SelectedIndex = 0;
        return;
    }
    
    var connector = TtlSchemeController?.EngineConnectorArray.FirstOrDefault(c => c.Id == engineId);
    if (connector != null)
    {
        this.cb_TTL方案_当前方案名称.SelectedItem = TtlSchemeController.GetEngineDisplayText(connector);
    }
    else
    {
        this.cb_TTL方案_当前方案名称.SelectedIndex = 0;
    }
}
```

## 五、注意事项

1. **事件触发时机**：确认逻辑需要在方案实际切换之前执行，避免状态已经改变后再弹出确认框

2. **恢复选择**：用户取消切换时，需要正确恢复下拉框的选中项，避免触发额外的事件

3. **初始化阶段跳过**：在控制器初始化阶段（`IsInitializing`为true时）不需要弹出确认框

4. **线程安全**：UI操作需要在UI线程执行，确保对话框正确显示

## 六、测试要点

1. 无任务时切换方案 - 应直接切换，不弹出确认框
2. 有排队中任务时切换方案 - 应弹出确认框
3. 有正在生成任务时切换方案 - 应弹出确认框
4. 有转换MP3任务时切换方案 - 应弹出确认框
5. 用户点击"否"取消切换 - 应恢复原方案选择
6. 用户点击"是"确认切换 - 应正常切换方案
7. 初始化阶段切换方案 - 不应弹出确认框
