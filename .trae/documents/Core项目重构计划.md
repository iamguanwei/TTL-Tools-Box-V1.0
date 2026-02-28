# Core项目重构计划

## 一、项目现状分析

### 1.1 当前目录结构
```
GW.TTLtoolsBox.Core/
├── FileAccesser/
│   ├── IniFileAccesser.cs
│   └── ProjectFile.cs (870行，包含6个嵌套类)
├── PolyReplace/
│   ├── Helper/PinyinHelper.cs
│   ├── APolyphonicScheme.cs
│   ├── HomophonePolyphonicScheme.cs
│   ├── IPolyphonicScheme.cs
│   ├── PolyphonicItem.cs
│   ├── ReplaceItem.cs
│   └── TonePinyinPolyphonicScheme.cs
├── SystemOption/
│   ├── Helper/MD5Helper.cs
│   └── TtlEngine/ (包含13个文件)
├── TextSplit/
│   └── TextSplitHelper.cs
└── ffmpeg/
```

### 1.2 发现的问题

#### 1.2.1 目录结构问题
- 目录命名不一致（`FileAccesser` vs `TtlEngine`）
- `SystemOption`目录混合了不同功能模块（Helper和TtlEngine）
- 缺少规范要求的目录结构（Entity、Interface、DTO、Enum等）

#### 1.2.2 文件结构问题
- `ProjectFile.cs`文件过大（870行），包含6个嵌套类
- 嵌套类`ProjectData`、`EngineProjectData`、`RoleMappingItem`、`RoleEmotionItem`、`VoiceGenerationTask`、`VoiceGenerationTaskItem`应拆分为独立文件

#### 1.2.3 代码规范问题
- 部分文件存在空的`#region`块（如`TtlEngineParameters.cs`、`TtlEngineTask.cs`）
- 常量命名不符合规范（如`_toString_Split`应为`_ToString_Split`）
- 中文常量命名（如`_换行_替换符`、`_旧版_项目文件_名称`）
- 部分方法逻辑有问题（`APolyphonicScheme.Replace`方法无实际效果）
- 异常处理使用空catch块

#### 1.2.4 架构设计问题
- `TtlEngineFactory`使用硬编码类型名称反射创建实例
- `PolyphonicItem.PolyphonicSchemes`属性每次访问都执行反射，效率低
- 命名空间结构扁平，未按模块细分

---

## 二、重构目标

### 2.1 目录结构重构
按照规范要求，调整为以下结构：
```
GW.TTLtoolsBox.Core/
├── Common/
│   └── Helper/
│       └── MD5Helper.cs
├── FileAccess/
│   ├── IniFileAccesser.cs
│   └── ProjectFile.cs
├── PolyReplace/
│   ├── Entity/
│   │   ├── PolyphonicItem.cs
│   │   └── ReplaceItem.cs
│   ├── Interface/
│   │   └── IPolyphonicScheme.cs
│   ├── Scheme/
│   │   ├── APolyphonicScheme.cs
│   │   ├── HomophonePolyphonicScheme.cs
│   │   └── TonePinyinPolyphonicScheme.cs
│   └── Helper/
│       └── PinyinHelper.cs
├── TtlEngine/
│   ├── Entity/
│   │   ├── ConnectionStatus.cs
│   │   ├── SpeakerInfo.cs
│   │   ├── TtlEngineParameters.cs
│   │   └── TtlEngineTask.cs
│   ├── Enum/
│   │   └── SpeakerEnums.cs
│   ├── Interface/
│   │   └── ITtlEngineConnector.cs
│   ├── Connector/
│   │   ├── ATtlEngineConnector.cs
│   │   ├── ANetworkTtlEngineConnector.cs
│   │   ├── ALocalFileTtlEngineConnector.cs
│   │   └── CosyVoiceV3LYttlEngineConnector.cs
│   ├── Factory/
│   │   └── TtlEngineFactory.cs
│   └── Events/
│       ├── TtlEngineCompletedEventArgs.cs
│       ├── TtlEngineConnectionEventArgs.cs
│       ├── TtlEngineProgressEventArgs.cs
│       ├── TtlEngineQueuedEventArgs.cs
│       └── TtlEngineReconnectEventArgs.cs
├── Project/
│   ├── Entity/
│   │   ├── ProjectData.cs
│   │   ├── EngineProjectData.cs
│   │   ├── RoleMappingItem.cs
│   │   ├── RoleEmotionItem.cs
│   │   ├── VoiceGenerationTask.cs
│   │   └── VoiceGenerationTaskItem.cs
│   └── ProjectFile.cs
├── TextSplit/
│   └── TextSplitHelper.cs
└── ffmpeg/
```

### 2.2 代码规范修正
1. 清理所有空的`#region`块
2. 常量命名规范化（使用大写字母开头的英文命名）
3. 移除中文常量命名，改为英文命名
4. 修复`APolyphonicScheme.Replace`方法的逻辑问题
5. 改进异常处理，避免空catch块

### 2.3 架构优化
1. 优化`TtlEngineFactory`，使用注册机制替代硬编码反射
2. 优化`PolyphonicItem.PolyphonicSchemes`，使用静态缓存避免重复反射
3. 细化命名空间结构

---

## 三、重构任务清单

### 阶段一：目录结构调整（影响较大，需谨慎）

| 序号 | 任务 | 优先级 | 说明 |
|------|------|--------|------|
| 1.1 | 创建新的目录结构 | 高 | 按规范创建Entity、Interface、Enum等子目录 |
| 1.2 | 迁移TtlEngine相关文件 | 高 | 从SystemOption/TtlEngine迁移到TtlEngine目录 |
| 1.3 | 迁移MD5Helper | 中 | 从SystemOption/Helper迁移到Common/Helper |
| 1.4 | 重命名FileAccesser目录 | 中 | 改为FileAccess |
| 1.5 | 拆分ProjectFile.cs中的嵌套类 | 高 | 拆分为独立文件放入Project/Entity目录 |

### 阶段二：代码规范修正

| 序号 | 任务 | 优先级 | 说明 |
|------|------|--------|------|
| 2.1 | 清理空region块 | 高 | TtlEngineParameters.cs、TtlEngineTask.cs等 |
| 2.2 | 常量命名规范化 | 中 | 将_toString_Split等改为规范命名 |
| 2.3 | 中文常量改为英文 | 中 | _换行_替换符 → _NewLine_Replacement等 |
| 2.4 | 修复Replace方法逻辑 | 高 | APolyphonicScheme.Replace方法 |
| 2.5 | 改进异常处理 | 中 | 添加日志记录或适当的异常传播 |

### 阶段三：架构优化

| 序号 | 任务 | 优先级 | 说明 |
|------|------|--------|------|
| 3.1 | 优化TtlEngineFactory | 中 | 使用注册机制替代硬编码反射 |
| 3.2 | 优化PolyphonicSchemes属性 | 中 | 使用静态缓存避免重复反射 |
| 3.3 | 更新命名空间 | 高 | 按新目录结构更新所有文件的命名空间 |
| 3.4 | 更新项目文件 | 高 | 更新.csproj文件中的文件路径 |

### 阶段四：验证与测试

| 序号 | 任务 | 优先级 | 说明 |
|------|------|--------|------|
| 4.1 | 编译验证 | 高 | 确保项目编译通过 |
| 4.2 | 更新WinFormUI项目引用 | 高 | 更新using语句和命名空间引用 |
| 4.3 | 功能测试 | 高 | 确保核心功能正常工作 |

---

## 四、风险评估

### 4.1 高风险项
- 目录结构调整会导致大量文件移动和命名空间变更
- WinFormUI项目需要同步更新所有引用

### 4.2 中风险项
- 常量命名变更可能影响序列化/反序列化
- 架构优化可能引入新的bug

### 4.3 低风险项
- 空region清理
- 注释完善

---

## 五、实施建议

### 5.1 实施顺序
建议按以下顺序执行，降低风险：
1. **先处理代码规范问题**（阶段二），不涉及文件移动
2. **再处理架构优化**（阶段三），主要是代码层面的改进
3. **最后处理目录结构调整**（阶段一），影响最大，需要同步更新所有引用

### 5.2 回滚策略
- 在开始重构前创建Git分支
- 每个阶段完成后创建提交点
- 保持WinFormUI项目的同步更新

---

## 六、预期成果

重构完成后，Core项目将具备：
1. 清晰的目录结构，符合团队编码规范
2. 规范的代码风格，提高可读性和可维护性
3. 优化的架构设计，提升性能和扩展性
4. 完善的文档注释，便于团队协作
