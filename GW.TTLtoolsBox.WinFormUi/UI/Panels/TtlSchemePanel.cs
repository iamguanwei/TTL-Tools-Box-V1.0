using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GW.TTLtoolsBox.Core.FileAccesser;
using GW.TTLtoolsBox.Core.TtlEngine;
using GW.TTLtoolsBox.WinFormUi.Base;
using GW.TTLtoolsBox.WinFormUi.Helper;
using GW.TTLtoolsBox.WinFormUi.Manager;

namespace GW.TTLtoolsBox.WinFormUi.UI.Panels
{
    /// <summary>
    /// TTL方案面板，提供TTL引擎方案配置和连接管理功能。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 管理TTL引擎方案的选择和配置
    /// - 处理TTL引擎的连接状态管理
    /// - 管理朗读者列表和声音预览
    /// 
    /// 使用场景：
    /// - 在TTL工具箱中配置TTL引擎连接参数
    /// - 管理不同TTL引擎方案的切换
    /// - 提供朗读者声音预览功能
    /// 
    /// 依赖关系：
    /// - 依赖ConnectionManager管理TTL引擎方案
    /// - 依赖PreviewVoiceManager进行声音预览
    /// - 依赖VoiceGenerationTaskQueue管理语音生成任务
    /// </remarks>
    public partial class TtlSchemePanel : ViewBase
    {
        #region 常量

        /// <summary>
        /// TTL角色预览声音的生成文件。
        /// </summary>
        private readonly string TTL角色预览声音_文件;

        /// <summary>
        /// TTL角色预览声音文件夹路径。
        /// </summary>
        private readonly string TTL角色预览声音_文件夹路径;

        #endregion

        #region public

        #region 事件

        /// <summary>
        /// 项目内容被修改时触发的事件。
        /// </summary>
        public event EventHandler ProjectModified;

        /// <summary>
        /// TTL引擎选择改变时触发的事件。
        /// </summary>
        public event EventHandler<TtlEngineChangedEventArgs> EngineChanged;

        /// <summary>
        /// 朗读者列表改变时触发的事件。
        /// </summary>
        public event EventHandler SpeakersChanged;

        /// <summary>
        /// 请求预览声音时触发的事件。
        /// </summary>
        public event EventHandler<PreviewVoiceRequestEventArgs> PreviewVoiceRequested;

        /// <summary>
        /// TTL引擎连接状态改变时触发的事件。
        /// </summary>
        public event EventHandler<TtlEngineConnectionStatusEventArgs> ConnectionStatusChanged;

        /// <summary>
        /// 请求保存语音生成任务清单时触发的事件。
        /// </summary>
        public event EventHandler SaveVoiceGenerationTasksRequested;

        /// <summary>
        /// 请求停止语音生成任务队列时触发的事件。
        /// </summary>
        public event EventHandler StopVoiceGenerationTaskQueueRequested;

        /// <summary>
        /// 请求开始连接TTL引擎时触发的事件。
        /// </summary>
        public event EventHandler StartConnectTtlEngineRequested;

        /// <summary>
        /// 请求更新连接状态标签时触发的事件。
        /// </summary>
        public event EventHandler UpdateConnectionStatusLabelRequested;

        /// <summary>
        /// 请求设置默认朗读者时触发的事件。
        /// </summary>
        public event EventHandler<SetDefaultSpeakerEventArgs> SetDefaultSpeakerRequested;

        /// <summary>
        /// 请求添加角色映射时触发的事件。
        /// </summary>
        public event EventHandler<AddRoleMappingEventArgs> AddRoleMappingRequested;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化TtlSchemePanel类的新实例。
        /// </summary>
        public TtlSchemePanel()
        {
            TTL角色预览声音_文件夹路径 = Constants.TTL角色预览声音_文件夹;
            TTL角色预览声音_文件 = Path.Combine(TTL角色预览声音_文件夹路径, "角色声音预览文本.txt");
            InitializeComponent();
        }

        /// <summary>
        /// 初始化TtlSchemePanel类的新实例。
        /// </summary>
        /// <param name="tempFolder">临时文件夹路径（已废弃，请使用无参构造函数）。</param>
        [Obsolete("请使用无参构造函数，预览文件夹已固定为程序目录下的RoldVoice文件夹。")]
        public TtlSchemePanel(string tempFolder) : this()
        {
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置TTL引擎连接管理器。
        /// </summary>
        public TtlEngineConnectionManager ConnectionManager { get; set; }

        /// <summary>
        /// 获取或设置预览声音管理器。
        /// </summary>
        public PreviewVoiceManager PreviewVoiceManager { get; set; }

        private VoiceGenerationTaskQueue _voiceGenerationTaskQueue;

        /// <summary>
        /// 获取或设置语音生成任务队列。
        /// </summary>
        public VoiceGenerationTaskQueue VoiceGenerationTaskQueue
        {
            get => _voiceGenerationTaskQueue;
            set
            {
                if (_voiceGenerationTaskQueue != null)
                {
                    _voiceGenerationTaskQueue.PreviewTaskCompleted -= VoiceGenerationTaskQueue_PreviewTaskCompleted;
                }
                _voiceGenerationTaskQueue = value;
                if (_voiceGenerationTaskQueue != null)
                {
                    _voiceGenerationTaskQueue.PreviewTaskCompleted += VoiceGenerationTaskQueue_PreviewTaskCompleted;
                }
            }
        }

        /// <summary>
        /// 事件处理：预览任务完成。
        /// </summary>
        private void VoiceGenerationTaskQueue_PreviewTaskCompleted(object sender, PreviewTaskCompletedEventArgs e)
        {
            string engineId = getCurrentEngineId();
            if (string.IsNullOrWhiteSpace(engineId) || string.IsNullOrWhiteSpace(e.SourceName))
            {
                return;
            }

            if (e.Success)
            {
                PreviewVoiceManager?.SetStatus(engineId, e.SourceName, PreviewVoiceStatus.已生成, e.Speed, e.Volume);
            }
            else
            {
                PreviewVoiceManager?.SetStatus(engineId, e.SourceName, PreviewVoiceStatus.生成失败, e.Speed, e.Volume);
            }
        }

        /// <summary>
        /// 获取或设置项目文件实例。
        /// </summary>
        public ProjectFile ProjectFile { get; set; }

        /// <summary>
        /// 获取当前TTL引擎连接状态。
        /// </summary>
        public TtlEngineConnectionStatus ConnectionStatus => _ttlEngineConnectionStatus;

        /// <summary>
        /// 获取当前引擎ID。
        /// </summary>
        public string CurrentEngineId => ConnectionManager?.CurrentEngineId;

        /// <summary>
        /// 获取或设置连接状态标签控件。
        /// </summary>
        public ToolStripStatusLabel ConnectionStatusLabel { get; set; }

        /// <summary>
        /// 获取或设置语音生成选项卡页。
        /// </summary>
        public TabPage VoiceGenerationTabPage { get; set; }

        /// <summary>
        /// 连接成功事件（从非成功状态变为成功状态时触发）
        /// </summary>
        public event EventHandler ConnectionSucceeded;

        #endregion

        #region 方法

        /// <summary>
        /// 初始化面板。
        /// </summary>
        public void InitializePanel()
        {
            initTTL方案Ui();

            if (ConnectionManager != null)
            {
                ConnectionManager.ConnectionStatusChanged += ConnectionManager_ConnectionStatusChanged;
            }
        }

        private void ConnectionManager_ConnectionStatusChanged(object sender, TtlEngineConnectionManager.ConnectionStatusChangedEventArgs e)
        {
            _ttlEngineConnectionStatus = e.Status;
            _ttlEngineConnectionCountdown = e.Countdown;

            updateTtlEngineConnectionStatusLabel();
            OnConnectionStatusChanged(e.Status, e.Countdown);

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
                        this.Invoke(new Action(() => bindSpeakersToGrid(currentEngine, true)));
                    }
                }

                VoiceGenerationTaskQueue?.TryResume();
                
                OnConnectionSucceeded();
            }
        }

        /// <summary>
        /// 触发连接成功事件。
        /// </summary>
        protected void OnConnectionSucceeded()
        {
            ConnectionSucceeded?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 刷新面板UI状态。
        /// </summary>
        public override void RefreshUi()
        {
            refreshTTL方案Ui();
        }

        /// <summary>
        /// 初始化TTL方案UI。
        /// </summary>
        public void InitTtlSchemeUi()
        {
            initTTL方案Ui();
        }

        /// <summary>
        /// 如果已选择TTL引擎，则启动连接。
        /// </summary>
        public void StartTtlEngineConnectionIfSelected()
        {
            startTtlEngineConnectionIfSelected();
        }

        /// <summary>
        /// 开始连接TTL引擎。
        /// </summary>
        public void StartConnectTtlEngine()
        {
            start连接TTL引擎();
        }

        /// <summary>
        /// 更新TTL引擎连接状态标签。
        /// </summary>
        public void UpdateConnectionStatusLabel()
        {
            updateTtlEngineConnectionStatusLabel();
        }

        /// <summary>
        /// 获取当前引擎ID。
        /// </summary>
        /// <returns>当前引擎ID。</returns>
        public string GetCurrentEngineId()
        {
            return getCurrentEngineId();
        }

        /// <summary>
        /// 请求生成预览声音。
        /// </summary>
        /// <param name="sourceName">朗读者源名称。</param>
        public void RequestPreviewVoice(string sourceName)
        {
            requestPreviewVoice(sourceName);
        }

        /// <summary>
        /// 获取预览声音状态。
        /// </summary>
        /// <param name="sourceName">朗读者源名称。</param>
        /// <returns>预览声音状态。</returns>
        public PreviewVoiceStatus GetPreviewVoiceStatus(string sourceName)
        {
            return getPreviewVoiceStatus(sourceName);
        }

        /// <summary>
        /// 更新"自动生成全部朗读者预览音频"按钮的启用状态。
        /// </summary>
        /// <param name="refreshStatus">是否先刷新预览状态，默认为false。</param>
        public void UpdateAutoGeneratePreviewButtonStatus(bool refreshStatus = false)
        {
            update自动生成全部朗读者预览音频按钮状态(refreshStatus);
        }

        /// <summary>
        /// 设置连接状态。
        /// </summary>
        /// <param name="status">连接状态。</param>
        /// <param name="countdown">连接倒计时。</param>
        public void SetConnectionStatus(TtlEngineConnectionStatus status, int countdown = 0)
        {
            _ttlEngineConnectionStatus = status;
            _ttlEngineConnectionCountdown = countdown;
        }

        /// <summary>
        /// 获取连接倒计时。
        /// </summary>
        public int ConnectionCountdown => _ttlEngineConnectionCountdown;

        /// <summary>
        /// 刷新指定源名称的朗读者行。
        /// </summary>
        /// <param name="sourceName">源名称。</param>
        public void RefreshSpeakerRow(string sourceName)
        {
            refreshTTL方案_朗读者参数配置Row(sourceName);
        }

        /// <summary>
        /// 刷新朗读者列表。
        /// </summary>
        public void RefreshSpeakers()
        {
            var currentEngine = ConnectionManager?.CurrentEngine;
            if (currentEngine != null)
            {
                createSpeakersFromRoles(currentEngine, true);
            }
        }

        /// <summary>
        /// 设置查找SpeakerInfo的委托。
        /// </summary>
        /// <param name="findSpeakerByShowName">查找SpeakerInfo的委托。</param>
        public void SetFindSpeakerDelegate(Func<string, SpeakerInfo> findSpeakerByShowName)
        {
            _findSpeakerByShowName = findSpeakerByShowName;
        }

        /// <summary>
        /// 设置播放声音的委托。
        /// </summary>
        /// <param name="playSound">播放声音的委托。</param>
        public void SetPlaySoundDelegate(Action<string> playSound)
        {
            _playSound = playSound;
        }

        /// <summary>
        /// 设置获取预览文本的委托。
        /// </summary>
        /// <param name="getPreviewText">获取预览文本的委托。</param>
        public void SetGetPreviewTextDelegate(Func<string> getPreviewText)
        {
            _getPreviewText = getPreviewText;
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
        /// 触发引擎选择改变事件。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        /// <param name="previousEngineId">前一个引擎ID。</param>
        protected void OnEngineChanged(string engineId, string previousEngineId)
        {
            EngineChanged?.Invoke(this, new TtlEngineChangedEventArgs(engineId, previousEngineId));
        }

        /// <summary>
        /// 触发朗读者列表改变事件。
        /// </summary>
        protected void OnSpeakersChanged()
        {
            SpeakersChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 触发预览声音请求事件。
        /// </summary>
        /// <param name="sourceName">朗读者源名称。</param>
        protected void OnPreviewVoiceRequested(string sourceName)
        {
            PreviewVoiceRequested?.Invoke(this, new PreviewVoiceRequestEventArgs(sourceName));
        }

        /// <summary>
        /// 触发连接状态改变事件。
        /// </summary>
        /// <param name="status">连接状态。</param>
        /// <param name="countdown">倒计时。</param>
        protected void OnConnectionStatusChanged(TtlEngineConnectionStatus status, int countdown)
        {
            ConnectionStatusChanged?.Invoke(this, new TtlEngineConnectionStatusEventArgs(status, countdown));
        }

        /// <summary>
        /// 触发保存语音生成任务清单请求事件。
        /// </summary>
        protected void OnSaveVoiceGenerationTasksRequested()
        {
            SaveVoiceGenerationTasksRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 触发停止语音生成任务队列请求事件。
        /// </summary>
        protected void OnStopVoiceGenerationTaskQueueRequested()
        {
            StopVoiceGenerationTaskQueueRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 触发开始连接TTL引擎请求事件。
        /// </summary>
        protected void OnStartConnectTtlEngineRequested()
        {
            StartConnectTtlEngineRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 触发更新连接状态标签请求事件。
        /// </summary>
        protected void OnUpdateConnectionStatusLabelRequested()
        {
            UpdateConnectionStatusLabelRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 触发设置默认朗读者请求事件。
        /// </summary>
        /// <param name="speaker">朗读者信息。</param>
        protected void OnSetDefaultSpeakerRequested(SpeakerInfo speaker)
        {
            SetDefaultSpeakerRequested?.Invoke(this, new SetDefaultSpeakerEventArgs(speaker));
        }

        /// <summary>
        /// 触发添加角色映射请求事件。
        /// </summary>
        /// <param name="sourceName">源名称。</param>
        protected void OnAddRoleMappingRequested(string sourceName)
        {
            AddRoleMappingRequested?.Invoke(this, new AddRoleMappingEventArgs(sourceName));
        }

        #endregion

        #endregion

        #region private

        #region 字段

        /// <summary>
        /// TTL引擎当前连接状态。
        /// </summary>
        private TtlEngineConnectionStatus _ttlEngineConnectionStatus = TtlEngineConnectionStatus.未连接;

        /// <summary>
        /// TTL引擎连接倒计时（秒）。
        /// </summary>
        private int _ttlEngineConnectionCountdown = 0;

        /// <summary>
        /// 上一次的连接状态，用于检测状态变化。
        /// </summary>
        private TtlEngineConnectionStatus _lastConnectionStatus = TtlEngineConnectionStatus.未连接;

        /// <summary>
        /// 查找SpeakerInfo的委托。
        /// </summary>
        private Func<string, SpeakerInfo> _findSpeakerByShowName;

        /// <summary>
        /// 播放声音的委托。
        /// </summary>
        private Action<string> _playSound;

        /// <summary>
        /// 获取预览文本的委托。
        /// </summary>
        private Func<string> _getPreviewText;

        /// <summary>
        /// 保存的朗读者表格滚动位置索引，用于还原配置后恢复滚动位置。
        /// </summary>
        private int _savedScrollRowIndex = -1;

        #endregion

        #region UI初始化

        /// <summary>
        /// 初始化TTL方案UI。
        /// </summary>
        private void initTTL方案Ui()
        {
            this.tb_多音字替换_最终文本.MaxLength = int.MaxValue;

            // 当前方案
            {
                ConnectionManager?.Initialize();

                // 初始化下拉选项
                this.cb_TTL方案_当前方案名称.Items.Clear();

                // 添加"无"选项
                this.cb_TTL方案_当前方案名称.Items.Add("无");

                // 添加所有可用的TTL引擎连接器名称，格式为"名称 [ID = id]"
                if (ConnectionManager != null)
                {
                    foreach (var connector in ConnectionManager.AllEngines)
                    {
                        this.cb_TTL方案_当前方案名称.Items.Add(ConnectionManager.GetEngineDisplayText(connector));
                    }
                }

                // 默认选择第一个选项（使用ID进行匹配）
                if (this.cb_TTL方案_当前方案名称.Items.Count > 0)
                {
                    var savedId = Setting.GetValue($"{this.cb_TTL方案_当前方案名称.Name}_Id", string.Empty);
                    if (!string.IsNullOrEmpty(savedId))
                    {
                        // 根据ID查找对应的显示项
                        var connector = ConnectionManager?.AllEngines.FirstOrDefault(c => c.Id == savedId);
                        if (connector != null)
                        {
                            this.cb_TTL方案_当前方案名称.SelectedItem = ConnectionManager.GetEngineDisplayText(connector);
                        }
                        else
                        {
                            this.cb_TTL方案_当前方案名称.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        // 兼容旧版：尝试使用名称匹配
                        var oldName = Setting.GetValue(this.cb_TTL方案_当前方案名称.Name, string.Empty);
                        if (!string.IsNullOrEmpty(oldName))
                        {
                            var connector = ConnectionManager?.AllEngines.FirstOrDefault(c => c.Name == oldName);
                            if (connector != null)
                            {
                                this.cb_TTL方案_当前方案名称.SelectedItem = ConnectionManager.GetEngineDisplayText(connector);
                            }
                            else
                            {
                                this.cb_TTL方案_当前方案名称.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            this.cb_TTL方案_当前方案名称.SelectedIndex = 0;
                        }
                    }
                }
            }

            // 配置
            {
                if (ConnectionManager != null)
                {
                    ConnectionManager.IsTtlEditing = false;
                }
            }

            refreshTTL方案Ui();
        }

        #endregion

        #region UI操作

        /// <summary>
        /// 刷新TTL方案UI。
        /// </summary>
        private void refreshTTL方案Ui()
        {
            var currentEngine = ConnectionManager?.CurrentEngine;
            var isTtlEditing = ConnectionManager?.IsTtlEditing ?? false;

            {
                this.cb_TTL方案_当前方案名称.Enabled = !isTtlEditing;
            }

            {
                this.tb_TTL方案_连接参数配置.ReadOnly = !isTtlEditing;
                this.dgv_TTL方案_朗读者参数配置.ReadOnly = !isTtlEditing;

                bool hasEngineConnector = currentEngine != null;
                bool isEngineConnected = hasEngineConnector && _ttlEngineConnectionStatus == TtlEngineConnectionStatus.连接成功;
                this.bt_TTL方案_编辑配置.Enabled = hasEngineConnector;
                this.bt_TTL方案_保存配置.Enabled = this.bt_TTL方案_还原配置.Enabled = isTtlEditing;
                this.bt_TTL方案_重新加载朗读者.Enabled = !isTtlEditing && isEngineConnected;

                update自动生成全部朗读者预览音频按钮状态();

                if (isTtlEditing && currentEngine != null)
                {
                    if (string.IsNullOrWhiteSpace(this.tb_TTL方案_连接参数配置.Text))
                    {
                        if (currentEngine.ParameterDescriptions != null && currentEngine.ParameterDescriptions.Length > 0)
                        {
                            this.tb_TTL方案_连接参数配置.Text = string.Join("\r\n", currentEngine.ParameterDescriptions);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 如果已选择TTL引擎，则启动连接。
        /// </summary>
        private void startTtlEngineConnectionIfSelected()
        {
            var currentEngine = ConnectionManager?.CurrentEngine;
            if (currentEngine != null)
            {
                _ttlEngineConnectionStatus = TtlEngineConnectionStatus.未连接;
                start连接TTL引擎();
            }
            updateTtlEngineConnectionStatusLabel();

            if (VoiceGenerationTabPage != null)
            {
                VoiceGenerationTabPage.Enabled = currentEngine != null;
            }
        }

        /// <summary>
        /// 开始连接TTL引擎。
        /// </summary>
        private async void start连接TTL引擎()
        {
            var currentEngine = ConnectionManager?.CurrentEngine;
            if (currentEngine == null)
            {
                _ttlEngineConnectionStatus = TtlEngineConnectionStatus.未连接;
                OnConnectionStatusChanged(_ttlEngineConnectionStatus, 0);
                updateTtlEngineConnectionStatusLabel();
                refreshTTL方案Ui();
                return;
            }

            await ConnectionManager.ConnectAsync();

            if (ConnectionManager.ConnectionStatus == TtlEngineConnectionStatus.连接成功)
            {
                VoiceGenerationTaskQueue?.TryResume();
            }

            updateTtlEngineConnectionStatusLabel();
            refreshTTL方案Ui();
        }

        /// <summary>
        /// 静默验证TTL引擎连接状态（不刷新表格）。
        /// </summary>
        private async void verifyTtlEngineConnectionSilently()
        {
            var currentEngine = ConnectionManager?.CurrentEngine;
            if (currentEngine == null)
            {
                _ttlEngineConnectionStatus = TtlEngineConnectionStatus.未连接;
                OnConnectionStatusChanged(_ttlEngineConnectionStatus, 0);
                updateTtlEngineConnectionStatusLabel();
                refreshTTL方案Ui();
                return;
            }

            await ConnectionManager.ConnectAsync();

            updateTtlEngineConnectionStatusLabel();
        }

        /// <summary>
        /// 更新TTL引擎连接状态标签。
        /// </summary>
        private void updateTtlEngineConnectionStatusLabel()
        {
            Action action = () =>
            {
                string statusText = ConnectionManager?.GetConnectionStatusText() ?? "未选择TTL引擎";
                var status = ConnectionManager?.ConnectionStatus ?? TtlEngineConnectionStatus.未连接;
                Color statusColor;
                switch (status)
                {
                    case TtlEngineConnectionStatus.连接成功:
                        statusColor = Color.FromArgb(0, 150, 0);
                        break;
                    case TtlEngineConnectionStatus.连接失败:
                        statusColor = Color.FromArgb(200, 0, 0);
                        break;
                    default:
                        statusColor = SystemColors.ControlText;
                        break;
                }

                if (ConnectionStatusLabel != null)
                {
                    ConnectionStatusLabel.Text = statusText;
                    ConnectionStatusLabel.ForeColor = statusColor;
                }
            };

            UpdateUi(action);
        }

        /// <summary>
        /// 获取当前TTL引擎的ID，如果没有选定引擎则返回表示"无"方案的特定ID。
        /// </summary>
        /// <returns>当前TTL引擎的ID，或表示"无"方案的特定ID。</returns>
        private string getCurrentEngineId()
        {
            return ConnectionManager?.CurrentEngineId;
        }

        /// <summary>
        /// 从TTL引擎获取角色列表并创建SpeakerInfo对象。
        /// </summary>
        /// <param name="ttl">TTL引擎连接器。</param>
        /// <param name="refreshFromEngine">是否从引擎刷新朗读者列表，默认为false。</param>
        private void createSpeakersFromRoles(ITtlEngineConnector ttl, bool refreshFromEngine = false)
        {
            try
            {
                // 如果需要从引擎刷新，在后台线程执行
                if (refreshFromEngine)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            ttl.RefreshSpeakers();
                            this.Invoke(new Action(() => bindSpeakersToGrid(ttl, true)));
                        }
                        catch
                        {
                            // 无代码
                        }
                    });
                }
                else
                {
                    bindSpeakersToGrid(ttl, true);
                }
            }
            catch (Exception ex)
            {
                // 如果获取角色失败，设置为null
                this.dgv_TTL方案_朗读者参数配置.DataSource = null;
            }
        }

        /// <summary>
        /// 将朗读者列表绑定到表格。
        /// </summary>
        /// <param name="ttl">TTL引擎连接器。</param>
        /// <param name="forceRefresh">是否强制刷新。</param>
        private void bindSpeakersToGrid(ITtlEngineConnector ttl, bool forceRefresh = false)
        {
            var savedSpeakers = new Dictionary<string, SpeakerInfo>();
            foreach (var k in Setting.GetAllKeys())
            {
                if (k.StartsWith($"Speaker_{ttl.Id}_"))
                {
                    string speakerName = k.Substring($"Speaker_{ttl.Id}_".Length);
                    string savedValue = Setting.GetValue(k, string.Empty);
                    if (!string.IsNullOrEmpty(savedValue))
                    {
                        var speaker = new SpeakerInfo(speakerName);
                        if (speaker.TryFromString(savedValue))
                        {
                            savedSpeakers[speakerName] = speaker;
                        }
                    }
                }
            }

            if (ttl.Speakers != null && savedSpeakers.Count > 0)
            {
                for (int i = 0; i < ttl.Speakers.Length; i++)
                {
                    var speaker = ttl.Speakers[i];
                    if (savedSpeakers.TryGetValue(speaker.SourceName, out var savedSpeaker))
                    {
                        ttl.Speakers[i] = savedSpeaker;
                    }
                }
            }

            var speakerArray = ttl.Speakers.Select(s => s.Clone()).ToArray();

            if (!forceRefresh && !hasSpeakerDataChanged(speakerArray))
            {
                return;
            }

            int scrollRowIndex = _savedScrollRowIndex >= 0
                ? _savedScrollRowIndex
                : this.dgv_TTL方案_朗读者参数配置.FirstDisplayedScrollingRowIndex;

            if (speakerArray != null && speakerArray.Length > 0)
            {
                setupSpeakerGridColumns();
                this.dgv_TTL方案_朗读者参数配置.DataSource = speakerArray;
                this.dgv_TTL方案_朗读者参数配置.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                if (scrollRowIndex >= 0 && scrollRowIndex < speakerArray.Length)
                {
                    this.dgv_TTL方案_朗读者参数配置.FirstDisplayedScrollingRowIndex = scrollRowIndex;
                }
            }
            else
            {
                this.dgv_TTL方案_朗读者参数配置.DataSource = null;
            }

            _savedScrollRowIndex = -1;

            OnSpeakersChanged();
            update自动生成全部朗读者预览音频按钮状态(true);
        }

        /// <summary>
        /// 检查朗读者数据是否发生变化。
        /// </summary>
        /// <param name="newSpeakers">新的朗读者数组。</param>
        /// <returns>是否发生变化。</returns>
        private bool hasSpeakerDataChanged(SpeakerInfo[] newSpeakers)
        {
            var currentData = this.dgv_TTL方案_朗读者参数配置.DataSource as SpeakerInfo[];
            if (currentData == null && newSpeakers == null)
            {
                return false;
            }

            if (currentData == null || newSpeakers == null)
            {
                return true;
            }

            if (currentData.Length != newSpeakers.Length)
            {
                return true;
            }

            for (int i = 0; i < currentData.Length; i++)
            {
                if (currentData[i].SourceName != newSpeakers[i].SourceName ||
                    currentData[i].Speed != newSpeakers[i].Speed ||
                    currentData[i].Volume != newSpeakers[i].Volume ||
                    currentData[i].Remark != newSpeakers[i].Remark)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 设置朗读者表格的列属性，包含源名称、速度、音量、备注和声音预览五列。
        /// </summary>
        private void setupSpeakerGridColumns()
        {
            this.dgv_TTL方案_朗读者参数配置.AutoGenerateColumns = false;
            this.dgv_TTL方案_朗读者参数配置.ShowCellToolTips = false;

            this.dgv_TTL方案_朗读者参数配置.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgv_TTL方案_朗读者参数配置.DefaultCellStyle.SelectionBackColor = this.dgv_TTL方案_朗读者参数配置.DefaultCellStyle.BackColor;
            this.dgv_TTL方案_朗读者参数配置.DefaultCellStyle.SelectionForeColor = this.dgv_TTL方案_朗读者参数配置.DefaultCellStyle.ForeColor;

            this.dgv_TTL方案_朗读者参数配置.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            UiControlHelper.SetupDataGridViewBasicStyle(this.dgv_TTL方案_朗读者参数配置);

            this.dgv_TTL方案_朗读者参数配置.Columns.Clear();

            DataGridViewTextBoxColumn sourceNameColumn = new DataGridViewTextBoxColumn();
            sourceNameColumn.Name = sourceNameColumn.DataPropertyName = "SourceName";
            sourceNameColumn.HeaderText = "源名称";
            sourceNameColumn.MinimumWidth = 200;
            sourceNameColumn.ReadOnly = true;
            this.dgv_TTL方案_朗读者参数配置.Columns.Add(sourceNameColumn);

            DataGridViewTextBoxColumn speedColumn = new DataGridViewTextBoxColumn();
            speedColumn.Name = speedColumn.DataPropertyName = "Speed";
            speedColumn.HeaderText = "速度(%)";
            speedColumn.MinimumWidth = 80;
            speedColumn.ReadOnly = false;
            speedColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgv_TTL方案_朗读者参数配置.Columns.Add(speedColumn);

            DataGridViewTextBoxColumn volumeColumn = new DataGridViewTextBoxColumn();
            volumeColumn.Name = volumeColumn.DataPropertyName = "Volume";
            volumeColumn.HeaderText = "音量(%)";
            volumeColumn.MinimumWidth = 80;
            volumeColumn.ReadOnly = false;
            volumeColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgv_TTL方案_朗读者参数配置.Columns.Add(volumeColumn);

            DataGridViewTextBoxColumn remarkColumn = new DataGridViewTextBoxColumn();
            remarkColumn.Name = remarkColumn.DataPropertyName = "Remark";
            remarkColumn.HeaderText = "备注";
            remarkColumn.MinimumWidth = 150;
            remarkColumn.ReadOnly = false;
            this.dgv_TTL方案_朗读者参数配置.Columns.Add(remarkColumn);

            DataGridViewButtonColumn voicePreviewColumn = new DataGridViewButtonColumn();
            voicePreviewColumn.Name = "VoicePreview";
            voicePreviewColumn.HeaderText = "声音预览";
            voicePreviewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            voicePreviewColumn.MinimumWidth = 150;
            voicePreviewColumn.FlatStyle = FlatStyle.Flat;
            voicePreviewColumn.UseColumnTextForButtonValue = false;
            this.dgv_TTL方案_朗读者参数配置.Columns.Add(voicePreviewColumn);

            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null,
                this.dgv_TTL方案_朗读者参数配置,
                new object[] { true });

            this.dgv_TTL方案_朗读者参数配置.CellClick -= dgv_TTL方案_朗读者参数配置_CellClick;
            this.dgv_TTL方案_朗读者参数配置.CellClick += dgv_TTL方案_朗读者参数配置_CellClick;

            this.dgv_TTL方案_朗读者参数配置.CellFormatting -= dgv_TTL方案_朗读者参数配置_CellFormatting;
            this.dgv_TTL方案_朗读者参数配置.CellFormatting += dgv_TTL方案_朗读者参数配置_CellFormatting;

            this.dgv_TTL方案_朗读者参数配置.CellValidating -= dgv_TTL方案_朗读者参数配置_CellValidating;
            this.dgv_TTL方案_朗读者参数配置.CellValidating += dgv_TTL方案_朗读者参数配置_CellValidating;

            this.dgv_TTL方案_朗读者参数配置.CellEndEdit -= dgv_TTL方案_朗读者参数配置_CellEndEdit;
            this.dgv_TTL方案_朗读者参数配置.CellEndEdit += dgv_TTL方案_朗读者参数配置_CellEndEdit;

            this.dgv_TTL方案_朗读者参数配置.MouseDown -= dgv_TTL方案_朗读者参数配置_MouseDown;
            this.dgv_TTL方案_朗读者参数配置.MouseDown += dgv_TTL方案_朗读者参数配置_MouseDown;

            this.dgv_TTL方案_朗读者参数配置.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        /// <summary>
        /// 刷新TTL方案朗读者参数配置表格中指定源名称的行。
        /// </summary>
        /// <param name="sourceName">源名称。</param>
        private void refreshTTL方案_朗读者参数配置Row(string sourceName)
        {
            if (string.IsNullOrWhiteSpace(sourceName))
            {
                return;
            }

            foreach (DataGridViewRow row in this.dgv_TTL方案_朗读者参数配置.Rows)
            {
                if (row.DataBoundItem is SpeakerInfo speaker && speaker.SourceName == sourceName)
                {
                    this.dgv_TTL方案_朗读者参数配置.InvalidateRow(row.Index);
                    break;
                }
            }
        }

        /// <summary>
        /// 更新"自动生成全部朗读者预览音频"按钮的启用状态。
        /// </summary>
        /// <param name="refreshStatus">是否先刷新预览状态，默认为false。</param>
        private void update自动生成全部朗读者预览音频按钮状态(bool refreshStatus = false)
        {
            bool isTtlEditing = ConnectionManager?.IsTtlEditing ?? false;
            if (isTtlEditing)
            {
                this.bt_TTL方案_自动生成全部朗读者预览音频.Enabled = false;
                this.bt_TTL方案_清理无效的预览音频.Enabled = false;
                return;
            }

            bool hasSpeakersWithoutPreview = false;
            string engineId = getCurrentEngineId();

            foreach (DataGridViewRow row in this.dgv_TTL方案_朗读者参数配置.Rows)
            {
                if (row.DataBoundItem is SpeakerInfo speaker)
                {
                    PreviewVoiceStatus status;
                    if (refreshStatus)
                    {
                        status = PreviewVoiceManager?.RefreshStatus(engineId, speaker.SourceName, speaker.Speed, speaker.Volume) ?? PreviewVoiceStatus.未生成;
                    }
                    else
                    {
                        status = PreviewVoiceManager?.GetStatus(engineId, speaker.SourceName, speaker.Speed, speaker.Volume) ?? PreviewVoiceStatus.未生成;
                    }
                    if (status == PreviewVoiceStatus.未生成)
                    {
                        hasSpeakersWithoutPreview = true;
                    }
                }
            }

            this.bt_TTL方案_自动生成全部朗读者预览音频.Enabled = hasSpeakersWithoutPreview;
            this.bt_TTL方案_清理无效的预览音频.Enabled = this.dgv_TTL方案_朗读者参数配置.Rows.Count > 0;
        }

        /// <summary>
        /// 请求生成预览声音。
        /// </summary>
        /// <param name="sourceName">朗读者源名称。</param>
        private void requestPreviewVoice(string sourceName)
        {
            string engineId = getCurrentEngineId();
            if (string.IsNullOrWhiteSpace(engineId) || string.IsNullOrWhiteSpace(sourceName))
            {
                return;
            }

            SpeakerInfo speakerInfo = _findSpeakerByShowName?.Invoke(sourceName);
            if (speakerInfo == null)
            {
                return;
            }

            int speed = speakerInfo.Speed;
            int volume = speakerInfo.Volume;
            {
                PreviewVoiceStatus status = PreviewVoiceManager?.GetStatus(engineId, sourceName, speed, volume) ?? PreviewVoiceStatus.未生成;

                if (status == PreviewVoiceStatus.已生成)
                {
                    string filePath = PreviewVoiceManager?.GetPreviewFilePath(engineId, sourceName, speed, volume);
                    if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                    {
                        _playSound?.Invoke(filePath);
                    }
                    return;
                }

                if (status == PreviewVoiceStatus.生成中)
                {
                    return;
                }

                bool alreadyHasPreviewTask = VoiceGenerationTaskQueue?.Tasks.Any(t =>
                    t.IsPreview && t.PreviewSourceName == sourceName && t.Speed == speed && t.Volume == volume && t.Status != VoiceGenerationTaskStatus.已完成) ?? false;
                if (alreadyHasPreviewTask)
                {
                    return;
                }

                if (VoiceGenerationTaskQueue != null)
                {
                    string previewText = _getPreviewText?.Invoke() ?? Constants.TTL角色预览声音_文本;
                    string outputPath = PreviewVoiceManager?.GetPreviewFilePath(engineId, sourceName, speed, volume);

                    VoiceGenerationTask previewTask = new VoiceGenerationTask();
                    previewTask.IsPreview = true;
                    previewTask.PreviewSourceName = sourceName;
                    previewTask.SaveFile = outputPath;
                    previewTask.Speed = speed;
                    previewTask.Volume = volume;
                    previewTask.SpaceTime = 1f;
                    previewTask.Status = VoiceGenerationTaskStatus.排队中;
                    previewTask.ProgressDetail = "预览任务排队中...";

                    string tempPath = VoiceGenerationTaskQueue.TempFolder;

                    VoiceGenerationTaskItem taskItem = new VoiceGenerationTaskItem
                    {
                        Speaker = speakerInfo,
                        Text = previewText,
                        EndNewLine = 0,
                        Speed = speed,
                        Volume = volume
                    };
                    taskItem.SetTempFile(tempPath, previewTask.Id);
                    previewTask.Items = new VoiceGenerationTaskItem[] { taskItem };

                    int insertIndex = VoiceGenerationTaskQueue?.Tasks.Count ?? 0;
                    for (int i = 0; i < VoiceGenerationTaskQueue.Tasks.Count; i++)
                    {
                        if (!VoiceGenerationTaskQueue.Tasks[i].IsPreview)
                        {
                            insertIndex = i;
                            break;
                        }
                    }
                    VoiceGenerationTaskQueue.Tasks.Insert(insertIndex, previewTask);

                    if (VoiceGenerationTaskQueue.IsRunning && VoiceGenerationTaskQueue.CurrentTask?.IsPreview != true)
                    {
                        VoiceGenerationTaskQueue.Stop();
                    }
                    VoiceGenerationTaskQueue.Start();
                }
            }

            PreviewVoiceManager?.SetStatus(engineId, sourceName, PreviewVoiceStatus.生成中, speed, volume);
        }

        /// <summary>
        /// 获取预览声音状态。
        /// </summary>
        /// <param name="sourceName">朗读者源名称。</param>
        /// <returns>预览声音状态。</returns>
        private PreviewVoiceStatus getPreviewVoiceStatus(string sourceName)
        {
            string engineId = getCurrentEngineId();
            if (string.IsNullOrWhiteSpace(engineId) || string.IsNullOrWhiteSpace(sourceName))
            {
                return PreviewVoiceStatus.未生成;
            }

            SpeakerInfo speakerInfo = null;
            foreach (DataGridViewRow row in this.dgv_TTL方案_朗读者参数配置.Rows)
            {
                if (row.DataBoundItem is SpeakerInfo speaker && speaker.SourceName == sourceName)
                {
                    speakerInfo = speaker;
                    break;
                }
            }

            int speed = speakerInfo?.Speed ?? 100;
            int volume = speakerInfo?.Volume ?? 100;

            return PreviewVoiceManager?.GetStatus(engineId, sourceName, speed, volume) ?? PreviewVoiceStatus.未生成;
        }

        #endregion

        /// <summary>
        /// 检查是否存在活跃的语音生成任务。
        /// </summary>
        /// <returns>如果存在活跃任务返回true，否则返回false。</returns>
        private bool hasActiveVoiceGenerationTasks()
        {
            return VoiceGenerationTaskQueue?.HasActiveTasks() ?? false;
        }

        /// <summary>
        /// 恢复引擎选择下拉框的选中项。
        /// </summary>
        /// <param name="engineId">要恢复的引擎ID。</param>
        private void restoreEngineSelection(string engineId)
        {
            if (string.IsNullOrEmpty(engineId) || engineId == Constants.无_引擎标识)
            {
                this.cb_TTL方案_当前方案名称.SelectedIndex = 0;
                return;
            }

            var connector = ConnectionManager?.AllEngines.FirstOrDefault(c => c.Id == engineId);
            if (connector != null)
            {
                this.cb_TTL方案_当前方案名称.SelectedItem = ConnectionManager.GetEngineDisplayText(connector);
            }
            else
            {
                this.cb_TTL方案_当前方案名称.SelectedIndex = 0;
            }
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 事件处理：当前选择的方案被改变。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void cb_TTL方案_当前方案名称_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = this.cb_TTL方案_当前方案名称.SelectedItem?.ToString() ?? string.Empty;
            ITtlEngineConnector ttl = ConnectionManager?.FindEngineByDisplayText(selectedItem);
            string newEngineId = ttl?.Id ?? Constants.无_引擎标识;

            string currentEngineId = ConnectionManager?.CurrentEngineId ?? Constants.无_引擎标识;
            bool isEngineChanging = (currentEngineId != newEngineId);

            if (isEngineChanging && !IsInitializing() && hasActiveVoiceGenerationTasks())
            {
                DialogResult result = MessageBox.Show(
                    "当前有正在执行或排队的语音生成任务。\n\n切换方案将停止所有正在执行的任务，部分任务可能需要手动重启。\n\n是否继续切换？",
                    "切换TTL方案",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (result != DialogResult.Yes)
                {
                    restoreEngineSelection(currentEngineId);
                    return;
                }
            }

            _savedScrollRowIndex = this.dgv_TTL方案_朗读者参数配置.FirstDisplayedScrollingRowIndex;

            this.tb_TTL方案_当前方案详情.Text = string.Empty;
            this.tb_TTL方案_连接参数配置.Text = string.Empty;
            this.dgv_TTL方案_朗读者参数配置.DataSource = null;

            if (ttl != null)
            {
                this.tb_TTL方案_当前方案详情.Text = ttl.Description;
                
                string key = $"Params_{ttl.Id}";
                string savedParams = Setting.GetValue(key, string.Empty);
                if (!string.IsNullOrEmpty(savedParams))
                {
                    this.tb_TTL方案_连接参数配置.Text = savedParams;
                    string[] parameters = savedParams.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    ttl.SetParameters(parameters);
                }
                else
                {
                    this.tb_TTL方案_连接参数配置.Text = ttl.Parameters == null ? string.Empty : string.Join("\r\n", ttl.Parameters);
                }

                createSpeakersFromRoles(ttl);
            }

            Setting.SetValue($"{this.cb_TTL方案_当前方案名称.Name}_Id", ttl == null ? string.Empty : ttl.Id);

            if (ConnectionManager != null && ConnectionManager.IsInitializing)
            {
                ConnectionManager.SelectEngine(ttl?.Id);
                return;
            }

            var previousEngine = ConnectionManager?.CurrentEngine;
            string previousEngineId = previousEngine?.Id;
            if (string.IsNullOrEmpty(previousEngineId))
            {
                previousEngineId = ConnectionManager?.CurrentEngineId;
            }

            if (previousEngine?.Id != ttl?.Id)
            {
                OnSaveVoiceGenerationTasksRequested();
                OnStopVoiceGenerationTaskQueueRequested();
            }

            // 如果选择"无"方案，传入无_引擎标识而不是null
            string engineIdToSelect = (ttl == null) ? Constants.无_引擎标识 : ttl.Id;
            ConnectionManager?.SelectEngine(engineIdToSelect);

            _ttlEngineConnectionStatus = TtlEngineConnectionStatus.未连接;
            _ttlEngineConnectionCountdown = 0;

            if (ttl != null)
            {
                start连接TTL引擎();
            }

            updateTtlEngineConnectionStatusLabel();

            if (VoiceGenerationTabPage != null)
            {
                VoiceGenerationTabPage.Enabled = ttl != null;
            }

            // 触发引擎改变事件，让MainForm处理其他面板的数据加载
            OnEngineChanged(ttl?.Id, previousEngineId);

            refreshTTL方案Ui();
        }

        /// <summary>
        /// 检查是否处于初始化阶段。
        /// </summary>
        /// <returns>如果处于初始化阶段返回true，否则返回false。</returns>
        private bool IsInitializing()
        {
            return ConnectionManager?.IsInitializing ?? false;
        }

        /// <summary>
        /// 事件处理：点击"编辑TTL方案配置"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_TTL方案_编辑配置_Click(object sender, EventArgs e)
        {
            if (ConnectionManager != null)
            {
                ConnectionManager.IsTtlEditing = true;
            }
            refreshTTL方案Ui();
        }

        /// <summary>
        /// 事件处理：点击"保存TTL方案配置"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private async void bt_TTL方案_保存配置_Click(object sender, EventArgs e)
        {
            var currentEngine = ConnectionManager?.CurrentEngine;
            if (currentEngine != null)
            {
                string[] parameters = this.tb_TTL方案_连接参数配置.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                ConnectionManager?.SaveEngineParameters(parameters);

                string key = $"Params_{currentEngine.Id}";
                string value = this.tb_TTL方案_连接参数配置.Text;
                Setting.SetValue(key, value);

                if (this.dgv_TTL方案_朗读者参数配置.DataSource != null)
                {
                    SpeakerInfo[] speakerArray = (SpeakerInfo[])this.dgv_TTL方案_朗读者参数配置.DataSource;
                    for (int i = 0; i < speakerArray.Length && i < currentEngine.Speakers.Length; i++)
                    {
                        currentEngine.Speakers[i] = speakerArray[i];
                    }

                    var keysToRemove = new List<string>();
                    foreach (var k in Setting.GetAllKeys())
                    {
                        if (k.StartsWith($"Speaker_{currentEngine.Id}_"))
                        {
                            keysToRemove.Add(k);
                        }
                    }
                    foreach (var k in keysToRemove)
                    {
                        Setting.Remove(k);
                    }

                    foreach (var speaker in speakerArray)
                    {
                        string speakerKey = $"Speaker_{currentEngine.Id}_{speaker.SourceName}";
                        Setting.SetValue(speakerKey, speaker.ToString());
                    }
                }

                Setting.Save();
                createSpeakersFromRoles(currentEngine);
            }

            if (ConnectionManager != null)
            {
                ConnectionManager.IsTtlEditing = false;
            }
            
            refreshTTL方案Ui();
            
            if (ConnectionManager != null)
            {
                var currentStatus = ConnectionManager.ConnectionStatus;
                if (currentStatus == TtlEngineConnectionStatus.连接成功)
                {
                    await ConnectionManager.DisconnectAsync();
                }
                await ConnectionManager.ConnectAsync();
            }
        }

        /// <summary>
        /// 事件处理：点击"还原TTL方案配置"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_TTL方案_还原配置_Click(object sender, EventArgs e)
        {
            if (ConnectionManager != null)
            {
                ConnectionManager.IsTtlEditing = false;
            }
            cb_TTL方案_当前方案名称_SelectedIndexChanged(sender, e);

            refreshTTL方案Ui();
        }

        /// <summary>
        /// 事件处理：点击"重新加载朗读者"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_TTL方案_重新加载朗读者_Click(object sender, EventArgs e)
        {
            var currentEngine = ConnectionManager?.CurrentEngine;
            if (currentEngine != null)
            {
                createSpeakersFromRoles(currentEngine, true);
            }
        }

        /// <summary>
        /// 事件处理：点击"自动生成全部朗读者预览音频"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_TTL方案_自动生成全部朗读者预览音频_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.dgv_TTL方案_朗读者参数配置.Rows)
            {
                if (row.DataBoundItem is SpeakerInfo speaker)
                {
                    string engineId = getCurrentEngineId();
                    if (string.IsNullOrWhiteSpace(engineId))
                    {
                        continue;
                    }

                    PreviewVoiceStatus status = PreviewVoiceManager?.RefreshStatus(engineId, speaker.SourceName, speaker.Speed, speaker.Volume) ?? PreviewVoiceStatus.未生成;
                    if (status != PreviewVoiceStatus.已生成 && status != PreviewVoiceStatus.生成中)
                    {
                        requestPreviewVoice(speaker.SourceName);
                    }
                }
            }
        }

        /// <summary>
        /// 事件处理：单元格点击，用于处理声音预览按钮的点击事件。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_TTL方案_朗读者参数配置_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var voicePreviewColumn = this.dgv_TTL方案_朗读者参数配置.Columns["VoicePreview"];
            if (voicePreviewColumn != null && e.RowIndex >= 0 && e.ColumnIndex == voicePreviewColumn.Index)
            {
                if (this.dgv_TTL方案_朗读者参数配置.Rows[e.RowIndex].DataBoundItem is SpeakerInfo speaker)
                {
                    requestPreviewVoice(speaker.SourceName);
                }
            }
        }

        /// <summary>
        /// 事件处理：单元格格式化，用于动态设置按钮文本和只读列样式。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_TTL方案_朗读者参数配置_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var voicePreviewColumn = this.dgv_TTL方案_朗读者参数配置.Columns["VoicePreview"];
            if (voicePreviewColumn != null && e.ColumnIndex == voicePreviewColumn.Index && e.RowIndex >= 0)
            {
                this.dgv_TTL方案_朗读者参数配置.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = string.Empty;
                if (this.dgv_TTL方案_朗读者参数配置.Rows[e.RowIndex].DataBoundItem is SpeakerInfo speaker)
                {
                    PreviewVoiceStatus status = getPreviewVoiceStatus(speaker.SourceName);

                    if (status == PreviewVoiceStatus.已生成)
                    {
                        e.Value = "播放";
                    }
                    else if (status == PreviewVoiceStatus.生成中)
                    {
                        e.Value = "生成中...";
                    }
                    else
                    {
                        e.Value = "生成";
                    }
                    e.FormattingApplied = true;
                }
            }

            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                e.CellStyle.BackColor = Color.LightGray;
                e.CellStyle.ForeColor = Color.Black;
                e.CellStyle.SelectionBackColor = Color.LightGray;
                e.CellStyle.SelectionForeColor = Color.Black;
            }
        }

        /// <summary>
        /// 事件处理：点击"打开角色声音预览目录"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_TTL方案_打开角色声音预览目录_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", TTL角色预览声音_文件夹路径);
        }

        /// <summary>
        /// 事件处理：点击"清理无效的预览音频"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_TTL方案_清理无效的预览音频_Click(object sender, EventArgs e)
        {
            string confirmMessage = "此功能将清理当前引擎下所有无效的预览音频文件：" +
                "\n\n• 不在当前角色列表中的音频文件" +
                "\n• 生成过程中的临时文件（.generating）" +
                "\n\n是否立即执行清理？";
            DialogResult result = MessageBox.Show(confirmMessage, "清理无效的预览音频", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result != DialogResult.OK)
            {
                return;
            }

            string engineId = getCurrentEngineId();
            if (string.IsNullOrWhiteSpace(engineId))
            {
                return;
            }

            var currentEngine = ConnectionManager?.CurrentEngine;
            if (currentEngine == null || currentEngine.Speakers == null)
            {
                return;
            }

            HashSet<string> validFileNames = new HashSet<string>();
            foreach (var speaker in currentEngine.Speakers)
            {
                string safeSourceName = getSafeFileName(speaker.SourceName);
                string validFileName = $"{engineId}_{safeSourceName}_{speaker.Speed}_{speaker.Volume}.mp3";
                validFileNames.Add(validFileName);
            }

            if (!Directory.Exists(TTL角色预览声音_文件夹路径))
            {
                return;
            }

            int deletedCount = 0;
            var files = Directory.GetFiles(TTL角色预览声音_文件夹路径, $"{engineId}_*.mp3");
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                if (!validFileNames.Contains(fileName))
                {
                    try
                    {
                        File.Delete(file);
                        deletedCount++;
                    }
                    catch { }
                }
            }

            var generatingFiles = Directory.GetFiles(TTL角色预览声音_文件夹路径, $"{engineId}_*.generating");
            foreach (var file in generatingFiles)
            {
                try
                {
                    File.Delete(file);
                    deletedCount++;
                }
                catch { }
            }

            if (deletedCount > 0)
            {
                MessageBox.Show($"已清理 {deletedCount} 个无效的预览音频文件", "清理完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("没有发现无效的预览音频文件", "清理完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            this.dgv_TTL方案_朗读者参数配置.Invalidate();
        }

        /// <summary>
        /// 获取安全的文件名（移除不允许的字符）。
        /// </summary>
        /// <param name="name">原始名称。</param>
        /// <returns>安全的文件名。</returns>
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
        /// 事件处理：单元格验证，用于验证速度列的输入值。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_TTL方案_朗读者参数配置_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var speedColumn = this.dgv_TTL方案_朗读者参数配置.Columns["Speed"];
            if (speedColumn != null && e.ColumnIndex == speedColumn.Index && e.RowIndex >= 0)
            {
                string inputValue = e.FormattedValue?.ToString() ?? string.Empty;
                if (int.TryParse(inputValue, out int speed))
                {
                    if (speed < 50 || speed > 200)
                    {
                        e.Cancel = true;
                        MessageBox.Show("速度值必须在50-200之间", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(inputValue))
                {
                    e.Cancel = true;
                    MessageBox.Show("请输入有效的数字", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            var volumeColumn = this.dgv_TTL方案_朗读者参数配置.Columns["Volume"];
            if (volumeColumn != null && e.ColumnIndex == volumeColumn.Index && e.RowIndex >= 0)
            {
                string inputValue = e.FormattedValue?.ToString() ?? string.Empty;
                if (int.TryParse(inputValue, out int volume))
                {
                    if (volume < 50 || volume > 200)
                    {
                        e.Cancel = true;
                        MessageBox.Show("音量值必须在50-200之间", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(inputValue))
                {
                    e.Cancel = true;
                    MessageBox.Show("请输入有效的数字", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// 事件处理：单元格编辑结束，用于刷新预览按钮状态。
        /// </summary>
        private void dgv_TTL方案_朗读者参数配置_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var speedColumn = this.dgv_TTL方案_朗读者参数配置.Columns["Speed"];
            var volumeColumn = this.dgv_TTL方案_朗读者参数配置.Columns["Volume"];
            var remarkColumn = this.dgv_TTL方案_朗读者参数配置.Columns["Remark"];

            if ((speedColumn != null && e.ColumnIndex == speedColumn.Index) ||
                (volumeColumn != null && e.ColumnIndex == volumeColumn.Index))
            {
                if (e.RowIndex >= 0)
                {
                    this.dgv_TTL方案_朗读者参数配置.InvalidateRow(e.RowIndex);

                    update自动生成全部朗读者预览音频按钮状态();
                }
            }

            if (remarkColumn != null && e.ColumnIndex == remarkColumn.Index && e.RowIndex >= 0)
            {
                OnSpeakersChanged();
            }
        }

        /// <summary>
        /// 事件处理：朗读者表格鼠标按下，用于处理行首右键菜单。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_TTL方案_朗读者参数配置_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTest = this.dgv_TTL方案_朗读者参数配置.HitTest(e.X, e.Y);
                if (hitTest.Type == DataGridViewHitTestType.RowHeader && hitTest.RowIndex >= 0)
                {
                    if (!this.dgv_TTL方案_朗读者参数配置.Rows[hitTest.RowIndex].Selected)
                    {
                        this.dgv_TTL方案_朗读者参数配置.ClearSelection();
                        this.dgv_TTL方案_朗读者参数配置.Rows[hitTest.RowIndex].Selected = true;
                    }

                    bool hasSelection = this.dgv_TTL方案_朗读者参数配置.SelectedRows.Count > 0;
                    bool isTtlEditing = ConnectionManager?.IsTtlEditing ?? false;

                    this.设置为默认朗读者DToolStripMenuItem.Enabled = hasSelection && !isTtlEditing;
                    this.添加为角色RToolStripMenuItem.Enabled = hasSelection && !isTtlEditing;

                    bool canRegeneratePreview = false;
                    if (hasSelection && !isTtlEditing)
                    {
                        var selectedRow = this.dgv_TTL方案_朗读者参数配置.SelectedRows.Cast<DataGridViewRow>().FirstOrDefault();
                        if (selectedRow?.DataBoundItem is SpeakerInfo speaker)
                        {
                            string engineId = getCurrentEngineId();
                            string previewFilePath = PreviewVoiceManager?.GetPreviewFilePath(engineId, speaker.SourceName, speaker.Speed, speaker.Volume);
                            canRegeneratePreview = !string.IsNullOrWhiteSpace(previewFilePath) && File.Exists(previewFilePath);
                        }
                    }
                    this.重新生成声音预览文件VToolStripMenuItem.Enabled = canRegeneratePreview;

                    this.cmd_TTL方案_朗读者.Show(this.dgv_TTL方案_朗读者参数配置, e.Location);
                }
            }
        }

        /// <summary>
        /// 事件处理：点击"设置为默认朗读者"菜单项。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void 设置为默认朗读者DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedRow = this.dgv_TTL方案_朗读者参数配置.SelectedRows.Cast<DataGridViewRow>().FirstOrDefault();
            if (selectedRow?.DataBoundItem is SpeakerInfo speaker)
            {
                OnSetDefaultSpeakerRequested(speaker);
            }
        }

        /// <summary>
        /// 事件处理：点击"添加为角色"菜单项。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void 添加为角色RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedRow = this.dgv_TTL方案_朗读者参数配置.SelectedRows.Cast<DataGridViewRow>().FirstOrDefault();
            if (selectedRow?.DataBoundItem is SpeakerInfo speaker)
            {
                OnAddRoleMappingRequested(speaker.SourceName);
            }
        }

        /// <summary>
        /// 事件处理：点击"重新生成声音预览文件"菜单项。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void 重新生成声音预览文件VToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedRow = this.dgv_TTL方案_朗读者参数配置.SelectedRows.Cast<DataGridViewRow>().FirstOrDefault();
            if (selectedRow?.DataBoundItem is SpeakerInfo speaker)
            {
                regeneratePreviewVoice(speaker.SourceName);
            }
        }

        /// <summary>
        /// 重新生成预览声音。
        /// </summary>
        /// <param name="sourceName">朗读者源名称。</param>
        private void regeneratePreviewVoice(string sourceName)
        {
            string engineId = getCurrentEngineId();
            if (string.IsNullOrWhiteSpace(engineId) || string.IsNullOrWhiteSpace(sourceName))
            {
                return;
            }

            SpeakerInfo speakerInfo = _findSpeakerByShowName?.Invoke(sourceName);
            if (speakerInfo == null)
            {
                return;
            }

            int speed = speakerInfo.Speed;
            int volume = speakerInfo.Volume;

            string previewFilePath = PreviewVoiceManager?.GetPreviewFilePath(engineId, sourceName, speed, volume);
            if (!string.IsNullOrWhiteSpace(previewFilePath) && File.Exists(previewFilePath))
            {
                try
                {
                    File.Delete(previewFilePath);
                }
                catch { }
            }

            string statusFilePath = PreviewVoiceManager?.GetStatusFilePath(engineId, sourceName, speed, volume);
            if (!string.IsNullOrWhiteSpace(statusFilePath) && File.Exists(statusFilePath))
            {
                try
                {
                    File.Delete(statusFilePath);
                }
                catch { }
            }

            PreviewVoiceManager?.ResetStatus(engineId, sourceName, speed, volume);

            bool alreadyHasPreviewTask = VoiceGenerationTaskQueue?.Tasks.Any(t =>
                t.IsPreview && t.PreviewSourceName == sourceName && t.Speed == speed && t.Volume == volume && t.Status != VoiceGenerationTaskStatus.已完成) ?? false;
            if (alreadyHasPreviewTask)
            {
                return;
            }

            if (VoiceGenerationTaskQueue != null)
            {
                string previewText = _getPreviewText?.Invoke() ?? Constants.TTL角色预览声音_文本;
                string outputPath = PreviewVoiceManager?.GetPreviewFilePath(engineId, sourceName, speed, volume);

                VoiceGenerationTask previewTask = new VoiceGenerationTask();
                previewTask.IsPreview = true;
                previewTask.PreviewSourceName = sourceName;
                previewTask.SaveFile = outputPath;
                previewTask.Speed = speed;
                previewTask.Volume = volume;
                previewTask.SpaceTime = 1f;
                previewTask.Status = VoiceGenerationTaskStatus.排队中;
                previewTask.ProgressDetail = "预览任务排队中...";

                string tempPath = VoiceGenerationTaskQueue.TempFolder;

                VoiceGenerationTaskItem taskItem = new VoiceGenerationTaskItem
                {
                    Speaker = speakerInfo,
                    Text = previewText,
                    EndNewLine = 0,
                    Speed = speed,
                    Volume = volume
                };
                taskItem.SetTempFile(tempPath, previewTask.Id);
                previewTask.Items = new VoiceGenerationTaskItem[] { taskItem };

                int insertIndex = VoiceGenerationTaskQueue?.Tasks.Count ?? 0;
                for (int i = 0; i < VoiceGenerationTaskQueue.Tasks.Count; i++)
                {
                    if (!VoiceGenerationTaskQueue.Tasks[i].IsPreview)
                    {
                        insertIndex = i;
                        break;
                    }
                }
                VoiceGenerationTaskQueue.Tasks.Insert(insertIndex, previewTask);

                if (VoiceGenerationTaskQueue.IsRunning && VoiceGenerationTaskQueue.CurrentTask?.IsPreview != true)
                {
                    VoiceGenerationTaskQueue.Stop();
                }
                VoiceGenerationTaskQueue.Start();
            }

            PreviewVoiceManager?.SetStatus(engineId, sourceName, PreviewVoiceStatus.生成中, speed, volume);

            this.dgv_TTL方案_朗读者参数配置.Invalidate();
        }

        #endregion
    }

    #region 事件参数类

    /// <summary>
    /// TTL引擎改变事件参数。
    /// </summary>
    public class TtlEngineChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 获取新引擎ID。
        /// </summary>
        public string EngineId { get; }

        /// <summary>
        /// 获取前一个引擎ID。
        /// </summary>
        public string PreviousEngineId { get; }

        /// <summary>
        /// 初始化TtlEngineChangedEventArgs类的新实例。
        /// </summary>
        /// <param name="engineId">新引擎ID。</param>
        /// <param name="previousEngineId">前一个引擎ID。</param>
        public TtlEngineChangedEventArgs(string engineId, string previousEngineId)
        {
            EngineId = engineId;
            PreviousEngineId = previousEngineId;
        }
    }

    /// <summary>
    /// TTL引擎连接状态事件参数。
    /// </summary>
    public class TtlEngineConnectionStatusEventArgs : EventArgs
    {
        /// <summary>
        /// 获取连接状态。
        /// </summary>
        public TtlEngineConnectionStatus Status { get; }

        /// <summary>
        /// 获取倒计时。
        /// </summary>
        public int Countdown { get; }

        /// <summary>
        /// 初始化TtlEngineConnectionStatusEventArgs类的新实例。
        /// </summary>
        /// <param name="status">连接状态。</param>
        /// <param name="countdown">倒计时。</param>
        public TtlEngineConnectionStatusEventArgs(TtlEngineConnectionStatus status, int countdown)
        {
            Status = status;
            Countdown = countdown;
        }
    }

    /// <summary>
    /// 设置默认朗读者事件参数。
    /// </summary>
    public class SetDefaultSpeakerEventArgs : EventArgs
    {
        /// <summary>
        /// 获取朗读者信息。
        /// </summary>
        public SpeakerInfo Speaker { get; }

        /// <summary>
        /// 初始化SetDefaultSpeakerEventArgs类的新实例。
        /// </summary>
        /// <param name="speaker">朗读者信息。</param>
        public SetDefaultSpeakerEventArgs(SpeakerInfo speaker)
        {
            Speaker = speaker;
        }
    }

    /// <summary>
    /// 添加角色映射事件参数。
    /// </summary>
    public class AddRoleMappingEventArgs : EventArgs
    {
        /// <summary>
        /// 获取源名称。
        /// </summary>
        public string SourceName { get; }

        /// <summary>
        /// 初始化AddRoleMappingEventArgs类的新实例。
        /// </summary>
        /// <param name="sourceName">源名称。</param>
        public AddRoleMappingEventArgs(string sourceName)
        {
            SourceName = sourceName;
        }
    }

    #endregion
}
