using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine
{
    /// <summary>
    /// 预览声音状态枚举
    /// </summary>
    public enum PreviewVoiceStatus
    {
        /// <summary>
        /// 未生成
        /// </summary>
        未生成,

        /// <summary>
        /// 生成中
        /// </summary>
        生成中,

        /// <summary>
        /// 已生成
        /// </summary>
        已生成,

        /// <summary>
        /// 生成失败
        /// </summary>
        生成失败
    }

    /// <summary>
    /// 预览声音任务信息类
    /// </summary>
    public class PreviewVoiceTask
    {
        /// <summary>
        /// 任务唯一标识
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// TTL引擎ID
        /// </summary>
        public string EngineId { get; set; }

        /// <summary>
        /// 朗读者源名称
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// 生成文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 输出文件路径
        /// </summary>
        public string OutputFilePath { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public PreviewVoiceStatus Status { get; set; }

        /// <summary>
        /// 任务完成事件源
        /// </summary>
        public TaskCompletionSource<string> TaskCompletionSource { get; set; }
    }

    /// <summary>
    /// 预览声音管理器，负责管理预览声音的生成、状态跟踪和文件管理。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 管理预览声音文件的路径和状态
    /// - 通过临时文件标记生成状态
    /// - 提供预览任务的创建和管理
    /// - 监控生成结果并更新状态
    /// 
    /// 使用场景：
    /// - TTL方案朗读者表格的声音预览
    /// - 角色映射表格的声音预览
    /// 
    /// 依赖关系：
    /// - 依赖ITtlEngineConnector进行语音生成
    /// - 依赖文件系统进行状态持久化
    /// </remarks>
    public class PreviewVoiceManager : IDisposable
    {
        #region public

        #region 构造函数

        /// <summary>
        /// 初始化PreviewVoiceManager类的新实例。
        /// </summary>
        /// <param name="previewFolder">预览声音文件夹路径</param>
        /// <param name="previewTextFile">预览文本文件路径</param>
        /// <param name="defaultPreviewText">默认预览文本</param>
        public PreviewVoiceManager(string previewFolder, string previewTextFile, string defaultPreviewText)
        {
            _previewFolder = previewFolder;
            _previewTextFile = previewTextFile;
            _defaultPreviewText = defaultPreviewText;

            if (!Directory.Exists(_previewFolder))
            {
                Directory.CreateDirectory(_previewFolder);
            }

            _previewStatusCache = new ConcurrentDictionary<string, PreviewVoiceStatus>();
            _previewTaskQueue = new ConcurrentQueue<PreviewVoiceTask>();
            _previewTasks = new ConcurrentDictionary<Guid, PreviewVoiceTask>();

            startStatusMonitor();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取当前正在执行的预览任务。
        /// </summary>
        public PreviewVoiceTask CurrentTask
        {
            get { return _currentTask; }
        }

        /// <summary>
        /// 获取是否有预览任务正在执行。
        /// </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 获取预览声音文件路径。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <param name="sourceName">朗读者源名称</param>
        /// <returns>预览声音文件路径</returns>
        public string GetPreviewFilePath(string engineId, string sourceName)
        {
            if (string.IsNullOrWhiteSpace(engineId) || string.IsNullOrWhiteSpace(sourceName))
            {
                return null;
            }

            string safeSourceName = getSafeFileName(sourceName);
            return Path.Combine(_previewFolder, $"{engineId}_{safeSourceName}.mp3");
        }

        /// <summary>
        /// 获取预览状态标记文件路径。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <param name="sourceName">朗读者源名称</param>
        /// <returns>状态标记文件路径</returns>
        public string GetStatusFilePath(string engineId, string sourceName)
        {
            if (string.IsNullOrWhiteSpace(engineId) || string.IsNullOrWhiteSpace(sourceName))
            {
                return null;
            }

            string safeSourceName = getSafeFileName(sourceName);
            return Path.Combine(_previewFolder, $"{engineId}_{safeSourceName}.generating");
        }

        /// <summary>
        /// 获取预览声音状态。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <param name="sourceName">朗读者源名称</param>
        /// <returns>预览声音状态</returns>
        public PreviewVoiceStatus GetStatus(string engineId, string sourceName)
        {
            if (string.IsNullOrWhiteSpace(engineId) || string.IsNullOrWhiteSpace(sourceName))
            {
                return PreviewVoiceStatus.未生成;
            }

            string cacheKey = $"{engineId}_{sourceName}";

            if (_previewStatusCache.TryGetValue(cacheKey, out var cachedStatus))
            {
                return cachedStatus;
            }

            PreviewVoiceStatus status = calculateStatus(engineId, sourceName);
            _previewStatusCache[cacheKey] = status;
            return status;
        }

        /// <summary>
        /// 刷新预览声音状态。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <param name="sourceName">朗读者源名称</param>
        /// <returns>刷新后的状态</returns>
        public PreviewVoiceStatus RefreshStatus(string engineId, string sourceName)
        {
            if (string.IsNullOrWhiteSpace(engineId) || string.IsNullOrWhiteSpace(sourceName))
            {
                return PreviewVoiceStatus.未生成;
            }

            string cacheKey = $"{engineId}_{sourceName}";
            PreviewVoiceStatus status = calculateStatus(engineId, sourceName);
            _previewStatusCache[cacheKey] = status;
            return status;
        }

        /// <summary>
        /// 设置预览声音状态（用于状态更新）。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <param name="sourceName">朗读者源名称</param>
        /// <param name="status">新状态</param>
        public void SetStatus(string engineId, string sourceName, PreviewVoiceStatus status)
        {
            if (string.IsNullOrWhiteSpace(engineId) || string.IsNullOrWhiteSpace(sourceName))
            {
                return;
            }

            string cacheKey = $"{engineId}_{sourceName}";
            _previewStatusCache[cacheKey] = status;

            string statusFilePath = GetStatusFilePath(engineId, sourceName);
            string previewFilePath = GetPreviewFilePath(engineId, sourceName);

            switch (status)
            {
                case PreviewVoiceStatus.生成中:
                    if (!string.IsNullOrWhiteSpace(statusFilePath))
                    {
                        try
                        {
                            File.WriteAllText(statusFilePath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        catch { }
                    }
                    break;

                case PreviewVoiceStatus.已生成:
                    if (!string.IsNullOrWhiteSpace(statusFilePath) && File.Exists(statusFilePath))
                    {
                        try { File.Delete(statusFilePath); } catch { }
                    }
                    break;

                case PreviewVoiceStatus.未生成:
                case PreviewVoiceStatus.生成失败:
                    if (!string.IsNullOrWhiteSpace(statusFilePath) && File.Exists(statusFilePath))
                    {
                        try { File.Delete(statusFilePath); } catch { }
                    }
                    break;
            }

            OnStatusChanged(engineId, sourceName, status);
        }

        /// <summary>
        /// 获取预览文本。
        /// </summary>
        /// <returns>预览文本</returns>
        public string GetPreviewText()
        {
            if (!string.IsNullOrWhiteSpace(_previewTextFile) && File.Exists(_previewTextFile))
            {
                try
                {
                    string text = File.ReadAllText(_previewTextFile);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        return text;
                    }
                }
                catch { }
            }

            return _defaultPreviewText;
        }

        /// <summary>
        /// 创建预览任务。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <param name="sourceName">朗读者源名称</param>
        /// <returns>预览任务</returns>
        public PreviewVoiceTask CreateTask(string engineId, string sourceName)
        {
            if (string.IsNullOrWhiteSpace(engineId) || string.IsNullOrWhiteSpace(sourceName))
            {
                return null;
            }

            PreviewVoiceTask task = new PreviewVoiceTask
            {
                TaskId = Guid.NewGuid(),
                EngineId = engineId,
                SourceName = sourceName,
                Text = GetPreviewText(),
                OutputFilePath = GetPreviewFilePath(engineId, sourceName),
                Status = PreviewVoiceStatus.未生成,
                TaskCompletionSource = new TaskCompletionSource<string>()
            };

            return task;
        }

        /// <summary>
        /// 将预览任务加入队列（插入到队列顶端）。
        /// </summary>
        /// <param name="task">预览任务</param>
        public void EnqueuePriorityTask(PreviewVoiceTask task)
        {
            if (task == null)
            {
                return;
            }

            task.Status = PreviewVoiceStatus.生成中;
            _previewTasks[task.TaskId] = task;

            List<PreviewVoiceTask> existingTasks = new List<PreviewVoiceTask>();
            while (_previewTaskQueue.TryDequeue(out var existingTask))
            {
                existingTasks.Add(existingTask);
            }

            _previewTaskQueue.Enqueue(task);

            foreach (var existingTask in existingTasks)
            {
                _previewTaskQueue.Enqueue(existingTask);
            }

            SetStatus(task.EngineId, task.SourceName, PreviewVoiceStatus.生成中);
            OnTaskQueued(task);
        }

        /// <summary>
        /// 尝试获取下一个预览任务。
        /// </summary>
        /// <param name="task">输出的预览任务</param>
        /// <returns>是否成功获取</returns>
        public bool TryDequeueTask(out PreviewVoiceTask task)
        {
            return _previewTaskQueue.TryDequeue(out task);
        }

        /// <summary>
        /// 获取队列中的任务数量。
        /// </summary>
        /// <returns>任务数量</returns>
        public int GetQueueCount()
        {
            return _previewTaskQueue.Count;
        }

        /// <summary>
        /// 完成预览任务。
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="success">是否成功</param>
        /// <param name="filePath">生成的文件路径</param>
        public void CompleteTask(Guid taskId, bool success, string filePath)
        {
            if (_previewTasks.TryRemove(taskId, out var task))
            {
                if (success)
                {
                    task.Status = PreviewVoiceStatus.已生成;
                    task.TaskCompletionSource.SetResult(filePath);
                    SetStatus(task.EngineId, task.SourceName, PreviewVoiceStatus.已生成);
                }
                else
                {
                    task.Status = PreviewVoiceStatus.生成失败;
                    task.TaskCompletionSource.SetResult(null);
                    SetStatus(task.EngineId, task.SourceName, PreviewVoiceStatus.生成失败);
                }

                OnTaskCompleted(task, success);
            }
        }

        /// <summary>
        /// 开始执行预览任务。
        /// </summary>
        /// <param name="task">要执行的任务</param>
        public void StartTask(PreviewVoiceTask task)
        {
            if (task == null)
            {
                return;
            }

            _currentTask = task;
            _isRunning = true;
            task.Status = PreviewVoiceStatus.生成中;
            SetStatus(task.EngineId, task.SourceName, PreviewVoiceStatus.生成中);
        }

        /// <summary>
        /// 结束当前预览任务执行。
        /// </summary>
        public void EndCurrentTask()
        {
            _currentTask = null;
            _isRunning = false;
        }

        /// <summary>
        /// 清除指定引擎的所有预览状态缓存。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        public void ClearCacheForEngine(string engineId)
        {
            if (string.IsNullOrWhiteSpace(engineId))
            {
                return;
            }

            List<string> keysToRemove = _previewStatusCache.Keys
                .Where(k => k.StartsWith(engineId + "_"))
                .ToList();

            foreach (var key in keysToRemove)
            {
                _previewStatusCache.TryRemove(key, out _);
            }
        }

        /// <summary>
        /// 重置指定预览声音的状态（删除标记文件）。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <param name="sourceName">朗读者源名称</param>
        public void ResetStatus(string engineId, string sourceName)
        {
            string statusFilePath = GetStatusFilePath(engineId, sourceName);
            if (!string.IsNullOrWhiteSpace(statusFilePath) && File.Exists(statusFilePath))
            {
                try { File.Delete(statusFilePath); } catch { }
            }

            string cacheKey = $"{engineId}_{sourceName}";
            _previewStatusCache.TryRemove(cacheKey, out _);

            RefreshStatus(engineId, sourceName);
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 状态变化事件
        /// </summary>
        public event EventHandler<PreviewVoiceStatusEventArgs> StatusChanged;

        /// <summary>
        /// 任务排队事件
        /// </summary>
        public event EventHandler<PreviewVoiceTaskEventArgs> TaskQueued;

        /// <summary>
        /// 任务完成事件
        /// </summary>
        public event EventHandler<PreviewVoiceTaskCompletedEventArgs> TaskCompleted;

        #endregion

        #endregion

        #region protected

        #region 方法

        /// <summary>
        /// 触发状态变化事件。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <param name="sourceName">朗读者源名称</param>
        /// <param name="status">新状态</param>
        protected virtual void OnStatusChanged(string engineId, string sourceName, PreviewVoiceStatus status)
        {
            StatusChanged?.Invoke(this, new PreviewVoiceStatusEventArgs(engineId, sourceName, status));
        }

        /// <summary>
        /// 触发任务排队事件。
        /// </summary>
        /// <param name="task">预览任务</param>
        protected virtual void OnTaskQueued(PreviewVoiceTask task)
        {
            TaskQueued?.Invoke(this, new PreviewVoiceTaskEventArgs(task));
        }

        /// <summary>
        /// 触发任务完成事件。
        /// </summary>
        /// <param name="task">预览任务</param>
        /// <param name="success">是否成功</param>
        protected virtual void OnTaskCompleted(PreviewVoiceTask task, bool success)
        {
            TaskCompleted?.Invoke(this, new PreviewVoiceTaskCompletedEventArgs(task, success));
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_statusMonitorTimer != null)
                {
                    _statusMonitorTimer.Dispose();
                    _statusMonitorTimer = null;
                }
            }
        }

        #endregion

        #endregion

        #region private

        #region 字段

        /// <summary>
        /// 预览声音文件夹路径
        /// </summary>
        private readonly string _previewFolder;

        /// <summary>
        /// 预览文本文件路径
        /// </summary>
        private readonly string _previewTextFile;

        /// <summary>
        /// 默认预览文本
        /// </summary>
        private readonly string _defaultPreviewText;

        /// <summary>
        /// 预览状态缓存
        /// </summary>
        private readonly ConcurrentDictionary<string, PreviewVoiceStatus> _previewStatusCache;

        /// <summary>
        /// 预览任务队列
        /// </summary>
        private readonly ConcurrentQueue<PreviewVoiceTask> _previewTaskQueue;

        /// <summary>
        /// 所有预览任务字典
        /// </summary>
        private readonly ConcurrentDictionary<Guid, PreviewVoiceTask> _previewTasks;

        /// <summary>
        /// 当前正在执行的任务
        /// </summary>
        private PreviewVoiceTask _currentTask;

        /// <summary>
        /// 是否正在执行任务
        /// </summary>
        private bool _isRunning;

        /// <summary>
        /// 状态监控定时器
        /// </summary>
        private Timer _statusMonitorTimer;

        #endregion

        #region 方法

        /// <summary>
        /// 计算预览声音状态。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <param name="sourceName">朗读者源名称</param>
        /// <returns>预览声音状态</returns>
        private PreviewVoiceStatus calculateStatus(string engineId, string sourceName)
        {
            string previewFilePath = GetPreviewFilePath(engineId, sourceName);
            string statusFilePath = GetStatusFilePath(engineId, sourceName);

            if (!string.IsNullOrWhiteSpace(previewFilePath) && File.Exists(previewFilePath))
            {
                return PreviewVoiceStatus.已生成;
            }

            if (!string.IsNullOrWhiteSpace(statusFilePath) && File.Exists(statusFilePath))
            {
                return PreviewVoiceStatus.生成中;
            }

            return PreviewVoiceStatus.未生成;
        }

        /// <summary>
        /// 获取安全的文件名（移除不允许的字符）。
        /// </summary>
        /// <param name="name">原始名称</param>
        /// <returns>安全的文件名</returns>
        private string getSafeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", name.Split(invalidChars));
        }

        /// <summary>
        /// 启动状态监控。
        /// </summary>
        private void startStatusMonitor()
        {
            _statusMonitorTimer = new Timer(statusMonitorCallback, null, 1000, 1000);
        }

        /// <summary>
        /// 状态监控回调。
        /// </summary>
        /// <param name="state">状态对象</param>
        private void statusMonitorCallback(object state)
        {
            if (_currentTask == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(_currentTask.OutputFilePath) && File.Exists(_currentTask.OutputFilePath))
            {
                CompleteTask(_currentTask.TaskId, true, _currentTask.OutputFilePath);
                EndCurrentTask();
            }
        }

        #endregion

        #endregion
    }

    #region 事件参数类

    /// <summary>
    /// 预览声音状态事件参数类
    /// </summary>
    public class PreviewVoiceStatusEventArgs : EventArgs
    {
        /// <summary>
        /// 获取TTL引擎ID
        /// </summary>
        public string EngineId { get; }

        /// <summary>
        /// 获取朗读者源名称
        /// </summary>
        public string SourceName { get; }

        /// <summary>
        /// 获取新状态
        /// </summary>
        public PreviewVoiceStatus Status { get; }

        /// <summary>
        /// 初始化PreviewVoiceStatusEventArgs类的新实例
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <param name="sourceName">朗读者源名称</param>
        /// <param name="status">新状态</param>
        public PreviewVoiceStatusEventArgs(string engineId, string sourceName, PreviewVoiceStatus status)
        {
            EngineId = engineId;
            SourceName = sourceName;
            Status = status;
        }
    }

    /// <summary>
    /// 预览声音任务事件参数类
    /// </summary>
    public class PreviewVoiceTaskEventArgs : EventArgs
    {
        /// <summary>
        /// 获取预览任务
        /// </summary>
        public PreviewVoiceTask Task { get; }

        /// <summary>
        /// 初始化PreviewVoiceTaskEventArgs类的新实例
        /// </summary>
        /// <param name="task">预览任务</param>
        public PreviewVoiceTaskEventArgs(PreviewVoiceTask task)
        {
            Task = task;
        }
    }

    /// <summary>
    /// 预览声音任务完成事件参数类
    /// </summary>
    public class PreviewVoiceTaskCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// 获取预览任务
        /// </summary>
        public PreviewVoiceTask Task { get; }

        /// <summary>
        /// 获取是否成功
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// 初始化PreviewVoiceTaskCompletedEventArgs类的新实例
        /// </summary>
        /// <param name="task">预览任务</param>
        /// <param name="success">是否成功</param>
        public PreviewVoiceTaskCompletedEventArgs(PreviewVoiceTask task, bool success)
        {
            Task = task;
            Success = success;
        }
    }

    #endregion
}
