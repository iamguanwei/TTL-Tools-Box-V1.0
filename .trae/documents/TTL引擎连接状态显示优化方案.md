# TTL引擎连接状态显示优化方案（修订版）

## 一、问题分析

### 问题1：保存配置后未立即连接

**当前实现：**

* `bt_TTL方案_保存配置_Click` 方法保存参数后只刷新了UI

* 没有触发重新连接

**期望行为：**

* 保存配置后应该立即断开旧连接并重新连接

### 问题2：状态显示机制需要优化

**当前实现：**

* `TtlEngineConnectionManager` 有倒计时机制，但事件参数缺少倒计时类型

* UI层需要知道倒计时类型（重连/验证）以便扩展

**期望行为：**

* `TtlEngineConnectionManager` 内部包含自动刷新机制，每秒触发倒计时事件

* 事件参数包含倒计时剩余时长和类型（重连/验证）

* 外部调用主动获取状态时，立即刷新并重置自动刷新计时

* UI层只显示状态，不显示倒计时

### 问题3：连接成功后未刷新语音生成预处理面板

**当前实现：**

* `TtlSchemePanel.ConnectionManager_ConnectionStatusChanged` 只刷新了朗读者列表

* 没有通知语音生成预处理面板刷新

**期望行为：**

* 当状态从"未连接/连接失败"变为"连接成功"时，触发语音生成预处理面板刷新

***

## 二、修改方案

### 2.1 TtlEngineConnectionManager 增强

#### 2.1.1 新增倒计时类型枚举

```csharp
/// <summary>
/// 倒计时类型
/// </summary>
public enum CountdownType
{
    /// <summary>
    /// 无倒计时
    /// </summary>
    无,
    
    /// <summary>
    /// 重连倒计时
    /// </summary>
    重连,
    
    /// <summary>
    /// 验证倒计时
    /// </summary>
    验证
}
```

#### 2.1.2 修改 ConnectionStatusChangedEventArgs

```csharp
public class ConnectionStatusChangedEventArgs : EventArgs
{
    public TtlEngineConnectionStatus Status { get; }
    public int Countdown { get; }
    public CountdownType CountdownType { get; }
    public string Message { get; }

    public ConnectionStatusChangedEventArgs(
        TtlEngineConnectionStatus status, 
        int countdown, 
        CountdownType countdownType,
        string message)
    {
        Status = status;
        Countdown = countdown;
        CountdownType = countdownType;
        Message = message;
    }
}
```

#### 2.1.3 新增主动获取状态方法

```csharp
/// <summary>
/// 获取当前连接状态（主动获取会重置倒计时）
/// </summary>
/// <returns>连接状态</returns>
public TtlEngineConnectionStatus GetConnectionStatusAndReset()
{
    // 如果正在倒计时，重置倒计时
    if (_reconnectCountdown > 0)
    {
        if (_currentCountdownType == CountdownType.重连)
        {
            _reconnectCountdown = ReconnectIntervalSeconds;
        }
        else if (_currentCountdownType == CountdownType.验证)
        {
            _reconnectCountdown = VerifyIntervalSeconds;
        }
        OnConnectionStatusChanged(_connectionStatus, _reconnectCountdown, _currentCountdownType, null);
    }
    
    return _connectionStatus;
}

/// <summary>
/// 获取当前倒计时类型
/// </summary>
public CountdownType CurrentCountdownType => _currentCountdownType;
```

#### 2.1.4 修改倒计时回调

```csharp
private CountdownType _currentCountdownType = CountdownType.无;

private void ReconnectTimerCallback(object state)
{
    if (_reconnectCountdown > 0)
    {
        _reconnectCountdown--;
        OnConnectionStatusChanged(_connectionStatus, _reconnectCountdown, _currentCountdownType, null);
    }

    if (_reconnectCountdown <= 0)
    {
        StopReconnectTimer();

        if (_connectionStatus == TtlEngineConnectionStatus.连接失败)
        {
            _ = ConnectAsync();
        }
        else if (_connectionStatus == TtlEngineConnectionStatus.连接成功)
        {
            _ = ConnectAsync();
        }
    }
}

private void StartReconnectTimer()
{
    _reconnectCountdown = ReconnectIntervalSeconds;
    _currentCountdownType = CountdownType.重连;
    _reconnectTimer.Change(1000, 1000);
}

private void StartVerifyTimer()
{
    _reconnectCountdown = VerifyIntervalSeconds;
    _currentCountdownType = CountdownType.验证;
    _reconnectTimer.Change(1000, 1000);
}

private void StopReconnectTimer()
{
    _reconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
    _reconnectCountdown = 0;
    _currentCountdownType = CountdownType.无;
}
```

#### 2.1.5 简化 GetConnectionStatusText

```csharp
public string GetConnectionStatusText()
{
    if (_currentEngine == null)
    {
        return "未选择TTL引擎";
    }

    return _connectionStatus switch
    {
        TtlEngineConnectionStatus.未连接 => $"{_currentEngine.Name}: 未连接",
        TtlEngineConnectionStatus.连接中 => $"{_currentEngine.Name}: 连接中",
        TtlEngineConnectionStatus.连接成功 => $"{_currentEngine.Name}: 已连接",
        TtlEngineConnectionStatus.连接失败 => $"{_currentEngine.Name}: 连接失败",
        _ => $"{_currentEngine.Name}: 未连接"
    };
}
```

### 2.2 TtlSchemePanel 修改

#### 2.2.1 添加 ConnectionSucceeded 事件

```csharp
/// <summary>
/// 连接成功事件（从非成功状态变为成功状态时触发）
/// </summary>
public event EventHandler ConnectionSucceeded;
```

#### 2.2.2 修改保存配置方法

```csharp
private async void bt_TTL方案_保存配置_Click(object sender, EventArgs e)
{
    var currentEngine = ConnectionManager?.CurrentEngine;
    if (currentEngine != null)
    {
        ConnectionManager?.SaveEngineParameters(
            this.tb_TTL方案_连接参数配置.Text.Split(
                new string[] { "\r\n" }, 
                StringSplitOptions.RemoveEmptyEntries));

        if (this.dgv_TTL方案_朗读者参数配置.DataSource != null)
        {
            SpeakerInfo[] speakerArray = (SpeakerInfo[])this.dgv_TTL方案_朗读者参数配置.DataSource;
            for (int i = 0; i < speakerArray.Length && i < currentEngine.Speakers.Length; i++)
            {
                currentEngine.Speakers[i] = speakerArray[i];
            }
        }

        createSpeakersFromRoles(currentEngine);
    }

    if (ConnectionManager != null)
    {
        ConnectionManager.IsTtlEditing = false;
    }
    
    refreshTTL方案Ui();
    
    // 保存配置后立即重新连接
    if (ConnectionManager != null)
    {
        await ConnectionManager.DisconnectAsync();
        await ConnectionManager.ConnectAsync();
    }
}
```

#### 2.2.3 修改事件处理

```csharp
private void ConnectionManager_ConnectionStatusChanged(
    object sender, 
    TtlEngineConnectionManager.ConnectionStatusChangedEventArgs e)
{
    _ttlEngineConnectionStatus = e.Status;
    _ttlEngineConnectionCountdown = e.Countdown;

    updateTtlEngineConnectionStatusLabel();
    OnConnectionStatusChanged(e.Status, e.Countdown);

    // 检测状态变化：从非成功状态变为成功状态
    bool statusChangedToConnected = e.Status == TtlEngineConnectionStatus.连接成功
        && _lastConnectionStatus != TtlEngineConnectionStatus.连接成功;
    _lastConnectionStatus = e.Status;

    if (statusChangedToConnected)
    {
        var currentEngine = ConnectionManager?.CurrentEngine;
        if (currentEngine != null)
        {
            if (currentEngine.Speakers == null || currentEngine.Speakers.Length == 0)
            {
                createSpeakersFromRoles(currentEngine, true);
            }
            else
            {
                bindSpeakersToGrid(currentEngine, true);
            }
        }

        VoiceGenerationTaskQueue?.TryResume();
        
        // 触发连接成功事件
        OnConnectionSucceeded();
    }
}

protected void OnConnectionSucceeded()
{
    ConnectionSucceeded?.Invoke(this, EventArgs.Empty);
}
```

#### 2.2.4 简化状态标签更新

```csharp
private void updateTtlEngineConnectionStatusLabel()
{
    Action action = () =>
    {
        string statusText = ConnectionManager?.GetConnectionStatusText() ?? "未选择TTL引擎";
        var status = ConnectionManager?.ConnectionStatus ?? TtlEngineConnectionStatus.未连接;
        Color statusColor = status switch
        {
            TtlEngineConnectionStatus.连接成功 => Color.FromArgb(0, 150, 0),
            TtlEngineConnectionStatus.连接失败 => Color.FromArgb(200, 0, 0),
            _ => SystemColors.ControlText
        };

        if (ConnectionStatusLabel != null)
        {
            ConnectionStatusLabel.Text = statusText;
            ConnectionStatusLabel.ForeColor = statusColor;
        }
    };

    UpdateUi(action);
}
```

### 2.3 MainForm 修改

#### 2.3.1 简化状态标签更新

```csharp
private void updateTtlEngineConnectionStatusLabel()
{
    Action action = () =>
    {
        this.tssl_TTL连接状态显示.Text = 
            _connectionManager?.GetConnectionStatusText() ?? "未选择TTL引擎";
        
        var status = _connectionManager?.ConnectionStatus ?? TtlEngineConnectionStatus.未连接;
        Color statusColor = status switch
        {
            TtlEngineConnectionStatus.连接成功 => Color.FromArgb(0, 150, 0),
            TtlEngineConnectionStatus.连接失败 => Color.FromArgb(200, 0, 0),
            _ => SystemColors.ControlText
        };
        this.tssl_TTL连接状态显示.ForeColor = statusColor;
    };

    UiHelper.UpdateUi(this, action);
}
```

#### 2.3.2 订阅 ConnectionSucceeded 事件

```csharp
private void initTTL方案Panel()
{
    _ttlSchemePanel = new TtlSchemePanel();
    _ttlSchemePanel.Dock = DockStyle.Fill;
    _ttlSchemePanel.ConnectionManager = _connectionManager;
    _ttlSchemePanel.PreviewVoiceManager = _previewVoiceManager;
    _ttlSchemePanel.ConnectionStatusLabel = this.tssl_TTL连接状态显示;

    // 现有事件订阅...
    
    // 新增：订阅连接成功事件
    _ttlSchemePanel.ConnectionSucceeded += (s, e) =>
    {
        // 刷新语音生成预处理面板
        _voicePreprocessPanel?.RefreshUi();
    };

    this.tp_TTL方案.Controls.Clear();
    this.tp_TTL方案.Controls.Add(_ttlSchemePanel);

    _ttlSchemePanel.InitializePanel();
}
```

***

## 三、修改文件清单

| 文件                              | 修改内容                                                                                                                                      |
| ------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- |
| `TtlEngineConnectionManager.cs` | 1. 新增 `CountdownType` 枚举2. 修改 `ConnectionStatusChangedEventArgs`3. 新增 `GetConnectionStatusAndReset()` 方法4. 简化 `GetConnectionStatusText()` |
| `TtlSchemePanel.cs`             | 1. 添加 `ConnectionSucceeded` 事件2. 保存配置后重新连接3. 简化状态显示                                                                                       |
| `MainForm.cs`                   | 1. 简化状态标签更新2. 订阅 `ConnectionSucceeded` 事件                                                                                                 |

***

## 四、实施步骤

1. 在 `TtlEngineConnectionManager.cs` 新增 `CountdownType` 枚举
2. 修改 `ConnectionStatusChangedEventArgs` 添加 `CountdownType` 属性
3. 修改 `TtlEngineConnectionManager` 内部倒计时逻辑，记录当前倒计时类型
4. 新增 `GetConnectionStatusAndReset()` 方法
5. 简化 `GetConnectionStatusText()` 方法
6. 修改 `TtlSchemePanel.bt_TTL方案_保存配置_Click()` 添加重新连接逻辑
7. 在 `TtlSchemePanel` 添加 `ConnectionSucceeded` 事件
8. 修改 `TtlSchemePanel.ConnectionManager_ConnectionStatusChanged()` 触发事件
9. 简化 `TtlSchemePanel.updateTtlEngineConnectionStatusLabel()`
10. 简化 `MainForm.updateTtlEngineConnectionStatusLabel()`
11. 在 `MainForm.initTTL方案Panel()` 订阅事件

***

## 五、验证要点

1. 保存配置后是否立即重新连接
2. 状态标签是否只显示状态，不显示倒计时
3. 倒计时事件是否包含类型信息
4. 连接成功后语音生成预处理面板是否自动刷新
5. 主动获取状态是否重置倒计时

