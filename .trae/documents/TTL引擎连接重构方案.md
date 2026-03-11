# TTL引擎连接重构方案

## 一、现状分析

### 1.1 实例管理现状

**当前实现：非单例模式，多实例并存**

```
应用启动
    ↓
LoadEngineConnectors() 通过反射创建所有引擎实例
    ↓
_engineConnectorArray = [IndexTTLv2LY, CosyVoiceV3LY, ...]
    ↓
_currentEngineConnector = 选中的引擎实例
```

**关键代码位置：**
- [TtlSchemeController.cs:325-376](file:///d:/User%20Data/guanw@hotmail.com/我的私有云/GW's%20Data/我的开发/GWs/TTL工具箱/TTL%20Tools%20Box%20V1.0/GW.TTLtoolsBox.WinFormUi/Manager/TtlSchemeController.cs#L325-L376)

### 1.2 连接状态管理现状

**双层状态枚举：**

| Core层 (ConnectionStatus) | UI层 (TtlEngineConnectionStatus) |
|---------------------------|----------------------------------|
| Disconnected = 0 | 未连接 |
| Connecting = 1 | 连接中 |
| Connected = 2 | 连接成功 |
| Failed = 3 | 连接失败 |

**状态管理位置：**
- 引擎实例内部：`ATtlEngineConnector._connectionStatus`
- UI控制器：`TtlSchemeController._connectionStatus`

### 1.3 连接事件定义

**事件触发流程：**
```
引擎实例.OnConnectionStatusChanged()
    ↓
触发 ITtlEngineConnector.ConnectionStatusChanged 事件
    ↓
（目前没有订阅此事件！）
```

**问题：引擎实例的 `ConnectionStatusChanged` 事件没有被UI层订阅！**

---

## 二、发现的问题

### 2.1 问题一：多实例并存导致资源浪费

**现象：**
- 所有引擎实例在应用启动时创建
- 每个实例都有自己的 `HttpClient`、任务队列、信号量等资源
- 即使不使用的引擎也会占用资源

**影响：**
- 内存资源浪费
- 可能存在连接资源泄漏

### 2.2 问题二：引擎切换时未断开旧连接

**代码位置：** [TtlSchemeController.cs:98-117](file:///d:/User%20Data/guanw@hotmail.com/我的私有云/GW's%20Data/我的开发/GWs/TTL工具箱/TTL%20Tools%20Box%20V1.0/GW.TTLtoolsBox.WinFormUi/Manager/TtlSchemeController.cs#L98-L117)

```csharp
public void SelectEngine(string engineId)
{
    string previousEngineId = _currentEngineConnector?.Id ?? 无_引擎标识;
    // ... 找到新引擎
    _currentEngineConnector = engine;  // 直接更换引用，没有断开旧引擎！
    _connectionStatus = TtlEngineConnectionStatus.未连接;
    // ...
}
```

**影响：**
- 旧引擎的连接状态可能不一致
- 旧引擎的任务队列可能残留未处理任务
- 网络连接资源可能泄漏

### 2.3 问题三：连接状态不同步

**现象：**
- 引擎实例内部维护 `_connectionStatus`
- `TtlSchemeController` 也维护独立的 `_connectionStatus`
- 两者可能不一致

**具体场景：**
1. 引擎连接成功后，内部状态为 `Connected`
2. 切换到其他引擎时，控制器状态重置为 `未连接`
3. 但原引擎实例内部状态仍为 `Connected`
4. 再次切换回来时，状态不一致

### 2.4 问题四：引擎事件未被订阅

**现象：**
- `ITtlEngineConnector.ConnectionStatusChanged` 事件定义了但未被使用
- UI层通过定时器轮询状态，而不是响应事件

---

## 三、重构方案

### 3.1 方案概述

**核心思路：保持多实例设计，但确保同一时刻只有一个引擎处于"活跃连接"状态**

### 3.2 重构步骤

#### 步骤一：增强引擎切换逻辑

**修改 `TtlSchemeController.SelectEngine()` 方法：**

```csharp
public async void SelectEngine(string engineId)
{
    string previousEngineId = _currentEngineConnector?.Id ?? 无_引擎标识;
    var previousEngine = _currentEngineConnector;
    
    // 1. 断开旧引擎连接
    if (previousEngine != null)
    {
        await DisconnectPreviousEngineAsync(previousEngine);
    }
    
    // 2. 选择新引擎
    ITtlEngineConnector engine = null;
    if (!string.IsNullOrEmpty(engineId) && engineId != 无_引擎标识)
    {
        engine = _engineConnectorArray?.FirstOrDefault(c => c.Id == engineId);
    }
    
    // 3. 更新当前引擎引用
    _currentEngineConnector = engine;
    
    // 4. 重置连接状态
    _connectionStatus = TtlEngineConnectionStatus.未连接;
    
    // 5. 触发事件
    OnEngineChanged(newEngineId, previousEngineId);
}

private async Task DisconnectPreviousEngineAsync(ITtlEngineConnector engine)
{
    try
    {
        // 停止自动重连
        engine.StopAutoReconnect();
        
        // 清空任务队列
        // （需要在ATtlEngineConnector中添加ClearQueue方法）
        
        // 断开连接
        await engine.DisconnectAsync();
    }
    catch
    {
        // 忽略断开连接时的异常
    }
}
```

#### 步骤二：统一连接状态管理

**方案A：移除UI层独立状态，直接使用引擎状态**

```csharp
public TtlEngineConnectionStatus ConnectionStatus
{
    get
    {
        if (_currentEngineConnector == null)
            return TtlEngineConnectionStatus.未连接;
            
        return _currentEngineConnector.GetConnectionStatus() switch
        {
            ConnectionStatus.Disconnected => TtlEngineConnectionStatus.未连接,
            ConnectionStatus.Connecting => TtlEngineConnectionStatus.连接中,
            ConnectionStatus.Connected => TtlEngineConnectionStatus.连接成功,
            ConnectionStatus.Failed => TtlEngineConnectionStatus.连接失败,
            _ => TtlEngineConnectionStatus.未连接
        };
    }
}
```

**方案B：订阅引擎事件同步状态（推荐）**

```csharp
public void SelectEngine(string engineId)
{
    // 取消订阅旧引擎事件
    if (_currentEngineConnector != null)
    {
        _currentEngineConnector.ConnectionStatusChanged -= OnEngineConnectionStatusChanged;
    }
    
    // ... 选择新引擎 ...
    
    // 订阅新引擎事件
    if (_currentEngineConnector != null)
    {
        _currentEngineConnector.ConnectionStatusChanged += OnEngineConnectionStatusChanged;
    }
}

private void OnEngineConnectionStatusChanged(object sender, TtlEngineConnectionEventArgs e)
{
    _connectionStatus = e.Status switch
    {
        ConnectionStatus.Disconnected => TtlEngineConnectionStatus.未连接,
        ConnectionStatus.Connecting => TtlEngineConnectionStatus.连接中,
        ConnectionStatus.Connected => TtlEngineConnectionStatus.连接成功,
        ConnectionStatus.Failed => TtlEngineConnectionStatus.连接失败,
        _ => TtlEngineConnectionStatus.未连接
    };
    
    OnConnectionStatusChanged();
}
```

#### 步骤三：添加任务队列清理功能

**在 `ATtlEngineConnector` 中添加：**

```csharp
/// <summary>
/// 清空任务队列
/// </summary>
/// <returns>被清除的任务数量</returns>
public int ClearQueue()
{
    int count = 0;
    while (_taskQueue.TryDequeue(out var task))
    {
        task.TaskCompletionSource.SetCanceled();
        count++;
    }
    return count;
}
```

#### 步骤四：优化连接定时器逻辑

**修改 `ConnectionTimer_Tick` 方法：**

```csharp
private void ConnectionTimer_Tick(object sender, EventArgs e)
{
    if (_currentEngineConnector == null)
    {
        _connectionStatus = TtlEngineConnectionStatus.未连接;
        OnConnectionStatusChanged();
        return;
    }
    
    // 使用引擎的实际状态
    var engineStatus = _currentEngineConnector.GetConnectionStatus();
    
    switch (_connectionStatus)
    {
        case TtlEngineConnectionStatus.连接中:
            HandleConnectingState();
            break;
        case TtlEngineConnectionStatus.连接成功:
            HandleConnectedState();
            break;
        case TtlEngineConnectionStatus.连接失败:
            HandleFailedState();
            break;
    }
    
    OnConnectionStatusChanged();
}
```

---

## 四、重构影响范围

### 4.1 需要修改的文件

| 文件 | 修改内容 |
|------|----------|
| `TtlSchemeController.cs` | 增强SelectEngine方法、订阅引擎事件、统一状态管理 |
| `ATtlEngineConnector.cs` | 添加ClearQueue方法 |
| `ITtlEngineConnector.cs` | 添加ClearQueue方法声明 |

### 4.2 不需要修改的文件

- 具体引擎实现类（`IndexTTLv2LYttlEngineConnector`、`CosyVoiceV3LYttlEngineConnector`）
- UI面板类（`TtlSchemePanel`等）
- 其他业务逻辑类

---

## 五、风险评估

### 5.1 低风险

- 添加 `ClearQueue` 方法：新功能，不影响现有逻辑
- 订阅引擎事件：增强功能，不影响现有逻辑

### 5.2 中风险

- 修改 `SelectEngine` 方法：需要确保异步操作正确处理
- 统一状态管理：需要确保所有状态转换路径都被覆盖

### 5.3 测试要点

1. 引擎切换时旧连接是否正确断开
2. 切换引擎后重新连接是否正常
3. 任务队列在引擎切换时是否正确处理
4. 连接状态显示是否与实际一致

---

## 六、实施建议

### 6.1 分阶段实施

**第一阶段：添加基础设施**
1. 在接口中添加 `ClearQueue` 方法声明
2. 在基类中实现 `ClearQueue` 方法

**第二阶段：增强引擎切换**
1. 修改 `SelectEngine` 方法，添加断开连接逻辑
2. 添加 `DisconnectPreviousEngineAsync` 辅助方法

**第三阶段：统一状态管理**
1. 订阅引擎的 `ConnectionStatusChanged` 事件
2. 移除重复的状态管理逻辑

### 6.2 回滚方案

如果重构后出现问题，可以：
1. 保留原有的状态管理逻辑
2. 仅回滚 `SelectEngine` 方法的修改
3. 新增的 `ClearQueue` 方法可以保留（不影响现有功能）

---

## 七、总结

| 问题 | 解决方案 | 优先级 |
|------|----------|--------|
| 引擎切换时未断开旧连接 | 在SelectEngine中调用DisconnectAsync | 高 |
| 连接状态不同步 | 订阅引擎事件同步状态 | 高 |
| 任务队列残留 | 添加ClearQueue方法 | 中 |
| 引擎事件未被使用 | 订阅ConnectionStatusChanged事件 | 中 |

**建议采用方案B（订阅引擎事件同步状态）**，这样可以：
1. 保持现有架构不变
2. 确保状态一致性
3. 减少代码重复
4. 提高可维护性
