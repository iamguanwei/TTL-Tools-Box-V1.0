using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GW.TTLtoolsBox.WinFormUi.Manager
{
    /// <summary>
    /// MP3转换队列管理器，封装MP3转换任务队列的所有操作。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 管理MP3转换任务队列
    /// - 后台线程依次执行转换
    /// - 支持停止当前转换
    /// - 提供状态变化事件通知
    /// 
    /// 使用场景：
    /// - 语音生成完成后转换为MP3
    /// - 批量转换wav文件为MP3
    /// 
    /// 依赖关系：
    /// - 依赖ffmpeg进行音频转换
    /// - 依赖VoiceGenerationTask类
    /// </remarks>
    public class Mp3ConversionQueue : IDisposable
    {
        #region 常量

        /// <summary>
        /// 临时MP3文件前缀。
        /// </summary>
        private const string _临时文件_前缀_MP3 = "mp3_";

        #endregion

        #region public

        #region 构造函数

        /// <summary>
        /// 初始化Mp3ConversionQueue类的新实例。
        /// </summary>
        /// <param name="ffmpegPath">FFmpeg可执行文件路径。</param>
        /// <param name="tempFolder">临时文件夹路径。</param>
        public Mp3ConversionQueue(string ffmpegPath, string tempFolder)
        {
            _ffmpegPath = ffmpegPath;
            _tempFolder = tempFolder;
        }

        #endregion

        #region 属性

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
        public int QueuedCount => _conversionQueue.Count(t => t.Task.Status == VoiceGenerationTaskStatus.转换MP3);

        #endregion

        #region 方法

        /// <summary>
        /// 添加转换任务到队列。
        /// </summary>
        /// <param name="task">语音生成任务。</param>
        /// <param name="mp3OutputFile">MP3输出文件路径。</param>
        public void AddConversionTask(VoiceGenerationTask task, string mp3OutputFile)
        {
            if (task == null) return;

            task.Mp3OutputFile = mp3OutputFile;
            task.Status = VoiceGenerationTaskStatus.转换MP3;
            task.ProgressDetail = "等待转换为MP3...";

            _conversionQueue.Add(new Mp3ConversionItem { Task = task, Mp3OutputFile = mp3OutputFile });
            OnConversionQueued(task);
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
            ProcessNextConversion();
        }

        /// <summary>
        /// 停止当前转换。
        /// </summary>
        public void Stop()
        {
            _shouldStop = true;
            _cancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// 停止当前转换并等待其完成。
        /// </summary>
        public void StopAndWait()
        {
            if (!_isRunning) return;

            _shouldStop = true;
            _cancellationTokenSource?.Cancel();

            int waitCount = 0;
            while (_isRunning && waitCount < 50)
            {
                Thread.Sleep(100);
                waitCount++;
            }

            if (_currentTask != null && _currentTask.Status == VoiceGenerationTaskStatus.转换MP3)
            {
                _currentTask.Status = VoiceGenerationTaskStatus.已完成;
                _currentTask.ProgressDetail = "转换已取消";
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 转换任务入队事件。
        /// </summary>
        public event EventHandler<Mp3ConversionEventArgs> ConversionQueued;

        /// <summary>
        /// 转换开始事件。
        /// </summary>
        public event EventHandler<Mp3ConversionEventArgs> ConversionStarted;

        /// <summary>
        /// 转换完成事件。
        /// </summary>
        public event EventHandler<Mp3ConversionEventArgs> ConversionCompleted;

        /// <summary>
        /// 转换失败事件。
        /// </summary>
        public event EventHandler<Mp3ConversionEventArgs> ConversionFailed;

        /// <summary>
        /// 转换取消事件。
        /// </summary>
        public event EventHandler<Mp3ConversionEventArgs> ConversionCancelled;

        #endregion

        #endregion

        #region protected

        #region 方法

        /// <summary>
        /// 触发转换任务入队事件。
        /// </summary>
        /// <param name="task">任务。</param>
        protected void OnConversionQueued(VoiceGenerationTask task)
        {
            ConversionQueued?.Invoke(this, new Mp3ConversionEventArgs(task));
        }

        /// <summary>
        /// 触发转换开始事件。
        /// </summary>
        /// <param name="task">任务。</param>
        protected void OnConversionStarted(VoiceGenerationTask task)
        {
            ConversionStarted?.Invoke(this, new Mp3ConversionEventArgs(task));
        }

        /// <summary>
        /// 触发转换完成事件。
        /// </summary>
        /// <param name="task">任务。</param>
        /// <param name="mp3File">MP3文件路径。</param>
        protected void OnConversionCompleted(VoiceGenerationTask task, string mp3File)
        {
            ConversionCompleted?.Invoke(this, new Mp3ConversionEventArgs(task, mp3File));
        }

        /// <summary>
        /// 触发转换失败事件。
        /// </summary>
        /// <param name="task">任务。</param>
        /// <param name="errorMessage">错误信息。</param>
        protected void OnConversionFailed(VoiceGenerationTask task, string errorMessage)
        {
            ConversionFailed?.Invoke(this, new Mp3ConversionEventArgs(task, errorMessage));
        }

        /// <summary>
        /// 触发转换取消事件。
        /// </summary>
        /// <param name="task">任务。</param>
        protected void OnConversionCancelled(VoiceGenerationTask task)
        {
            ConversionCancelled?.Invoke(this, new Mp3ConversionEventArgs(task));
        }

        #endregion

        #endregion

        #region private

        #region 字段

        private readonly string _ffmpegPath;
        private readonly string _tempFolder;
        private readonly List<Mp3ConversionItem> _conversionQueue = new List<Mp3ConversionItem>();
        private readonly object _lock = new object();

        private VoiceGenerationTask _currentTask;
        private bool _isRunning = false;
        private bool _shouldStop = false;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region 方法

        /// <summary>
        /// 处理下一个转换任务。
        /// </summary>
        private void ProcessNextConversion()
        {
            lock (_lock)
            {
                if (_isRunning) return;

                var nextItem = _conversionQueue.FirstOrDefault(t => t.Task.Status == VoiceGenerationTaskStatus.转换MP3);
                if (nextItem == null) return;

                _currentTask = nextItem.Task;
                _isRunning = true;
                _shouldStop = false;
                _cancellationTokenSource = new CancellationTokenSource();
                _conversionQueue.Remove(nextItem);
            }

            Task.Run(() => ExecuteConversion(_currentTask, _currentTask.Mp3OutputFile, _cancellationTokenSource.Token), _cancellationTokenSource.Token)
                .ContinueWith(t =>
                {
                    lock (_lock)
                    {
                        _isRunning = false;
                        _currentTask = null;
                        _cancellationTokenSource?.Dispose();
                        _cancellationTokenSource = null;
                    }

                    if (!_shouldStop)
                    {
                        ProcessNextConversion();
                    }
                });
        }

        /// <summary>
        /// 执行转换。
        /// </summary>
        /// <param name="task">任务。</param>
        /// <param name="mp3OutputFile">MP3输出文件路径。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        private void ExecuteConversion(VoiceGenerationTask task, string mp3OutputFile, CancellationToken cancellationToken)
        {
            try
            {
                OnConversionStarted(task);

                if (cancellationToken.IsCancellationRequested)
                {
                    task.Status = VoiceGenerationTaskStatus.已完成;
                    task.ProgressDetail = "转换已取消";
                    OnConversionCancelled(task);
                    return;
                }

                string wavFile = task.SaveFile;
                if (string.IsNullOrWhiteSpace(wavFile) || !File.Exists(wavFile))
                {
                    task.Status = VoiceGenerationTaskStatus.转换MP3失败;
                    task.ProgressDetail = "WAV文件不存在";
                    OnConversionFailed(task, "WAV文件不存在");
                    return;
                }

                string mp3File = Path.Combine(Path.GetDirectoryName(wavFile), $"{Path.GetFileNameWithoutExtension(wavFile)}.mp3");
                string inputFileName = Path.GetFileName(wavFile);
                string outputFileName = Path.GetFileName(mp3File);
                string ffmpegArgs = $"-y -i \"{inputFileName}\" -q:a 2 -acodec libmp3lame \"{outputFileName}\"";

                RunFFmpegCommand(ffmpegArgs, Path.GetDirectoryName(wavFile), cancellationToken).Wait();

                if (cancellationToken.IsCancellationRequested)
                {
                    if (File.Exists(mp3File))
                    {
                        try { File.Delete(mp3File); } catch { }
                    }
                    task.Status = VoiceGenerationTaskStatus.已完成;
                    task.ProgressDetail = "转换已取消";
                    OnConversionCancelled(task);
                    return;
                }

                string outputFolder = Path.GetDirectoryName(mp3OutputFile);
                if (!string.IsNullOrWhiteSpace(outputFolder) && !Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                task.Status = VoiceGenerationTaskStatus.已完成;
                task.ProgressDetail = "MP3转换完成";
                OnConversionCompleted(task, mp3OutputFile);
            }
            catch (OperationCanceledException)
            {
                task.Status = VoiceGenerationTaskStatus.已完成;
                task.ProgressDetail = "转换已取消";
                OnConversionCancelled(task);
            }
            catch (Exception ex)
            {
                task.Status = VoiceGenerationTaskStatus.转换MP3失败;
                task.ProgressDetail = $"MP3转换失败: {ex.Message}";
                OnConversionFailed(task, ex.Message);
            }
        }

        /// <summary>
        /// 运行FFmpeg命令。
        /// </summary>
        /// <param name="arguments">命令参数。</param>
        /// <param name="workingDirectory">工作目录。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        private async Task RunFFmpegCommand(string arguments, string workingDirectory, CancellationToken cancellationToken)
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

                    TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

                    cancellationToken.Register(() =>
                    {
                        try
                        {
                            if (!process.HasExited)
                            {
                                process.Kill();
                                process.WaitForExit(1000);
                            }
                        }
                        catch { }
                        finally
                        {
                            outputWaitHandle.Set();
                            errorWaitHandle.Set();
                            tcs.TrySetCanceled();
                        }
                    });

                    Task processTask = Task.Run(() =>
                    {
                        outputWaitHandle.WaitOne();
                        errorWaitHandle.WaitOne();
                        process.WaitForExit();
                        tcs.TrySetResult(true);
                    }, cancellationToken);

                    await tcs.Task;

                    process.CancelErrorRead();
                    process.CancelOutputRead();

                    if (!cancellationToken.IsCancellationRequested && process.ExitCode != 0)
                    {
                        throw new Exception($"FFmpeg命令执行失败: {error.ToString()}");
                    }
                }

                process.Close();
                await Task.Delay(100);
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
            _cancellationTokenSource?.Dispose();
        }

        #endregion

        #region 嵌套类型

        /// <summary>
        /// MP3转换项。
        /// </summary>
        private class Mp3ConversionItem
        {
            /// <summary>
            /// 获取或设置任务。
            /// </summary>
            public VoiceGenerationTask Task { get; set; }

            /// <summary>
            /// 获取或设置MP3输出文件路径。
            /// </summary>
            public string Mp3OutputFile { get; set; }
        }

        #endregion
    }

    #region 事件参数类

    /// <summary>
    /// MP3转换事件参数。
    /// </summary>
    public class Mp3ConversionEventArgs : EventArgs
    {
        /// <summary>
        /// 获取任务。
        /// </summary>
        public VoiceGenerationTask Task { get; }

        /// <summary>
        /// 获取MP3文件路径（转换完成时有效）。
        /// </summary>
        public string Mp3File { get; }

        /// <summary>
        /// 获取错误信息（转换失败时有效）。
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// 初始化Mp3ConversionEventArgs类的新实例。
        /// </summary>
        /// <param name="task">任务。</param>
        public Mp3ConversionEventArgs(VoiceGenerationTask task)
        {
            Task = task;
        }

        /// <summary>
        /// 初始化Mp3ConversionEventArgs类的新实例。
        /// </summary>
        /// <param name="task">任务。</param>
        /// <param name="result">结果（MP3文件路径或错误信息）。</param>
        public Mp3ConversionEventArgs(VoiceGenerationTask task, string result)
        {
            Task = task;
            Mp3File = result;
            ErrorMessage = result;
        }
    }

    #endregion
}
