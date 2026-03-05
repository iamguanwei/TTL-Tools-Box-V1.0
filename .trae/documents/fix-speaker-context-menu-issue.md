# 修复朗读者表格右键菜单问题

## 问题分析

### 根本原因
在 `setupSpeakerGridColumns()` 方法中，SelectionMode 被设置为 `CellSelect`：
```csharp
this.dgv_TTL方案_朗读者参数配置.SelectionMode = DataGridViewSelectionMode.CellSelect;
```

这会覆盖 Designer.cs 中的 `FullRowSelect` 设置。当 SelectionMode 是 CellSelect 时，即使点击行首也不会选中整行，导致 `SelectedRows` 集合为空，菜单项点击事件中无法获取选中行。

### 次要问题
菜单项没有根据选中状态启用/禁用，用户无法直观判断功能是否可用。

---

## 修复步骤

### 步骤1：修改 SelectionMode (TtlSchemePanel.cs)

在 `setupSpeakerGridColumns()` 方法中，将：
```csharp
this.dgv_TTL方案_朗读者参数配置.SelectionMode = DataGridViewSelectionMode.CellSelect;
```
改为：
```csharp
this.dgv_TTL方案_朗读者参数配置.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
```

### 步骤2：添加菜单启用/禁用逻辑 (TtlSchemePanel.cs)

修改 `dgv_TTL方案_朗读者参数配置_MouseDown` 方法，在显示菜单前设置菜单项的启用状态：

```csharp
private void dgv_TTL方案_朗读者参数配置_MouseDown(object sender, MouseEventArgs e)
{
    if (e.Button == MouseButtons.Right)
    {
        var hitTest = this.dgv_TTL方案_朗读者参数配置.HitTest(e.X, e.Y);
        if (hitTest.Type == DataGridViewHitTestType.RowHeader && hitTest.RowIndex >= 0)
        {
            if (!this.dgv_TTL方案_朗读者参数配置.Rows[hitTest.RowIndex].Selected)
            {
                this.dgv_TTL方案_朗读者参数配置.ClearSelection();
                this.dgv_TTL方案_朗读者参数配置.Rows[hitTest.RowIndex].Selected = true;
            }
            
            // 根据是否有选中行设置菜单项启用状态
            bool hasSelection = this.dgv_TTL方案_朗读者参数配置.SelectedRows.Count > 0;
            this.设置为默认朗读者DToolStripMenuItem.Enabled = hasSelection;
            this.添加为角色RToolStripMenuItem.Enabled = hasSelection;
            
            this.cmd_TTL方案_朗读者.Show(this.dgv_TTL方案_朗读者参数配置, e.Location);
        }
    }
}
```

---

## 文件修改清单

| 文件 | 修改内容 |
|------|----------|
| TtlSchemePanel.cs | 1. 将 SelectionMode 从 CellSelect 改为 FullRowSelect<br>2. 在显示菜单前设置菜单项启用状态 |

---

## 验证要点

1. 点击行首是否能选中整行
2. 右击行首时菜单项是否启用
3. "设置为默认朗读者"是否正确更新语音生成预处理面板的下拉框
4. "添加为角色"是否在角色映射列表中添加新行
