# MP3转换功能重构计划

## 一、问题分析

### 1.1 界面假死原因

**根本原因**：`ConvertToMp3`方法内部调用`RunFFmpegCommand(ffmpegArgs, _tempFolder).Wait()`，这是一个异步方法，但在UI线程上使用`.Wait()`同步等待。

**死锁机制**：

1. UI线程调用`.Wait()`阻塞等待异步任务完成
2. 异步任务完成后，回调需要回到UI线程（同步上下文）执行
3. 但UI线程已被阻塞，无法处理回调
4. 形成死锁

**解决方案**：使用后台线程执行转换，不阻塞UI线程。

### 1.2 需要新增的功能

1. 新增"转换MP3"和"转换MP3失败"状态
2. 创建独立的MP3转换队列
3. 支持停止转换操作
4. 状态保存和恢复
5. 菜单操作限制
6. 表格背景颜色区分

***

## 二、涉及文件

| 文件                                   | 修改内容                  |
| ------------------------------------ | --------------------- |
| `VoiceGenerationTaskStatus.cs`       | 添加"转换MP3"和"转换MP3失败"状态 |
| `VoiceGenerationTask.cs` (WinFormUi) | 添加MP3转换相关属性           |
| `VoiceGenerationTask.cs` (Core)      | 添加MP3转换相关属性           |
| `Mp3ConversionQueue.cs`              | **新建** - MP3转换队列管理器   |
| `VoiceGenerationPanel.cs`            | 修改转换逻辑，集成转换队列，添加背景颜色  |
| `VoiceGenerationPanel.Designer.cs`   | 无需修改                  |

***

## 三、实施步骤

### 步骤1：修改VoiceGenerationTaskStatus枚举

**文件**：`VoiceGenerationTaskStatus.cs`

**修改内容**：添加"转换MP3"和"转换MP3失败"状态

```csharp
public enum VoiceGenerationTaskStatus
{
    未开始,
    排队中,
    正在生成,
    已完成,
    生成失败,
    转换MP3,       // 新增：正在转换为MP3
    转换MP3失败    // 新增：MP3转换失败
}
```

### 步骤2：修改VoiceGenerationTask类（WinFormUi层）

**文件**：`VoiceGenerationTask.cs` (WinFormUi)

**修改内容**：添加MP3转换相关属性

```csharp
/// <summary>
/// 获取或设置MP3输出文件路径。
/// </summary>
public string Mp3OutputFile { get; set; } = string.Empty;
```

### 步骤3：修改VoiceGenerationTask类（Core层）

**文件**：`VoiceGenerationTask.cs` (Core)

**修改内容**：添加MP3转换相关属性，用于项目文件保存/恢复

```csharp
/// <summary>
/// 获取或设置MP3输出文件路径。
/// </summary>
[DataMember(Name = "mp3OutputFile")]
public string Mp3OutputFile { get; set; } = string.Empty;
```

### 步骤4：创建Mp3ConversionQueue类

**文件**：新建 `Mp3ConversionQueue.cs`

**核心设计**：

* 独立的转换队列，与语音生成队列分离

* 支持添加转换任务到队列

* 后台线程依次执行转换

* 支持停止当前转换

* 提供状态变化事件通知

**主要成员**：

```csharp
public class Mp3ConversionQueue : IDisposable
{
    // 属性
    public bool IsRunning { get; }
    public VoiceGenerationTask CurrentTask { get; }
    public int QueuedCount { get; }
    
    // 方法
    public void AddConversionTask(VoiceGenerationTask task, string mp3OutputFile);
    public void Start();
    public void Stop();
    
    // 事件
    public event EventHandler<Mp3ConversionEventArgs> ConversionStarted;
    public event EventHandler<Mp3ConversionEventArgs> ConversionCompleted;
    public event EventHandler<Mp3ConversionEventArgs> ConversionFailed;
    public event EventHandler<Mp3ConversionProgressEventArgs> ConversionProgress;
}
```

**转换执行逻辑**：

* 使用`Task.Run()`在后台线程执行

* 使用`CancellationToken`支持取消操作

* 转换完成后更新任务状态为"已完成"

* 转换失败时更新任务状态为"转换MP3失败"

### 步骤5：修改VoiceGenerationPanel

**文件**：`VoiceGenerationPanel.cs`

**修改内容**：

#### 5.1 添加Mp3ConversionQueue字段

```csharp
private Mp3ConversionQueue _mp3ConversionQueue;
```

#### 5.2 在初始化方法中创建转换队列

```csharp
_mp3ConversionQueue = new Mp3ConversionQueue(FfmpegPath, _tempFolder);
_mp3ConversionQueue.ConversionStarted += Mp3ConversionQueue_ConversionStarted;
_mp3ConversionQueue.ConversionCompleted += Mp3ConversionQueue_ConversionCompleted;
_mp3ConversionQueue.ConversionFailed += Mp3ConversionQueue_ConversionFailed;
```

#### 5.3 修改转换为MP3菜单点击事件

**单个任务**：

1. 弹出保存对话框获取MP3路径
2. 设置任务状态为"转换MP3"
3. 将任务添加到MP3转换队列
4. 启动转换队列

**多个任务**：

1. 弹出确认对话框
2. 为每个任务设置状态为"转换MP3"
3. 依次添加到MP3转换队列
4. 启动转换队列

#### 5.4 修改菜单启用逻辑

在`cms_语音生成_任务控制_Opening`方法中，针对"转换MP3"和"转换MP3失败"状态的任务：

**"转换MP3"状态允许的操作**：

* 停止（停止MP3转换）

* 删除

* 上移/下移（仅影响队列位置，不影响正在转换的任务）

**"转换MP3"状态禁用的操作**：

* 启动

* 重新启动

* 预览声音

* 修改存储文件夹

* 转换为MP3

* 清理临时文件

**"转换MP3失败"状态允许的操作**：

* 重新启动（重新尝试转换）

* 删除

* 上移/下移

* 预览声音（如果原wav文件存在）

* 打开文件夹

* 复制

**"转换MP3失败"状态禁用的操作**：

* 启动

* 停止

* 修改存储文件夹

* 清理临时文件

* 转换为MP3

#### 5.5 处理转换状态变化

```csharp
private void Mp3ConversionQueue_ConversionStarted(object sender, Mp3ConversionEventArgs e)
{
    // 更新UI显示转换中状态
    e.Task.ProgressDetail = "正在转换为MP3...";
}

private void Mp3ConversionQueue_ConversionCompleted(object sender, Mp3ConversionEventArgs e)
{
    // 恢复为已完成状态
    e.Task.Status = VoiceGenerationTaskStatus.已完成;
    e.Task.ProgressDetail = "MP3转换完成";
}

private void Mp3ConversionQueue_ConversionFailed(object sender, Mp3ConversionEventArgs e)
{
    // 设置为转换MP3失败状态
    e.Task.Status = VoiceGenerationTaskStatus.转换MP3失败;
    e.Task.ProgressDetail = $"MP3转换失败: {e.ErrorMessage}";
}
```

#### 5.6 修改表格背景颜色

在`dgv_语音生成_任务清单_CellFormatting`方法中添加"转换MP3"和"转换MP3失败"状态的颜色处理：

**"转换MP3"状态**：

* 背景色：LightGray

* 选中背景色：Gray

* **前景色：采用默认值不变**（不修改ForeColor和SelectionForeColor）

```csharp
case VoiceGenerationTaskStatus.转换MP3:
    if (isSelected)
    {
        row.DefaultCellStyle.SelectionBackColor = Color.Gray;
    }
    else
    {
        row.DefaultCellStyle.BackColor = Color.LightGray;
    }
    // 前景色采用默认值，不做修改
    row.DefaultCellStyle.ForeColor = dgv_语音生成_任务清单.DefaultCellStyle.ForeColor;
    row.DefaultCellStyle.SelectionForeColor = dgv_语音生成_任务清单.DefaultCellStyle.SelectionForeColor;
    break;
```

**"转换MP3失败"状态**：

* 前景色：红色（与"生成失败"一致）

* 背景色：默认

```csharp
case VoiceGenerationTaskStatus.转换MP3失败:
    row.DefaultCellStyle.ForeColor = Color.FromArgb(191, 0, 0);
    row.DefaultCellStyle.SelectionForeColor = dgv_语音生成_任务清单.DefaultCellStyle.SelectionForeColor;
    row.DefaultCellStyle.BackColor = dgv_语音生成_任务清单.DefaultCellStyle.BackColor;
    row.DefaultCellStyle.SelectionBackColor = dgv_语音生成_任务清单.DefaultCellStyle.SelectionBackColor;
    break;
```

### 步骤6：修改项目文件保存/加载

**文件**：`VoiceGenerationPanel.cs` 中的 `save语音生成任务清单Data` 和 `load语音生成任务清单` 方法

**修改内容**：

* 保存时：保存`Mp3OutputFile`属性，保存`Status`（包括"转换MP3"和"转换MP3失败"状态）

* 加载时：恢复任务状态

  * 如果状态为"转换MP3"则重置为"已完成"（因为转换被中断了）

  * 如果状态为"转换MP3失败"则保持该状态（用户可以选择重新尝试）

***

## 四、状态转换图

```
已完成 ──[点击转换为MP3]──> 转换MP3 ──[转换完成]──> 已完成
                              │
                              ├──[转换失败]──> 转换MP3失败
                              │                      │
                              │                      └──[重新启动]──> 转换MP3
                              │
                              └──[用户停止]──> 已完成（恢复原状态）
```

***

## 五、菜单操作矩阵

| 操作      | 未开始 | 排队中 | 正在生成 | 已完成 | 生成失败 | 转换MP3 | 转换MP3失败 |
| ------- | --- | --- | ---- | --- | ---- | ----- | ------- |
| 启动      | ✓   | ✗   | ✗    | ✗   | ✗    | ✗     | ✗       |
| 停止      | ✗   | ✓   | ✓    | ✗   | ✗    | ✓     | ✗       |
| 重新启动    | ✗   | ✗   | ✗    | ✓   | ✓    | ✗     | ✓       |
| 上移      | ✓   | ✓   | ✓    | ✓   | ✓    | ✓     | ✓       |
| 下移      | ✓   | ✓   | ✓    | ✓   | ✓    | ✓     | ✓       |
| 预览声音    | ✗   | ✗   | ✗    | ✓\* | ✗    | ✗     | ✓\*     |
| 打开文件夹   | ✓   | ✓   | ✓    | ✓   | ✓    | ✓     | ✓       |
| 修改存储文件夹 | ✓   | ✓   | ✓    | ✓   | ✓    | ✗     | ✗       |
| 复制      | ✓   | ✓   | ✓    | ✓   | ✓    | ✓     | ✓       |
| 删除      | ✓   | ✓   | ✓    | ✓   | ✓    | ✓     | ✓       |
| 清理临时文件  | ✗   | ✗   | ✗    | ✓   | ✗    | ✗     | ✗       |
| 转换为MP3  | ✗   | ✗   | ✗    | ✓\* | ✗    | ✗     | ✗       |

注：✓\* 表示有额外条件（文件存在且为wav格式）

***

## 六、表格背景颜色

| 状态          | 背景色                | 选中背景色            |
| ----------- | ------------------ | ---------------- |
| 未开始         | 默认                 | 默认               |
| 排队中         | 浅黄 (252, 237, 124) | 橙黄 (204, 153, 0) |
| 正在生成        | 浅绿 (154, 230, 154) | 深绿 (84, 130, 53) |
| 已完成         | 默认（灰色文字）           | 默认               |
| 生成失败        | 默认（红色文字）           | 默认               |
| **转换MP3**   | **LightGray**      | **Gray**         |
| **转换MP3失败** | **默认（红色文字）**       | **默认**           |

***

## 七、注意事项

1. **线程安全**：MP3转换在后台线程执行，更新UI时需要使用`Invoke`
2. **资源清理**：转换完成后清理临时文件
3. **错误处理**：捕获ffmpeg执行异常，更新任务状态为"转换MP3失败"
4. **取消处理**：用户停止时，正确取消正在进行的ffmpeg进程
5. **状态恢复**：程序重启后，"转换MP3"状态的任务应恢复为"已完成"，"转换MP3失败"状态保持不变

***

## 八、实施顺序

1. 修改枚举和实体类（步骤1-3）
2. 创建Mp3ConversionQueue类（步骤4）
3. 修改VoiceGenerationPanel（步骤5）
4. 修改项目文件保存/加载（步骤6）
5. 测试验证

