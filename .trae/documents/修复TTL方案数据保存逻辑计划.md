# 计划：修复TTL方案数据保存和加载逻辑

## 问题分析

现有程序有以下问题：

1. **退出时只保存当前引擎数据** - saveProjectFile只保存当前引擎的数据，而不是所有引擎
2. 其他逻辑基本正确

## 需要修复

### 问题：保存到磁盘时需要保存所有引擎数据

根据第3点要求：
> 保存文件时...需要把所有项目文件的内容都写入磁盘中

当前saveProjectFile只调用：
- save角色映射PanelData() - 只保存当前引擎
- save角色和情绪指定PanelData() - 只保存当前引擎
- save语音生成预处理PanelData() - 只保存当前引擎
- save语音生成PanelData() - 只保存当前引擎

这意味着如果用户没有切换引擎就退出，其他引擎的数据不会被保存。

## 实施步骤

1. 修改saveProjectFile方法：
   - 遍历所有引擎ID
   - 为每个引擎设置CurrentEngineId并调用SaveData
   - 然后将项目文件写入磁盘

2. 确保EngineChanged中的逻辑不变：
   - 切换引擎时保存旧引擎数据到项目文件内存

3. 确保选择"无"方案时禁用UI：
   - VoiceGenerationTabPage.Enabled = (currentEngine != null)

