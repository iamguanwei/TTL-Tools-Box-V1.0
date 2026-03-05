# 修复"设置为默认朗读者"功能

## 问题分析

### 根本原因
在 `TtlSchemePanel` 中，朗读者表格的数据源是通过 `Clone()` 创建的克隆对象：
```csharp
var speakerArray = ttl.Speakers.Select(s => s.Clone()).ToArray();
```

而在 `VoicePreprocessPanel` 中，下拉框的 Items 是原始对象：
```csharp
foreach (var speaker in speakers)
{
    this.cb_语音生成预处理_默认朗读者设置.Items.Add(speaker);
}
```

当 `SetDefaultSpeaker` 方法设置 `SelectedItem = speaker` 时，`speaker` 是克隆对象，而下拉框中存储的是原始对象。由于它们是不同的对象实例，`SelectedItem` 的比较会失败（默认使用引用比较），所以无法正确设置选中项。

---

## 修复方案

修改 `VoicePreprocessPanel.SetDefaultSpeaker` 方法，通过 `SourceName` 查找下拉框中的匹配项：

```csharp
/// <summary>
/// 设置默认朗读者。
/// </summary>
/// <param name="speaker">朗读者信息。</param>
public void SetDefaultSpeaker(SpeakerInfo speaker)
{
    if (speaker != null)
    {
        UpdateUi(() =>
        {
            // 通过 SourceName 查找匹配项（因为表格中的 speaker 是克隆对象）
            for (int i = 0; i < this.cb_语音生成预处理_默认朗读者设置.Items.Count; i++)
            {
                if (this.cb_语音生成预处理_默认朗读者设置.Items[i] is SpeakerInfo item 
                    && item.SourceName == speaker.SourceName)
                {
                    this.cb_语音生成预处理_默认朗读者设置.SelectedIndex = i;
                    break;
                }
            }
        });
    }
}
```

---

## 文件修改清单

| 文件 | 修改内容 |
|------|----------|
| VoicePreprocessPanel.cs | 修改 SetDefaultSpeaker 方法，通过 SourceName 查找匹配项 |

---

## 验证要点

1. 点击"设置为默认朗读者"后，语音生成预处理面板的默认朗读者下拉框是否正确选中
