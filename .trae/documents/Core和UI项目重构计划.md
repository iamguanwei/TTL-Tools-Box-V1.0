# Core和UI项目重构计划

## 一、重构进度

### 已完成的工作 ✅

#### 阶段0：代码规范清理
- [x] 修正了19个常量的命名规范（从三段式改为两段式格式）
- [x] 检查了代码区域结构
- [x] 确认无无用代码

#### 阶段1：基础设施准备
**新增Base目录：**
- [x] `IView.cs` - 视图接口
- [x] `IPresenter.cs` - 呈现器接口
- [x] `ViewBase.cs` - 视图基类 ✅已使用

**新增Manager目录：**
- [x] `TtlEngineConnectionStatus.cs` - 连接状态枚举 ✅已使用
- [x] `VoiceGenerationTaskStatus.cs` - 任务状态枚举 ✅已使用
- [x] `VoiceGenerationTaskItem.cs` - 任务项类（UI绑定用）✅已使用
- [x] `VoiceGenerationTask.cs` - 任务类（UI绑定用，实现INotifyPropertyChanged）✅已使用
- [x] `RoleTextItem.cs` - 角色文本项类 ✅已使用
- [x] `VoiceGenerationTaskQueue.cs` - 任务队列管理器 ✅已使用
- [x] `TtlSchemeController.cs` - TTL方案管理控制器 ✅已使用

**新增Service目录（预留，暂未使用）：**
- [x] `IAudioService.cs` - 音频服务接口
- [x] `AudioService.cs` - 音频服务实现

**新增Helper目录：**
- [x] `FileHelper.cs` ✅已使用
- [x] `DataGridViewHelper.cs` ✅已使用
- [x] `AudioPlayer.cs` ✅已使用
- [x] `UiControlHelper.cs` ✅已使用
- [x] `Constants.cs` ✅已使用

#### 阶段2：常量迁移
- [x] 将MainForm中的17个常量迁移到Constants类
- [x] 删除MainForm中的冗余常量定义
- [x] 删除TtlSchemeController中的重复常量定义
- [x] 添加`using static GW.TTLtoolsBox.WinFormUi.Helper.Constants;`

#### 阶段3：面板拆分（阶段4）
**新增UI/Panels目录：**
- [x] `TextSplitPanel.cs` + `TextSplitPanel.Designer.cs` - 文本拆分面板（~300行）
- [x] `RoleMappingPanel.cs` + `RoleMappingPanel.Designer.cs` - 角色映射面板（~420行）
- [x] `PolyphonicReplacePanel.cs` + `PolyphonicReplacePanel.Designer.cs` - 多音字替换面板（~660行）
- [x] `TtlSchemePanel.cs` + `TtlSchemePanel.Designer.cs` - TTL方案面板（~600行）
- [x] `VoicePreprocessPanel.cs` + `VoicePreprocessPanel.Designer.cs` - 语音生成预处理面板（~560行）
- [x] `RoleEmotionPanel.cs` - 角色和情绪指定面板（~970行）
- [x] `VoiceGenerationPanel.cs` + `VoiceGenerationPanel.Designer.cs` - 语音生成面板（~1560行）

---

## 二、当前项目结构

```
GW.TTLtoolsBox.WinFormUi/
├── Base/                           # 基础类
│   ├── IView.cs                    ✅已使用
│   ├── IPresenter.cs               ✅已使用
│   └── ViewBase.cs                 ✅已使用
│
├── Manager/                        # 管理器类
│   ├── TtlEngineConnectionStatus.cs   ✅已使用
│   ├── TtlSchemeController.cs         ✅已使用
│   ├── VoiceGenerationTaskStatus.cs   ✅已使用
│   ├── VoiceGenerationTaskItem.cs     ✅已使用
│   ├── VoiceGenerationTask.cs         ✅已使用
│   ├── RoleTextItem.cs                ✅已使用
│   └── VoiceGenerationTaskQueue.cs    ✅已使用
│
├── Service/                        # 服务类（预留）
│   ├── IAudioService.cs
│   └── AudioService.cs
│
├── Helper/
│   ├── Setting.cs
│   ├── TextBoxExtension.cs
│   ├── UiHelper.cs
│   ├── FileHelper.cs              ✅已使用
│   ├── DataGridViewHelper.cs      ✅已使用
│   ├── AudioPlayer.cs             ✅已使用
│   ├── UiControlHelper.cs         ✅已使用
│   └── Constants.cs               ✅已使用
│
├── UI/
│   ├── CustomNewLineDataGridView.cs
│   └── Panels/
│       ├── TextSplitPanel.cs           ✅新增
│       ├── TextSplitPanel.Designer.cs  ✅新增
│       ├── RoleMappingPanel.cs         ✅新增
│       ├── RoleMappingPanel.Designer.cs✅新增
│       ├── PolyphonicReplacePanel.cs   ✅新增
│       ├── PolyphonicReplacePanel.Designer.cs ✅新增
│       ├── TtlSchemePanel.cs           ✅新增
│       ├── TtlSchemePanel.Designer.cs  ✅新增
│       ├── VoicePreprocessPanel.cs     ✅新增
│       ├── VoicePreprocessPanel.Designer.cs ✅新增
│       ├── RoleEmotionPanel.cs         ✅新增
│       ├── VoiceGenerationPanel.cs     ✅新增
│       └── VoiceGenerationPanel.Designer.cs ✅新增
│
├── MainForm.cs                     # 主窗体（已精简）
├── MainForm.Designer.cs            # 控件访问级别已改为internal
└── Program.cs
```

---

## 三、重构效果

### 代码量变化
| 文件/目录 | 重构前 | 重构后 | 变化 |
|-----------|--------|--------|------|
| MainForm.cs | ~6200行 | ~2000行 | -4200行 |
| 新增Panel文件 | 0 | 14个文件 | +约5000行 |
| 净增加代码 | - | - | +约800行（含重复引用） |

### 新增文件统计
| 目录 | 文件数 | 已使用 | 说明 |
|------|--------|--------|------|
| Base | 3 | 3 | 基础接口和类 |
| Manager | 7 | 7 | 实体类和管理器 ✅全部已使用 |
| Helper | 8 | 8 | 工具类 ✅全部已使用 |
| Service | 2 | 0 | 服务接口和实现（预留） |
| UI/Panels | 14 | 14 | 面板类 ✅全部已使用 |

### 架构改进
1. **解耦**：嵌套类型移至独立文件，降低MainForm复杂度
2. **封装**：VoiceGenerationTaskQueue封装了任务队列操作
3. **TTL方案管理**：TtlSchemeController封装了TTL引擎连接管理
4. **面板模块化**：7个TabPage拆分为独立的UserControl
5. **事件驱动**：面板通过事件与MainForm通信，保持松耦合
6. **可维护性**：代码结构更清晰，便于查找和修改
7. **可扩展性**：基础设施已就位，便于后续添加新功能

---

## 四、面板拆分详情

### 面板与MainForm的交互方式

每个面板都继承自`ViewBase`，通过以下方式与MainForm通信：

1. **依赖注入**：MainForm向面板注入必要的服务
   ```csharp
   _textSplitPanel = new TextSplitPanel();
   _textSplitPanel.TtlSchemeController = _ttlSchemeController;
   _textSplitPanel.ProjectFile = getProjectFile();
   ```

2. **事件通知**：面板通过事件向MainForm通知状态变化
   ```csharp
   _textSplitPanel.ProjectModified += (s, e) => markProjectModified();
   _textSplitPanel.SwitchToNextPageRequested += (s, e) => switchToNextPage(e.Text);
   ```

3. **公共接口**：面板暴露公共方法供MainForm调用
   ```csharp
   _textSplitPanel.InitializePanel();
   _textSplitPanel.LoadData();
   _textSplitPanel.SaveData();
   ```

### 各面板功能

| 面板 | 功能 | 主要控件 |
|------|------|---------|
| TextSplitPanel | 文本拆分 | TextBox, RadioButton, NumericUpDown |
| RoleMappingPanel | 角色映射 | DataGridView |
| PolyphonicReplacePanel | 多音字替换 | TextBox, DataGridView, ComboBox |
| TtlSchemePanel | TTL方案管理 | ComboBox, DataGridView, TextBox |
| VoicePreprocessPanel | 语音生成预处理 | TextBox, ComboBox, NumericUpDown |
| RoleEmotionPanel | 角色和情绪指定 | DataGridView, ComboBox |
| VoiceGenerationPanel | 语音生成 | DataGridView, CheckBox, Button |

---

## 五、注意事项

### Core层与Manager层的区别
- **Core/Entity/VoiceGenerationTask**：用于数据序列化，简单POCO类
- **Manager/VoiceGenerationTask**：用于UI绑定，实现INotifyPropertyChanged

### 控件访问级别
MainForm.Designer.cs中的控件已改为internal访问级别，允许同一程序集内的Controller类访问。

### Git提交记录
- 第一次提交：`重构前备份：准备完成常量迁移和面板拆分`
- 第二次提交：`完成全面重构：常量迁移和7个面板拆分`

---

## 六、后续建议

1. **渐进式清理**：MainForm.Designer.cs中可能还有部分冗余的控件定义，可以逐步清理
2. **添加单元测试**：为Manager和Panel类添加测试
3. **文档更新**：更新相关技术文档
4. **性能优化**：检查面板加载性能，必要时使用延迟加载
