using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GW.TTLtoolsBox.Core.PolyReplace;
using GW.TTLtoolsBox.Core.SystemOption.TtlEngine.Events;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine
{
    /// <summary>
    /// TTL引擎连接器抽象基类，实现了ITtlEngineConnector接口的大部分功能
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 连接管理：支持异步连接和断开TTL引擎
    /// - 文本发送：支持异步发送文本到TTL引擎并获取音频文件
    /// - 任务队列：支持任务排队和并发控制
    /// - 状态监控：提供连接状态变化和任务进度事件
    /// - 自动重连：支持连接断开后的自动重连
    /// - 多音字替换：支持文本的多音字替换
    /// 
    /// 使用场景：
    /// - 作为不同类型TTL引擎连接器的基类
    /// - 提供统一的连接和操作接口
    /// 
    /// 依赖关系：
    /// - 依赖HttpClient进行HTTP请求
    /// - 依赖PolyReplace命名空间进行多音字替换
    /// - 依赖Events命名空间进行事件定义
    /// </remarks>
    public abstract class ATtlEngineConnector : ITtlEngineConnector, IDisposable
    {
        #region public

        #region 构造函数

        /// <summary>
        /// 初始化ATtlEngineConnector类的新实例
        /// </summary>
        public ATtlEngineConnector()
        {
        }

        #endregion

        #region ITtlEngineConnector 实现

        /// <summary>
        /// 获取引擎的唯一标识符
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// 获取保存的多个参数
        /// </summary>
        public string[] Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// 获取各个参数的意义和格式说明
        /// </summary>
        public abstract string[] ParameterDescriptions { get; }

        /// <summary>
        /// 获取或设置连接超时时间
        /// </summary>
        /// <value>默认值为30秒</value>
        public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// 获取或设置请求超时时间
        /// </summary>
        /// <value>默认值为10分钟</value>
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// 获取或设置是否启用多音字替换
        /// </summary>
        /// <value>默认值为true</value>
        public bool EnablePolyphonicReplace { get; set; } = true;

        /// <summary>
        /// 获取当前引擎所使用的多音字方案
        /// </summary>
        public abstract IPolyphonicScheme PolyphonicScheme { get; }

        /// <summary>
        /// 获取连接器的名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 获取连接器的说明或详情
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 异步连接到TTL引擎
        /// </summary>
        /// <returns>表示异步操作的任务</returns>
        /// <exception cref="Exception">连接失败时抛出异常</exception>
        public abstract Task ConnectAsync();

        /// <summary>
        /// 异步断开与TTL引擎的连接
        /// </summary>
        /// <returns>表示异步操作的任务</returns>
        /// <exception cref="Exception">断开连接失败时抛出异常</exception>
        public abstract Task DisconnectAsync();

        /// <summary>
        /// 异步发送文本到TTL引擎
        /// </summary>
        /// <param name="text">要发送的文本</param>
        /// <param name="parameters">TTL引擎参数，可为null</param>
        /// <returns>返回音频文件的保存路径</returns>
        /// <exception cref="Exception">发送失败时抛出异常</exception>
        public async Task<string> SendTextAsync(string text, TtlEngineParameters parameters = null)
        {
            if (parameters == null)
            {
                parameters = new TtlEngineParameters(text);
            }
            else if (string.IsNullOrEmpty(parameters.Text))
            {
                parameters.Text = text;
            }

            // 处理多音字替换
            if (EnablePolyphonicReplace && PolyphonicScheme != null && !string.IsNullOrEmpty(parameters.Text))
            {
                parameters.Text = PolyphonicScheme.Replace(parameters.Text);
            }

            var task = new TtlEngineTask(parameters);
            _taskQueue.Enqueue(task);

            int queuePosition = _taskQueue.Count;
            OnTaskQueued(task.TaskId, queuePosition, queuePosition);

            await ProcessTasksAsync();
            return await task.TaskCompletionSource.Task;
        }

        /// <summary>
        /// 获取当前连接状态
        /// </summary>
        /// <returns>当前连接状态</returns>
        public ConnectionStatus GetConnectionStatus()
        {
            return _connectionStatus;
        }

        /// <summary>
        /// 获取当前队列中的任务数
        /// </summary>
        /// <returns>队列中的任务数</returns>
        public int GetQueueCount()
        {
            return _taskQueue.Count;
        }

        /// <summary>
        /// 启动自动重连
        /// </summary>
        /// <param name="intervalSeconds">重连间隔（秒）</param>
        public void StartAutoReconnect(int intervalSeconds)
        {
            _reconnectIntervalSeconds = intervalSeconds;
            _reconnectAttemptCount = 0;
            _remainingReconnectSeconds = intervalSeconds;

            StopAutoReconnect();

            _reconnectCountdownTimer = new Timer(ReconnectCountdownCallback, null, 1000, 1000);
        }

        /// <summary>
        /// 停止自动重连
        /// </summary>
        public void StopAutoReconnect()
        {
            if (_reconnectCountdownTimer != null)
            {
                _reconnectCountdownTimer.Dispose();
                _reconnectCountdownTimer = null;
            }
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="parameters">要设置的参数数组</param>
        public void SetParameters(string[] parameters)
        {
            // 检查参数是否合法
            ValidateParameters(parameters);
            
            // 设置参数
            _parameters = parameters;
            OnConnectionStatusChanged(ConnectionStatus.Disconnected, "Parameters changed");
        }

        /// <summary>
        /// 获取角色名单
        /// </summary>
        public GW.TTLtoolsBox.Core.SystemOption.TtlEngine.SpeakerInfo[] Speakers
        {
            get { return _roles ?? new GW.TTLtoolsBox.Core.SystemOption.TtlEngine.SpeakerInfo[0]; }
            protected set { _roles = value; }
        }

        /// <summary>
        /// 获取引擎支持的特性定义列表
        /// </summary>
        /// <remarks>
        /// 默认返回空数组，子类可以重写此属性以提供特性定义。
        /// </remarks>
        public virtual TtlEngineFeatureDefinition[] FeatureDefinitions
        {
            get { return Array.Empty<TtlEngineFeatureDefinition>(); }
        }

        /// <summary>
        /// 刷新角色名单
        /// </summary>
        public abstract void RefreshSpeakers();

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 进度变化事件，每秒触发一次
        /// </summary>
        public event EventHandler<TtlEngineProgressEventArgs> ProgressChanged;

        /// <summary>
        /// 任务完成事件
        /// </summary>
        public event EventHandler<TtlEngineCompletedEventArgs> Completed;

        /// <summary>
        /// 任务排队事件
        /// </summary>
        public event EventHandler<TtlEngineQueuedEventArgs> TaskQueued;

        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        public event EventHandler<TtlEngineConnectionEventArgs> ConnectionStatusChanged;

        /// <summary>
        /// 重连倒秒事件
        /// </summary>
        public event EventHandler<TtlEngineReconnectEventArgs> ReconnectCountdown;

        #endregion

        #endregion

        #region protected

        #region 构造函数

        #endregion

        #region 方法

        /// <summary>
        /// 处理任务队列
        /// </summary>
        /// <returns>表示异步操作的任务</returns>
        protected virtual async Task ProcessTasksAsync()
        {
            if (!await _semaphore.WaitAsync(0))
            {
                return;
            }

            try
            {
                while (_taskQueue.TryDequeue(out var task))
                {
                    await ProcessTaskAsync(task);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// 处理单个任务
        /// </summary>
        /// <param name="task">要处理的任务</param>
        /// <returns>表示异步操作的任务</returns>
        protected virtual async Task ProcessTaskAsync(TtlEngineTask task)
        {
            string audioFilePath = null;
            bool success = false;
            string errorMessage = null;

            try
            {
                // 确保连接状态
                await EnsureConnectionAsync();

                // 执行任务并获取音频文件路径
                audioFilePath = await ExecuteTaskWithProgressAsync(task);
                success = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                HandleTaskError(ex);
            }
            finally
            {
                CompleteTask(task, audioFilePath, success, errorMessage);
            }
        }

        /// <summary>
        /// 确保连接状态
        /// </summary>
        /// <returns>表示异步操作的任务</returns>
        /// <exception cref="Exception">连接失败时抛出异常</exception>
        protected virtual async Task EnsureConnectionAsync()
        {
            if (_connectionStatus != ConnectionStatus.Connected)
            {
                await ConnectAsync();
                if (_connectionStatus != ConnectionStatus.Connected)
                {
                    throw new Exception("Failed to connect to TTL engine");
                }
            }
        }

        /// <summary>
        /// 执行任务并报告进度
        /// </summary>
        /// <param name="task">要执行的任务</param>
        /// <returns>音频文件路径</returns>
        protected virtual async Task<string> ExecuteTaskWithProgressAsync(TtlEngineTask task)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // 创建进度报告定时器
            var timer = new Timer(_ =>
            {
                int elapsedSeconds = (int)stopwatch.Elapsed.TotalSeconds;
                OnProgressChanged(elapsedSeconds, task.TaskId, 0);
            }, null, 1000, 1000);

            try
            {
                // 发送文本并获取音频
                return await ExecuteTaskAsync(task.Parameters);
            }
            finally
            {
                timer.Dispose();
                stopwatch.Stop();
            }
        }

        /// <summary>
        /// 处理任务错误
        /// </summary>
        /// <param name="ex">异常信息</param>
        protected virtual void HandleTaskError(Exception ex)
        {
            _connectionStatus = ConnectionStatus.Failed;
            OnConnectionStatusChanged(ConnectionStatus.Failed, $"Error processing task: {ex.Message}", ex);
        }

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="task">任务对象</param>
        /// <param name="audioFilePath">音频文件路径</param>
        /// <param name="success">是否成功</param>
        /// <param name="errorMessage">错误信息</param>
        protected virtual void CompleteTask(TtlEngineTask task, string audioFilePath, bool success, string errorMessage)
        {
            task.TaskCompletionSource.SetResult(audioFilePath);
            OnCompleted(audioFilePath, success, errorMessage, task.TaskId);
        }



        /// <summary>
        /// 保存音频数据到临时文件
        /// </summary>
        /// <param name="audioData">音频数据</param>
        /// <returns>临时文件路径</returns>
        protected virtual async Task<string> SaveAudioDataAsync(byte[] audioData)
        {
            var tempFilePath = Path.GetTempFileName() + ".wav";
            // 使用异步写入文件
            await Task.Run(() => File.WriteAllBytes(tempFilePath, audioData));
            return tempFilePath;
        }

        /// <summary>
        /// 构建请求URL
        /// </summary>
        /// <param name="parameters">TTL引擎参数</param>
        /// <returns>构建好的请求URL</returns>
        protected abstract string BuildRequestUrl(TtlEngineParameters parameters);

        /// <summary>
        /// 执行任务并获取音频文件路径
        /// </summary>
        /// <param name="parameters">TTL引擎参数</param>
        /// <returns>音频文件路径</returns>
        protected abstract Task<string> ExecuteTaskAsync(TtlEngineParameters parameters);

        /// <summary>
        /// 验证参数是否合法
        /// </summary>
        /// <param name="parameters">要验证的参数数组</param>
        protected abstract void ValidateParameters(string[] parameters);

        /// <summary>
        /// 触发进度变化事件
        /// </summary>
        /// <param name="elapsedTime">自调用开始经过的时间（秒）</param>
        /// <param name="taskId">当前任务ID</param>
        /// <param name="queuePosition">当前任务在队列中的位置</param>
        protected virtual void OnProgressChanged(int elapsedTime, Guid taskId, int queuePosition)
        {
            ProgressChanged?.Invoke(this, new TtlEngineProgressEventArgs(elapsedTime, taskId, queuePosition));
        }

        /// <summary>
        /// 触发任务完成事件
        /// </summary>
        /// <param name="audioFilePath">音频文件的保存位置</param>
        /// <param name="success">是否成功</param>
        /// <param name="errorMessage">错误信息（如果失败）</param>
        /// <param name="taskId">完成的任务ID</param>
        protected virtual void OnCompleted(string audioFilePath, bool success, string errorMessage, Guid taskId)
        {
            Completed?.Invoke(this, new TtlEngineCompletedEventArgs(audioFilePath, success, errorMessage, taskId));
        }

        /// <summary>
        /// 触发任务排队事件
        /// </summary>
        /// <param name="taskId">排队的任务ID</param>
        /// <param name="queuePosition">任务在队列中的位置</param>
        /// <param name="queueCount">当前队列总数</param>
        protected virtual void OnTaskQueued(Guid taskId, int queuePosition, int queueCount)
        {
            TaskQueued?.Invoke(this, new TtlEngineQueuedEventArgs(taskId, queuePosition, queueCount));
        }

        /// <summary>
        /// 触发连接状态变化事件
        /// </summary>
        /// <param name="status">连接状态</param>
        /// <param name="message">状态消息</param>
        /// <param name="error">错误信息（如果有）</param>
        protected virtual void OnConnectionStatusChanged(ConnectionStatus status, string message, Exception error = null)
        {
            _connectionStatus = status;
            ConnectionStatusChanged?.Invoke(this, new TtlEngineConnectionEventArgs(status, message, error));
        }

        /// <summary>
        /// 触发重连倒秒事件
        /// </summary>
        /// <param name="remainingSeconds">剩余重连等待时间（秒）</param>
        /// <param name="attemptCount">重连尝试次数</param>
        protected virtual void OnReconnectCountdown(int remainingSeconds, int attemptCount)
        {
            ReconnectCountdown?.Invoke(this, new TtlEngineReconnectEventArgs(remainingSeconds, attemptCount));
        }

        /// <summary>
        /// 重连倒秒回调
        /// </summary>
        /// <param name="state">状态对象</param>
        protected virtual async void ReconnectCountdownCallback(object state)
        {
            _remainingReconnectSeconds--;
            OnReconnectCountdown(_remainingReconnectSeconds, _reconnectAttemptCount);

            if (_remainingReconnectSeconds <= 0)
            {
                _reconnectAttemptCount++;
                _remainingReconnectSeconds = _reconnectIntervalSeconds;

                await ConnectAsync();
                if (_connectionStatus == ConnectionStatus.Connected)
                {
                    StopAutoReconnect();
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopAutoReconnect();
                _semaphore.Dispose();
            }
        }

        #endregion

        #endregion

        #region private

        /// <summary>
        /// 保存的多个参数
        /// </summary>
        private string[] _parameters;

        /// <summary>
        /// 连接状态
        /// </summary>
        private ConnectionStatus _connectionStatus = ConnectionStatus.Disconnected;

        /// <summary>
        /// 任务队列
        /// </summary>
        private readonly ConcurrentQueue<TtlEngineTask> _taskQueue = new ConcurrentQueue<TtlEngineTask>();

        /// <summary>
        /// 信号量，用于控制并发
        /// </summary>
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 重连间隔（秒）
        /// </summary>
        private int _reconnectIntervalSeconds;

        /// <summary>
        /// 重连尝试次数
        /// </summary>
        private int _reconnectAttemptCount;

        /// <summary>
        /// 剩余重连等待时间（秒）
        /// </summary>
        private int _remainingReconnectSeconds;

        /// <summary>
        /// 重连倒秒定时器
        /// </summary>
        private Timer _reconnectCountdownTimer;

        /// <summary>
        /// 角色名单
        /// </summary>
        private GW.TTLtoolsBox.Core.SystemOption.TtlEngine.SpeakerInfo[] _roles;

        #endregion
    }
}