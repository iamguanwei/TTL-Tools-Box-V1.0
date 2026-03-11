# TTL引擎连接重构方案（修订版）

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

### 1.2 连接状态管理现状

**双层枚举（需要统一）：**

| Core层 (ConnectionStatus) | UI层 (TtlEngineConnectionStatus) |
| ------------------------ | ------------------------------- |
| Disconnected = 0         | 未连接                             |
| Connecting = 1           | 连接中                             |
| Connected = 2            | 连接成功                            |
| Failed = 3               | 连接失败                            |

**状态管理位置：**

* 引擎实例内部：`ATtlEngineConnector._connectionStatus`

* UI控制器：`TtlSchemeController._connectionStatus`

* UI面板：`TtlSchemePanel._ttlEngineConnectionStatus`

### 1.3 连接定时器机制（需要删除）

**UI层轮询机制：**

```
TtlSchemeController._connectionTimer (每秒触发)
    ↓
ConnectionTimer_Tick()
    ↓
HandleConnectingState() / HandleConnectedState() / HandleFailedState()
    ↓
OnConnectionStatusChanged() 触发事件
    ↓
TtlSchemePanel 订阅事件，更新UI
```

**问题：轮询机制冗余，应该使用事件驱动**

***

## 二、重构方案

### 2.1 方案概述

**核心思路：在Core层创建专用的连接管理器，将连接管理逻辑从UI层下沉到Core层**

```
┌─────────────────────────────────────────────────────────┐
│                      UI层                                │
│  ┌─────────────────┐    ┌─────────────────────────┐    │
│  │ TtlSchemePanel  │    │ VoiceGenerationTaskQueue │    │
│  │                 │    │                          │    │
│  │ 订阅事件        │    │ 订阅事件                 │    │
│  └────────┬────────┘    └────────────┬────────────┘    │
│           │                          │                  │
│           └──────────────┬───────────┘                  │
│                          │ 订阅事件                     │
│                          ↓                              │
└─────────────────────────────────────────────────────────┘
                           │
┌──────────────────────────┼──────────────────────────────┐
│                     Core层                              │
│                          ↓                              │
│           ┌──────────────────────────────┐              │
│           │  TtlEngineConnectionManager  │              │
│           │                              │              │
│           │  - 管理所有引擎实例          │              │
│           │  - 管理当前活跃引擎          │              │
│           │  - 连接状态管理              │              │
│           │  - 自动重连逻辑              │              │
│           │  - 连接超时处理              │              │
│           │  - 事件触发                  │              │
│           └──────────────┬───────────────┘              │
│                          │ 管理                         │
│                          ↓                              │
│           ┌──────────────────────────────┐              │
│           │    ITtlEngineConnector[]     │              │
│           │    (引擎实例数组)            │              │
│           └──────────────────────────────┘              │
└─────────────────────────────────────────────────────────┘
```

### 2.2 详细设计

#### 2.2.1 新建：TtlEngineConnectionStatus枚举（Core层）

**位置：** `GW.TTLtoolsBox.Core/TtlEngine/TtlEngineConnectionStatus.cs`

```csharp
namespace GW.TTLtoolsBox.Core.TtlEngine
{
    /// <summary>
    /// TTL引擎连接状态枚举
    /// </summary>
    /// <remarks>
    /// 用于表示TTL引擎的当前连接状态
    /// 状态流转：
    /// - 未连接 → 连接中 → 连接成功
    /// - 连接中 → 连接失败
    /// - 连接成功 → 未连接（断开）
    /// - 连接失败 → 连接中（重试）
    /// </remarks>
    public enum TtlEngineConnectionStatus
    {
        /// <summary>
        /// 引擎未连接
        /// </summary>
        未连接 = 0,

        /// <summary>
        /// 正在连接
        /// </summary>
        连接中 = 1,

        /// <summary>
        /// 连接成功
        /// </summary>
        连接成功 = 2,

        /// <summary>
        /// 连接失败
        /// </summary>
        连接失败 = 3
    }
}
```

**操作：**

* 删除原UI层的 `TtlEngineConnectionStatus.cs`

* 删除原Core层的 `ConnectionStatus.cs`

* 修改 `ATtlEngineConnector` 使用新枚举

#### 2.2.2 新建：TtlEngineConnectionManager类（Core层）

**位置：** `GW.TTLtoolsBox.Core/TtlEngine/TtlEngineConnectionManager.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GW.TTLtoolsBox.Core.TtlEngine
{
    /// <summary>
    /// TTL引擎连接管理器，负责管理所有引擎实例的连接状态
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 管理所有引擎实例
    /// - 管理当前活跃引擎
    /// - 连接状态管理和事件通知
    /// - 自动重连逻辑
    /// - 连接超时处理
    /// - 引擎切换时自动断开旧连接
    /// 
    /// 使用场景：
    /// - 作为UI层与引擎实例之间的桥梁
    /// - 提供统一的连接管理接口
    /// 
    /// 依赖关系：
    /// - 依赖ITtlEngineConnector接口
    /// - 不依赖UI层
    /// </remarks>
    public class TtlEngineConnectionManager : IDisposable
    {
        #region 常量

        private const int DefaultConnectionTimeoutSeconds = 30;
        private const int DefaultRequestTimeoutMinutes = 10;
        private const int DefaultReconnectIntervalSeconds = 5;
        private const int DefaultVerifyIntervalSeconds = 60;

        #endregion

        #region public

        #region 构造函数

        public TtlEngineConnectionManager()
        {
            _reconnectTimer = new Timer(ReconnectTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取当前选中的引擎连接器
        /// </summary>
        public ITtlEngineConnector CurrentEngine => _currentEngine;

        /// <summary>
        /// 获取所有引擎连接器
        /// </summary>
        public ITtlEngineConnector[] AllEngines => _allEngines;

        /// <summary>
        /// 获取当前连接状态
        /// </summary>
        public TtlEngineConnectionStatus ConnectionStatus => _connectionStatus;

        /// <summary>
        /// 获取当前引擎ID
        /// </summary>
        public string CurrentEngineId => _currentEngine?.Id ?? string.Empty;

        /// <summary>
        /// 获取或设置连接超时时间（秒）
        /// </summary>
        public int ConnectionTimeoutSeconds { get; set; } = DefaultConnectionTimeoutSeconds;

        /// <summary>
        /// 获取或设置请求超时时间（分钟）
        /// </summary>
        public int RequestTimeoutMinutes { get; set; } = DefaultRequestTimeoutMinutes;

        /// <summary>
        /// 获取或设置重连间隔（秒）
        /// </summary>
        public int ReconnectIntervalSeconds { get; set; } = DefaultReconnectIntervalSeconds;

        /// <summary>
        /// 获取或设置连接成功后的验证间隔（秒）
        /// </summary>
        public int VerifyIntervalSeconds { get; set; } = DefaultVerifyIntervalSeconds;

        /// <summary>
        /// 获取剩余重连倒计时（秒）
        /// </summary>
        public int ReconnectCountdown => _reconnectCountdown;

        #endregion

        #region 事件

        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

        /// <summary>
        /// 引擎切换事件
        /// </summary>
        public event EventHandler<EngineChangedEventArgs> EngineChanged;

        /// <summary>
        /// 朗读者列表加载完成事件
        /// </summary>
        public event EventHandler<SpeakersLoadedEventArgs> SpeakersLoaded;

        #endregion

        #region 方法

        /// <summary>
        /// 初始化管理器，加载所有引擎实例
        /// </summary>
        public void Initialize()
        {
            if (_allEngines != null) return;
            LoadAllEngines();
        }

        /// <summary>
        /// 选择引擎
        /// </summary>
        /// <param name="engineId">引擎ID</param>
        public async void SelectEngine(string engineId)
        {
            string previousEngineId = _currentEngine?.Id ?? string.Empty;
            var previousEngine = _currentEngine;

            // 断开旧引擎连接
            if (previousEngine != null)
            {
                await DisconnectEngineAsync(previousEngine);
            }

            // 查找新引擎
            ITtlEngineConnector newEngine = null;
            if (!string.IsNullOrEmpty(engineId))
            {
                newEngine = _allEngines?.FirstOrDefault(e => e.Id == engineId);
            }

            // 更新当前引擎
            _currentEngine = newEngine;
            _connectionStatus = TtlEngineConnectionStatus.未连接;
            _reconnectCountdown = 0;

            // 订阅新引擎事件
            if (_currentEngine != null)
            {
                _currentEngine.ConnectionStatusChanged += OnEngineConnectionStatusChanged;
            }

            // 触发引擎切换事件
            string newEngineId = newEngine?.Id ?? string.Empty;
            OnEngineChanged(newEngineId, previousEngineId);
        }

        /// <summary>
        /// 连接到当前引擎
        /// </summary>
        public async Task ConnectAsync()
        {
            if (_currentEngine == null)
            {
                UpdateConnectionStatus(TtlEngineConnectionStatus.未连接);
                return;
            }

            _connectionCts?.Cancel();
            _connectionCts?.Dispose();
            _connectionCts = new CancellationTokenSource();

            UpdateConnectionStatus(TtlEngineConnectionStatus.连接中);

            try
            {
                await _currentEngine.ConnectAsync();
                // 连接成功后由事件回调处理状态更新
            }
            catch (Exception ex)
            {
                UpdateConnectionStatus(TtlEngineConnectionStatus.连接失败, ex.Message);
                StartReconnectTimer();
            }
        }

        /// <summary>
        /// 断开当前连接
        /// </summary>
        public async Task DisconnectAsync()
        {
            if (_currentEngine == null) return;

            StopReconnectTimer();
            await DisconnectEngineAsync(_currentEngine);
            UpdateConnectionStatus(TtlEngineConnectionStatus.未连接);
        }

        /// <summary>
        /// 刷新当前引擎的朗读者列表
        /// </summary>
        public async Task RefreshSpeakersAsync()
        {
            if (_currentEngine == null) return;

            await Task.Run(() =>
            {
                try
                {
                    _currentEngine.RefreshSpeakers();
                    OnSpeakersLoaded(_currentEngine.Speakers, true);
                }
                catch
                {
                }
            });
        }

        /// <summary>
        /// 保存引擎参数
        /// </summary>
        public void SaveEngineParameters(string[] parameters)
        {
            _currentEngine?.SetParameters(parameters);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _reconnectTimer?.Dispose();
            _connectionCts?.Dispose();

            if (_allEngines != null)
            {
                foreach (var engine in _allEngines)
                {
                    engine.ConnectionStatusChanged -= OnEngineConnectionStatusChanged;
                    engine.Dispose();
                }
            }
        }

        #endregion

        #endregion

        #region private

        #region 字段

        private ITtlEngineConnector _currentEngine;
        private ITtlEngineConnector[] _allEngines;
        private TtlEngineConnectionStatus _connectionStatus = TtlEngineConnectionStatus.未连接;
        private Timer _reconnectTimer;
        private CancellationTokenSource _connectionCts;
        private int _reconnectCountdown;

        #endregion

        #region 方法

        private void LoadAllEngines()
        {
            var connectors = new List<ITtlEngineConnector>();

            try
            {
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(ITtlEngineConnector));
                var connectorTypes = assembly.GetTypes().Where(t =>
                    t.IsClass && !t.IsAbstract && t.GetInterface(typeof(ITtlEngineConnector).FullName) != null);

                foreach (var type in connectorTypes)
                {
                    try
                    {
                        var constructor = type.GetConstructor(Type.EmptyTypes);
                        if (constructor != null)
                        {
                            var connector = (ITtlEngineConnector)constructor.Invoke(null);
                            connector.ConnectionTimeout = TimeSpan.FromSeconds(ConnectionTimeoutSeconds);
                            connector.RequestTimeout = TimeSpan.FromMinutes(RequestTimeoutMinutes);
                            connectors.Add(connector);
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }

            _allEngines = connectors.ToArray();
        }

        private async Task DisconnectEngineAsync(ITtlEngineConnector engine)
        {
            if (engine == null) return;

            try
            {
                engine.StopAutoReconnect();
                engine.ConnectionStatusChanged -= OnEngineConnectionStatusChanged;
                engine.ClearQueue();
                await engine.DisconnectAsync();
            }
            catch
            {
            }
        }

        private void OnEngineConnectionStatusChanged(object sender, TtlEngineConnectionEventArgs e)
        {
            var newStatus = e.Status switch
            {
                ConnectionStatus.Disconnected => TtlEngineConnectionStatus.未连接,
                ConnectionStatus.Connecting => TtlEngineConnectionStatus.连接中,
                ConnectionStatus.Connected => TtlEngineConnectionStatus.连接成功,
                ConnectionStatus.Failed => TtlEngineConnectionStatus.连接失败,
                _ => TtlEngineConnectionStatus.未连接
            };

            UpdateConnectionStatus(newStatus, e.Message);

            // 连接成功后启动验证定时器
            if (newStatus == TtlEngineConnectionStatus.连接成功)
            {
                _reconnectCountdown = VerifyIntervalSeconds;
                StartVerifyTimer();
            }
            // 连接失败后启动重连定时器
            else if (newStatus == TtlEngineConnectionStatus.连接失败)
            {
                _reconnectCountdown = ReconnectIntervalSeconds;
                StartReconnectTimer();
            }
        }

        private void UpdateConnectionStatus(TtlEngineConnectionStatus status, string message = null)
        {
            _connectionStatus = status;
            OnConnectionStatusChanged(status, message);
        }

        private void StartReconnectTimer()
        {
            _reconnectCountdown = ReconnectIntervalSeconds;
            _reconnectTimer.Change(1000, 1000);
        }

        private void StartVerifyTimer()
        {
            _reconnectCountdown = VerifyIntervalSeconds;
            _reconnectTimer.Change(1000, 1000);
        }

        private void StopReconnectTimer()
        {
            _reconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _reconnectCountdown = 0;
        }

        private void ReconnectTimerCallback(object state)
        {
            if (_reconnectCountdown > 0)
            {
                _reconnectCountdown--;
                OnConnectionStatusChanged(_connectionStatus, null);
            }

            if (_reconnectCountdown <= 0)
            {
                StopReconnectTimer();

                // 根据当前状态决定行为
                if (_connectionStatus == TtlEngineConnectionStatus.连接失败)
                {
                    _ = ConnectAsync();
                }
                else if (_connectionStatus == TtlEngineConnectionStatus.连接成功)
                {
                    _ = ConnectAsync(); // 验证连接
                }
            }
        }

        private void OnConnectionStatusChanged(TtlEngineConnectionStatus status, string message)
        {
            ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs(status, _reconnectCountdown, message));
        }

        private void OnEngineChanged(string newEngineId, string previousEngineId)
        {
            EngineChanged?.Invoke(this, new EngineChangedEventArgs(newEngineId, previousEngineId));
        }

        private void OnSpeakersLoaded(SpeakerInfo[] speakers, bool refreshedFromEngine)
        {
            SpeakersLoaded?.Invoke(this, new SpeakersLoadedEventArgs(speakers, refreshedFromEngine));
        }

        #endregion

        #endregion

        #region 嵌套类型

        public class ConnectionStatusChangedEventArgs : EventArgs
        {
            public TtlEngineConnectionStatus Status { get; }
            public int Countdown { get; }
            public string Message { get; }

            public ConnectionStatusChangedEventArgs(TtlEngineConnectionStatus status, int countdown, string message)
            {
                Status = status;
                Countdown = countdown;
                Message = message;
            }
        }

        public class EngineChangedEventArgs : EventArgs
        {
            public string NewEngineId { get; }
            public string PreviousEngineId { get; }

            public EngineChangedEventArgs(string newEngineId, string previousEngineId)
            {
                NewEngineId = newEngineId;
                PreviousEngineId = previousEngineId;
            }
        }

        public class SpeakersLoadedEventArgs : EventArgs
        {
            public SpeakerInfo[] Speakers { get; }
            public bool RefreshedFromEngine { get; }

            public SpeakersLoadedEventArgs(SpeakerInfo[] speakers, bool refreshedFromEngine)
            {
                Speakers = speakers;
                RefreshedFromEngine = refreshedFromEngine;
            }
        }

        #endregion
    }
}
```

#### 2.2.3 修改：ITtlEngineConnector接口

**添加** **`ClearQueue`** **方法：**

```csharp
/// <summary>
/// 清空任务队列
/// </summary>
/// <returns>被清除的任务数量</returns>
int ClearQueue();
```

#### 2.2.4 修改：ATtlEngineConnector类

**修改枚举类型并实现** **`ClearQueue`** **方法：**

```csharp
// 修改字段类型
private TtlEngineConnectionStatus _connectionStatus = TtlEngineConnectionStatus.未连接;

// 添加ClearQueue方法
public int ClearQueue()
{
    int count = 0;
    while (_taskQueue.TryDequeue(out var task))
    {
        task.TaskCompletionSource.TrySetCanceled();
        count++;
    }
    return count;
}

// 修改OnConnectionStatusChanged方法签名
protected virtual void OnConnectionStatusChanged(TtlEngineConnectionStatus status, string message, Exception error = null)
{
    _connectionStatus = status;
    ConnectionStatusChanged?.Invoke(this, new TtlEngineConnectionEventArgs(status, message, error));
}
```

#### 2.2.5 修改：TtlEngineConnectionEventArgs

**修改为使用新枚举：**

```csharp
public class TtlEngineConnectionEventArgs : EventArgs
{
    public TtlEngineConnectionStatus Status { get; }
    public string Message { get; }
    public Exception Error { get; }

    public TtlEngineConnectionEventArgs(TtlEngineConnectionStatus status, string message, Exception error = null)
    {
        Status = status;
        Message = message;
        Error = error;
    }
}
```

#### 2.2.6 删除：UI层冗余代码

**删除文件：**

* `GW.TTLtoolsBox.WinFormUi/Manager/TtlEngineConnectionStatus.cs` - 枚举已移至Core层

* `GW.TTLtoolsBox.WinFormUi/Manager/TtlSchemeController.cs` - 功能已移至Core层的 `TtlEngineConnectionManager`

**删除代码（TtlSchemePanel.cs）：**

* `_connectionStatusTimer` 定时器

* `initConnectionStatusTimer()` 方法

* `connectionStatusTimer_Tick()` 方法

* `handleConnectionStatusCountdown()` 方法

* `_ttlEngineConnectionCountdown` 字段

* `_ttlEngineRetryCountdown` 字段

**删除代码（Core层）：**

* `GW.TTLtoolsBox.Core/TtlEngine/ConnectionStatus.cs` - 使用新枚举替代

***

## 三、重构影响范围

### 3.1 新建文件

| 文件路径                                           | 说明      |
| ---------------------------------------------- | ------- |
| `Core/TtlEngine/TtlEngineConnectionStatus.cs`  | 新枚举（中文） |
| `Core/TtlEngine/TtlEngineConnectionManager.cs` | 连接管理器   |

### 3.2 修改文件

| 文件路径                                                    | 修改内容                   |
| ------------------------------------------------------- | ---------------------- |
| `Core/TtlEngine/ITtlEngineConnector.cs`                 | 添加 `ClearQueue` 方法声明   |
| `Core/TtlEngine/ATtlEngineConnector.cs`                 | 修改枚举类型、实现 `ClearQueue` |
| `Core/TtlEngine/Events/TtlEngineConnectionEventArgs.cs` | 修改枚举类型                 |
| `WinFormUi/UI/Panels/TtlSchemePanel.cs`                 | 删除定时器、改用事件订阅           |
| `WinFormUi/Manager/VoiceGenerationTaskQueue.cs`         | 修改获取状态的委托类型            |
| `WinFormUi/MainForm.cs`                                 | 修改初始化逻辑                |

### 3.3 删除文件

| 文件路径                                             | 说明         |
| ------------------------------------------------ | ---------- |
| `WinFormUi/Manager/TtlEngineConnectionStatus.cs` | 枚举已移至Core层 |
| `WinFormUi/Manager/TtlSchemeController.cs`       | 功能已移至Core层 |
| `Core/TtlEngine/ConnectionStatus.cs`             | 使用新枚举替代    |

***

## 四、UI层适配

### 4.1 TtlSchemePanel修改

```csharp
// 原来的TtlSchemeController替换为TtlEngineConnectionManager
public TtlEngineConnectionManager ConnectionManager { get; set; }

// 初始化时订阅事件
public void InitializePanel()
{
    if (ConnectionManager != null)
    {
        ConnectionManager.ConnectionStatusChanged += OnConnectionStatusChanged;
        ConnectionManager.EngineChanged += OnEngineChanged;
        ConnectionManager.SpeakersLoaded += OnSpeakersLoaded;
    }
}

// 事件处理
private void OnConnectionStatusChanged(object sender, TtlEngineConnectionManager.ConnectionStatusChangedEventArgs e)
{
    updateTtlEngineConnectionStatusLabel(e.Status, e.Countdown);
}

private void OnEngineChanged(object sender, TtlEngineConnectionManager.EngineChangedEventArgs e)
{
    // 处理引擎切换
}

private void OnSpeakersLoaded(object sender, TtlEngineConnectionManager.SpeakersLoadedEventArgs e)
{
    // 处理朗读者列表加载
}
```

### 4.2 VoiceGenerationTaskQueue修改

```csharp
// 修改构造函数参数类型
public VoiceGenerationTaskQueue(
    Func<ITtlEngineConnector> getEngineConnector,
    Func<TtlEngineConnectionStatus> getConnectionStatus,  // 使用Core层枚举
    string ffmpegPath,
    string tempFolder)
{
    // ...
}
```

***

## 五、实施步骤

### 第一阶段：Core层基础设施

1. 新建 `TtlEngineConnectionStatus.cs` 枚举（Core层）
2. 修改 `TtlEngineConnectionEventArgs.cs` 使用新枚举
3. 修改 `ATtlEngineConnector.cs` 使用新枚举并添加 `ClearQueue` 方法
4. 修改 `ITtlEngineConnector.cs` 添加 `ClearQueue` 方法声明
5. 删除旧的 `ConnectionStatus.cs`

### 第二阶段：连接管理器

1. 新建 `TtlEngineConnectionManager.cs`
2. 实现所有连接管理逻辑

### 第三阶段：UI层适配

1. 修改 `TtlSchemePanel.cs` 删除定时器，改用事件订阅
2. 修改 `VoiceGenerationTaskQueue.cs` 使用新枚举
3. 修改 `MainForm.cs` 初始化逻辑
4. 删除 `TtlSchemeController.cs`
5. 删除UI层的 `TtlEngineConnectionStatus.cs`

***

## 六、风险评估

### 6.1 低风险

* 新建枚举和类：不影响现有功能

* 添加 `ClearQueue` 方法：新功能

### 6.2 中风险

* 修改枚举类型：需要确保所有引用都已更新

* 删除定时器机制：需要确保事件驱动能完全替代

### 6.3 测试要点

1. 引擎切换时旧连接是否正确断开
2. 连接状态变化时UI是否正确更新
3. 自动重连是否正常工作
4. 任务队列在引擎切换时是否正确处理

***

## 七、总结

| 原问题          | 解决方案                                       |
| ------------ | ------------------------------------------ |
| 多实例并存导致状态不一致 | `TtlEngineConnectionManager` 统一管理，切换时断开旧连接 |
| 双层枚举不一致      | 统一使用Core层中文枚举                              |
| 轮询机制冗余       | 改用事件驱动，删除定时器                               |
| Core耦合UI     | 连接管理逻辑全部下沉到Core层                           |

