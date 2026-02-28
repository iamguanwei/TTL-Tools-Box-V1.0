﻿﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GW.TTLtoolsBox.Core.SystemOption.Helper;
using GW.TTLtoolsBox.Core.SystemOption.TtlEngine;
using GW.TTLtoolsBox.WinFormUi.Helper;
using static GW.TTLtoolsBox.WinFormUi.Helper.Constants;

namespace GW.TTLtoolsBox.WinFormUi.Manager
{
    public class TtlSchemeController : IDisposable
    {
        #region 常量

        #endregion

        #region public

        #region 构造函数

        public TtlSchemeController()
        {
            _connectionTimer = new System.Windows.Forms.Timer();
            _connectionTimer.Interval = 1000;
            _connectionTimer.Tick += ConnectionTimer_Tick;
        }

        #endregion

        #region 属性

        public ITtlEngineConnector CurrentEngineConnector => _currentEngineConnector;

        public ITtlEngineConnector[] EngineConnectorArray => _engineConnectorArray;

        public TtlEngineConnectionStatus ConnectionStatus => _connectionStatus;

        public bool IsInitializing => _isInitializing;

        public bool IsTtlEditing
        {
            get => _isTtlEditing;
            set => _isTtlEditing = value;
        }

        public int ConnectionCountdown => _connectionCountdown;

        public int RetryCountdown => _retryCountdown;

        #endregion

        #region 事件

        public event EventHandler<EngineChangedEventArgs> EngineChanged;

        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

        public event EventHandler<SpeakersLoadedEventArgs> SpeakersLoaded;

        #endregion

        #region 方法

        public void Initialize()
        {
            if (_engineConnectorArray == null)
            {
                LoadEngineConnectors();
            }
        }

        public void StartConnectionTimer()
        {
            if (_connectionTimer != null && !_connectionTimer.Enabled)
            {
                _connectionTimer.Start();
            }
        }

        public void StopConnectionTimer()
        {
            _connectionTimer?.Stop();
        }

        public void StartInitializing()
        {
            _isInitializing = true;
        }

        public void EndInitializing()
        {
            _isInitializing = false;
        }

        public void SelectEngine(string engineId)
        {
            string previousEngineId = _currentEngineConnector?.Id ?? None_Engine_Id;

            ITtlEngineConnector engine = null;
            if (!string.IsNullOrEmpty(engineId) && engineId != None_Engine_Id)
            {
                engine = _engineConnectorArray?.FirstOrDefault(c => c.Id == engineId);
            }

            string newEngineId = engine?.Id ?? None_Engine_Id;
            bool changed = previousEngineId != newEngineId;
            _currentEngineConnector = engine;

            _connectionStatus = TtlEngineConnectionStatus.未连接;
            _connectionCountdown = 0;
            _retryCountdown = 0;

            OnEngineChanged(newEngineId, previousEngineId);
        }

        public string GetCurrentEngineId()
        {
            return _currentEngineConnector?.Id ?? None_Engine_Id;
        }

        public async Task ConnectAsync()
        {
            if (_currentEngineConnector == null)
            {
                _connectionStatus = TtlEngineConnectionStatus.未连接;
                OnConnectionStatusChanged();
                return;
            }

            int timeoutSeconds = (int)_currentEngineConnector.ConnectionTimeout.TotalSeconds;
            if (timeoutSeconds <= 0)
            {
                timeoutSeconds = 30;
            }

            _connectionStatus = TtlEngineConnectionStatus.连接中;
            _connectionCountdown = timeoutSeconds;
            OnConnectionStatusChanged();

            try
            {
                _connectionCts?.Dispose();
                _connectionCts = new CancellationTokenSource();

                await _currentEngineConnector.ConnectAsync();

                _connectionStatus = TtlEngineConnectionStatus.连接成功;
                _retryCountdown = 连接成功_验证间隔秒数;

                if (_currentEngineConnector.Speakers == null || _currentEngineConnector.Speakers.Length == 0)
                {
                    await RefreshSpeakersAsync(_currentEngineConnector);
                }
                else
                {
                    OnSpeakersLoaded(_currentEngineConnector.Speakers, false);
                }
            }
            catch
            {
                _connectionStatus = TtlEngineConnectionStatus.连接失败;
                _retryCountdown = 连接失败_重试间隔秒数;
            }
            finally
            {
                _connectionCts?.Dispose();
                _connectionCts = null;
            }

            OnConnectionStatusChanged();
        }

        public void CancelConnection()
        {
            _connectionCts?.Cancel();
        }

        public async Task RefreshSpeakersAsync(ITtlEngineConnector engine, bool refreshFromEngine = false)
        {
            if (engine == null) return;

            if (refreshFromEngine)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        engine.RefreshSpeakers();
                        OnSpeakersLoaded(engine.Speakers, true);
                    }
                    catch
                    {
                    }
                });
            }
            else
            {
                OnSpeakersLoaded(engine.Speakers, false);
            }
        }

        public void SaveEngineParameters(string parametersText)
        {
            if (_currentEngineConnector == null) return;

            Setting.SetValue($"Params_{_currentEngineConnector.Id}", parametersText);
            var parameters = parametersText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            _currentEngineConnector.SetParameters(parameters);
        }

        public void SaveSpeakerParameters(SpeakerInfo[] speakers)
        {
            if (_currentEngineConnector == null || speakers == null) return;

            for (int i = 0; i < speakers.Length && i < _currentEngineConnector.Speakers.Length; i++)
            {
                _currentEngineConnector.Speakers[i] = speakers[i];
                Setting.SetValue($"Speaker_{_currentEngineConnector.Id}_{speakers[i].SourceName}",
                    _currentEngineConnector.Speakers[i].ToString());
            }
        }

        public void LoadSpeakerSettings(ITtlEngineConnector connector)
        {
            if (connector?.Speakers == null) return;

            foreach (var speaker in connector.Speakers)
            {
                var speakerStr = Setting.GetValue($"Speaker_{connector.Id}_{speaker.SourceName}", string.Empty);
                if (string.IsNullOrEmpty(speakerStr))
                {
                    speakerStr = Setting.GetValue($"Speaker_{connector.Name}_{speaker.SourceName}", string.Empty);
                }
                if (!string.IsNullOrEmpty(speakerStr))
                {
                    speaker.TryFromString(speakerStr);
                }
            }
        }

        public string GetEngineDisplayText(ITtlEngineConnector connector)
        {
            if (connector == null) return "无";
            return $"{connector.Name} [ID = {connector.Id}]";
        }

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
                    return _engineConnectorArray?.FirstOrDefault(c => c.Id == id);
                }
            }

            return null;
        }

        public string GetConnectionStatusText()
        {
            if (_currentEngineConnector == null)
            {
                return "未选择TTL引擎";
            }

            switch (_connectionStatus)
            {
                case TtlEngineConnectionStatus.未连接:
                    return $"{_currentEngineConnector.Name}: 未连接";
                case TtlEngineConnectionStatus.连接中:
                    return $"{_currentEngineConnector.Name}: 连接中 ({_connectionCountdown}秒)";
                case TtlEngineConnectionStatus.连接成功:
                    return $"{_currentEngineConnector.Name}: 已连接 ({_retryCountdown}秒后验证)";
                case TtlEngineConnectionStatus.连接失败:
                    return $"{_currentEngineConnector.Name}: 连接失败 ({_retryCountdown}秒后重试)";
                default:
                    return $"{_currentEngineConnector.Name}: 未连接";
            }
        }

        public void Dispose()
        {
            _connectionTimer?.Stop();
            _connectionTimer?.Dispose();
            _connectionCts?.Dispose();
        }

        #endregion

        #endregion

        #region private

        #region 字段

        private ITtlEngineConnector _currentEngineConnector = null;
        private ITtlEngineConnector[] _engineConnectorArray = null;
        private bool _isInitializing = true;
        private bool _isTtlEditing = false;

        private System.Windows.Forms.Timer _connectionTimer = null;
        private TtlEngineConnectionStatus _connectionStatus = TtlEngineConnectionStatus.未连接;
        private int _connectionCountdown = 0;
        private int _retryCountdown = 0;
        private CancellationTokenSource _connectionCts = null;

        #endregion

        #region 方法

        private void LoadEngineConnectors()
        {
            List<ITtlEngineConnector> connectors = new List<ITtlEngineConnector>();

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

                            try
                            {
                                var paramStr = Setting.GetValue($"Params_{connector.Id}", string.Empty);
                                if (string.IsNullOrEmpty(paramStr))
                                {
                                    paramStr = Setting.GetValue($"Params_{connector.Name}", string.Empty);
                                }
                                var parameters = paramStr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                                connector.SetParameters(parameters);
                            }
                            catch
                            {
                            }

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

            _engineConnectorArray = connectors.ToArray();

            Task.Run(() => LoadAllSpeakerSettings(connectors));
        }

        private void LoadAllSpeakerSettings(List<ITtlEngineConnector> connectors)
        {
            foreach (var connector in connectors)
            {
                try
                {
                    connector.RefreshSpeakers();
                    LoadSpeakerSettings(connector);
                }
                catch
                {
                }
            }
        }

        private void ConnectionTimer_Tick(object sender, EventArgs e)
        {
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
                case TtlEngineConnectionStatus.未连接:
                default:
                    break;
            }

            OnConnectionStatusChanged();
        }

        private void HandleConnectingState()
        {
            _connectionCountdown--;
            if (_connectionCountdown <= 0)
            {
                _connectionStatus = TtlEngineConnectionStatus.连接失败;
                _retryCountdown = 连接失败_重试间隔秒数;
                CancelConnection();
            }
        }

        private void HandleConnectedState()
        {
            _retryCountdown--;
            if (_retryCountdown <= 0)
            {
                _ = ConnectAsync();
            }
        }

        private void HandleFailedState()
        {
            _retryCountdown--;
            if (_retryCountdown <= 0)
            {
                _ = ConnectAsync();
            }
        }

        private void OnEngineChanged(string newEngineId, string previousEngineId)
        {
            EngineChanged?.Invoke(this, new EngineChangedEventArgs(newEngineId, previousEngineId));
        }

        private void OnConnectionStatusChanged()
        {
            ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs(
                _connectionStatus, _connectionCountdown, _retryCountdown));
        }

        private void OnSpeakersLoaded(SpeakerInfo[] speakers, bool refreshedFromEngine)
        {
            SpeakersLoaded?.Invoke(this, new SpeakersLoadedEventArgs(speakers, refreshedFromEngine));
        }

        #endregion

        #endregion

        #region 嵌套类型

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

        public class ConnectionStatusChangedEventArgs : EventArgs
        {
            public TtlEngineConnectionStatus Status { get; }
            public int ConnectionCountdown { get; }
            public int RetryCountdown { get; }

            public ConnectionStatusChangedEventArgs(
                TtlEngineConnectionStatus status,
                int connectionCountdown,
                int retryCountdown)
            {
                Status = status;
                ConnectionCountdown = connectionCountdown;
                RetryCountdown = retryCountdown;
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
