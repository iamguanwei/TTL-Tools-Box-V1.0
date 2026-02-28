using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GW.TTLtoolsBox.Core.SystemOption.TtlEngine;

namespace GW.TTLtoolsBox.WinFormUi.Manager
{
    /// <summary>
    /// 语音生成任务队列管理器，封装任务队列的所有操作。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 管理任务列表的增删改查
    /// - 控制任务的执行、暂停、恢复
    /// - 提供任务状态变化的事件通知
    /// 
    /// 使用场景：
    /// - 语音生成模块需要管理多个生成任务
    /// - 需要按顺序执行任务
    /// - 需要支持任务的暂停和恢复
    /// 
    /// 依赖关系：
    /// - 依赖ITtlEngineConnector进行语音生成
    /// - 依赖VoiceGenerationTask等实体类
    /// </remarks>
    public class VoiceGenerationTaskQueue : IDisposable
    {
        #region public

        #region 构造函数

        /// <summary>
        /// 初始化VoiceGenerationTaskQueue类的新实例。
        /// </summary>
        /// <param name="getEngineConnector">获取TTL引擎连接器的函数。</param>
        /// <param name="getConnectionStatus">获取连接状态的函数。</param>
        /// <param name="ffmpegPath">FFmpeg可执行文件路径。</param>
        /// <param name="tempFolder">临时文件夹路径。</param>
        public VoiceGenerationTaskQueue(
            Func<ITtlEngineConnector> getEngineConnector,
            Func<TtlEngineConnectionStatus> getConnectionStatus,
            string ffmpegPath,
            string tempFolder = null)
        {
            _getEngineConnector = getEngineConnector;
            _getConnectionStatus = getConnectionStatus;
            _ffmpegPath = ffmpegPath;
            _tempFolder = tempFolder ?? Path.Combine(System.Windows.Forms.Application.StartupPath, "Temp");
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取任务列表。
        /// </summary>
        public BindingList<VoiceGenerationTask> Tasks => _tasks;

        /// <summary>
        /// 获取当前是否有任务正在执行。
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// 获取当前正在执行的任务。
        /// </summary>
        public VoiceGenerationTask CurrentTask => _currentTask;

        /// <summary>
        /// 获取排队中的任务数量。
        /// </summary>
        public int QueuedCount => _tasks.Count(t => t.Status == VoiceGenerationTaskStatus.排队中);

        #endregion

        #region 方法

        /// <summary>
        /// 添加任务到队列。
        /// </summary>
        /// <param name="task">要添加的任务。</param>
        public void AddTask(VoiceGenerationTask task)
        {
            if (task == null) return;
            _tasks.Add(task);
            OnTaskListChanged(TaskListChangeType.Added, task);
        }

        /// <summary>
        /// 添加多个任务到队列。
        /// </summary>
        /// <param name="tasks">要添加的任务集合。</param>
        public void AddTasks(IEnumerable<VoiceGenerationTask> tasks)
        {
            if (tasks == null) return;
            foreach (var task in tasks)
            {
                AddTask(task);
            }
        }

        /// <summary>
        /// 从队列移除任务。
        /// </summary>
        /// <param name="task">要移除的任务。</param>
        /// <returns>是否成功移除。</returns>
        public bool RemoveTask(VoiceGenerationTask task)
        {
            if (task == null || task.Status == VoiceGenerationTaskStatus.正在生成) return false;
            bool removed = _tasks.Remove(task);
            if (removed)
            {
                OnTaskListChanged(TaskListChangeType.Removed, task);
            }
            return removed;
        }

        /// <summary>
        /// 清空所有任务。
        /// </summary>
        public void ClearTasks()
        {
            _tasks.Clear();
            OnTaskListChanged(TaskListChangeType.Cleared, null);
        }

        /// <summary>
        /// 开始执行队列。
        /// </summary>
        public void Start()
        {
            lock (_lock)
            {
                if (_isRunning) return;
            }
            ProcessNextTask();
        }

        /// <summary>
        /// 停止当前任务。
        /// </summary>
        public void Stop()
        {
            _shouldStop = true;
        }

        /// <summary>
        /// 停止当前任务并等待其完成。
        /// </summary>
        public void StopAndWait()
        {
            if (!_isRunning) return;

            _shouldStop = true;

            int waitCount = 0;
            while (_isRunning && waitCount < 50)
            {
                System.Threading.Thread.Sleep(100);
                waitCount++;
            }

            if (_currentTask != null &&
                (_currentTask.Status == VoiceGenerationTaskStatus.正在生成 ||
                 _currentTask.Status == VoiceGenerationTaskStatus.排队中))
            {
                _currentTask.Status = VoiceGenerationTaskStatus.排队中;
                _currentTask.ProgressDetail = "方案切换，等待继续...";
            }
        }

        /// <summary>
        /// 重新执行任务。
        /// </summary>
        /// <param name="task">要重新执行的任务。</param>
        public void RestartTask(VoiceGenerationTask task)
        {
            if (task == null) return;
            task.Status = VoiceGenerationTaskStatus.排队中;
            task.Progress = 0;
            task.ProgressDetail = "等待重新执行...";
            OnTaskStatusChanged(task);
            if (!_isRunning) Start();
        }

        /// <summary>
        /// 移动任务位置。
        /// </summary>
        /// <param name="task">要移动的任务。</param>
        /// <param name="newIndex">新的索引位置。</param>
        public void MoveTask(VoiceGenerationTask task, int newIndex)
        {
            int currentIndex = _tasks.IndexOf(task);
            if (currentIndex < 0 || currentIndex == newIndex) return;
            _tasks.RemoveAt(currentIndex);
            _tasks.Insert(newIndex, task);
            OnTaskListChanged(TaskListChangeType.Moved, task);
        }

        /// <summary>
        /// 尝试恢复任务队列执行。
        /// </summary>
        public void TryResume()
        {
            if (_getEngineConnector?.Invoke() == null) return;

            bool hasQueuedTask = _tasks.Any(t => t.Status == VoiceGenerationTaskStatus.排队中);
            lock (_lock)
            {
                if (hasQueuedTask && !_isRunning)
                {
                    ProcessNextTask();
                }
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 任务状态变化事件。
        /// </summary>
        public event EventHandler<TaskStatusChangedEventArgs> TaskStatusChanged;

        /// <summary>
        /// 任务列表变化事件。
        /// </summary>
        public event EventHandler<TaskListChangedEventArgs> TaskListChanged;

        /// <summary>
        /// 任务进度更新事件。
        /// </summary>
        public event EventHandler<TaskProgressEventArgs> TaskProgressUpdated;

        /// <summary>
        /// 请求检查并连接TTL引擎事件。
        /// </summary>
        public event EventHandler RequestEngineConnection;

        /// <summary>
        /// 请求刷新UI事件。
        /// </summary>
        public event EventHandler RequestRefreshUi;

        /// <summary>
        /// 任务提交信息事件。
        /// </summary>
        public event EventHandler<TaskSubmitInfoEventArgs> TaskSubmitInfo;

        /// <summary>
        /// 预览任务完成事件。
        /// </summary>
        public event EventHandler<PreviewTaskCompletedEventArgs> PreviewTaskCompleted;

        #endregion

        #endregion

        #region protected

        #region 方法

        /// <summary>
        /// 处理下一个任务。
        /// </summary>
        protected virtual async void ProcessNextTask()
        {
            lock (_lock)
            {
                if (_isRunning) return;
                _isRunning = true;
            }

            try
            {
                _shouldStop = false;

                var engineConnector = _getEngineConnector?.Invoke();
                if (engineConnector == null)
                {
                    lock (_lock)
                    {
                        _isRunning = false;
                    }
                    _currentTask = null;
                    return;
                }

                VoiceGenerationTask nextTask = null;
                foreach (var task in _tasks)
                {
                    if (task.Status == VoiceGenerationTaskStatus.排队中)
                    {
                        nextTask = task;
                        break;
                    }
                }

                if (nextTask == null)
                {
                    lock (_lock)
                    {
                        _isRunning = false;
                    }
                    _currentTask = null;
                    return;
                }

                var connectionStatus = _getConnectionStatus?.Invoke() ?? TtlEngineConnectionStatus.未连接;
                if (connectionStatus == TtlEngineConnectionStatus.未连接 ||
                    connectionStatus == TtlEngineConnectionStatus.连接失败)
                {
                    lock (_lock)
                    {
                        _isRunning = false;
                    }
                    _currentTask = null;
                    RequestEngineConnection?.Invoke(this, EventArgs.Empty);
                    return;
                }

                _currentTask = nextTask;
                nextTask.Status = VoiceGenerationTaskStatus.正在生成;
                nextTask.ProgressDetail = "正在生成...";
                OnTaskStatusChanged(nextTask);

                await ExecuteTaskAsync(nextTask);
            }
            catch
            {
                lock (_lock)
                {
                    _isRunning = false;
                }
                _currentTask = null;
            }
        }

        /// <summary>
        /// 执行单个任务。
        /// </summary>
        /// <param name="task">要执行的任务。</param>
        protected virtual async Task ExecuteTaskAsync(VoiceGenerationTask task)
        {
            List<string> tempFilesToCleanup = new List<string>();
            string mergedFile = null;
            string speedAdjustedFile = null;
            bool success = false;
            bool engineConnectionLost = false;

            try
            {
                if (_shouldStop) return;

                var engineConnector = _getEngineConnector?.Invoke();
                var connectionStatus = _getConnectionStatus?.Invoke() ?? TtlEngineConnectionStatus.未连接;

                if (engineConnector == null ||
                    connectionStatus == TtlEngineConnectionStatus.未连接 ||
                    connectionStatus == TtlEngineConnectionStatus.连接失败)
                {
                    task.Status = VoiceGenerationTaskStatus.排队中;
                    task.ProgressDetail = "等待TTL引擎连接...";
                    engineConnectionLost = true;
                    RequestEngineConnection?.Invoke(this, EventArgs.Empty);
                    return;
                }

                if (!Directory.Exists(_tempFolder))
                {
                    Directory.CreateDirectory(_tempFolder);
                }

                int totalItems = task.Items.Length;
                int completedItems = 0;

                for (int i = 0; i < totalItems; i++)
                {
                    VoiceGenerationTaskItem taskItem = task.Items[i];

                    if (string.IsNullOrWhiteSpace(taskItem.TempFile) ||
                        !Path.IsPathRooted(taskItem.TempFile) ||
                        !taskItem.TempFile.StartsWith(_tempFolder, StringComparison.OrdinalIgnoreCase))
                    {
                        taskItem.TempFile = Path.Combine(_tempFolder, $"temp_{task.Id}_{i + 1}.wav");
                    }

                    if (File.Exists(taskItem.TempFile))
                    {
                        completedItems++;
                    }
                }

                task.Progress = (decimal)completedItems / totalItems;

                for (int i = 0; i < totalItems; i++)
                {
                    if (_shouldStop) return;

                    connectionStatus = _getConnectionStatus?.Invoke() ?? TtlEngineConnectionStatus.未连接;
                    if (connectionStatus == TtlEngineConnectionStatus.未连接 ||
                        connectionStatus == TtlEngineConnectionStatus.连接失败)
                    {
                        task.Status = VoiceGenerationTaskStatus.排队中;
                        task.ProgressDetail = "等待TTL引擎连接...";
                        engineConnectionLost = true;
                        RequestEngineConnection?.Invoke(this, EventArgs.Empty);
                        return;
                    }

                    VoiceGenerationTaskItem taskItem = task.Items[i];

                    if (!string.IsNullOrWhiteSpace(taskItem.TempFile) && File.Exists(taskItem.TempFile))
                    {
                        continue;
                    }

                    task.ProgressDetail = $"正在生成第 {i + 1}/{totalItems} 段";
                    OnTaskProgressUpdated(task, task.Progress, task.ProgressDetail);

                    string tempFilePath = taskItem.TempFile;

                    TtlEngineParameters parameters = new TtlEngineParameters(taskItem.Text, taskItem.Speaker);
                    parameters.OutputFilePath = tempFilePath;

                    if (taskItem.FeatureSelections != null && taskItem.FeatureSelections.Count > 0)
                    {
                        parameters.FeatureSelections = new Dictionary<string, int>(taskItem.FeatureSelections);
                    }

                    bool itemCompleted = false;
                    Task<string> sendTask = null;

                    while (!itemCompleted && !_shouldStop)
                    {
                        try
                        {
                            if (sendTask == null)
                            {
                                string textPreview = taskItem.Text.Length > 10 ? taskItem.Text.Substring(0, 10) + "……" : taskItem.Text;
                                OnTaskSubmitInfo(task.Id, i + 1, textPreview);
                                sendTask = engineConnector.SendTextAsync(taskItem.Text, parameters);
                            }

                            Task stopCheckTask = Task.Delay(100);
                            await Task.WhenAny(sendTask, stopCheckTask);

                            if (_shouldStop) return;

                            if (sendTask.IsCompleted)
                            {
                                string tempAudioFilePath = sendTask.Result;
                                if (!string.IsNullOrWhiteSpace(tempAudioFilePath) && File.Exists(tempAudioFilePath))
                                {
                                    completedItems++;
                                    task.Progress = (decimal)completedItems / totalItems;
                                    itemCompleted = true;
                                }
                                else
                                {
                                    sendTask = null;
                                }
                            }
                        }
                        catch
                        {
                            sendTask = null;
                            connectionStatus = _getConnectionStatus?.Invoke() ?? TtlEngineConnectionStatus.未连接;
                            if (connectionStatus == TtlEngineConnectionStatus.未连接 ||
                                connectionStatus == TtlEngineConnectionStatus.连接失败)
                            {
                                task.Status = VoiceGenerationTaskStatus.排队中;
                                task.ProgressDetail = "等待TTL引擎连接...";
                                engineConnectionLost = true;
                                RequestEngineConnection?.Invoke(this, EventArgs.Empty);
                                return;
                            }
                        }
                    }

                    if (_shouldStop) return;
                }

                if (_shouldStop) return;

                task.ProgressDetail = "正在合并音频...";
                OnTaskProgressUpdated(task, task.Progress, task.ProgressDetail);

                mergedFile = await MergeAudioFilesAsync(task.Items, task.SpaceTime, task.SaveFile);
                tempFilesToCleanup.Add(mergedFile);

                if (_shouldStop) return;

                task.ProgressDetail = "正在调整音频速度...";
                OnTaskProgressUpdated(task, task.Progress, task.ProgressDetail);

                speedAdjustedFile = await AdjustAudioSpeedAsync(mergedFile, task.Speed);
                if (speedAdjustedFile != mergedFile)
                {
                    tempFilesToCleanup.Add(speedAdjustedFile);
                }

                if (_shouldStop) return;

                string outputDir = Path.GetDirectoryName(task.SaveFile);
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                task.ProgressDetail = "正在保存最终文件...";
                OnTaskProgressUpdated(task, task.Progress, task.ProgressDetail);

                string fileExtension = Path.GetExtension(task.SaveFile).ToLowerInvariant();

                if (fileExtension == ".mp3")
                {
                    string mp3TempFile = await ConvertToMp3Async(speedAdjustedFile, task.SaveFile);
                    tempFilesToCleanup.Add(mp3TempFile);
                }
                else
                {
                    File.Copy(speedAdjustedFile, task.SaveFile, true);
                }

                success = true;

                task.Status = VoiceGenerationTaskStatus.已完成;
                task.Progress = 1;
                task.ProgressDetail = "生成完成";
            }
            catch (Exception ex)
            {
                if (!_shouldStop)
                {
                    var connectionStatus = _getConnectionStatus?.Invoke() ?? TtlEngineConnectionStatus.未连接;
                    if (connectionStatus == TtlEngineConnectionStatus.未连接 ||
                        connectionStatus == TtlEngineConnectionStatus.连接失败)
                    {
                        task.Status = VoiceGenerationTaskStatus.排队中;
                        task.ProgressDetail = "等待TTL引擎连接...";
                        engineConnectionLost = true;
                        RequestEngineConnection?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        task.Status = VoiceGenerationTaskStatus.生成失败;
                        task.ProgressDetail = $"生成失败: {ex.Message}";
                    }
                }
            }
            finally
            {
                if (success)
                {
                    foreach (var item in task.Items)
                    {
                        if (!string.IsNullOrWhiteSpace(item.TempFile) && File.Exists(item.TempFile))
                        {
                            try { File.Delete(item.TempFile); } catch { }
                        }
                    }

                    foreach (string tempFile in tempFilesToCleanup)
                    {
                        try
                        {
                            if (File.Exists(tempFile))
                            {
                                File.Delete(tempFile);
                            }
                        }
                        catch { }
                    }

                    if (task.IsPreview && !string.IsNullOrWhiteSpace(task.PreviewSourceName))
                    {
                        OnPreviewTaskCompleted(task.PreviewSourceName, true);
                    }
                }
                else if (task.IsPreview && !string.IsNullOrWhiteSpace(task.PreviewSourceName))
                {
                    OnPreviewTaskCompleted(task.PreviewSourceName, false);
                }

                if (!success && task.Status == VoiceGenerationTaskStatus.正在生成)
                {
                    task.Status = VoiceGenerationTaskStatus.排队中;
                    task.ProgressDetail = "任务已暂停，等待继续...";
                }

                _currentTask = null;
                OnTaskStatusChanged(task);

                if (!engineConnectionLost)
                {
                    lock (_lock)
                    {
                        _isRunning = false;
                        ProcessNextTask();
                    }
                }
                else
                {
                    lock (_lock)
                    {
                        _isRunning = false;
                    }
                }
            }
        }

        /// <summary>
        /// 合并多个音频文件。
        /// </summary>
        protected virtual async Task<string> MergeAudioFilesAsync(VoiceGenerationTaskItem[] taskItems, float spaceTime, string outputFile)
        {
            return await Task.Run(() => MergeAudioFiles(taskItems, spaceTime, outputFile));
        }

        /// <summary>
        /// 调整音频速度。
        /// </summary>
        protected virtual async Task<string> AdjustAudioSpeedAsync(string inputFile, int speed)
        {
            return await Task.Run(() => AdjustAudioSpeed(inputFile, speed));
        }

        /// <summary>
        /// 转换为MP3格式。
        /// </summary>
        protected virtual async Task<string> ConvertToMp3Async(string inputFile, string outputFile)
        {
            return await Task.Run(() => ConvertToMp3(inputFile, outputFile));
        }

        /// <summary>
        /// 触发任务状态变化事件。
        /// </summary>
        protected virtual void OnTaskStatusChanged(VoiceGenerationTask task)
        {
            TaskStatusChanged?.Invoke(this, new TaskStatusChangedEventArgs(task));
        }

        /// <summary>
        /// 触发任务列表变化事件。
        /// </summary>
        protected virtual void OnTaskListChanged(TaskListChangeType changeType, VoiceGenerationTask task)
        {
            TaskListChanged?.Invoke(this, new TaskListChangedEventArgs(changeType, task));
        }

        /// <summary>
        /// 触发任务进度更新事件。
        /// </summary>
        protected virtual void OnTaskProgressUpdated(VoiceGenerationTask task, decimal progress, string detail)
        {
            TaskProgressUpdated?.Invoke(this, new TaskProgressEventArgs(task, progress, detail));
        }

        /// <summary>
        /// 触发任务提交信息事件。
        /// </summary>
        protected virtual void OnTaskSubmitInfo(string taskId, int itemIndex, string textPreview)
        {
            TaskSubmitInfo?.Invoke(this, new TaskSubmitInfoEventArgs(taskId, itemIndex, textPreview));
        }

        /// <summary>
        /// 触发预览任务完成事件。
        /// </summary>
        protected virtual void OnPreviewTaskCompleted(string sourceName, bool success)
        {
            PreviewTaskCompleted?.Invoke(this, new PreviewTaskCompletedEventArgs(sourceName, success));
        }

        #endregion

        #endregion

        #region private

        #region 字段

        private readonly BindingList<VoiceGenerationTask> _tasks = new BindingList<VoiceGenerationTask> { RaiseListChangedEvents = true };
        private readonly object _lock = new object();
        private readonly Func<ITtlEngineConnector> _getEngineConnector;
        private readonly Func<TtlEngineConnectionStatus> _getConnectionStatus;
        private readonly string _tempFolder;
        private readonly string _ffmpegPath;
        private bool _isRunning = false;
        private bool _shouldStop = false;
        private VoiceGenerationTask _currentTask = null;

        #endregion

        #region 方法

        private string MergeAudioFiles(VoiceGenerationTaskItem[] taskItems, float spaceTime, string outputFile)
        {
            List<string> validFiles = new List<string>();
            List<float> silenceDurations = new List<float>();

            try
            {
                for (int i = 0; i < taskItems.Length; i++)
                {
                    VoiceGenerationTaskItem taskItem = taskItems[i];

                    if (string.IsNullOrWhiteSpace(taskItem.TempFile) || !File.Exists(taskItem.TempFile))
                    {
                        continue;
                    }

                    validFiles.Add(taskItem.TempFile);
                    silenceDurations.Add(taskItem.EndNewLine * spaceTime);
                }

                if (validFiles.Count == 0)
                {
                    throw new Exception("没有有效的音频文件可合并");
                }

                string tempOutput = Path.Combine(_tempFolder, $"merged_{Guid.NewGuid()}.wav");

                if (validFiles.Count == 1)
                {
                    float silenceDuration = silenceDurations[0];
                    if (silenceDuration > 0)
                    {
                        string inputFileName = Path.GetFileName(validFiles[0]);
                        string outputFileName = Path.GetFileName(tempOutput);
                        string ffmpegArgs = $"-i \"{inputFileName}\" -af \"apad=pad_dur={silenceDuration}\" -y \"{outputFileName}\"";
                        RunFFmpegCommand(ffmpegArgs, _tempFolder).Wait();
                    }
                    else
                    {
                        File.Copy(validFiles[0], tempOutput, true);
                    }
                }
                else
                {
                    StringBuilder inputArgs = new StringBuilder();
                    StringBuilder filterComplex = new StringBuilder();
                    StringBuilder concatInputs = new StringBuilder();

                    for (int i = 0; i < validFiles.Count; i++)
                    {
                        string inputFileName = Path.GetFileName(validFiles[i]);
                        inputArgs.Append($"-i \"{inputFileName}\" ");

                        float silenceDuration = silenceDurations[i];
                        if (silenceDuration > 0)
                        {
                            filterComplex.Append($"[{i}:a]apad=pad_dur={silenceDuration}[a{i}];");
                            concatInputs.Append($"[a{i}]");
                        }
                        else
                        {
                            filterComplex.Append($"[{i}:a]anull[a{i}];");
                            concatInputs.Append($"[a{i}]");
                        }
                    }

                    filterComplex.Append($"{concatInputs}concat=n={validFiles.Count}:v=0:a=1[out]");

                    string outputFileName = Path.GetFileName(tempOutput);
                    string ffmpegArgs = $"{inputArgs}-filter_complex \"{filterComplex}\" -map \"[out]\" -y \"{outputFileName}\"";

                    RunFFmpegCommand(ffmpegArgs, _tempFolder).Wait();
                }

                return tempOutput;
            }
            catch (Exception ex)
            {
                throw new Exception($"合并音频文件失败: {ex.Message}");
            }
        }

        private string AdjustAudioSpeed(string inputFile, int speed)
        {
            try
            {
                if (speed == 100)
                {
                    return inputFile;
                }

                float tempo = speed / 100f;
                string inputFileName = Path.GetFileName(inputFile);
                string outputFileName = $"speed_{Guid.NewGuid()}.wav";
                string outputFile = Path.Combine(_tempFolder, outputFileName);
                string ffmpegArgs = $"-i \"{inputFileName}\" -filter:a \"atempo={tempo}\" -y \"{outputFileName}\"";

                RunFFmpegCommand(ffmpegArgs, _tempFolder).Wait();

                return outputFile;
            }
            catch (Exception ex)
            {
                throw new Exception($"调整音频速度失败: {ex.Message}");
            }
        }

        private string ConvertToMp3(string inputFile, string outputFile)
        {
            string inputFileName = Path.GetFileName(inputFile);
            string outputFileName = $"temp_mp3_{Guid.NewGuid()}.mp3";
            string tempOutputFile = Path.Combine(_tempFolder, outputFileName);
            string ffmpegArgs = $"-i \"{inputFileName}\" -q:a 2 -acodec libmp3lame \"{outputFileName}\"";

            RunFFmpegCommand(ffmpegArgs, _tempFolder).Wait();

            File.Copy(tempOutputFile, outputFile, true);

            return tempOutputFile;
        }

        private async Task RunFFmpegCommand(string arguments, string workingDirectory = null)
        {
            if (string.IsNullOrEmpty(_ffmpegPath) || !File.Exists(_ffmpegPath))
            {
                throw new Exception("FFmpeg路径未配置或文件不存在");
            }

            using (Process process = new Process())
            {
                process.StartInfo.FileName = _ffmpegPath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                if (!string.IsNullOrWhiteSpace(workingDirectory))
                {
                    process.StartInfo.WorkingDirectory = workingDirectory;
                }

                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null) outputWaitHandle.Set();
                        else output.AppendLine(e.Data);
                    };
                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null) errorWaitHandle.Set();
                        else error.AppendLine(e.Data);
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    await Task.WhenAll(
                        Task.Run(() => outputWaitHandle.WaitOne()),
                        Task.Run(() => errorWaitHandle.WaitOne())
                    );

                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"FFmpeg命令执行失败: {error.ToString()}");
                    }
                }
            }
        }

        #endregion

        #endregion

        #region IDisposable

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            StopAndWait();
        }

        #endregion

        #region 嵌套类型

        /// <summary>
        /// 任务列表变化类型。
        /// </summary>
        public enum TaskListChangeType
        {
            /// <summary>
            /// 任务已添加。
            /// </summary>
            Added,

            /// <summary>
            /// 任务已移除。
            /// </summary>
            Removed,

            /// <summary>
            /// 任务列表已清空。
            /// </summary>
            Cleared,

            /// <summary>
            /// 任务已移动。
            /// </summary>
            Moved
        }

        #endregion
    }

    #region 事件参数类

    /// <summary>
    /// 任务状态变化事件参数。
    /// </summary>
    public class TaskStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 获取任务。
        /// </summary>
        public VoiceGenerationTask Task { get; }

        /// <summary>
        /// 初始化TaskStatusChangedEventArgs类的新实例。
        /// </summary>
        public TaskStatusChangedEventArgs(VoiceGenerationTask task)
        {
            Task = task;
        }
    }

    /// <summary>
    /// 任务列表变化事件参数。
    /// </summary>
    public class TaskListChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 获取变化类型。
        /// </summary>
        public VoiceGenerationTaskQueue.TaskListChangeType ChangeType { get; }

        /// <summary>
        /// 获取任务。
        /// </summary>
        public VoiceGenerationTask Task { get; }

        /// <summary>
        /// 初始化TaskListChangedEventArgs类的新实例。
        /// </summary>
        public TaskListChangedEventArgs(VoiceGenerationTaskQueue.TaskListChangeType changeType, VoiceGenerationTask task)
        {
            ChangeType = changeType;
            Task = task;
        }
    }

    /// <summary>
    /// 任务进度事件参数。
    /// </summary>
    public class TaskProgressEventArgs : EventArgs
    {
        /// <summary>
        /// 获取任务。
        /// </summary>
        public VoiceGenerationTask Task { get; }

        /// <summary>
        /// 获取进度。
        /// </summary>
        public decimal Progress { get; }

        /// <summary>
        /// 获取进度详情。
        /// </summary>
        public string Detail { get; }

        /// <summary>
        /// 初始化TaskProgressEventArgs类的新实例。
        /// </summary>
        public TaskProgressEventArgs(VoiceGenerationTask task, decimal progress, string detail)
        {
            Task = task;
            Progress = progress;
            Detail = detail;
        }
    }

    /// <summary>
    /// 任务提交信息事件参数。
    /// </summary>
    public class TaskSubmitInfoEventArgs : EventArgs
    {
        /// <summary>
        /// 获取任务ID。
        /// </summary>
        public string TaskId { get; }

        /// <summary>
        /// 获取任务项索引。
        /// </summary>
        public int ItemIndex { get; }

        /// <summary>
        /// 获取文本预览。
        /// </summary>
        public string TextPreview { get; }

        /// <summary>
        /// 初始化TaskSubmitInfoEventArgs类的新实例。
        /// </summary>
        public TaskSubmitInfoEventArgs(string taskId, int itemIndex, string textPreview)
        {
            TaskId = taskId;
            ItemIndex = itemIndex;
            TextPreview = textPreview;
        }
    }

    /// <summary>
    /// 预览任务完成事件参数。
    /// </summary>
    public class PreviewTaskCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// 获取源名称。
        /// </summary>
        public string SourceName { get; }

        /// <summary>
        /// 获取是否成功。
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// 初始化PreviewTaskCompletedEventArgs类的新实例。
        /// </summary>
        public PreviewTaskCompletedEventArgs(string sourceName, bool success)
        {
            SourceName = sourceName;
            Success = success;
        }
    }

    #endregion
}
