# TTL方案面板朗读者列表添加"备注"列实施计划

## 一、需求概述
为TTL方案面板的朗读者列表添加一列"备注"，可编辑文本框，放在第4列，最小长度300，长度自动扩充到表格边沿。内容要保存到系统配置，跟速度和音量类似，备注也需要跟TTL方案和朗读者绑定。

## 二、涉及文件
1. `GW.TTLtoolsBox.Core\TtlEngine\SpeakerInfo.cs` - 朗读者实体类
2. `GW.TTLtoolsBox.WinFormUi\UI\Panels\TtlSchemePanel.cs` - TTL方案面板

## 三、实施步骤

### 步骤1：修改SpeakerInfo实体类
**文件：** `GW.TTLtoolsBox.Core\TtlEngine\SpeakerInfo.cs`

**修改内容：**
1. 添加`Remark`属性（string类型，默认空字符串）
2. 修改`ToString()`方法，序列化格式从`源名称|声音样本路径|速度|音量`改为`源名称|声音样本路径|速度|音量|备注`
3. 修改`TryFromString()`方法，支持反序列化新格式，同时兼容旧格式（无备注时默认空字符串）

### 步骤2：修改TtlSchemePanel的DataGridView列设置
**文件：** `GW.TTLtoolsBox.WinFormUi\UI\Panels\TtlSchemePanel.cs`

**修改内容：**
在`setupSpeakerGridColumns()`方法中：
1. 在音量列之后、声音预览列之前添加"备注"列
2. 列配置：
   - 名称：`Remark`
   - 数据绑定属性：`Remark`
   - 标题：`备注`
   - 最小宽度：300
   - 只读：false（可编辑）
   - 自动填充：设置`AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill`

### 步骤3：调整列顺序
确保列顺序为：
1. 源名称（只读）
2. 速度（可编辑）
3. 音量（可编辑）
4. **备注（可编辑）** ← 新增
5. 声音预览（按钮）

## 四、数据存储机制
- 存储格式：`Speaker_{引擎ID}_{源名称} = 源名称|声音样本路径|速度|音量|备注`
- 存储位置：`Setting.ini`文件
- 兼容性：反序列化时兼容旧格式（无备注字段）

## 五、注意事项
1. 备注可能包含特殊字符（如`|`），需要在序列化时进行转义处理
2. 保持与现有保存/加载逻辑的一致性
3. 确保向后兼容，旧配置文件无备注字段时正常加载
