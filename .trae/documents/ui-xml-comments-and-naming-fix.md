# UI项目XML注释和命名规范修复计划

## 一、问题汇总

### 1. 缺少XML注释的问题

| 文件 | 位置 | 问题 |
|-----|------|-----|
| AudioService.cs | 第161-164行 | `OnPlaybackCompleted()` 方法缺少XML注释 |
| AudioService.cs | 第166-169行 | `OnPlaybackError(string message)` 方法缺少XML注释 |
| TextBoxExtension.cs | 第14-23行 | Windows API常量和DllImport声明缺少XML注释 |
| CustomNewLineDataGridView.cs | 第345-355行 | `GetAutoRowHeight()` 方法缺少XML注释 |
| CustomNewLineDataGridView.cs | 第357-370行 | `IsCellContentEmpty()` 方法缺少XML注释 |
| CustomNewLineDataGridView.cs | 第308-343行 | `CustomNewLineDataGridView_EditingControlShowing()` 方法缺少XML注释 |

### 2. 命名不规范的问题

| 文件 | 当前命名 | 问题 | 修改为 |
|-----|---------|------|-------|
| Constants.cs | `None_Engine_Id` | 与其他常量命名风格不一致（其他常量都是中文命名） | `无_引擎标识` |
| Setting.cs | `Setting_File_Full_Name` | 常量命名风格不一致 | `设置_文件_全名` |

## 二、修改步骤

### 步骤1：修复 AudioService.cs 的XML注释
- 为 `OnPlaybackCompleted()` 方法添加XML注释
- 为 `OnPlaybackError(string message)` 方法添加XML注释

### 步骤2：修复 TextBoxExtension.cs 的XML注释
- 为Windows API常量添加XML注释
- 为DllImport声明添加XML注释

### 步骤3：修复 CustomNewLineDataGridView.cs 的XML注释
- 为 `GetAutoRowHeight()` 方法添加XML注释
- 为 `IsCellContentEmpty()` 方法添加XML注释
- 为 `CustomNewLineDataGridView_EditingControlShowing()` 方法添加XML注释

### 步骤4：修复 Constants.cs 的命名
- 将 `None_Engine_Id` 改为 `无_引擎标识`

### 步骤5：修复 Setting.cs 的命名
- 将 `Setting_File_Full_Name` 改为 `设置_文件_全名`

### 步骤6：检查并修复引用
- 搜索项目中所有对 `None_Engine_Id` 的引用并更新
- 搜索项目中所有对 `Setting_File_Full_Name` 的引用并更新

## 三、常量引用分析

### None_Engine_Id 引用位置
- `Constants.cs:117` - 定义处
- `TtlSchemePanel.cs:1103-1104` - 使用处（含注释）
- `TtlSchemeController.cs:100, 103, 108, 121` - 使用处

### Setting_File_Full_Name 引用位置
- `Setting.cs:16` - 定义处
- `Setting.cs:34` - 内部使用

## 四、预期修改文件

1. `GW.TTLtoolsBox.WinFormUi\Service\AudioService.cs` - 添加XML注释
2. `GW.TTLtoolsBox.WinFormUi\Helper\TextBoxExtension.cs` - 添加XML注释
3. `GW.TTLtoolsBox.WinFormUi\UI\CustomNewLineDataGridView.cs` - 添加XML注释
4. `GW.TTLtoolsBox.WinFormUi\Helper\Constants.cs` - 修改常量命名
5. `GW.TTLtoolsBox.WinFormUi\Helper\Setting.cs` - 修改常量命名
6. `GW.TTLtoolsBox.WinFormUi\UI\Panels\TtlSchemePanel.cs` - 更新常量引用
7. `GW.TTLtoolsBox.WinFormUi\Manager\TtlSchemeController.cs` - 更新常量引用
