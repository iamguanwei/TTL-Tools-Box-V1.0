using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GW.TTLtoolsBox.Core.FileAccesser;
using GW.TTLtoolsBox.Core.SystemOption.Helper;
using GW.TTLtoolsBox.Core.SystemOption.TtlEngine;
using GW.TTLtoolsBox.WinFormUi.Base;
using GW.TTLtoolsBox.WinFormUi.Helper;
using GW.TTLtoolsBox.WinFormUi.Manager;
using CoreVoiceGenerationTask = GW.TTLtoolsBox.Core.Entity.VoiceGenerationTask;
using CoreVoiceGenerationTaskItem = GW.TTLtoolsBox.Core.Entity.VoiceGenerationTaskItem;
using static GW.TTLtoolsBox.WinFormUi.Helper.Constants;

namespace GW.TTLtoolsBox.WinFormUi.UI.Panels
{
    /// <summary>
    /// 语音生成面板，提供语音生成任务队列管理功能。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 管理语音生成任务队列
    /// - 支持任务的启动、停止、重启操作
    /// - 支持任务的上下移动和删除
    /// - 支持音频预览和文件夹操作
    /// - 支持任务数据的保存和加载
    /// 
    /// 使用场景：
    /// - 在TTL工具箱中进行批量语音生成
    /// - 作为工作流的最后一步，执行语音生成任务
    /// 
    /// 依赖关系：
    /// - 依赖TtlSchemeController获取当前引擎信息
    /// - 依赖VoiceGenerationTaskQueue管理任务队列
    /// - 依赖ProjectFile保存/加载任务数据
    /// - 通过事件与MainForm通信
    /// </remarks>
    public partial class VoiceGenerationPanel : ViewBase
    {
        #region 常量

        /// <summary>
        /// 角色和文本之间的分隔符。
        /// </summary>
        private const string 角色_文本分隔符 = Constants.角色_文本分隔符;

        /// <summary>
        /// 生成段落分隔符。
        /// </summary>
        private const string 生成段落_分隔符 = Constants.生成段落_分隔符;

        /// <summary>
        /// 默认角色标识。
        /// </summary>
        private const string 默认_角色标识 = Constants.默认_角色标识;

        #endregion

        #region public

        #region 事件

        /// <summary>
        /// 项目内容被修改时触发的事件。
        /// </summary>
        public event EventHandler ProjectModified;

        /// <summary>
        /// 请求打开角色声音预览目录时触发的事件。
        /// </summary>
        public event EventHandler OpenPreviewVoiceFolderRequested;

        /// <summary>
        /// 任务状态变化时触发的事件。
        /// </summary>
        public event EventHandler<TaskStatusChangedEventArgs> TaskStatusChanged;

        /// <summary>
        /// 预览任务完成时触发的事件。
        /// </summary>
        public event EventHandler<PreviewTaskCompletedEventArgs> PreviewTaskCompleted;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化VoiceGenerationPanel类的新实例。
        /// </summary>
        public VoiceGenerationPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取语音生成任务队列。
        /// </summary>
        public VoiceGenerationTaskQueue TaskQueue => _voiceGenerationTaskQueue;

        /// <summary>
        /// 获取或设置TTL方案控制器。
        /// </summary>
        public TtlSchemeController TtlSchemeController { get; set; }

        /// <summary>
        /// 获取或设置项目文件实例。
        /// </summary>
        public ProjectFile ProjectFile { get; set; }

        /// <summary>
        /// 获取或设置当前引擎ID。
        /// </summary>
        public string CurrentEngineId { get; set; }

        /// <summary>
        /// 获取或设置临时文件夹路径。
        /// </summary>
        public string TempFolder
        {
            get => _tempFolder;
            set => _tempFolder = value;
        }

        /// <summary>
        /// 获取或设置FFmpeg文件路径。
        /// </summary>
        public string FfmpegPath { get; set; }

        /// <summary>
        /// 获取或设置引擎连接状态获取委托。
        /// </summary>
        public Func<TtlEngineConnectionStatus> GetEngineConnectionStatus { get; set; }

        /// <summary>
        /// 获取或设置请求引擎连接的委托。
        /// </summary>
        public Action RequestEngineConnection { get; set; }

        /// <summary>
        /// 获取或设置播放声音的委托。
        /// </summary>
        public Action<string> PlaySound { get; set; }

        /// <summary>
        /// 获取或设置查找朗读者的委托。
        /// </summary>
        public Func<string, SpeakerInfo> FindSpeaker { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 初始化面板。
        /// </summary>
        public void InitializePanel()
        {
            init语音生成Ui();
        }

        /// <summary>
        /// 刷新面板UI状态。
        /// </summary>
        public override void RefreshUi()
        {
            refresh语音生成任务清单DataGridView();
        }

        /// <summary>
        /// 加载语音生成任务清单数据。
        /// </summary>
        public void LoadData()
        {
            load语音生成任务清单();
        }

        /// <summary>
        /// 加载指定引擎的语音生成任务清单数据。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        public void LoadData(string engineId)
        {
            load语音生成任务清单(engineId);
        }

        /// <summary>
        /// 保存语音生成任务清单数据。
        /// </summary>
        public void SaveData()
        {
            save语音生成任务清单Data();
        }

        /// <summary>
        /// 保存语音生成任务清单数据到指定引擎。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        public void SaveData(string engineId)
        {
            save语音生成任务清单Data(engineId);
        }

        /// <summary>
        /// 填充语音生成任务清单。
        /// </summary>
        /// <param name="text">包含任务信息的文本。</param>
        public void FillTaskList(string text)
        {
            fill语音生成任务清单(text);
        }

        /// <summary>
        /// 清空所有任务。
        /// </summary>
        public void ClearAllTasks()
        {
            clearAllTasksAndCleanup();
        }

        /// <summary>
        /// 停止任务队列。
        /// </summary>
        public void StopQueue()
        {
            _voiceGenerationTaskQueue?.Stop();
        }

        /// <summary>
        /// 停止任务队列并等待完成。
        /// </summary>
        public void StopQueueAndWait()
        {
            _voiceGenerationTaskQueue?.StopAndWait();
        }

        /// <summary>
        /// 尝试恢复任务队列。
        /// </summary>
        public void TryResumeQueue()
        {
            _voiceGenerationTaskQueue?.TryResume();
        }

        /// <summary>
        /// 更新自动生成全部朗读者预览音频按钮状态。
        /// </summary>
        /// <param name="enabled">是否启用。</param>
        public void UpdateAutoGeneratePreviewButtonStatus(bool enabled)
        {
            // 此方法由MainForm调用，用于更新TTL方案面板的按钮状态
            // 在面板内部不需要实现
        }

        #endregion

        #endregion

        #region protected

        #region 方法

        /// <summary>
        /// 触发项目修改事件。
        /// </summary>
        protected void OnProjectModified()
        {
            ProjectModified?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 触发打开角色声音预览目录请求事件。
        /// </summary>
        protected void OnOpenPreviewVoiceFolderRequested()
        {
            OpenPreviewVoiceFolderRequested?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region private

        #region 字段

        /// <summary>
        /// 语音生成任务队列管理器。
        /// </summary>
        private VoiceGenerationTaskQueue _voiceGenerationTaskQueue;

        /// <summary>
        /// 临时工作目录。
        /// </summary>
        private string _tempFolder = 临时_工作目录;

        #endregion

        #region UI初始化

        /// <summary>
        /// 初始化语音生成UI。
        /// </summary>
        private void init语音生成Ui()
        {
            _voiceGenerationTaskQueue = new VoiceGenerationTaskQueue(
                () => TtlSchemeController?.CurrentEngineConnector,
                () => GetEngineConnectionStatus?.Invoke() ?? TtlEngineConnectionStatus.未连接,
                FfmpegPath ?? Ffmpeg_文件,
                _tempFolder);

            _voiceGenerationTaskQueue.TaskStatusChanged += VoiceGenerationTaskQueue_TaskStatusChanged;
            _voiceGenerationTaskQueue.TaskListChanged += VoiceGenerationTaskQueue_TaskListChanged;
            _voiceGenerationTaskQueue.TaskProgressUpdated += VoiceGenerationTaskQueue_TaskProgressUpdated;
            _voiceGenerationTaskQueue.RequestEngineConnection += VoiceGenerationTaskQueue_RequestEngineConnection;
            _voiceGenerationTaskQueue.PreviewTaskCompleted += VoiceGenerationTaskQueue_PreviewTaskCompleted;
            _voiceGenerationTaskQueue.TaskSubmitInfo += VoiceGenerationTaskQueue_TaskSubmitInfo;

            dgv_语音生成_任务清单.AutoGenerateColumns = false;
            dgv_语音生成_任务清单.AllowUserToAddRows = false;
            dgv_语音生成_任务清单.DataSource = _voiceGenerationTaskQueue.Tasks;

            dgv_语音生成_任务清单.Columns.Clear();
            dgv_语音生成_任务清单.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", DataPropertyName = "Id", HeaderText = "编号", Width = 150, ReadOnly = true });
            dgv_语音生成_任务清单.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", DataPropertyName = "Status", HeaderText = "任务状态", Width = 100, ReadOnly = true });
            dgv_语音生成_任务清单.Columns.Add(new DataGridViewTextBoxColumn { Name = "ShowProgress", DataPropertyName = "ShowProgress", HeaderText = "进度", Width = 80, ReadOnly = true });
            dgv_语音生成_任务清单.Columns.Add(new DataGridViewTextBoxColumn { Name = "ProgressDetail", DataPropertyName = "ProgressDetail", HeaderText = "进度详情", Width = 200, ReadOnly = true });
            dgv_语音生成_任务清单.Columns.Add(new DataGridViewTextBoxColumn { Name = "FileName", DataPropertyName = "FileName", HeaderText = "保存位置", Width = 150, ReadOnly = true });
            dgv_语音生成_任务清单.Columns.Add(new DataGridViewTextBoxColumn { Name = "ShowSpeed", DataPropertyName = "ShowSpeed", HeaderText = "语速", Width = 80, ReadOnly = true });
            dgv_语音生成_任务清单.Columns.Add(new DataGridViewTextBoxColumn { Name = "ShowSpaceTime", DataPropertyName = "ShowSpaceTime", HeaderText = "空白时长", Width = 80, ReadOnly = true });
            var textColumn = new DataGridViewTextBoxColumn { Name = "Text", DataPropertyName = "Text", HeaderText = "文本预览", MinimumWidth = 300, Width = 300, ReadOnly = true };
            textColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv_语音生成_任务清单.Columns.Add(textColumn);

            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null,
                dgv_语音生成_任务清单,
                new object[] { true });

            // 加载"自动开始任务"复选框状态
            bool autoStart = bool.Parse(Setting.GetValue(nameof(cb_语音生成_自动开始任务), true.ToString()));
            cb_语音生成_自动开始任务.Checked = autoStart;
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 事件处理：任务状态变化。
        /// </summary>
        private void VoiceGenerationTaskQueue_TaskStatusChanged(object sender, TaskStatusChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => VoiceGenerationTaskQueue_TaskStatusChanged(sender, e)));
                return;
            }
            refresh语音生成任务清单DataGridView();
            TaskStatusChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 事件处理：任务列表变化。
        /// </summary>
        private void VoiceGenerationTaskQueue_TaskListChanged(object sender, TaskListChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => VoiceGenerationTaskQueue_TaskListChanged(sender, e)));
                return;
            }
            OnProjectModified();
        }

        /// <summary>
        /// 事件处理：任务进度更新。
        /// </summary>
        private void VoiceGenerationTaskQueue_TaskProgressUpdated(object sender, TaskProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => VoiceGenerationTaskQueue_TaskProgressUpdated(sender, e)));
                return;
            }
            refresh语音生成任务清单DataGridView();
        }

        /// <summary>
        /// 事件处理：请求引擎连接。
        /// </summary>
        private void VoiceGenerationTaskQueue_RequestEngineConnection(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => VoiceGenerationTaskQueue_RequestEngineConnection(sender, e)));
                return;
            }
            RequestEngineConnection?.Invoke();
        }

        /// <summary>
        /// 事件处理：预览任务完成。
        /// </summary>
        private void VoiceGenerationTaskQueue_PreviewTaskCompleted(object sender, PreviewTaskCompletedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => VoiceGenerationTaskQueue_PreviewTaskCompleted(sender, e)));
                return;
            }
            PreviewTaskCompleted?.Invoke(this, e);
        }

        /// <summary>
        /// 事件处理：任务提交信息。
        /// </summary>
        private void VoiceGenerationTaskQueue_TaskSubmitInfo(object sender, TaskSubmitInfoEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => VoiceGenerationTaskQueue_TaskSubmitInfo(sender, e)));
                return;
            }
            this.lab_语音生成_任务提交信息.Text = $"正在提交任务 {e.TaskId} 第 {e.ItemIndex} 项：{e.TextPreview}";
        }

        /// <summary>
        /// 事件处理：点击"打开角色声音预览目录"按钮。
        /// </summary>
        private void bt_语音生成_打开角色声音预览目录_Click(object sender, EventArgs e)
        {
            OnOpenPreviewVoiceFolderRequested();
        }

        /// <summary>
        /// 事件处理：自动开始任务复选框状态变更。
        /// </summary>
        private void cb_语音生成_自动开始任务_CheckedChanged(object sender, EventArgs e)
        {
            Setting.SetValue(nameof(cb_语音生成_自动开始任务), cb_语音生成_自动开始任务.Checked.ToString());
        }

        /// <summary>
        /// 事件处理：语音生成任务清单表格鼠标按下。
        /// </summary>
        private void dgv_语音生成_任务清单_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo hitTest = dgv_语音生成_任务清单.HitTest(e.X, e.Y);

                if (hitTest.Type == DataGridViewHitTestType.Cell && hitTest.RowIndex >= 0)
                {
                    DataGridViewRow clickedRow = dgv_语音生成_任务清单.Rows[hitTest.RowIndex];

                    if (clickedRow.Selected)
                    {
                        return;
                    }
                    else
                    {
                        dgv_语音生成_任务清单.ClearSelection();
                        clickedRow.Selected = true;
                    }
                }
            }
        }

        /// <summary>
        /// 事件处理：语音生成任务控制菜单打开。
        /// </summary>
        private void cms_语音生成_任务控制_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool hasSelectedRows = dgv_语音生成_任务清单.SelectedRows.Count > 0;
            int totalRows = dgv_语音生成_任务清单.Rows.Count;
            int selectedCount = dgv_语音生成_任务清单.SelectedRows.Count;

            启动SToolStripMenuItem.Enabled = false;
            停止TToolStripMenuItem.Enabled = false;
            重新启动SToolStripMenuItem.Enabled = false;
            上移UToolStripMenuItem.Enabled = false;
            下移DToolStripMenuItem.Enabled = false;
            预览声音MToolStripMenuItem.Enabled = false;
            打开文件夹FToolStripMenuItem.Enabled = false;
            打开临时文件夹EToolStripMenuItem.Enabled = selectedCount == 1;
            修改存储文件夹MToolStripMenuItem.Enabled = false;
            复制CToolStripMenuItem.Enabled = false;
            删除RToolStripMenuItem.Enabled = false;
            清空所有任务AToolStripMenuItem.Enabled = totalRows > 0;

            if (hasSelectedRows)
            {
                List<VoiceGenerationTask> selectedTasks = new List<VoiceGenerationTask>();
                foreach (DataGridViewRow row in dgv_语音生成_任务清单.SelectedRows)
                {
                    if (row.DataBoundItem is VoiceGenerationTask task)
                    {
                        selectedTasks.Add(task);
                    }
                }

                if (selectedTasks.Count > 0)
                {
                    上移UToolStripMenuItem.Enabled = true;
                    下移DToolStripMenuItem.Enabled = true;
                    删除RToolStripMenuItem.Enabled = true;

                    bool isSingleSelection = selectedCount == 1;
                    打开文件夹FToolStripMenuItem.Enabled = isSingleSelection;
                    复制CToolStripMenuItem.Enabled = isSingleSelection;

                    bool allCanStart = selectedTasks.All(t => t.Status == VoiceGenerationTaskStatus.未开始);
                    bool allCanPause = selectedTasks.All(t => t.Status == VoiceGenerationTaskStatus.排队中 || t.Status == VoiceGenerationTaskStatus.正在生成);
                    bool allCanRestart = selectedTasks.All(t => t.Status == VoiceGenerationTaskStatus.已完成 || t.Status == VoiceGenerationTaskStatus.生成失败);
                    bool allCompleted = selectedTasks.All(t => t.Status == VoiceGenerationTaskStatus.已完成);

                    启动SToolStripMenuItem.Enabled = allCanStart && !allCanRestart;
                    停止TToolStripMenuItem.Enabled = allCanPause;
                    重新启动SToolStripMenuItem.Enabled = allCanRestart;

                    if (allCompleted && isSingleSelection)
                    {
                        VoiceGenerationTask singleTask = selectedTasks[0];
                        bool fileExists = !string.IsNullOrWhiteSpace(singleTask.SaveFile) && File.Exists(singleTask.SaveFile);
                        预览声音MToolStripMenuItem.Enabled = fileExists;
                    }

                    bool hasPreviewTask = selectedTasks.Any(t => isPreviewVoiceTask(t));
                    修改存储文件夹MToolStripMenuItem.Enabled = !hasPreviewTask;
                }
            }
        }

        /// <summary>
        /// 事件处理：点击"清空所有任务"菜单项。
        /// </summary>
        private void 清空所有任务AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool hasRunningTask = _voiceGenerationTaskQueue.Tasks.Any(t =>
                t.Status == VoiceGenerationTaskStatus.正在生成 ||
                t.Status == VoiceGenerationTaskStatus.排队中);

            string message = hasRunningTask
                ? "有任务正在运行中，确定要停止并清空所有任务吗？"
                : "确定要清空所有任务吗？";

            DialogResult result = MessageBox.Show(
                message,
                "确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                if (hasRunningTask)
                {
                    _voiceGenerationTaskQueue.Stop();

                    Task.Run(async () =>
                    {
                        int waitCount = 0;
                        while (_voiceGenerationTaskQueue.IsRunning && waitCount < 50)
                        {
                            await Task.Delay(100);
                            waitCount++;
                        }

                        this.Invoke(new Action(() =>
                        {
                            clearAllTasksAndCleanup();
                        }));
                    });
                }
                else
                {
                    clearAllTasksAndCleanup();
                }
            }
        }

        /// <summary>
        /// 事件处理：点击"启动"菜单项。
        /// </summary>
        private void 启动SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv_语音生成_任务清单.SelectedRows.Count > 0)
            {
                bool hasStarted = false;
                foreach (DataGridViewRow row in dgv_语音生成_任务清单.SelectedRows)
                {
                    VoiceGenerationTask task = row.DataBoundItem as VoiceGenerationTask;
                    if (task != null && task.Status == VoiceGenerationTaskStatus.未开始)
                    {
                        task.Status = VoiceGenerationTaskStatus.排队中;
                        hasStarted = true;
                    }
                }
                if (hasStarted)
                {
                    OnProjectModified();
                    refresh语音生成任务清单DataGridView();
                    _voiceGenerationTaskQueue.Start();
                }
            }
        }

        /// <summary>
        /// 事件处理：点击"暂停"菜单项。
        /// </summary>
        private void 暂停TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv_语音生成_任务清单.SelectedRows.Count > 0)
            {
                bool hasChanged = false;

                foreach (DataGridViewRow row in dgv_语音生成_任务清单.SelectedRows)
                {
                    VoiceGenerationTask task = row.DataBoundItem as VoiceGenerationTask;
                    if (task != null)
                    {
                        if (task.Status == VoiceGenerationTaskStatus.正在生成)
                        {
                            task.Status = VoiceGenerationTaskStatus.未开始;
                            task.ProgressDetail = "已手动停止";
                            _voiceGenerationTaskQueue.Stop();
                            hasChanged = true;
                        }
                        else if (task.Status == VoiceGenerationTaskStatus.排队中)
                        {
                            task.Status = VoiceGenerationTaskStatus.未开始;
                            hasChanged = true;
                        }
                    }
                }

                if (hasChanged)
                {
                    OnProjectModified();
                    refresh语音生成任务清单DataGridView();
                }
            }
        }

        /// <summary>
        /// 事件处理：点击"重新启动"菜单项。
        /// </summary>
        private void 重新启动SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv_语音生成_任务清单.SelectedRows.Count > 0)
            {
                List<VoiceGenerationTask> tasksToRestart = new List<VoiceGenerationTask>();
                bool hasCurrentExecutingTask = false;

                foreach (DataGridViewRow row in dgv_语音生成_任务清单.SelectedRows)
                {
                    VoiceGenerationTask task = row.DataBoundItem as VoiceGenerationTask;
                    if (task != null)
                    {
                        tasksToRestart.Add(task);
                        if (_voiceGenerationTaskQueue.CurrentTask == task)
                        {
                            hasCurrentExecutingTask = true;
                        }
                    }
                }

                if (hasCurrentExecutingTask)
                {
                    _voiceGenerationTaskQueue.Stop();
                    Task.Run(async () =>
                    {
                        int waitCount = 0;
                        while (_voiceGenerationTaskQueue.IsRunning && waitCount < 50)
                        {
                            await Task.Delay(100);
                            waitCount++;
                        }

                        this.Invoke(new Action(() =>
                        {
                            foreach (var task in tasksToRestart)
                            {
                                restartTaskWithVerification(task);
                            }
                            OnProjectModified();
                            refresh语音生成任务清单DataGridView();
                            _voiceGenerationTaskQueue.Start();
                        }));
                    });
                }
                else
                {
                    foreach (var task in tasksToRestart)
                    {
                        restartTaskWithVerification(task);
                    }
                    OnProjectModified();
                    refresh语音生成任务清单DataGridView();
                    _voiceGenerationTaskQueue.Start();
                }
            }
        }

        /// <summary>
        /// 事件处理：点击"删除"菜单项。
        /// </summary>
        private void 删除RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv_语音生成_任务清单.SelectedRows.Count > 0)
            {
                List<VoiceGenerationTask> tasksToDelete = new List<VoiceGenerationTask>();
                bool hasRunningTask = false;

                foreach (DataGridViewRow row in dgv_语音生成_任务清单.SelectedRows)
                {
                    VoiceGenerationTask task = row.DataBoundItem as VoiceGenerationTask;
                    if (task != null)
                    {
                        tasksToDelete.Add(task);
                        if (task.Status == VoiceGenerationTaskStatus.正在生成 ||
                            task.Status == VoiceGenerationTaskStatus.排队中)
                        {
                            hasRunningTask = true;
                        }
                    }
                }

                if (tasksToDelete.Count == 0)
                {
                    return;
                }

                string message = hasRunningTask
                    ? $"选中的 {tasksToDelete.Count} 个任务中有正在运行的任务，确定要停止并删除吗？"
                    : $"确定要删除选中的 {tasksToDelete.Count} 个任务吗？";

                DialogResult result = MessageBox.Show(
                    message,
                    "确认删除",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (result != DialogResult.Yes)
                {
                    return;
                }

                if (hasRunningTask)
                {
                    _voiceGenerationTaskQueue.Stop();
                    Task.Run(async () =>
                    {
                        int waitCount = 0;
                        while (_voiceGenerationTaskQueue.IsRunning && waitCount < 50)
                        {
                            await Task.Delay(100);
                            waitCount++;
                        }

                        this.Invoke(new Action(() =>
                        {
                            deleteTasksAndCleanup(tasksToDelete);
                        }));
                    });
                }
                else
                {
                    deleteTasksAndCleanup(tasksToDelete);
                }
            }
        }

        /// <summary>
        /// 事件处理：点击"上移"菜单项。
        /// </summary>
        private void 上移UToolStripMenuItem_Click(object sender, EventArgs e)
        {
            moveSelectedRowsUp();
        }

        /// <summary>
        /// 事件处理：点击"下移"菜单项。
        /// </summary>
        private void 下移DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            moveSelectedRowsDown();
        }

        /// <summary>
        /// 事件处理：点击"预览声音"菜单项。
        /// </summary>
        private void 预览声音MToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv_语音生成_任务清单.SelectedRows.Count > 0)
            {
                VoiceGenerationTask selectedTask = dgv_语音生成_任务清单.SelectedRows[0].DataBoundItem as VoiceGenerationTask;
                if (selectedTask != null && selectedTask.Status == VoiceGenerationTaskStatus.已完成)
                {
                    if (string.IsNullOrWhiteSpace(selectedTask.SaveFile))
                    {
                        MessageBox.Show("未设置保存文件路径，无法预览。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!File.Exists(selectedTask.SaveFile))
                    {
                        MessageBox.Show($"文件不存在：{selectedTask.SaveFile}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    PlaySound?.Invoke(selectedTask.SaveFile);
                }
            }
        }

        /// <summary>
        /// 事件处理：点击"打开文件夹"菜单项。
        /// </summary>
        private void 打开文件夹FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv_语音生成_任务清单.SelectedRows.Count > 0)
            {
                VoiceGenerationTask selectedTask = dgv_语音生成_任务清单.SelectedRows[0].DataBoundItem as VoiceGenerationTask;
                if (selectedTask != null)
                {
                    string folderPath = Path.GetDirectoryName(selectedTask.SaveFile);
                    if (!string.IsNullOrWhiteSpace(folderPath) && Directory.Exists(folderPath))
                    {
                        Process.Start("explorer.exe", folderPath);
                    }
                }
            }
        }

        /// <summary>
        /// 事件处理：点击"打开临时文件夹"菜单项。
        /// </summary>
        private void 打开临时文件夹EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(_tempFolder))
            {
                Process.Start("explorer.exe", _tempFolder);
            }
            else
            {
                Directory.CreateDirectory(_tempFolder);
                Process.Start("explorer.exe", _tempFolder);
            }
        }

        /// <summary>
        /// 事件处理：点击"修改存储文件夹"菜单项。
        /// </summary>
        private void 修改存储文件夹MToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv_语音生成_任务清单.SelectedRows.Count > 0)
            {
                List<VoiceGenerationTask> selectedTasks = new List<VoiceGenerationTask>();
                foreach (DataGridViewRow row in dgv_语音生成_任务清单.SelectedRows)
                {
                    VoiceGenerationTask task = row.DataBoundItem as VoiceGenerationTask;
                    if (task != null)
                    {
                        selectedTasks.Add(task);
                    }
                }

                if (selectedTasks.Count == 0)
                {
                    return;
                }

                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "选择保存文件夹";
                    folderDialog.ShowNewFolderButton = true;

                    VoiceGenerationTask firstTask = selectedTasks[0];
                    string currentFolder = Path.GetDirectoryName(firstTask.SaveFile);
                    if (!string.IsNullOrWhiteSpace(currentFolder) && Directory.Exists(currentFolder))
                    {
                        folderDialog.SelectedPath = currentFolder;
                    }

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        foreach (var task in selectedTasks)
                        {
                            string fileName = Path.GetFileName(task.SaveFile);
                            task.SaveFile = Path.Combine(folderDialog.SelectedPath, fileName);
                        }
                        OnProjectModified();
                        refresh语音生成任务清单DataGridView();
                    }
                }
            }
        }

        /// <summary>
        /// 事件处理：点击"复制"菜单项。
        /// </summary>
        private void 复制CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgv_语音生成_任务清单.SelectedRows.Count > 0)
            {
                VoiceGenerationTask selectedTask = dgv_语音生成_任务清单.SelectedRows[0].DataBoundItem as VoiceGenerationTask;
                if (selectedTask != null && selectedTask.Items != null && selectedTask.Items.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (var item in selectedTask.Items)
                    {
                        var parts = new List<string>();
                        string speakerName = item.Speaker?.SourceName ?? "未知角色";
                        parts.Add(speakerName);

                        // 添加特性选择信息
                        if (item.FeatureSelections != null && item.FeatureSelections.Count > 0)
                        {
                            foreach (var kvp in item.FeatureSelections)
                            {
                                if (kvp.Value != 0)
                                {
                                    parts.Add($"{kvp.Key}={kvp.Value}");
                                }
                            }
                        }

                        parts.Add(item.Text);

                        sb.AppendLine(string.Join(角色_文本分隔符, parts));

                        for (int i = 0; i < item.EndNewLine; i++)
                        {
                            sb.AppendLine();
                        }
                    }

                    // 复制到剪贴板
                    UiControlHelper.CopyToClipboard(sb.ToString());
                }
            }
        }

        /// <summary>
        /// 事件处理：语音生成任务清单选择变更。
        /// </summary>
        private void dgv_语音生成_任务清单_SelectionChanged(object sender, EventArgs e)
        {
            update语音生成选中行信息();
        }

        /// <summary>
        /// 事件处理：点击"打开临时目录"按钮。
        /// </summary>
        private void bt_语音生成_打开临时目录_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", _tempFolder);
        }

        /// <summary>
        /// 事件处理：点击"设置临时目录"按钮。
        /// </summary>
        private void bt_语音生成_设置临时目录_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "请选择临时工作目录";
                folderBrowserDialog.SelectedPath = _tempFolder;
                folderBrowserDialog.ShowNewFolderButton = true;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    _tempFolder = folderBrowserDialog.SelectedPath;
                    Setting.SetValue(nameof(临时_工作目录), _tempFolder);
                }
            }
        }

        /// <summary>
        /// 事件处理：点击"提前"按钮。
        /// </summary>
        private void bt_语音生成_提前_Click(object sender, EventArgs e)
        {
            moveSelectedRowsUp();
        }

        /// <summary>
        /// 事件处理：点击"错后"按钮。
        /// </summary>
        private void bt_语音生成_错后_Click(object sender, EventArgs e)
        {
            moveSelectedRowsDown();
        }

        #endregion

        #region UI操作

        /// <summary>
        /// 刷新语音生成任务清单DataGridView。
        /// </summary>
        private void refresh语音生成任务清单DataGridView()
        {
            dgv_语音生成_任务清单.Refresh();
        }

        /// <summary>
        /// 更新语音生成选中行信息标签。
        /// </summary>
        private void update语音生成选中行信息()
        {
            int totalCount = dgv_语音生成_任务清单.Rows.Count;
            int selectedCount = dgv_语音生成_任务清单.SelectedRows.Count;
            this.lab_语音生成_选中行信息.Text = $"共有 {totalCount} 行，选中 {selectedCount} 行";
        }

        /// <summary>
        /// 清空所有任务并清理相关资源。
        /// </summary>
        private void clearAllTasksAndCleanup()
        {
            foreach (var task in _voiceGenerationTaskQueue.Tasks)
            {
                if (task.Items != null)
                {
                    foreach (var item in task.Items)
                    {
                        if (!string.IsNullOrWhiteSpace(item.TempFile) && File.Exists(item.TempFile))
                        {
                            try
                            {
                                File.Delete(item.TempFile);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }

            _voiceGenerationTaskQueue.ClearTasks();
            dgv_语音生成_任务清单.DataSource = null;
            dgv_语音生成_任务清单.DataSource = _voiceGenerationTaskQueue.Tasks;
            update语音生成选中行信息();
        }

        #endregion

        #region 业务操作

        /// <summary>
        /// 判断任务是否为预览声音任务。
        /// </summary>
        /// <param name="task">语音生成任务</param>
        /// <returns>是否为预览声音任务</returns>
        private bool isPreviewVoiceTask(VoiceGenerationTask task)
        {
            if (task == null || string.IsNullOrWhiteSpace(task.SaveFile))
            {
                return false;
            }

            string previewFolder = Path.GetFullPath(TTL角色预览声音_文件夹);
            string taskFolder = Path.GetFullPath(Path.GetDirectoryName(task.SaveFile));

            return string.Equals(previewFolder, taskFolder, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 重启任务并验证已完成的子任务。
        /// </summary>
        /// <param name="task">要重启的任务</param>
        private void restartTaskWithVerification(VoiceGenerationTask task)
        {
            task.Status = VoiceGenerationTaskStatus.排队中;
            task.ProgressDetail = string.Empty;
        }

        /// <summary>
        /// 批量删除任务并清理相关资源。
        /// </summary>
        /// <param name="tasks">要删除的任务列表</param>
        private void deleteTasksAndCleanup(List<VoiceGenerationTask> tasks)
        {
            foreach (var task in tasks)
            {
                if (task.Items != null)
                {
                    foreach (var item in task.Items)
                    {
                        if (!string.IsNullOrWhiteSpace(item.TempFile) && File.Exists(item.TempFile))
                        {
                            try
                            {
                                File.Delete(item.TempFile);
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                _voiceGenerationTaskQueue.RemoveTask(task);
            }

            update语音生成选中行信息();
        }

        /// <summary>
        /// 将选中的行向上移动（提前）。
        /// </summary>
        private void moveSelectedRowsUp()
        {
            var selectedRows = dgv_语音生成_任务清单.SelectedRows;
            if (selectedRows.Count == 0)
            {
                return;
            }

            // 记录选中的实例对象
            HashSet<VoiceGenerationTask> selectedItems = new HashSet<VoiceGenerationTask>();
            List<int> selectedIndices = new List<int>();
            foreach (DataGridViewRow row in selectedRows)
            {
                if (row.DataBoundItem is VoiceGenerationTask item)
                {
                    selectedItems.Add(item);
                    selectedIndices.Add(row.Index);
                }
            }
            selectedIndices.Sort();

            // 如果第一行已经选中，则不能向上移动
            if (selectedIndices[0] == 0)
            {
                if (DataGridViewHelper.IsConsecutiveRows(selectedIndices))
                {
                    return;
                }
            }

            // 分析选中行的分组（连续行作为一组）
            List<List<int>> groups = DataGridViewHelper.GroupConsecutiveRows(selectedIndices);

            // 处理每个组（从前向后处理）
            bool hasMoved = false;
            foreach (var group in groups)
            {
                int groupStart = group[0];
                int groupCount = group.Count;

                if (groupStart == 0)
                {
                    continue;
                }

                int rowAbove = groupStart - 1;
                var itemToMove = _voiceGenerationTaskQueue.Tasks[rowAbove];
                _voiceGenerationTaskQueue.Tasks.RemoveAt(rowAbove);
                _voiceGenerationTaskQueue.Tasks.Insert(groupStart - 1 + groupCount, itemToMove);
                hasMoved = true;
            }

            if (hasMoved)
            {
                OnProjectModified();
            }

            DataGridViewHelper.RefreshAndRestoreSelection(dgv_语音生成_任务清单, _voiceGenerationTaskQueue.Tasks, selectedItems);
        }

        /// <summary>
        /// 将选中的行向下移动（错后）。
        /// </summary>
        private void moveSelectedRowsDown()
        {
            var selectedRows = dgv_语音生成_任务清单.SelectedRows;
            if (selectedRows.Count == 0)
            {
                return;
            }

            // 记录选中的实例对象
            HashSet<VoiceGenerationTask> selectedItems = new HashSet<VoiceGenerationTask>();
            List<int> selectedIndices = new List<int>();
            foreach (DataGridViewRow row in selectedRows)
            {
                if (row.DataBoundItem is VoiceGenerationTask item)
                {
                    selectedItems.Add(item);
                    selectedIndices.Add(row.Index);
                }
            }
            selectedIndices.Sort();

            int lastIndex = dgv_语音生成_任务清单.Rows.Count - 1;

            // 如果最后一行已经选中，则不能向下移动
            if (selectedIndices[selectedIndices.Count - 1] == lastIndex)
            {
                if (DataGridViewHelper.IsConsecutiveRows(selectedIndices))
                {
                    return;
                }
            }

            // 分析选中行的分组（连续行作为一组）
            List<List<int>> groups = DataGridViewHelper.GroupConsecutiveRows(selectedIndices);

            // 处理每个组（从后向前处理）
            bool hasMoved = false;
            for (int g = groups.Count - 1; g >= 0; g--)
            {
                var group = groups[g];
                int groupStart = group[0];
                int groupCount = group.Count;
                int groupEnd = groupStart + groupCount - 1;

                if (groupEnd == lastIndex)
                {
                    continue;
                }

                int rowBelow = groupEnd + 1;
                var itemToMove = _voiceGenerationTaskQueue.Tasks[rowBelow];
                _voiceGenerationTaskQueue.Tasks.RemoveAt(rowBelow);
                _voiceGenerationTaskQueue.Tasks.Insert(groupStart, itemToMove);
                hasMoved = true;
            }

            if (hasMoved)
            {
                OnProjectModified();
            }

            DataGridViewHelper.RefreshAndRestoreSelection(dgv_语音生成_任务清单, _voiceGenerationTaskQueue.Tasks, selectedItems);
        }

        /// <summary>
        /// 填充语音生成任务清单
        /// </summary>
        /// <param name="text">包含任务信息的文本</param>
        private void fill语音生成任务清单(string text)
        {
            if (string.IsNullOrWhiteSpace(text) == false)
            {
                // 剥离公共参数
                string folderPath = null;
                string mFileName = null;
                string eFileName = null;
                int speed = -1;
                float spaceTime = -1f;
                {
                    int paramIndex = text.IndexOf(Environment.NewLine);
                    if (paramIndex > 0)
                    {
                        string paramLine = text.Substring(0, paramIndex);
                        if (paramLine.StartsWith(角色_文本分隔符) == true)
                        {
                            var paramStrArray = paramLine.Split(new string[] { 角色_文本分隔符 }, StringSplitOptions.RemoveEmptyEntries);
                            if (paramStrArray.Length >= 3)
                            {
                                folderPath = Path.GetDirectoryName(paramStrArray[0]);
                                mFileName = Path.GetFileNameWithoutExtension(paramStrArray[0]);
                                eFileName = Path.GetExtension(paramStrArray[0]);

                                speed = int.Parse(paramStrArray[1]);
                                spaceTime = float.Parse(paramStrArray[2]);

                                text = text.Substring(paramIndex + 2);
                            }
                        }
                    }
                }

                // 创建任务
                {
                    if (folderPath != null &&
                        mFileName != null &&
                        eFileName != null &&
                        speed > 0 &&
                        spaceTime >= 0)
                    {
                        string[] paragraphArray = text.Split(new string[] { 生成段落_分隔符 }, StringSplitOptions.RemoveEmptyEntries);

                        int fileCountMax = paragraphArray.Length.ToString().Length;
                        int fileCount = 1;
                        {
                            var strArray = FileHelper.SplitTrailingNumber(mFileName);
                            if (strArray != null && strArray.Length > 0)
                            {
                                mFileName = strArray[0];
                                if (strArray.Length > 1) fileCount = int.Parse(strArray[1]);
                            }
                        }

                        foreach (var parapraph in paragraphArray)
                        {
                            VoiceGenerationTask task = new VoiceGenerationTask();
                            task.SaveFile = Path.Combine(folderPath, $"{mFileName}{FileHelper.PadNumber(fileCount++, fileCountMax)}{eFileName}");
                            task.Speed = speed;
                            task.SpaceTime = spaceTime;

                            if (cb_语音生成_自动开始任务.Checked)
                            {
                                task.Status = VoiceGenerationTaskStatus.排队中;
                            }

                            List<VoiceGenerationTaskItem> taskItems = new List<VoiceGenerationTaskItem>();
                            string[] lineArray = parapraph.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                            for (int i = 0; i < lineArray.Length; i++)
                            {
                                var line = lineArray[i];
                                if (string.IsNullOrWhiteSpace(line) == false)
                                {
                                    var itemStrArray = line.Split(new string[] { 角色_文本分隔符 }, StringSplitOptions.RemoveEmptyEntries);
                                    if (itemStrArray.Length >= 2)
                                    {
                                        // 第一个元素是角色
                                        SpeakerInfo speakerInfo = FindSpeaker?.Invoke(itemStrArray[0]);
                                        Dictionary<string, int> featureSelections = new Dictionary<string, int>();
                                        string lineText = string.Empty;

                                        // 解析中间的特性键值对，最后一个元素是文本
                                        for (int j = 1; j < itemStrArray.Length; j++)
                                        {
                                            string part = itemStrArray[j];
                                            // 检查是否是特性键值对（格式：特性名=值）
                                            bool isFeature = false;
                                            if (part.Contains("="))
                                            {
                                                var keyValue = part.Split(new char[] { '=' }, 2);
                                                if (keyValue.Length == 2)
                                                {
                                                    string featureName = keyValue[0];
                                                    // 检查是否是已知特性
                                                    bool isKnownFeature = false;
                                                    var currentEngine = TtlSchemeController?.CurrentEngineConnector;
                                                    if (currentEngine != null)
                                                    {
                                                        foreach (var def in currentEngine.FeatureDefinitions)
                                                        {
                                                            if (def.Name == featureName)
                                                            {
                                                                isKnownFeature = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // 备用方案：使用硬编码的特性名称列表
                                                        string[] knownFeatures = { "方言", "情感风格", "场景" };
                                                        isKnownFeature = knownFeatures.Contains(featureName);
                                                    }

                                                    if (isKnownFeature && int.TryParse(keyValue[1], out int value))
                                                    {
                                                        isFeature = true;
                                                        featureSelections[featureName] = value;
                                                    }
                                                }
                                            }

                                            if (!isFeature)
                                            {
                                                // 不是特性键值对，作为文本内容的一部分
                                                lineText = string.IsNullOrEmpty(lineText) ? part : lineText + 角色_文本分隔符 + part;
                                            }
                                        }

                                        // 如果没有解析到文本
                                        if (string.IsNullOrEmpty(lineText))
                                        {
                                            // 找到最后一个不是特性键值对的元素作为文本
                                            for (int k = itemStrArray.Length - 1; k >= 1; k--)
                                            {
                                                string lastPart = itemStrArray[k];
                                                bool isFeaturePart = false;
                                                if (lastPart.Contains("="))
                                                {
                                                    var kv = lastPart.Split(new char[] { '=' }, 2);
                                                    if (kv.Length == 2)
                                                    {
                                                        string fname = kv[0];
                                                        var currentEngine2 = TtlSchemeController?.CurrentEngineConnector;
                                                        if (currentEngine2 != null)
                                                        {
                                                            foreach (var def in currentEngine2.FeatureDefinitions)
                                                            {
                                                                if (def.Name == fname)
                                                                {
                                                                    isFeaturePart = true;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            string[] knownFeatures = { "方言", "情感风格", "场景" };
                                                            isFeaturePart = knownFeatures.Contains(fname);
                                                        }
                                                    }
                                                }
                                                if (!isFeaturePart)
                                                {
                                                    lineText = lastPart;
                                                    break;
                                                }
                                            }
                                        }

                                        if (speakerInfo != null && string.IsNullOrWhiteSpace(lineText) == false)
                                        {
                                            taskItems.Add(new VoiceGenerationTaskItem
                                            {
                                                TempFile = Path.Combine(_tempFolder, $"temp_{task.Id}_{taskItems.Count + 1}.wav"),
                                                Speaker = speakerInfo,
                                                Text = lineText,
                                                FeatureSelections = featureSelections
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    if (taskItems.Count > 0) taskItems.Last().EndNewLine++;
                                }
                            }

                            if (taskItems.Count > 0)
                            {
                                task.Items = taskItems.ToArray();
                                _voiceGenerationTaskQueue.AddTask(task);
                            }
                        }
                    }
                }
            }

            if (_voiceGenerationTaskQueue.Tasks.Count == 0)
            {
                MessageBox.Show("选定的朗读者不存在，无法生成语音", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            update语音生成选中行信息();

            if (_voiceGenerationTaskQueue.Tasks.Count > 0 && cb_语音生成_自动开始任务.Checked)
            {
                _voiceGenerationTaskQueue.Start();
            }
        }

        /// <summary>
        /// 保存语音生成任务清单到项目文件对象。
        /// </summary>
        private void save语音生成任务清单Data()
        {
            save语音生成任务清单Data(CurrentEngineId);
        }

        /// <summary>
        /// 保存语音生成任务清单到项目文件对象。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        private void save语音生成任务清单Data(string engineId)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return;
            }

            List<CoreVoiceGenerationTask> tasks = new List<CoreVoiceGenerationTask>();
            foreach (VoiceGenerationTask task in _voiceGenerationTaskQueue.Tasks)
            {
                CoreVoiceGenerationTask voiceTask = new CoreVoiceGenerationTask
                {
                    Id = task.Id,
                    Status = task.Status.ToString(),
                    Progress = task.Progress,
                    ProgressDetail = task.ProgressDetail,
                    SaveFile = task.SaveFile,
                    Speed = task.Speed,
                    SpaceTime = task.SpaceTime
                };

                foreach (VoiceGenerationTaskItem item in task.Items)
                {
                    voiceTask.Items.Add(new CoreVoiceGenerationTaskItem
                    {
                        Speaker = item.Speaker?.SourceName ?? string.Empty,
                        Text = item.Text,
                        TempFile = item.TempFile,
                        EndNewLine = item.EndNewLine,
                        FeatureSelections = item.FeatureSelections != null
                            ? new Dictionary<string, int>(item.FeatureSelections)
                            : new Dictionary<string, int>()
                    });
                }

                tasks.Add(voiceTask);
            }
            ProjectFile?.SaveVoiceGenerationTasks(engineId, tasks);
        }

        /// <summary>
        /// 加载语音生成任务清单
        /// </summary>
        private void load语音生成任务清单()
        {
            load语音生成任务清单(CurrentEngineId);
        }

        /// <summary>
        /// 加载语音生成任务清单
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        private void load语音生成任务清单(string engineId)
        {
            List<CoreVoiceGenerationTask> loadedTasks = string.IsNullOrEmpty(engineId)
                ? new List<CoreVoiceGenerationTask>()
                : ProjectFile?.LoadVoiceGenerationTasks(engineId) ?? new List<CoreVoiceGenerationTask>();

            _voiceGenerationTaskQueue.Tasks.RaiseListChangedEvents = false;
            _voiceGenerationTaskQueue.Tasks.Clear();

            bool hasInterruptedTask = false;

            try
            {
                foreach (CoreVoiceGenerationTask task in loadedTasks)
                {
                    VoiceGenerationTask voiceTask = new VoiceGenerationTask
                    {
                        Id = string.IsNullOrEmpty(task.Id) ? MD5Helper.GetShortMd5(Guid.NewGuid().ToString()) : task.Id,
                        Status = (VoiceGenerationTaskStatus)Enum.Parse(typeof(VoiceGenerationTaskStatus), task.Status),
                        Progress = task.Progress,
                        ProgressDetail = task.ProgressDetail,
                        SaveFile = task.SaveFile,
                        Speed = task.Speed,
                        SpaceTime = task.SpaceTime
                    };

                    if (voiceTask.Status == VoiceGenerationTaskStatus.正在生成 ||
                        voiceTask.Status == VoiceGenerationTaskStatus.排队中)
                    {
                        voiceTask.Status = VoiceGenerationTaskStatus.排队中;
                        hasInterruptedTask = true;
                    }

                    List<VoiceGenerationTaskItem> items = new List<VoiceGenerationTaskItem>();
                    foreach (CoreVoiceGenerationTaskItem item in task.Items)
                    {
                        SpeakerInfo speaker = !string.IsNullOrWhiteSpace(item.Speaker) ? new SpeakerInfo(item.Speaker) : null;
                        items.Add(new VoiceGenerationTaskItem
                        {
                            TempFile = item.TempFile,
                            Speaker = speaker,
                            Text = item.Text,
                            EndNewLine = item.EndNewLine,
                            FeatureSelections = item.FeatureSelections != null
                                ? new Dictionary<string, int>(item.FeatureSelections)
                                : new Dictionary<string, int>()
                        });
                    }
                    voiceTask.Items = items.ToArray();

                    _voiceGenerationTaskQueue.AddTask(voiceTask);
                }
            }
            finally
            {
                _voiceGenerationTaskQueue.Tasks.RaiseListChangedEvents = true;
                _voiceGenerationTaskQueue.Tasks.ResetBindings();
            }

            update语音生成选中行信息();

            if (hasInterruptedTask)
            {
                RequestEngineConnection?.Invoke();
            }
        }

        #endregion

        #endregion
    }
}
