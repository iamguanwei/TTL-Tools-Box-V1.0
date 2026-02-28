# PolyphonicReplacePanel 重构计划

## 重构目标
对 PolyphonicReplacePanel.cs 进行以下两个方面的重构：
1. **代码结构优化** - 简化 refresh多音字替换Ui 方法中的待选词面板逻辑
2. **提取重复逻辑** - 提取重复代码为独立方法

---

## 一、代码结构优化

### 1.1 简化 refresh多音字替换Ui 中的待选词面板逻辑

**优化内容：**

1. **提取 RadioButton 创建逻辑**
   - 创建新方法 `CreateAlternativeRadioButton(text: string, index: int, yOffset: int): RadioButton`
   - 封装 RadioButton 的创建、属性设置和事件绑定

2. **提取选中索引计算逻辑**
   - 封装为独立方法：`CalculateSelectedIndex(currentItem: ReplaceItem, alternativeTexts: string[], previousItem: ReplaceItem?): int`
   - 保留现有的 selectedIndex 计算逻辑

3. **简化面板控件清理逻辑**
   - 提取为方法：`ClearAlternativePanelControls()`

4. **简化工作统计信息显示逻辑**
   - 提取为方法：`BuildWorkStatisticsText(): string`

---

## 二、提取重复逻辑

### 2.1 提取获取选中行索引的逻辑

**问题位置：**
- `showRowHeaderContextMenu` 方法（约第710-722行）
- `autoGenerateForSelectedRows` 方法（约第765-777行）

**提取方法：**
```
GetSelectedRowIndexes(): HashSet<int>
```

### 2.2 提取获取选中行是否有非空首列的逻辑

**问题位置：**
- `showRowHeaderContextMenu` 方法（约第723-737行）

**提取方法：**
```
HasNonEmptyFirstColumn(rowIndexes: HashSet<int>): bool
```

---

## 三、重构实施步骤

### 步骤 1：代码结构优化
1. 提取 RadioButton 创建逻辑 → `CreateAlternativeRadioButton`
2. 提取选中索引计算逻辑 → `CalculateSelectedIndex`
3. 提取面板控件清理逻辑 → `ClearAlternativePanelControls`
4. 提取工作统计信息文本 → `BuildWorkStatisticsText`

### 步骤 2：提取重复逻辑
1. 创建 `GetSelectedRowIndexes()` 方法
2. 创建 `HasNonEmptyFirstColumn()` 方法

---

## 四、注意事项

1. **保持功能不变**：重构过程中不能改变现有功能
2. **保持代码风格一致**：参考现有代码风格编写新代码
3. **逐步实施**：每完成一个步骤后进行验证
4. **更新引用**：确保所有对字段和方法的引用都得到更新
