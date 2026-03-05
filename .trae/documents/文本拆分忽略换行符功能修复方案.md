# 文本拆分"忽略换行符"功能修复方案

## 一、问题分析

### 问题1：配置保存/加载问题

**现象**："忽略换行符"复选框的状态没有正确保存或加载。

**原因**：在`init文本拆分Ui()`方法中，复选框的初始状态被硬编码为`true`：

```csharp
this.cb_文本拆分_按句子拆分_忽略换行符.Checked = true;
```

虽然`LoadData()`中会从配置加载，但初始化时设置的硬编码值可能在某些情况下覆盖了配置。

### 问题2：连续空行产生空段被删除

**现象**：选中"忽略换行符"后，连续两个以上空行产生的空行段落被删除了。

**空行与空行段落的关系**：

| 文本示例                     | 空行数 | 空行段落数 | 说明              |
| ------------------------ | --- | ----- | --------------- |
| `文本A<br>文本B`             | 0   | 0     | 普通换行，无空行        |
| `文本A<br><br>文本B`         | 1   | 0     | 1个空行作为分隔符，不产生空段 |
| `文本A<br><br><br>文本B`     | 2   | 1     | 2个空行产生1个空行段落    |
| `文本A<br><br><br><br>文本B` | 3   | 2     | 3个空行产生2个空行段落    |

**公式**：空行段落数 = max(0, 空行数 - 1)

**原因**：当前`SplitTextIgnoreLineBreaks`方法没有正确处理连续空行产生的空行段落。

***

## 二、修复方案

### 修复1：配置保存/加载

**文件**：`GW.TTLtoolsBox.WinFormUi\UI\Panels\TextSplitPanel.cs`

**修改位置**：`init文本拆分Ui()`方法

**修改内容**：

```csharp
// 修改前
this.cb_文本拆分_按句子拆分_忽略换行符.Checked = true;

// 修改后
this.cb_文本拆分_按句子拆分_忽略换行符.Checked = Setting.GetTextSplit_IgnoreLineBreaks(CurrentEngineId, true);
```

### 修复2：连续空行产生空行段落

**文件**：`GW.TTLtoolsBox.Core\TextSplit\TextSplitHelper.cs`

**修改位置**：`SplitTextIgnoreLineBreaks`方法

**修改内容**：重写空行处理逻辑，正确保留空行段落

```csharp
public static string SplitTextIgnoreLineBreaks(
    string originalText,
    int minLength,
    int maxLength,
    char[] mainSeparators,
    char[] subSeparators,
    bool trimSegments = false)
{
    if (string.IsNullOrEmpty(originalText)) return string.Empty;

    string[] _lines = originalText.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
    List<string> _paragraphs = new List<string>();
    StringBuilder _currentParagraph = new StringBuilder();
    int _emptyLineCount = 0;

    foreach (string _line in _lines)
    {
        if (string.IsNullOrWhiteSpace(_line))
        {
            _emptyLineCount++;
        }
        else
        {
            // 处理之前的空行
            if (_emptyLineCount > 0)
            {
                // 结束当前段落
                if (_currentParagraph.Length > 0)
                {
                    string _paragraph = trimSegments ? _currentParagraph.ToString().Trim() : _currentParagraph.ToString();
                    _paragraphs.Add(_paragraph);
                    _currentParagraph.Clear();
                }
                // 添加空行段落（空行数-1个）
                for (int i = 0; i < _emptyLineCount - 1; i++)
                {
                    _paragraphs.Add(string.Empty);
                }
            }
            _emptyLineCount = 0;
            
            // 添加非空行到当前段落
            if (_currentParagraph.Length > 0)
            {
                _currentParagraph.Append(" ");
            }
            _currentParagraph.Append(_line);
        }
    }

    // 处理末尾的空行
    if (_emptyLineCount > 0)
    {
        // 结束当前段落
        if (_currentParagraph.Length > 0)
        {
            string _paragraph = trimSegments ? _currentParagraph.ToString().Trim() : _currentParagraph.ToString();
            _paragraphs.Add(_paragraph);
            _currentParagraph.Clear();
        }
        // 添加空行段落（空行数-1个）
        for (int i = 0; i < _emptyLineCount - 1; i++)
        {
            _paragraphs.Add(string.Empty);
        }
    }
    // 处理最后一段（没有空行结尾的情况）
    else if (_currentParagraph.Length > 0)
    {
        string _lastParagraph = trimSegments ? _currentParagraph.ToString().Trim() : _currentParagraph.ToString();
        _paragraphs.Add(_lastParagraph);
    }

    // 对每段调用原有的句子拆分逻辑
    List<string> _resultParagraphs = new List<string>();
    foreach (string _paragraph in _paragraphs)
    {
        string _splitResult = SplitText(_paragraph, minLength, maxLength, mainSeparators, subSeparators, trimSegments);
        _resultParagraphs.Add(_splitResult);
    }

    return string.Join("\r\n", _resultParagraphs);
}
```

***

## 三、测试用例

### 测试1：配置保存/加载

| 步骤 | 操作               | 预期结果        |
| -- | ---------------- | ----------- |
| 1  | 启动软件，取消勾选"忽略换行符" | 复选框取消勾选     |
| 2  | 切换到另一个TTL方案      | 配置保存        |
| 3  | 切换回原TTL方案        | 复选框保持取消勾选状态 |

### 测试2：空行段落处理

| 输入文本                       | 空行数 | 预期输出段数     | 说明              |
| -------------------------- | --- | ---------- | --------------- |
| `文本A。<br>文本B！`             | 0   | 1或2（视拆分长度） | 无空行，合并后拆分       |
| `文本A。<br><br>文本B！`         | 1   | 2          | 1个空行作为分隔符，不产生空段 |
| `文本A。<br><br><br>文本B！`     | 2   | 3          | 2个空行产生1个空行段落    |
| `文本A。<br><br><br><br>文本B！` | 3   | 4          | 3个空行产生2个空行段落    |

***

## 四、代码修改清单

| 序号 | 文件                 | 修改位置                          | 修改内容            |
| -- | ------------------ | ----------------------------- | --------------- |
| 1  | TextSplitPanel.cs  | `init文本拆分Ui()`                | 从配置加载复选框初始状态    |
| 2  | TextSplitHelper.cs | `SplitTextIgnoreLineBreaks()` | 修复连续空行产生空行段落的逻辑 |

