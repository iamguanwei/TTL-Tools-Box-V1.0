# MP3临时文件命名修改计划

## 一、问题分析

### 1.1 当前问题
MP3转换时生成的临时文件名为 `temp_mp3_{guid}.mp3`，以 `temp_` 开头。

### 1.2 问题原因
根据临时文件删除策略：
- 当"保留临时文件"复选框被选中时，`temp_` 开头的文件会被保留
- 但 MP3 转换的临时文件应该像合并、调速的临时文件一样，无论复选框是否选中都应该删除

### 1.3 用户需求
将 `temp_mp3_` 改为 `mp3_`，使其不被"保留临时文件"复选框保护。

---

## 二、修改方案

### 2.1 VoiceGenerationTaskQueue.cs

#### 2.1.1 修改 ConvertToMp3 方法
**位置**：第 985-997 行

**当前代码**：
```csharp
private string ConvertToMp3(string inputFile, string outputFile)
{
    string inputFileName = Path.GetFileName(inputFile);
    string outputFileName = $"temp_mp3_{Guid.NewGuid()}.mp3";
    string tempOutputFile = Path.Combine(_tempFolder, outputFileName);
    string ffmpegArgs = $"-i \"{inputFileName}\" -q:a 2 -acodec libmp3lame \"{outputFileName}\"";

    RunFFmpegCommand(ffmpegArgs, _tempFolder).Wait();

    File.Copy(tempOutputFile, outputFile, true);

    return tempOutputFile;
}
```

**修改为**：
```csharp
private string ConvertToMp3(string inputFile, string outputFile)
{
    string inputFileName = Path.GetFileName(inputFile);
    string outputFileName = $"mp3_{Guid.NewGuid()}.mp3";
    string tempOutputFile = Path.Combine(_tempFolder, outputFileName);
    string ffmpegArgs = $"-i \"{inputFileName}\" -q:a 2 -acodec libmp3lame \"{outputFileName}\"";

    RunFFmpegCommand(ffmpegArgs, _tempFolder).Wait();

    File.Copy(tempOutputFile, outputFile, true);

    return tempOutputFile;
}
```

---

## 三、实施步骤

### 步骤1：修改 VoiceGenerationTaskQueue.cs
将 `temp_mp3_` 改为 `mp3_`

---

## 四、文件修改清单

| 文件路径 | 修改类型 | 主要修改内容 |
|---------|---------|-------------|
| `WinFormUi/Manager/VoiceGenerationTaskQueue.cs` | 修改命名 | 将 `temp_mp3_` 改为 `mp3_` |
