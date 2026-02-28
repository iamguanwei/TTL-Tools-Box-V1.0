using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GW.TTLtoolsBox.Core.FileAccesser;
using GW.TTLtoolsBox.Core.SystemOption.TtlEngine;
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
    /// - 依赖TtlSchemeController管理TTL引擎方案
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
        /// <param name="tempFolder">临时文件夹路径。</param>
        public TtlSchemePanel(string tempFolder) : this()
        {
            if (!string.IsNullOrWhiteSpace(tempFolder))
            {
                TTL角色预览声音_文件夹路径 = tempFolder;
                TTL角色预览声音_文件 = Path.Combine(tempFolder, "角色声音预览文本.txt");
            }
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置TTL方案控制器。
        /// </summary>
        public TtlSchemeController TtlSchemeController { get; set; }

        /// <summary>
        /// 获取或设置预览声音管理器。
        /// </summary>
        public PreviewVoiceManager PreviewVoiceManager { get; set; }

        /// <summary>
        /// 获取或设置语音生成任务队列。
        /// </summary>
        public VoiceGenerationTaskQueue VoiceGenerationTaskQueue { get; set; }

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
        public string CurrentEngineId => TtlSchemeController?.GetCurrentEngineId();

        /// <summary>
        /// 获取或设置连接状态标签控件。
        /// </summary>
        public ToolStripStatusLabel ConnectionStatusLabel { get; set; }

        /// <summary>
        /// 获取或设置语音生成选项卡页。
        /// </summary>
        public TabPage VoiceGenerationTabPage { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 初始化面板。
        /// </summary>
        public void InitializePanel()
        {
            initTTL方案Ui();
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
        /// <param name="retryCountdown">重试倒计时。</param>
        public void SetConnectionStatus(TtlEngineConnectionStatus status, int countdown = 0, int retryCountdown = 0)
        {
            _ttlEngineConnectionStatus = status;
            _ttlEngineConnectionCountdown = countdown;
            _ttlEngineRetryCountdown = retryCountdown;
        }

        /// <summary>
        /// 获取连接倒计时。
        /// </summary>
        public int ConnectionCountdown => _ttlEngineConnectionCountdown;

        /// <summary>
        /// 获取重试倒计时。
        /// </summary>
        public int RetryCountdown => _ttlEngineRetryCountdown;

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
            var currentEngine = TtlSchemeController?.CurrentEngineConnector;
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
        /// <param name="retryCountdown">重试倒计时。</param>
        protected void OnConnectionStatusChanged(TtlEngineConnectionStatus status, int countdown, int retryCountdown)
        {
            ConnectionStatusChanged?.Invoke(this, new TtlEngineConnectionStatusEventArgs(status, countdown, retryCountdown));
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
        /// TTL引擎连接/重试倒计时（秒）。
        /// </summary>
        private int _ttlEngineRetryCountdown = 0;

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
                TtlSchemeController?.Initialize();

                // 初始化下拉选项
                this.cb_TTL方案_当前方案名称.Items.Clear();

                // 添加"无"选项
                this.cb_TTL方案_当前方案名称.Items.Add("无");

                // 添加所有可用的TTL引擎连接器名称，格式为"名称 [ID = id]"
                if (TtlSchemeController != null)
                {
                    foreach (var connector in TtlSchemeController.EngineConnectorArray)
                    {
                        this.cb_TTL方案_当前方案名称.Items.Add(TtlSchemeController.GetEngineDisplayText(connector));
                    }
                }

                // 默认选择第一个选项（使用ID进行匹配）
                if (this.cb_TTL方案_当前方案名称.Items.Count > 0)
                {
                    var savedId = Setting.GetValue($"{this.cb_TTL方案_当前方案名称.Name}_Id", string.Empty);
                    if (!string.IsNullOrEmpty(savedId))
                    {
                        // 根据ID查找对应的显示项
                        var connector = TtlSchemeController?.EngineConnectorArray.FirstOrDefault(c => c.Id == savedId);
                        if (connector != null)
                        {
                            this.cb_TTL方案_当前方案名称.SelectedItem = TtlSchemeController.GetEngineDisplayText(connector);
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
                            var connector = TtlSchemeController?.EngineConnectorArray.FirstOrDefault(c => c.Name == oldName);
                            if (connector != null)
                            {
                                this.cb_TTL方案_当前方案名称.SelectedItem = TtlSchemeController.GetEngineDisplayText(connector);
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
                if (TtlSchemeController != null)
                {
                    TtlSchemeController.IsTtlEditing = false;
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
            var currentEngine = TtlSchemeController?.CurrentEngineConnector;
            var isTtlEditing = TtlSchemeController?.IsTtlEditing ?? false;

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
            var currentEngine = TtlSchemeController?.CurrentEngineConnector;
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
            var currentEngine = TtlSchemeController?.CurrentEngineConnector;
            if (currentEngine == null)
            {
                _ttlEngineConnectionStatus = TtlEngineConnectionStatus.未连接;
                updateTtlEngineConnectionStatusLabel();
                refreshTTL方案Ui();
                return;
            }

            int timeoutSeconds = (int)currentEngine.ConnectionTimeout.TotalSeconds;
            if (timeoutSeconds <= 0)
            {
                timeoutSeconds = 30;
            }

            _ttlEngineConnectionStatus = TtlEngineConnectionStatus.连接中;
            _ttlEngineConnectionCountdown = timeoutSeconds;
            updateTtlEngineConnectionStatusLabel();
            refreshTTL方案Ui();

            try
            {
                await currentEngine.ConnectAsync();

                _ttlEngineConnectionStatus = TtlEngineConnectionStatus.连接成功;
                _ttlEngineRetryCountdown = Constants.连接成功_验证间隔秒数;

                if (currentEngine.Speakers == null || currentEngine.Speakers.Length == 0)
                {
                    createSpeakersFromRoles(currentEngine, true);
                }
                else
                {
                    bindSpeakersToGrid(currentEngine);
                }

                VoiceGenerationTaskQueue?.TryResume();
            }
            catch
            {
                _ttlEngineConnectionStatus = TtlEngineConnectionStatus.连接失败;
                _ttlEngineRetryCountdown = Constants.连接失败_重试间隔秒数;
            }

            updateTtlEngineConnectionStatusLabel();
            refreshTTL方案Ui();
        }

        /// <summary>
        /// 更新TTL引擎连接状态标签。
        /// </summary>
        private void updateTtlEngineConnectionStatusLabel()
        {
            Action action = () =>
            {
                string statusText = string.Empty;
                Color statusColor = SystemColors.ControlText;

                var currentEngine = TtlSchemeController?.CurrentEngineConnector;
                if (currentEngine == null)
                {
                    statusText = "未选择TTL引擎";
                }
                else
                {
                    switch (_ttlEngineConnectionStatus)
                    {
                        case TtlEngineConnectionStatus.未连接:
                            statusText = $"{currentEngine.Name}: 未连接";
                            break;
                        case TtlEngineConnectionStatus.连接中:
                            statusText = $"{currentEngine.Name}: 连接中 ({_ttlEngineConnectionCountdown}秒)";
                            break;
                        case TtlEngineConnectionStatus.连接成功:
                            statusText = $"{currentEngine.Name}: 已连接 ({_ttlEngineRetryCountdown}秒后验证)";
                            statusColor = Color.FromArgb(0, 150, 0);
                            break;
                        case TtlEngineConnectionStatus.连接失败:
                            statusText = $"{currentEngine.Name}: 连接失败 ({_ttlEngineRetryCountdown}秒后重试)";
                            statusColor = Color.FromArgb(200, 0, 0);
                            break;
                        default:
                            statusText = $"{currentEngine.Name}: 未连接";
                            break;
                    }
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
            return TtlSchemeController?.GetCurrentEngineId();
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
                            this.Invoke(new Action(() => bindSpeakersToGrid(ttl)));
                        }
                        catch
                        {
                            // 无代码
                        }
                    });
                }
                else
                {
                    // 直接使用已加载的Speakers列表
                    bindSpeakersToGrid(ttl);
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
        private void bindSpeakersToGrid(ITtlEngineConnector ttl)
        {
            var speakerArray = ttl.Speakers.Select(s => s.Clone()).ToArray();
            if (speakerArray != null && speakerArray.Length > 0)
            {
                this.dgv_TTL方案_朗读者参数配置.DataSource = speakerArray;
                setupSpeakerGridColumns();
                this.dgv_TTL方案_朗读者参数配置.Refresh();
                this.dgv_TTL方案_朗读者参数配置.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            else
            {
                this.dgv_TTL方案_朗读者参数配置.DataSource = null;
            }

            OnSpeakersChanged();
            update自动生成全部朗读者预览音频按钮状态(true);
        }

        /// <summary>
        /// 设置朗读者表格的列属性，只包含源名称和声音预览两列。
        /// </summary>
        private void setupSpeakerGridColumns()
        {
            this.dgv_TTL方案_朗读者参数配置.AutoGenerateColumns = false;

            this.dgv_TTL方案_朗读者参数配置.SelectionMode = DataGridViewSelectionMode.CellSelect;
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
            bool hasSpeakersWithoutPreview = false;
            var currentEngine = TtlSchemeController?.CurrentEngineConnector;
            if (currentEngine != null && currentEngine.Speakers != null)
            {
                string engineId = currentEngine.Id;
                foreach (var speaker in currentEngine.Speakers)
                {
                    PreviewVoiceStatus status;
                    if (refreshStatus)
                    {
                        status = PreviewVoiceManager?.RefreshStatus(engineId, speaker.SourceName) ?? PreviewVoiceStatus.未生成;
                    }
                    else
                    {
                        status = PreviewVoiceManager?.GetStatus(engineId, speaker.SourceName) ?? PreviewVoiceStatus.未生成;
                    }
                    if (status == PreviewVoiceStatus.未生成)
                    {
                        hasSpeakersWithoutPreview = true;
                        break;
                    }
                }
            }
            this.bt_TTL方案_自动生成全部朗读者预览音频.Enabled = currentEngine != null && hasSpeakersWithoutPreview;
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

            PreviewVoiceStatus status = PreviewVoiceManager?.GetStatus(engineId, sourceName) ?? PreviewVoiceStatus.未生成;

            if (status == PreviewVoiceStatus.已生成)
            {
                string filePath = PreviewVoiceManager?.GetPreviewFilePath(engineId, sourceName);
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
                t.IsPreview && t.PreviewSourceName == sourceName && t.Status != VoiceGenerationTaskStatus.已完成) ?? false;
            if (alreadyHasPreviewTask)
            {
                return;
            }

            SpeakerInfo speakerInfo = _findSpeakerByShowName?.Invoke(sourceName);
            if (speakerInfo == null)
            {
                return;
            }

            string previewText = _getPreviewText?.Invoke() ?? Constants.TTL角色预览声音_文本;
            string outputPath = PreviewVoiceManager?.GetPreviewFilePath(engineId, sourceName);

            VoiceGenerationTask previewTask = new VoiceGenerationTask();
            previewTask.IsPreview = true;
            previewTask.PreviewSourceName = sourceName;
            previewTask.SaveFile = outputPath;
            previewTask.Speed = 100;
            previewTask.SpaceTime = 1f;
            previewTask.Status = VoiceGenerationTaskStatus.排队中;
            previewTask.ProgressDetail = "预览任务排队中...";

            VoiceGenerationTaskItem taskItem = new VoiceGenerationTaskItem
            {
                TempFile = Path.Combine(TTL角色预览声音_文件夹路径, $"preview_{previewTask.Id}.wav"),
                Speaker = speakerInfo,
                Text = previewText,
                EndNewLine = 0
            };
            previewTask.Items = new VoiceGenerationTaskItem[] { taskItem };

            int insertIndex = 0;
            if (VoiceGenerationTaskQueue != null)
            {
                for (int i = 0; i < VoiceGenerationTaskQueue.Tasks.Count; i++)
                {
                    if (VoiceGenerationTaskQueue.Tasks[i].IsPreview)
                    {
                        insertIndex = i + 1;
                    }
                }
                VoiceGenerationTaskQueue.Tasks.Insert(insertIndex, previewTask);
            }

            PreviewVoiceManager?.SetStatus(engineId, sourceName, PreviewVoiceStatus.生成中);

            VoiceGenerationTaskQueue?.Start();
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

            return PreviewVoiceManager?.GetStatus(engineId, sourceName) ?? PreviewVoiceStatus.未生成;
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
            this.tb_TTL方案_当前方案详情.Text = string.Empty;
            this.tb_TTL方案_连接参数配置.Text = string.Empty;
            this.dgv_TTL方案_朗读者参数配置.DataSource = null;

            string selectedItem = this.cb_TTL方案_当前方案名称.SelectedItem?.ToString() ?? string.Empty;

            ITtlEngineConnector ttl = TtlSchemeController?.FindEngineByDisplayText(selectedItem);

            if (ttl != null)
            {
                this.tb_TTL方案_当前方案详情.Text = ttl.Description;
                this.tb_TTL方案_连接参数配置.Text = ttl.Parameters == null ? string.Empty : string.Join("\r\n", ttl.Parameters);

                createSpeakersFromRoles(ttl);
                setupSpeakerGridColumns();
            }

            Setting.SetValue($"{this.cb_TTL方案_当前方案名称.Name}_Id", ttl == null ? string.Empty : ttl.Id);

            if (TtlSchemeController != null && TtlSchemeController.IsInitializing)
            {
                TtlSchemeController.SelectEngine(ttl?.Id);
                return;
            }

            var previousEngine = TtlSchemeController?.CurrentEngineConnector;
            string previousEngineId = previousEngine?.Id;
            if (string.IsNullOrEmpty(previousEngineId))
            {
                previousEngineId = TtlSchemeController?.GetCurrentEngineId();
            }

            if (previousEngine?.Id != ttl?.Id)
            {
                OnSaveVoiceGenerationTasksRequested();
                OnStopVoiceGenerationTaskQueueRequested();
            }

            // 如果选择"无"方案，传入None_Engine_Id而不是null
            string engineIdToSelect = (ttl == null) ? Constants.None_Engine_Id : ttl.Id;
            TtlSchemeController?.SelectEngine(engineIdToSelect);

            _ttlEngineConnectionStatus = TtlEngineConnectionStatus.未连接;
            _ttlEngineConnectionCountdown = 0;
            _ttlEngineRetryCountdown = 0;

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
        /// 事件处理：点击"编辑TTL方案配置"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_TTL方案_编辑配置_Click(object sender, EventArgs e)
        {
            if (TtlSchemeController != null)
            {
                TtlSchemeController.IsTtlEditing = true;
            }
            refreshTTL方案Ui();
        }

        /// <summary>
        /// 事件处理：点击"保存TTL方案配置"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_TTL方案_保存配置_Click(object sender, EventArgs e)
        {
            var currentEngine = TtlSchemeController?.CurrentEngineConnector;
            if (currentEngine != null)
            {
                TtlSchemeController?.SaveEngineParameters(this.tb_TTL方案_连接参数配置.Text);

                if (this.dgv_TTL方案_朗读者参数配置.DataSource != null)
                {
                    SpeakerInfo[] speakerArray = (SpeakerInfo[])this.dgv_TTL方案_朗读者参数配置.DataSource;
                    TtlSchemeController?.SaveSpeakerParameters(speakerArray);
                }

                createSpeakersFromRoles(currentEngine);
            }

            if (TtlSchemeController != null)
            {
                TtlSchemeController.IsTtlEditing = false;
            }
            refreshTTL方案Ui();
        }

        /// <summary>
        /// 事件处理：点击"还原TTL方案配置"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_TTL方案_还原配置_Click(object sender, EventArgs e)
        {
            if (TtlSchemeController != null)
            {
                TtlSchemeController.IsTtlEditing = false;
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
            var currentEngine = TtlSchemeController?.CurrentEngineConnector;
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
            var currentEngine = TtlSchemeController?.CurrentEngineConnector;
            if (currentEngine == null || currentEngine.Speakers == null)
            {
                return;
            }

            string engineId = currentEngine.Id;
            foreach (var speaker in currentEngine.Speakers)
            {
                PreviewVoiceStatus status = PreviewVoiceManager?.RefreshStatus(engineId, speaker.SourceName) ?? PreviewVoiceStatus.未生成;
                if (status != PreviewVoiceStatus.已生成 && status != PreviewVoiceStatus.生成中)
                {
                    requestPreviewVoice(speaker.SourceName);
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

        #endregion

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
        /// 获取连接倒计时。
        /// </summary>
        public int Countdown { get; }

        /// <summary>
        /// 获取重试倒计时。
        /// </summary>
        public int RetryCountdown { get; }

        /// <summary>
        /// 初始化TtlEngineConnectionStatusEventArgs类的新实例。
        /// </summary>
        /// <param name="status">连接状态。</param>
        /// <param name="countdown">连接倒计时。</param>
        /// <param name="retryCountdown">重试倒计时。</param>
        public TtlEngineConnectionStatusEventArgs(TtlEngineConnectionStatus status, int countdown, int retryCountdown)
        {
            Status = status;
            Countdown = countdown;
            RetryCountdown = retryCountdown;
        }
    }

    #endregion
}
