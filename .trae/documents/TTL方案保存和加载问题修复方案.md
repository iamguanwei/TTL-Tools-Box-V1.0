# TTL方案保存和加载问题修复方案

## 一、问题分析

### 问题1：TTL方案朗读者列表数据没有保存/加载

**用户提供的格式**：
```
Speaker_a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d_豆包-暖心小妍 = 豆包-暖心小妍||100|100|※ 女，少年，略沙哑，低落，讲述
```

**当前问题**：
- 在 `bt_TTL方案_保存配置_Click` 方法中，只是将 DataGridView 中的数据复制到引擎对象内存，**没有保存到配置文件**
- 在引擎切换时也没有从配置文件加载朗读者数据

### 问题2："按句子拆分"、"按对话拆分"保存/加载问题

**当前问题**：
- 加载：在 `init文本拆分Ui()` 中正常加载
- 保存：只在 `bt_开始拆分_Click` 点击时保存
- **问题**：如果用户修改了选项但没有点击"开始拆分"，修改会丢失

---

## 二、修改方案

### 2.1 朗读者列表保存/加载

#### 2.1.1 保存朗读者数据
**文件**: `TtlSchemePanel.cs`

**修改 `bt_TTL方案_保存配置_Click` 方法**，在保存参数后添加保存朗读者数据的逻辑：
```csharp
// 保存朗读者数据
if (this.dgv_TTL方案_朗读者参数配置.DataSource != null)
{
    SpeakerInfo[] speakerArray = (SpeakerInfo[])this.dgv_TTL方案_朗读者参数配置.DataSource;
    
    // 先删除该引擎的所有旧朗读者配置
    var keysToRemove = new List<string>();
    foreach (var key in Setting.GetAllKeys())
    {
        if (key.StartsWith($"Speaker_{currentEngine.Id}_"))
        {
            keysToRemove.Add(key);
        }
    }
    foreach (var key in keysToRemove)
    {
        Setting.Remove(key);
    }
    
    // 保存每个朗读者的数据
    foreach (var speaker in speakerArray)
    {
        string speakerKey = $"Speaker_{currentEngine.Id}_{speaker.SourceName}";
        Setting.SetValue(speakerKey, speaker.ToString());
    }
    
    Setting.Save();
}
```

#### 2.1.2 加载朗读者数据
**文件**: `TtlSchemePanel.cs`

**修改 `bindSpeakersToGrid` 方法**，在绑定数据前先从配置文件加载保存的朗读者数据：
```csharp
private void bindSpeakersToGrid(ITtlEngineConnector ttl, bool forceRefresh = false)
{
    // 从配置文件加载保存的朗读者数据
    var savedSpeakers = new Dictionary<string, SpeakerInfo>();
    foreach (var key in Setting.GetAllKeys())
    {
        if (key.StartsWith($"Speaker_{ttl.Id}_"))
        {
            string speakerName = key.Substring($"Speaker_{ttl.Id}_".Length);
            string value = Setting.GetValue(key, string.Empty);
            if (!string.IsNullOrEmpty(value))
            {
                var speaker = new SpeakerInfo(speakerName);
                if (speaker.TryFromString(speakerName + "|" + value))
                {
                    savedSpeakers[speakerName] = speaker;
                }
            }
        }
    }
    
    // 合并保存的数据到引擎的朗读者列表
    if (ttl.Speakers != null && savedSpeakers.Count > 0)
    {
        for (int i = 0; i < ttl.Speakers.Length; i++)
        {
            var speaker = ttl.Speakers[i];
            if (savedSpeakers.TryGetValue(speaker.SourceName, out var savedSpeaker))
            {
                ttl.Speakers[i] = savedSpeaker;
            }
        }
    }
    
    var speakerArray = ttl.Speakers.Select(s => s.Clone()).ToArray();
    // ... 后续绑定逻辑
}
```

### 2.2 文本拆分参数实时保存

**文件**: `TextSplitPanel.cs`

**修改 `rb_拆分方式_CheckedChanged` 方法**，在选项变化时立即保存：
```csharp
private void rb_拆分方式_CheckedChanged(object sender, EventArgs e)
{
    if (!_isLoading)
    {
        Setting.SetValue(this.rb_拆分方式_按句子拆分.Name, this.rb_拆分方式_按句子拆分.Checked.ToString());
        Setting.SetValue(this.rb_拆分参数_按对话拆分.Name, this.rb_拆分参数_按对话拆分.Checked.ToString());
    }
    refresh文本拆分Ui();
}
```

---

## 三、修改文件清单

| 文件 | 修改内容 |
|------|----------|
| `TtlSchemePanel.cs` | 1. 在保存配置时保存朗读者数据到配置文件<br>2. 在绑定朗读者数据时从配置文件加载保存的数据 |
| `TextSplitPanel.cs` | 在 `rb_拆分方式_CheckedChanged` 中实时保存选项变化 |

---

## 四、实施步骤

1. 在 `TtlSchemePanel.cs` 的 `bt_TTL方案_保存配置_Click` 方法中添加保存朗读者数据的逻辑
2. 在 `TtlSchemePanel.cs` 的 `bindSpeakersToGrid` 方法中添加加载朗读者数据的逻辑
3. 在 `TextSplitPanel.cs` 的 `rb_拆分方式_CheckedChanged` 方法中添加实时保存选项变化的逻辑

---

## 五、格式说明

### 朗读者数据格式
- **Key**: `Speaker_{engineId}_{speakerSourceName}`
- **Value**: `speaker.ToString()` 输出格式为 `源名称|声音样本路径|速度|音量|备注`
- **示例**: `Speaker_abc123_豆包-暖心小妍 = 豆包-暖心小妍|H:\voices\doubao|100|100|※ 女，少年`

### 文本拆分参数格式
- **Key**: `rb_拆分方式_按句子拆分`, `rb_拆分参数_按对话拆分`, `TextSplit_IgnoreLineBreaks_{engineId}`
- **Value**: `true` 或 `false`
