# 语音生成列表"转换为MP3"功能实施计划

## 一、需求概述

在"语音生成"列表的快捷菜单中实现"转换为MP3"功能：
1. 菜单项仅在任务完成且保存文件为wav时启用
2. 单个任务：弹出保存文件对话框，确定后转换
3. 多个任务：弹出确认框后批量转换，无需单独确认文件名
4. 使用现有的ffmpeg转换方法

## 二、涉及文件

| 文件 | 修改内容 |
|------|----------|
| `VoiceGenerationPanel.cs` | 添加菜单启用逻辑和点击事件处理 |
| `VoiceGenerationPanel.Designer.cs` | 绑定菜单项点击事件（如需） |
| `VoiceGenerationTaskQueue.cs` | 将`ConvertToMp3`方法改为public（重构） |

## 三、实施步骤

### 步骤1：重构VoiceGenerationTaskQueue中的ConvertToMp3方法

**位置**：`VoiceGenerationTaskQueue.cs` 第1115-1127行

**修改内容**：
- 将`private`改为`public`
- 添加XML文档注释
- 方法签名保持不变：`public string ConvertToMp3(string inputFile, string outputFile)`

### 步骤2：添加菜单启用逻辑

**位置**：`VoiceGenerationPanel.cs` 的 `cms_语音生成_任务控制_Opening` 方法（第548-610行）

**修改内容**：
在方法中添加"转换为MP3"菜单项的启用条件判断：
```csharp
// 初始化为禁用
转换为MP3PToolStripMenuItem.Enabled = false;

// 在hasSelectedRows判断块内添加：
if (hasSelectedRows && selectedTasks.Count > 0)
{
    // 所有选中任务必须已完成
    bool allCompleted = selectedTasks.All(t => t.Status == VoiceGenerationTaskStatus.已完成);
    
    // 所有选中任务的保存文件必须是wav格式且存在
    bool allWavAndExist = selectedTasks.All(t => 
        !string.IsNullOrWhiteSpace(t.SaveFile) && 
        File.Exists(t.SaveFile) && 
        Path.GetExtension(t.SaveFile).Equals(".wav", StringComparison.OrdinalIgnoreCase));
    
    转换为MP3PToolStripMenuItem.Enabled = allCompleted && allWavAndExist;
}
```

### 步骤3：实现菜单点击事件处理方法

**位置**：`VoiceGenerationPanel.cs` 的 `#region 事件处理` 区域

**新增方法**：`转换为MP3PToolStripMenuItem_Click`

**实现逻辑**：
1. 获取选中的任务列表
2. 判断选中数量：
   - **单个任务**：
     - 弹出`SaveFileDialog`，默认路径为原wav文件所在文件夹
     - 默认文件名为原wav文件的主文件名，扩展名改为.mp3
     - 确定后调用`_voiceGenerationTaskQueue.ConvertToMp3`进行转换
   - **多个任务**：
     - 弹出`MessageBox`确认框，提示mp3将保存到同文件夹下的同文件名
     - 确定后遍历所有任务，依次调用转换方法

### 步骤4：绑定菜单项点击事件

**位置**：`VoiceGenerationPanel.Designer.cs` 第366-370行

**修改内容**：
在`转换为MP3PToolStripMenuItem`定义后添加事件绑定：
```csharp
this.转换为MP3PToolStripMenuItem.Click += new System.EventHandler(this.转换为MP3PToolStripMenuItem_Click);
```

## 四、代码实现细节

### 4.1 单个任务转换流程
```
用户点击菜单 → 获取选中任务 → 创建SaveFileDialog
→ 设置初始目录和文件名 → 用户确认 → 调用ConvertToMp3
→ 显示转换成功提示
```

### 4.2 多个任务转换流程
```
用户点击菜单 → 获取选中任务列表 → 弹出确认对话框
→ 用户确认 → 遍历任务列表 → 计算mp3路径 → 调用ConvertToMp3
→ 显示批量转换完成提示
```

### 4.3 异常处理
- 转换失败时显示错误信息
- 使用try-catch捕获ffmpeg执行异常

## 五、注意事项

1. **不删除原始wav文件**：按要求保留原文件
2. **复用现有方法**：直接使用`VoiceGenerationTaskQueue.ConvertToMp3`，不重新编写ffmpeg调用代码
3. **UI响应**：转换过程可能耗时，考虑显示等待光标
4. **文件覆盖**：如果mp3文件已存在，需要确认是否覆盖（SaveFileDialog自带此功能，批量转换需手动处理）
