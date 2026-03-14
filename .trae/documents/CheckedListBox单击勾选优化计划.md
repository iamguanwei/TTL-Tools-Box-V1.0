# CheckedListBox单击勾选优化计划

## 一、问题分析

### 问题1：需要点两次才能勾选
**现状**：CheckedListBox默认行为是：
- 第一次点击：选中该项（高亮显示）
- 第二次点击：切换勾选状态

**原因**：之前移除了Click事件中的手动切换逻辑，导致只能依赖默认行为。

### 问题2：不需要选中高亮效果
**现状**：点击某项后，该项会被高亮显示（选中状态）。

**原因**：CheckedListBox默认会在点击时选中该项。

## 二、修复方案

### 2.1 实现单击即勾选

**修改Click事件**：
```csharp
private void clb_选择多音字方案_Click(object sender, EventArgs e)
{
    // 获取鼠标点击位置对应的项索引
    Point point = this.clb_选择多音字方案.PointToClient(Cursor.Position);
    int index = this.clb_选择多音字方案.IndexFromPoint(point);
    
    if (index >= 0)
    {
        // 切换勾选状态
        bool currentState = this.clb_选择多音字方案.GetItemChecked(index);
        this.clb_选择多音字方案.SetItemChecked(index, !currentState);
        
        // 取消选中高亮效果
        this.clb_选择多音字方案.SelectedIndex = -1;
    }
}
```

### 2.2 关键点说明

1. **使用`Cursor.Position`和`PointToClient`**：准确获取鼠标点击位置相对于控件的坐标
2. **使用`IndexFromPoint`**：根据坐标获取点击的项索引
3. **设置`SelectedIndex = -1`**：取消选中高亮效果

### 2.3 注意事项

- `SetItemChecked`会触发`ItemCheck`事件，所以不需要担心状态保存问题
- 取消选中状态后，下次点击仍然可以正确获取点击位置

## 三、实施步骤

1. 修改`clb_选择多音字方案_Click`事件处理方法
2. 编译验证

## 四、修改文件

| 文件 | 修改内容 |
|------|----------|
| `PolyphonicReplacePanel.cs` | 修改Click事件处理方法 |
