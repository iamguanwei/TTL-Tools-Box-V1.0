# TTL方案面板朗读者快捷菜单实现计划

## 需求概述
为TTL方案面板的朗读者表格添加右键快捷菜单功能，实现设置默认朗读者和添加角色映射功能。

## 当前状态分析

### 已有组件
1. **快捷菜单** `cmd_TTL方案_朗读者` 已创建，包含两个菜单项：
   - `设置为默认朗读者DToolStripMenuItem` - "设置为默认朗读者(&D)"
   - `添加为角色RToolStripMenuItem` - "添加为角色(&R)"

2. **朗读者表格** `dgv_TTL方案_朗读者参数配置` 当前设置：
   - `RowHeadersVisible = false` - 行首隐藏
   - `SelectionMode = FullRowSelect` - 全行选择模式
   - `MultiSelect` 默认为 true - 允许多选

### 相关面板
- **VoicePreprocessPanel**: 包含默认朗读者下拉框 `cb_语音生成预处理_默认朗读者设置`
- **RoleMappingPanel**: 包含角色映射列表 `_roleMappingItems`

---

## 实现步骤

### 步骤1：修改朗读者表格属性 (TtlSchemePanel.Designer.cs)

1. 设置 `RowHeadersVisible = true` 显示行首
2. 设置 `MultiSelect = false` 禁止多选
3. 为两个菜单项添加 Click 事件处理程序

### 步骤2：添加事件定义 (TtlSchemePanel.cs)

在 `#region 事件` 区域添加：
```csharp
/// <summary>
/// 请求设置默认朗读者时触发的事件。
/// </summary>
public event EventHandler<SetDefaultSpeakerEventArgs> SetDefaultSpeakerRequested;

/// <summary>
/// 请求添加角色映射时触发的事件。
/// </summary>
public event EventHandler<AddRoleMappingEventArgs> AddRoleMappingRequested;
```

### 步骤3：添加事件参数类 (TtlSchemePanel.cs 文件末尾)

```csharp
/// <summary>
/// 设置默认朗读者事件参数。
/// </summary>
public class SetDefaultSpeakerEventArgs : EventArgs
{
    public SpeakerInfo Speaker { get; }
    public SetDefaultSpeakerEventArgs(SpeakerInfo speaker) { Speaker = speaker; }
}

/// <summary>
/// 添加角色映射事件参数。
/// </summary>
public class AddRoleMappingEventArgs : EventArgs
{
    public string SourceName { get; }
    public AddRoleMappingEventArgs(string sourceName) { SourceName = sourceName; }
}
```

### 步骤4：实现行首右键菜单逻辑 (TtlSchemePanel.cs)

在 `#region 事件处理` 区域添加：

```csharp
/// <summary>
/// 事件处理：朗读者表格鼠标按下，用于处理行首右键菜单。
/// </summary>
private void dgv_TTL方案_朗读者参数配置_MouseDown(object sender, MouseEventArgs e)
{
    if (e.Button == MouseButtons.Right)
    {
        var hitTest = this.dgv_TTL方案_朗读者参数配置.HitTest(e.X, e.Y);
        if (hitTest.Type == DataGridViewHitTestType.RowHeader && hitTest.RowIndex >= 0)
        {
            // 如果右击的行不是选中行，先选中该行
            if (!this.dgv_TTL方案_朗读者参数配置.Rows[hitTest.RowIndex].Selected)
            {
                this.dgv_TTL方案_朗读者参数配置.ClearSelection();
                this.dgv_TTL方案_朗读者参数配置.Rows[hitTest.RowIndex].Selected = true;
            }
            // 显示快捷菜单
            this.cmd_TTL方案_朗读者.Show(this.dgv_TTL方案_朗读者参数配置, e.Location);
        }
    }
}
```

### 步骤5：实现菜单项点击事件 (TtlSchemePanel.cs)

```csharp
/// <summary>
/// 事件处理：点击"设置为默认朗读者"菜单项。
/// </summary>
private void 设置为默认朗读者DToolStripMenuItem_Click(object sender, EventArgs e)
{
    var selectedRow = this.dgv_TTL方案_朗读者参数配置.SelectedRows.Cast<DataGridViewRow>().FirstOrDefault();
    if (selectedRow?.DataBoundItem is SpeakerInfo speaker)
    {
        OnSetDefaultSpeakerRequested(speaker);
    }
}

/// <summary>
/// 事件处理：点击"添加为角色"菜单项。
/// </summary>
private void 添加为角色RToolStripMenuItem_Click(object sender, EventArgs e)
{
    var selectedRow = this.dgv_TTL方案_朗读者参数配置.SelectedRows.Cast<DataGridViewRow>().FirstOrDefault();
    if (selectedRow?.DataBoundItem is SpeakerInfo speaker)
    {
        OnAddRoleMappingRequested(speaker.SourceName);
    }
}
```

### 步骤6：添加事件触发方法 (TtlSchemePanel.cs)

在 `#region protected` 区域添加：

```csharp
/// <summary>
/// 触发设置默认朗读者请求事件。
/// </summary>
protected void OnSetDefaultSpeakerRequested(SpeakerInfo speaker)
{
    SetDefaultSpeakerRequested?.Invoke(this, new SetDefaultSpeakerEventArgs(speaker));
}

/// <summary>
/// 触发添加角色映射请求事件。
/// </summary>
protected void OnAddRoleMappingRequested(string sourceName)
{
    AddRoleMappingRequested?.Invoke(this, new AddRoleMappingEventArgs(sourceName));
}
```

### 步骤7：绑定事件处理程序 (TtlSchemePanel.cs)

在 `setupSpeakerGridColumns()` 方法中添加鼠标事件绑定：

```csharp
this.dgv_TTL方案_朗读者参数配置.MouseDown -= dgv_TTL方案_朗读者参数配置_MouseDown;
this.dgv_TTL方案_朗读者参数配置.MouseDown += dgv_TTL方案_朗读者参数配置_MouseDown;
```

### 步骤8：在MainForm中订阅事件 (MainForm.cs)

在初始化TtlSchemePanel的方法中添加：

```csharp
_ttlSchemePanel.SetDefaultSpeakerRequested += (s, e) =>
{
    // 设置默认朗读者
    _voicePreprocessPanel.SetDefaultSpeaker(e.Speaker);
};

_ttlSchemePanel.AddRoleMappingRequested += (s, e) =>
{
    // 添加角色映射
    _roleMappingPanel.AddRoleMapping(e.SourceName);
};
```

### 步骤9：在VoicePreprocessPanel中添加设置方法 (VoicePreprocessPanel.cs)

```csharp
/// <summary>
/// 设置默认朗读者。
/// </summary>
/// <param name="speaker">朗读者信息。</param>
public void SetDefaultSpeaker(SpeakerInfo speaker)
{
    if (speaker != null)
    {
        this.cb_语音生成预处理_默认朗读者设置.SelectedItem = speaker;
    }
}
```

### 步骤10：在RoleMappingPanel中添加添加映射方法 (RoleMappingPanel.cs)

```csharp
/// <summary>
/// 添加角色映射项。
/// </summary>
/// <param name="sourceName">源名称。</param>
public void AddRoleMapping(string sourceName)
{
    var newItem = new RoleMappingItem
    {
        RoleName = string.Empty,
        SourceName = sourceName
    };
    _roleMappingItems.Add(newItem);
    refresh角色映射SourceNameOptions();
    OnProjectModified();
}
```

---

## 文件修改清单

| 文件 | 修改内容 |
|------|----------|
| TtlSchemePanel.Designer.cs | 1. RowHeadersVisible = true<br>2. MultiSelect = false<br>3. 添加菜单项Click事件绑定 |
| TtlSchemePanel.cs | 1. 添加事件定义<br>2. 添加事件参数类<br>3. 添加MouseDown事件处理<br>4. 添加菜单项Click事件处理<br>5. 添加事件触发方法 |
| VoicePreprocessPanel.cs | 添加SetDefaultSpeaker方法 |
| RoleMappingPanel.cs | 添加AddRoleMapping方法 |
| MainForm.cs | 订阅TtlSchemePanel的新事件 |

---

## 验证要点

1. 朗读者表格行首是否显示
2. 朗读者表格是否禁止多选
3. 右击行首时是否正确显示菜单
4. 右击非选中行时是否先选中再显示菜单
5. "设置为默认朗读者"是否正确更新语音生成预处理面板的下拉框
6. "添加为角色"是否在角色映射列表中添加新行（角色名为空，源名称为朗读者）
