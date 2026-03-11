using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GW.TTLtoolsBox.Core.TtlEngine.Events;

namespace GW.TTLtoolsBox.Core.TtlEngine
{
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

        /// <summary>
        /// 初始化TtlEngineConnectionManager类的新实例
        /// </summary>
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

        /// <summary>
        /// 获取当前倒计时类型
        /// </summary>
        public CountdownType CurrentCountdownType => _currentCountdownType;

        /// <summary>
        /// 获取或设置是否正在初始化
        /// </summary>
        public bool IsInitializing { get; set; } = true;

        /// <summary>
        /// 获取或设置是否正在编辑TTL
        /// </summary>
        public bool IsTtlEditing { get; set; } = false;

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

            if (previousEngine != null)
            {
                await DisconnectEngineAsync(previousEngine);
            }

            ITtlEngineConnector newEngine = null;
            if (!string.IsNullOrEmpty(engineId))
            {
                newEngine = _allEngines?.FirstOrDefault(e => e.Id == engineId);
            }

            _currentEngine = newEngine;
            _connectionStatus = TtlEngineConnectionStatus.未连接;
            _reconnectCountdown = 0;

            if (_currentEngine != null)
            {
                _currentEngine.ConnectionStatusChanged += OnEngineConnectionStatusChanged;
            }

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
        /// <param name="parameters">参数数组</param>
        public void SaveEngineParameters(string[] parameters)
        {
            _currentEngine?.SetParameters(parameters);
        }

        /// <summary>
        /// 根据显示文本查找引擎
        /// </summary>
        /// <param name="displayText">显示文本</param>
        /// <returns>引擎连接器</returns>
        public ITtlEngineConnector FindEngineByDisplayText(string displayText)
        {
            if (string.IsNullOrEmpty(displayText) || displayText == "无")
            {
                return null;
            }

            int idStart = displayText.IndexOf("[ID = ");
            if (idStart >= 0)
            {
                int idEnd = displayText.IndexOf("]", idStart);
                if (idEnd > idStart)
                {
                    string id = displayText.Substring(idStart + 6, idEnd - idStart - 6);
                    return _allEngines?.FirstOrDefault(c => c.Id == id);
                }
            }

            return null;
        }

        /// <summary>
        /// 获取引擎显示文本
        /// </summary>
        /// <param name="connector">引擎连接器</param>
        /// <returns>显示文本</returns>
        public string GetEngineDisplayText(ITtlEngineConnector connector)
        {
            if (connector == null) return "无";
            return $"{connector.Name} [ID = {connector.Id}]";
        }

        /// <summary>
        /// 获取连接状态文本
        /// </summary>
        /// <returns>连接状态文本</returns>
        public string GetConnectionStatusText()
        {
            if (_currentEngine == null)
            {
                return "未选择TTL引擎";
            }

            switch (_connectionStatus)
            {
                case TtlEngineConnectionStatus.未连接:
                    return $"{_currentEngine.Name}: 未连接";
                case TtlEngineConnectionStatus.连接中:
                    return $"{_currentEngine.Name}: 连接中";
                case TtlEngineConnectionStatus.连接成功:
                    return $"{_currentEngine.Name}: 已连接";
                case TtlEngineConnectionStatus.连接失败:
                    return $"{_currentEngine.Name}: 连接失败";
                default:
                    return $"{_currentEngine.Name}: 未连接";
            }
        }

        /// <summary>
        /// 获取当前连接状态（主动获取会重置倒计时）
        /// </summary>
        /// <returns>连接状态</returns>
        public TtlEngineConnectionStatus GetConnectionStatusAndReset()
        {
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
        private CountdownType _currentCountdownType = CountdownType.无;

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
            UpdateConnectionStatus(e.Status, e.Message);

            if (e.Status == TtlEngineConnectionStatus.连接成功)
            {
                _reconnectCountdown = VerifyIntervalSeconds;
                StartVerifyTimer();
            }
            else if (e.Status == TtlEngineConnectionStatus.连接失败)
            {
                _reconnectCountdown = ReconnectIntervalSeconds;
                StartReconnectTimer();
            }
        }

        private void UpdateConnectionStatus(TtlEngineConnectionStatus status, string message = null)
        {
            _connectionStatus = status;
            OnConnectionStatusChanged(status, _reconnectCountdown, _currentCountdownType, message);
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

        private void OnConnectionStatusChanged(TtlEngineConnectionStatus status, int countdown, CountdownType countdownType, string message)
        {
            ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs(status, countdown, countdownType, message));
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

        /// <summary>
        /// 连接状态变化事件参数
        /// </summary>
        public class ConnectionStatusChangedEventArgs : EventArgs
        {
            /// <summary>
            /// 获取连接状态
            /// </summary>
            public TtlEngineConnectionStatus Status { get; }

            /// <summary>
            /// 获取倒计时
            /// </summary>
            public int Countdown { get; }

            /// <summary>
            /// 获取倒计时类型
            /// </summary>
            public CountdownType CountdownType { get; }

            /// <summary>
            /// 获取消息
            /// </summary>
            public string Message { get; }

            /// <summary>
            /// 初始化ConnectionStatusChangedEventArgs类的新实例
            /// </summary>
            /// <param name="status">连接状态</param>
            /// <param name="countdown">倒计时</param>
            /// <param name="countdownType">倒计时类型</param>
            /// <param name="message">消息</param>
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

        /// <summary>
        /// 引擎切换事件参数
        /// </summary>
        public class EngineChangedEventArgs : EventArgs
        {
            /// <summary>
            /// 获取新引擎ID
            /// </summary>
            public string NewEngineId { get; }

            /// <summary>
            /// 获取旧引擎ID
            /// </summary>
            public string PreviousEngineId { get; }

            /// <summary>
            /// 初始化EngineChangedEventArgs类的新实例
            /// </summary>
            /// <param name="newEngineId">新引擎ID</param>
            /// <param name="previousEngineId">旧引擎ID</param>
            public EngineChangedEventArgs(string newEngineId, string previousEngineId)
            {
                NewEngineId = newEngineId;
                PreviousEngineId = previousEngineId;
            }
        }

        /// <summary>
        /// 朗读者列表加载完成事件参数
        /// </summary>
        public class SpeakersLoadedEventArgs : EventArgs
        {
            /// <summary>
            /// 获取朗读者列表
            /// </summary>
            public SpeakerInfo[] Speakers { get; }

            /// <summary>
            /// 获取是否从引擎刷新
            /// </summary>
            public bool RefreshedFromEngine { get; }

            /// <summary>
            /// 初始化SpeakersLoadedEventArgs类的新实例
            /// </summary>
            /// <param name="speakers">朗读者列表</param>
            /// <param name="refreshedFromEngine">是否从引擎刷新</param>
            public SpeakersLoadedEventArgs(SpeakerInfo[] speakers, bool refreshedFromEngine)
            {
                Speakers = speakers;
                RefreshedFromEngine = refreshedFromEngine;
            }
        }

        #endregion
    }
}
