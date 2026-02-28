using GW.TTLtoolsBox.Core.FileAccesser;
using GW.TTLtoolsBox.Core.PolyReplace;
using GW.TTLtoolsBox.Core.SystemOption.Helper;
using GW.TTLtoolsBox.Core.SystemOption.TtlEngine;
using GW.TTLtoolsBox.Core.TextSplit;
using GW.TTLtoolsBox.WinFormUi.Helper;
using GW.TTLtoolsBox.WinFormUi.Manager;
using GW.TTLtoolsBox.WinFormUi.UI.Panels;
using CoreVoiceGenerationTask = GW.TTLtoolsBox.Core.Entity.VoiceGenerationTask;
using CoreVoiceGenerationTaskItem = GW.TTLtoolsBox.Core.Entity.VoiceGenerationTaskItem;
using RoleMappingItem = GW.TTLtoolsBox.Core.Entity.RoleMappingItem;
using RoleEmotionItem = GW.TTLtoolsBox.Core.Entity.RoleEmotionItem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GW.TTLtoolsBox.WinFormUi.Helper.Constants;

namespace GW.TTLtoolsBox.WinFormUi
{
    /// <summary>
    /// 主窗体。
    /// </summary>
    public partial class MainForm : Form
    {
        #region 常量

        /// <summary>
        /// 退出软件时默认使用的项目文件。
        /// </summary>
        private string 默认项目_文件 => Path.Combine(
            string.IsNullOrEmpty(_tempFolder) ? 临时_工作目录 : _tempFolder,
            $"默认项目文件{项目文件_扩展名}");

        /// <summary>
        /// TTL角色预览声音的生成文件。
        /// </summary>
        private string TTL角色预览声音_文件 => Path.Combine(_tempFolder, @"角色声音预览文本.txt");

        #endregion

        #region public

        #region 构造函数

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="args">命令行参数</param>
        public MainForm(string[] args)
        {
            InitializeComponent();

            this.Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Icons", "app.ico"));

            // 解析命令行参数
            if (args != null && args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            {
                _currentProjectFilePath = args[0];
            }

            init主Ui();
            initTTL方案Panel();
            init角色映射Panel();
            init文本拆分Panel();
            init多音字替换Panel();
            init角色和情绪指定Panel();
            init语音生成预处理Panel();
            init语音生成Panel();
            initTtlEngineConnectionManager();

            _ttlSchemeController.EndInitializing();
        }

        #endregion

        #endregion

        #region private

        #region 字段

        /// <summary>
        /// 当前使用的项目文件路径。
        /// </summary>
        private string _currentProjectFilePath = null;

        /// <summary>
        /// 预览声音管理器实例。
        /// </summary>
        private PreviewVoiceManager _previewVoiceManager = null;

        /// <summary>
        /// 是否正在初始化（防止在初始化时触发任务列表加载）。
        /// </summary>
        private bool _isInitializing = true;

        /// <summary>
        /// TTL方案控制器实例。
        /// </summary>
        private TtlSchemeController _ttlSchemeController = new TtlSchemeController();

        /// <summary>
        /// TTL方案面板实例。
        /// </summary>
        private TtlSchemePanel _ttlSchemePanel = null;

        /// <summary>
        /// 文本拆分面板实例。
        /// </summary>
        private TextSplitPanel _textSplitPanel = null;

        /// <summary>
        /// 角色映射面板实例。
        /// </summary>
        private RoleMappingPanel _roleMappingPanel = null;

        /// <summary>
        /// 多音字替换面板实例。
        /// </summary>
        private PolyphonicReplacePanel _polyphonicReplacePanel = null;

        /// <summary>
        /// 语音生成预处理面板实例。
        /// </summary>
        private VoicePreprocessPanel _voicePreprocessPanel = null;

        /// <summary>
        /// 角色和情绪指定面板实例。
        /// </summary>
        private RoleEmotionPanel _roleEmotionPanel = null;

        /// <summary>
        /// 语音生成面板实例。
        /// </summary>
        private VoiceGenerationPanel _voiceGenerationPanel = null;

        #endregion

        #region 公用操作

        #region 音频播放

        /// <summary>
        /// 当前音频播放器实例。
        /// </summary>
        private AudioPlayer _audioPlayer = new AudioPlayer();

        /// <summary>
        /// 播放指定的音频文件。
        /// <para>如果已有音频正在播放，将立即停止并播放新的音频。</para>
        /// <para>支持 wav 和 mp3 格式。</para>
        /// </summary>
        /// <param name="filePath">音频文件路径。</param>
        private void playSound(string filePath)
        {
            _audioPlayer.Play(filePath);
        }

        /// <summary>
        /// 停止当前音频播放。
        /// </summary>
        private void stopSound()
        {
            _audioPlayer.Stop();
        }

        #endregion

        #region 预览声音管理

        /// <summary>
        /// 事件处理：预览声音状态变化。
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void previewVoiceManager_StatusChanged(object sender, PreviewVoiceStatusEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => previewVoiceManager_StatusChanged(sender, e)));
                return;
            }

            _ttlSchemePanel?.RefreshSpeakerRow(e.SourceName);
            refresh角色映射Row(e.SourceName);
            _ttlSchemePanel?.UpdateAutoGeneratePreviewButtonStatus();
        }

        /// <summary>
        /// 刷新角色映射表格中指定源名称的行。
        /// </summary>
        /// <param name="sourceName">源名称</param>
        private void refresh角色映射Row(string sourceName)
        {
            _roleMappingPanel?.RefreshVoicePreviewButtons();
        }

        /// <summary>
        /// 请求生成预览声音。
        /// </summary>
        /// <param name="sourceName">朗读者源名称</param>
        private void requestPreviewVoice(string sourceName)
        {
            _ttlSchemePanel?.RequestPreviewVoice(sourceName);
        }

        /// <summary>
        /// 获取预览声音状态。
        /// </summary>
        /// <param name="sourceName">朗读者源名称</param>
        /// <returns>预览声音状态</returns>
        private PreviewVoiceStatus getPreviewVoiceStatus(string sourceName)
        {
            return _ttlSchemePanel?.GetPreviewVoiceStatus(sourceName) ?? PreviewVoiceStatus.未生成;
        }

        #endregion

        #region UI操作

        /// <summary>
        /// 复制给定文本到剪贴板。
        /// </summary>
        /// <param name="text"></param>
        private static void copyToClipboard(string text)
        {
            UiControlHelper.CopyToClipboard(text);
        }

        /// <summary>
        /// 在UI线程上执行指定操作。
        /// </summary>
        /// <param name="action">要执行的操作。</param>
        private void updateUi(Action action)
        {
            UiHelper.UpdateUi(this, action);
        }

        /// <summary>
        /// 配置DataGridView的基本样式。
        /// </summary>
        /// <param name="grid">要配置的表格。</param>
        /// <param name="fontSize">字体大小，默认10.5。</param>
        private static void setupDataGridViewBasicStyle(DataGridView grid, float fontSize = 10.5f)
        {
            UiControlHelper.SetupDataGridViewBasicStyle(grid, fontSize);
        }

        /// <summary>
        /// 在文本框的光标位置插入分隔符。
        /// </summary>
        /// <param name="textBox">文本框控件。</param>
        /// <param name="separator">要插入的分隔符。</param>
        private static void insertSeparatorAtCursor(TextBox textBox, string separator)
        {
            UiControlHelper.InsertSeparatorAtCursor(textBox, separator);
        }

        /// <summary>
        /// 显示确认对话框。
        /// </summary>
        /// <param name="message">确认消息。</param>
        /// <param name="title">对话框标题。</param>
        /// <returns>用户是否确认。</returns>
        private static bool showConfirmDialog(string message, string title = "确认")
        {
            return UiControlHelper.ShowConfirmDialog(message, title);
        }

        #endregion

        #region 项目文件操作

        /// <summary>
        /// 项目文件管理器实例。
        /// </summary>
        private ProjectFile _projectFile = null;

        /// <summary>
        /// 获取项目文件管理器实例（延迟初始化）。
        /// </summary>
        private ProjectFile getProjectFile()
        {
            if (_projectFile == null)
            {
                string filePath = !string.IsNullOrEmpty(_currentProjectFilePath) ? _currentProjectFilePath : 默认项目_文件;
                _projectFile = new ProjectFile(filePath);
            }
            return _projectFile;
        }

        /// <summary>
        /// 加载指定项目文件。
        /// </summary>
        /// <param name="filePath">指定项目文件。</param>
        private void loadProjectFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) == false)
            {
                try
                {
                    _currentProjectFilePath = filePath;
                    _projectFile = new ProjectFile(_currentProjectFilePath);

                    load角色映射PanelData();
                    load文本拆分();
                    load多音字替换PanelData();
                    load角色和情绪指定PanelData();
                    load语音生成预处理PanelData();
                    load语音生成PanelData();

                    _isSaveOnClose = true;
                    _isProjectModified = false;
                    setProjectMenuEnabled(true);
                }
                catch (Exception ex)
                {
                    if (filePath.ToLower().Equals(默认项目_文件.ToLower()))
                        MessageBox.Show($"加载默认项目文件失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show($"加载项目文件 {filePath} 失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    _currentProjectFilePath = null;
                }
            }

            updateTitleBar();
        }

        /// <summary>
        /// 保存所有项目数据到文件。
        /// </summary>
        private void saveProjectFile()
        {
            save文本拆分Data();
            save多音字替换PanelData();
            save角色映射PanelData();
            save角色和情绪指定PanelData();
            save语音生成预处理PanelData();
            save语音生成PanelData();

            getProjectFile()?.Save();
            _isProjectModified = false;
            updateTitleBar();
        }

        #endregion

        #region TTL方案操作

        /// <summary>
        /// 标记项目内容已被修改。
        /// </summary>
        private void markProjectModified()
        {
            if (!_isProjectModified)
            {
                _isProjectModified = true;
                updateTitleBar();
            }
        }

        /// <summary>
        /// 获取当前TTL引擎的ID，如果没有选定引擎则返回表示"无"方案的特定ID。
        /// </summary>
        /// <returns>当前TTL引擎的ID，或表示"无"方案的特定ID</returns>
        private string getCurrentEngineId()
        {
            return _ttlSchemePanel?.GetCurrentEngineId();
        }

        #endregion

        #endregion

        #region 主UI操作

        #region UI事件处理

        /// <summary>
        /// 事件处理：主窗体显示。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Shown(object sender, EventArgs e)
        {
            string filePath = null;

            // 如果通过命令行指定了项目文件，直接加载
            if (!string.IsNullOrEmpty(_currentProjectFilePath) && File.Exists(_currentProjectFilePath))
            {
                filePath = _currentProjectFilePath;
            }
            else
            {
                // 使用默认项目文件，沿用之前的逻辑
                var button = MessageBox.Show(
                    "选择\"是\"将恢复上次关闭时编辑的内容；" +
                    "选择\"否\"将不恢复之前的内容，且在退出时保存新的内容；" +
                    "选择\"取消\"将暂时不恢复之前的内容，但在退出时也不会保存新的内容。",
                    "恢复之前的内容吗？",
                    MessageBoxButtons.YesNoCancel);
                if (button == DialogResult.Yes)
                {
                    filePath = 默认项目_文件;
                }
                else if (button == DialogResult.No)
                {
                    _isInitializing = false;
                    _isProjectModified = false;
                    _isSaveOnClose = true;
                    clearProjectContent();
                    _ttlSchemePanel?.StartTtlEngineConnectionIfSelected();
                    updateTitleBar();
                    return;
                }
                else if (button == DialogResult.Cancel)
                {
                    _isInitializing = false;
                    _isSaveOnClose = false;
                    _isProjectModified = false;
                    setProjectMenuEnabled(false);
                    clearProjectContent();
                    updateTitleBar();
                    return;
                }
            }

            loadProjectFile(filePath);
            _isInitializing = false;
            _ttlSchemePanel?.StartTtlEngineConnectionIfSelected();
        }

        /// <summary>
        /// 设置项目相关菜单的启用状态。
        /// </summary>
        /// <param name="enabled">是否启用</param>
        private void setProjectMenuEnabled(bool enabled)
        {
            this.新建NToolStripMenuItem.Enabled = enabled;
            this.打开OToolStripMenuItem.Enabled = enabled;
            this.保存SToolStripMenuItem.Enabled = enabled;
            this.另存为AToolStripMenuItem.Enabled = enabled;
        }

        /// <summary>
        /// 检查是否为默认项目文件。
        /// </summary>
        /// <returns>如果是默认项目文件返回true，否则返回false</returns>
        private bool isDefaultProjectFile()
        {
            return string.IsNullOrEmpty(_currentProjectFilePath) ||
                   _currentProjectFilePath.Equals(默认项目_文件, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 检查是否需要保存并提示用户。
        /// </summary>
        /// <returns>返回true表示可以继续操作（已保存或用户选择不保存），返回false表示用户取消操作</returns>
        private bool checkSaveAndConfirm()
        {
            if (!_isProjectModified)
            {
                return true;
            }

            DialogResult result = MessageBox.Show(
                "项目内容已修改，是否保存？",
                "保存提示",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (isDefaultProjectFile())
                {
                    return performSaveAs();
                }
                else
                {
                    saveProjectFile();
                    MessageBox.Show("项目已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
            }
            else if (result == DialogResult.No)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 执行另存为操作。
        /// </summary>
        /// <returns>返回true表示保存成功，返回false表示用户取消</returns>
        private bool performSaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = $"TTL工具箱项目文件|*{项目文件_扩展名}";
            saveFileDialog.Title = "保存项目文件";
            saveFileDialog.FileName = $"新项目{项目文件_扩展名}";

            string lastFolder = Setting.GetValue(_Setting_Key_LastProjectFolder, string.Empty);
            if (!string.IsNullOrWhiteSpace(lastFolder) && Directory.Exists(lastFolder))
            {
                saveFileDialog.InitialDirectory = lastFolder;
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _currentProjectFilePath = saveFileDialog.FileName;
                Setting.SetValue(_Setting_Key_LastProjectFolder, Path.GetDirectoryName(saveFileDialog.FileName));
                _projectFile = new ProjectFile(_currentProjectFilePath);
                saveProjectFile();
                _isSaveOnClose = true;
                setProjectMenuEnabled(true);
                MessageBox.Show("项目已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 清空所有项目内容。
        /// </summary>
        private void clearProjectContent()
        {
            getProjectFile()?.ClearAllData();
            _textSplitPanel?.ClearPanel();
            _polyphonicReplacePanel?.ClearPanel();
            _roleEmotionPanel?.ClearData();
            _voicePreprocessPanel?.ClearFinalText();
            _voiceGenerationPanel?.ClearAllTasks();
            _roleMappingPanel?.ClearCachedRoleNames();

            load角色映射PanelData();
            load多音字替换PanelData();
            load角色和情绪指定PanelData();
            load语音生成预处理PanelData();
            load语音生成PanelData();
            _voicePreprocessPanel?.LoadDefaultSpeakerSettings();
        }

        /// <summary>
        /// 事件处理：点击"新建"菜单。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 新建NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!checkSaveAndConfirm())
            {
                return;
            }

            if (isDefaultProjectFile())
            {
                DialogResult result = MessageBox.Show(
                    "确定要清空当前项目并新建吗？",
                    "新建项目",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            _currentProjectFilePath = 默认项目_文件;
            _projectFile = null;
            _isProjectModified = false;

            clearProjectContent();

            updateTitleBar();
            _isSaveOnClose = true;
            setProjectMenuEnabled(true);
        }

        /// <summary>
        /// 事件处理：点击"打开"菜单。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!checkSaveAndConfirm())
            {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = $"TTL工具箱项目文件|*{项目文件_扩展名}";
            openFileDialog.Title = "打开项目文件";

            string lastFolder = Setting.GetValue(_Setting_Key_LastProjectFolder, string.Empty);
            if (!string.IsNullOrWhiteSpace(lastFolder) && Directory.Exists(lastFolder))
            {
                openFileDialog.InitialDirectory = lastFolder;
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Setting.SetValue(_Setting_Key_LastProjectFolder, Path.GetDirectoryName(openFileDialog.FileName));
                    loadProjectFile(openFileDialog.FileName);
                    _isProjectModified = false;
                    updateTitleBar();
                    _isSaveOnClose = true;
                    setProjectMenuEnabled(true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载项目文件失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 事件处理：点击"保存"菜单。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isDefaultProjectFile())
            {
                performSaveAs();
            }
            else
            {
                saveProjectFile();
                MessageBox.Show("项目已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 事件处理：点击"另存为"菜单。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 另存为AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            performSaveAs();
        }

        /// <summary>
        /// 事件处理：点击"退出"菜单。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 退出EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 事件处理：主窗体正在关闭。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_polyphonicReplacePanel != null && _polyphonicReplacePanel.IsSchemeTableChanged)
            {
                DialogResult result = MessageBox.Show(
                    "多音字替换方案表有未保存的更改，是否保存？",
                    "保存提示",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }

                if (result == DialogResult.Yes)
                {
                    _polyphonicReplacePanel.SaveData();
                }
            }

            if (isDefaultProjectFile())
            {
                if (_isSaveOnClose)
                {
                    if (MessageBox.Show(
                        $"即将关闭软件，正在编辑的内容将会被自动保存到默认项目文件。确认退出吗？",
                        "确认退出",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }

                    stopSound();
                    _currentProjectFilePath = 默认项目_文件;
                    saveProjectFile();
                }
                else
                {
                    if (MessageBox.Show(
                        $"即将关闭软件，正在编辑的内容不会被保存。确认退出吗？",
                        "确认退出",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }

                    stopSound();
                }
            }
            else
            {
                if (_isProjectModified)
                {
                    DialogResult result = MessageBox.Show(
                        "项目内容已修改，是否保存？",
                        "保存提示",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }

                    stopSound();

                    if (result == DialogResult.Yes)
                    {
                        saveProjectFile();
                    }
                }
                else
                {
                    if (MessageBox.Show(
                        "确认退出吗？",
                        "确认退出",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }

                    stopSound();
                }
            }
        }

        /// <summary>
        /// 事件处理：主选项卡切换。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tc_工具箱_SelectedIndexChanged(object sender, EventArgs e)
        {
            var currPage = this.tc_工具箱.SelectedTab;
            if (currPage == this.tp_TTL方案) _ttlSchemePanel?.RefreshUi();
            else if (currPage == this.tp_文本拆分) _textSplitPanel?.RefreshUi();
            else if (currPage == this.tp_多音字替换)
            {
                if (_polyphonicReplacePanel != null)
                {
                    _polyphonicReplacePanel.InitializeSchemeDataSource();
                    _polyphonicReplacePanel.ResetChangedFlag();
                    _polyphonicReplacePanel.RefreshUi();
                }
            }
            else if (currPage == this.tp_角色和情绪指定)
            {
                _roleEmotionPanel?.RefreshUi();
            }
            else if (currPage == this.tp_语音生成预处理)
            {
                _voicePreprocessPanel?.RefreshUi();
            }
        }

        #endregion

        #region UI操作

        /// <summary>
        /// 软件版本号。
        /// </summary>
        private static readonly string _软件版本 = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// 初始化主UI。
        /// </summary>
        private void init主Ui()
        {
            // 设置自动保存
            {
                Setting.IsAutoSave = true;
            }

            // 初始化预览声音管理器
            {
                if (!File.Exists(TTL角色预览声音_文件))
                {
                    try
                    {
                        string directory = Path.GetDirectoryName(TTL角色预览声音_文件);
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        File.WriteAllText(TTL角色预览声音_文件, TTL角色预览声音_文本);
                    }
                    catch
                    {
                    }
                }

                _previewVoiceManager = new PreviewVoiceManager(
                    TTL角色预览声音_文件夹,
                    TTL角色预览声音_文件,
                    TTL角色预览声音_文本);

                _previewVoiceManager.StatusChanged += previewVoiceManager_StatusChanged;
            }

            // 更新标题栏
            updateTitleBar();
        }

        /// <summary>
        /// 更新标题栏显示。
        /// </summary>
        private void updateTitleBar()
        {
            string projectFileName = "未命名";
            string modifiedMark = _isProjectModified ? "*" : "";

            if (_isSaveOnClose == false)
            {
                projectFileName = "[不自动保存]";
            }
            else if (string.IsNullOrEmpty(_currentProjectFilePath))
            {
                projectFileName = "[默认项目]";
            }
            else
            {
                projectFileName = Path.GetFileName(_currentProjectFilePath);
            }

            this.Text = $"TTL 工具箱 {_软件版本} - {modifiedMark}{projectFileName}";
        }

        /// <summary>
        /// 切换到下一个选项卡。
        /// </summary>
        /// <param name="text">当前选项卡处理完的文本。</param>
        private void switchToNextPage(string text)
        {
            if (this.tc_工具箱.TabPages.Count > 0 &&
                this.tc_工具箱.SelectedIndex < this.tc_工具箱.TabPages.Count - 1)
            {
                this.tc_工具箱.SelectedIndex++;
                text = text == null ? string.Empty : text;

                var currPage = this.tc_工具箱.SelectedTab;
                if (currPage == this.tp_多音字替换)
                {
                    _polyphonicReplacePanel?.SetFinalText(text);
                }
                else if (currPage == this.tp_角色和情绪指定)
                {
                    _roleEmotionPanel?.FillTable(text);
                }
                else if (currPage == this.tp_语音生成预处理)
                {
                    _voicePreprocessPanel?.SetFinalText(text);
                }
                else if (currPage == this.tp_语音生成)
                {
                    _voiceGenerationPanel?.FillTaskList(text);
                }
            }
        }

        #endregion

        #region 业务操作

        /// <summary>
        /// 是否在退出时保存正在编辑的内容。
        /// </summary>
        private bool _isSaveOnClose = true;

        /// <summary>
        /// 项目内容是否已被修改。
        /// </summary>
        private bool _isProjectModified = false;

        /// <summary>
        /// 上次保存对话框使用的文件夹路径。
        /// </summary>
        private const string _Setting_Key_LastProjectFolder = "LastProjectFolder";

        /// <summary>
        /// TTL引擎连接状态管理器。
        /// </summary>
        private System.Windows.Forms.Timer _ttlEngineConnectionTimer = null;

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
        /// 连接任务取消令牌源。
        /// </summary>
        private System.Threading.CancellationTokenSource _connectionCancellationTokenSource = null;

        #endregion

        #endregion

        #region TTL方案Panel操作

        /// <summary>
        /// 初始化TTL方案面板。
        /// </summary>
        private void initTTL方案Panel()
        {
            // 创建面板实例
            _ttlSchemePanel = new TtlSchemePanel(_tempFolder);
            _ttlSchemePanel.Dock = DockStyle.Fill;

            // 设置面板依赖
            _ttlSchemePanel.TtlSchemeController = _ttlSchemeController;
            _ttlSchemePanel.PreviewVoiceManager = _previewVoiceManager;
            _ttlSchemePanel.ConnectionStatusLabel = this.tssl_TTL连接状态显示;
            _ttlSchemePanel.VoiceGenerationTabPage = this.tp_语音生成;

            // 设置委托
            _ttlSchemePanel.SetFindSpeakerDelegate(findSpeakerByShowName);
            _ttlSchemePanel.SetPlaySoundDelegate(playSound);
            _ttlSchemePanel.SetGetPreviewTextDelegate(() => _previewVoiceManager?.GetPreviewText() ?? Constants.TTL角色预览声音_文本);

            // 订阅面板事件
            _ttlSchemePanel.ProjectModified += (s, e) => markProjectModified();
            _ttlSchemePanel.EngineChanged += (s, e) =>
            {
                // 先保存旧引擎的数据
                if (!string.IsNullOrEmpty(e.PreviousEngineId))
                {
                    _roleMappingPanel.ProjectFile = getProjectFile();
                    _roleMappingPanel.CurrentEngineId = e.PreviousEngineId;
                    _roleMappingPanel.SaveData();

                    _polyphonicReplacePanel.ProjectFile = getProjectFile();
                    _polyphonicReplacePanel.CurrentEngineId = e.PreviousEngineId;
                    _polyphonicReplacePanel.SaveData();

                    _roleEmotionPanel.ProjectFile = getProjectFile();
                    _roleEmotionPanel.CurrentEngineId = e.PreviousEngineId;
                    _roleEmotionPanel.SaveData();

                    _voicePreprocessPanel.ProjectFile = getProjectFile();
                    _voicePreprocessPanel.CurrentEngineId = e.PreviousEngineId;
                    _voicePreprocessPanel.SaveData();

                    _voiceGenerationPanel.ProjectFile = getProjectFile();
                    _voiceGenerationPanel.CurrentEngineId = e.PreviousEngineId;
                    _voiceGenerationPanel.SaveData();
                }

                // 加载新引擎的数据
                load角色映射PanelData();
                load多音字替换PanelData();
                _roleEmotionPanel?.SetupDataGridViewColumns();
                load角色和情绪指定PanelData();
                load语音生成预处理PanelData();
                load语音生成PanelData();
                _voicePreprocessPanel?.LoadDefaultSpeakerSettings();
            };
            _ttlSchemePanel.SpeakersChanged += (s, e) =>
            {
                refresh角色映射SourceNameOptions();
                _roleEmotionPanel?.RefreshRoleList();
                _voicePreprocessPanel?.LoadDefaultSpeakerSettings();
            };
            _ttlSchemePanel.SaveVoiceGenerationTasksRequested += (s, e) =>
            {
                // 使用当前引擎ID保存（在引擎切换前调用，此时CurrentEngineId还是旧引擎）
                save语音生成PanelData(getCurrentEngineId());
            };
            _ttlSchemePanel.StopVoiceGenerationTaskQueueRequested += (s, e) => _voiceGenerationPanel?.StopQueueAndWait();

            // 将面板添加到TabPage
            this.tp_TTL方案.Controls.Clear();
            this.tp_TTL方案.Controls.Add(_ttlSchemePanel);

            // 初始化面板
            _ttlSchemePanel.InitializePanel();
        }

        /// <summary>
        /// 刷新角色映射表格的源名称下拉列表选项。
        /// </summary>
        private void refresh角色映射SourceNameOptions()
        {
            _roleMappingPanel?.RefreshUi();
        }

        #endregion

        #region 文本拆分面板操作

        /// <summary>
        /// 初始化文本拆分面板。
        /// </summary>
        private void init文本拆分Panel()
        {
            // 创建面板实例
            _textSplitPanel = new TextSplitPanel();
            _textSplitPanel.Dock = DockStyle.Fill;

            // 将面板添加到TabPage
            this.tp_文本拆分.Controls.Clear();
            this.tp_文本拆分.Controls.Add(_textSplitPanel);

            // 订阅面板事件
            _textSplitPanel.ProjectModified += (s, e) => markProjectModified();
            _textSplitPanel.SwitchToNextPageRequested += (s, e) => switchToNextPage(e.Text);

            // 初始化面板
            _textSplitPanel.InitializePanel();
        }

        /// <summary>
        /// 保存文本拆分工作内容到项目文件对象。
        /// </summary>
        private void save文本拆分Data()
        {
            if (_textSplitPanel != null)
            {
                _textSplitPanel.SaveData(getProjectFile());
            }
        }

        /// <summary>
        /// 加载文本拆分工作内容。
        /// </summary>
        private void load文本拆分()
        {
            if (_textSplitPanel != null)
            {
                _textSplitPanel.LoadData(getProjectFile());
            }
        }

        #endregion

        #region 多音字替换Panel操作

        /// <summary>
        /// 初始化多音字替换面板。
        /// </summary>
        private void init多音字替换Panel()
        {
            _polyphonicReplacePanel = new PolyphonicReplacePanel();
            _polyphonicReplacePanel.Dock = DockStyle.Fill;
            _polyphonicReplacePanel.TtlSchemeController = _ttlSchemeController;

            _polyphonicReplacePanel.ProjectModified += (s, e) => markProjectModified();
            _polyphonicReplacePanel.SwitchToNextPageRequested += (s, e) => switchToNextPage(e.Text);

            this.tp_多音字替换.Controls.Clear();
            this.tp_多音字替换.Controls.Add(_polyphonicReplacePanel);

            _polyphonicReplacePanel.InitializePanel();
        }

        /// <summary>
        /// 加载多音字替换面板数据。
        /// </summary>
        private void load多音字替换PanelData()
        {
            if (_polyphonicReplacePanel != null)
            {
                _polyphonicReplacePanel.ProjectFile = getProjectFile();
                _polyphonicReplacePanel.CurrentEngineId = getCurrentEngineId();
                _polyphonicReplacePanel.LoadData();
            }
        }

        /// <summary>
        /// 保存多音字替换面板数据。
        /// </summary>
        private void save多音字替换PanelData()
        {
            if (_polyphonicReplacePanel != null)
            {
                _polyphonicReplacePanel.ProjectFile = getProjectFile();
                _polyphonicReplacePanel.CurrentEngineId = getCurrentEngineId();
                _polyphonicReplacePanel.SaveData();
            }
        }

        #endregion

        #region 语音生成Panel操作

        /// <summary>
        /// 初始化语音生成面板。
        /// </summary>
        private void init语音生成Panel()
        {
            _voiceGenerationPanel = new VoiceGenerationPanel();
            _voiceGenerationPanel.Dock = DockStyle.Fill;
            _voiceGenerationPanel.TtlSchemeController = _ttlSchemeController;
            _voiceGenerationPanel.TempFolder = _tempFolder;
            _voiceGenerationPanel.FfmpegPath = Ffmpeg_文件;
            _voiceGenerationPanel.GetEngineConnectionStatus = () => _ttlEngineConnectionStatus;
            _voiceGenerationPanel.RequestEngineConnection = () => checkAndConnectTtlEngineWhenTaskStarts();
            _voiceGenerationPanel.PlaySound = playSound;
            _voiceGenerationPanel.FindSpeaker = findSpeakerByShowName;

            _voiceGenerationPanel.ProjectModified += (s, e) => markProjectModified();
            _voiceGenerationPanel.PreviewTaskCompleted += (s, e) => _ttlSchemePanel?.UpdateAutoGeneratePreviewButtonStatus(true);
            _voiceGenerationPanel.OpenPreviewVoiceFolderRequested += (s, e) => Process.Start("explorer.exe", TTL角色预览声音_文件夹);

            this.pan_语音生成.Controls.Clear();
            this.pan_语音生成.Controls.Add(_voiceGenerationPanel);

            _voiceGenerationPanel.InitializePanel();
        }

        /// <summary>
        /// 加载语音生成面板数据。
        /// </summary>
        private void load语音生成PanelData()
        {
            if (_voiceGenerationPanel != null)
            {
                _voiceGenerationPanel.ProjectFile = getProjectFile();
                _voiceGenerationPanel.CurrentEngineId = getCurrentEngineId();
                _voiceGenerationPanel.LoadData();
            }
        }

        /// <summary>
        /// 保存语音生成面板数据。
        /// </summary>
        private void save语音生成PanelData()
        {
            if (_voiceGenerationPanel != null)
            {
                _voiceGenerationPanel.ProjectFile = getProjectFile();
                _voiceGenerationPanel.CurrentEngineId = getCurrentEngineId();
                _voiceGenerationPanel.SaveData();
            }
        }

        /// <summary>
        /// 保存语音生成面板数据到指定引擎。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        private void save语音生成PanelData(string engineId)
        {
            if (_voiceGenerationPanel != null)
            {
                _voiceGenerationPanel.ProjectFile = getProjectFile();
                _voiceGenerationPanel.SaveData(engineId);
            }
        }

        #endregion

        #region 语音生成预处理Panel操作

        /// <summary>
        /// 初始化语音生成预处理面板。
        /// </summary>
        private void init语音生成预处理Panel()
        {
            _voicePreprocessPanel = new VoicePreprocessPanel();
            _voicePreprocessPanel.Dock = DockStyle.Fill;
            _voicePreprocessPanel.TtlSchemeController = _ttlSchemeController;
            _voicePreprocessPanel.RoleMappingPanel = _roleMappingPanel;

            _voicePreprocessPanel.ProjectModified += (s, e) => markProjectModified();
            _voicePreprocessPanel.SwitchToNextPageRequested += (s, e) => switchToNextPage(e.Text);
            _voicePreprocessPanel.InsertSeparatorRequested += (s, e) => insertSeparatorAtCursor(e.TextBox, e.Separator);

            this.pan_语音生成预处理.Controls.Clear();
            this.pan_语音生成预处理.Controls.Add(_voicePreprocessPanel);

            _voicePreprocessPanel.InitializePanel();
        }

        /// <summary>
        /// 加载语音生成预处理面板数据。
        /// </summary>
        private void load语音生成预处理PanelData()
        {
            if (_voicePreprocessPanel != null)
            {
                _voicePreprocessPanel.ProjectFile = getProjectFile();
                _voicePreprocessPanel.CurrentEngineId = getCurrentEngineId();
                _voicePreprocessPanel.LoadData();
            }
        }

        /// <summary>
        /// 保存语音生成预处理面板数据。
        /// </summary>
        private void save语音生成预处理PanelData()
        {
            if (_voicePreprocessPanel != null)
            {
                _voicePreprocessPanel.ProjectFile = getProjectFile();
                _voicePreprocessPanel.CurrentEngineId = getCurrentEngineId();
                _voicePreprocessPanel.SaveData();
            }
        }

        /// <summary>
        /// 保存语音生成预处理面板数据到指定引擎。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        private void save语音生成预处理PanelData(string engineId)
        {
            if (_voicePreprocessPanel != null)
            {
                _voicePreprocessPanel.ProjectFile = getProjectFile();
                _voicePreprocessPanel.SaveData(engineId);
            }
        }

        #endregion

        #region 角色映射Panel操作

        /// <summary>
        /// 初始化角色映射面板。
        /// </summary>
        private void init角色映射Panel()
        {
            _roleMappingPanel = new RoleMappingPanel();
            _roleMappingPanel.Dock = DockStyle.Fill;
            _roleMappingPanel.TtlSchemeController = _ttlSchemeController;

            _roleMappingPanel.ProjectModified += (s, e) => markProjectModified();
            _roleMappingPanel.RoleMappingListChanged += (s, e) => _roleEmotionPanel?.RefreshRoleList();
            _roleMappingPanel.PreviewVoiceRequested += (s, e) => requestPreviewVoice(e.SourceName);
            _roleMappingPanel.SetPreviewVoiceStatusGetter(sourceName => getPreviewVoiceStatus(sourceName));

            this.pan_角色映射.Controls.Clear();
            this.pan_角色映射.Controls.Add(_roleMappingPanel);

            _roleMappingPanel.InitializePanel();
        }

        /// <summary>
        /// 加载角色映射面板数据。
        /// </summary>
        private void load角色映射PanelData()
        {
            if (_roleMappingPanel != null)
            {
                _roleMappingPanel.ProjectFile = getProjectFile();
                _roleMappingPanel.CurrentEngineId = getCurrentEngineId();
                _roleMappingPanel.LoadData();
            }
        }

        /// <summary>
        /// 保存角色映射面板数据。
        /// </summary>
        private void save角色映射PanelData()
        {
            if (_roleMappingPanel != null)
            {
                _roleMappingPanel.ProjectFile = getProjectFile();
                _roleMappingPanel.CurrentEngineId = getCurrentEngineId();
                _roleMappingPanel.SaveData();
            }
        }

        /// <summary>
        /// 保存角色映射面板数据到指定引擎。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        private void save角色映射PanelData(string engineId)
        {
            if (_roleMappingPanel != null)
            {
                _roleMappingPanel.ProjectFile = getProjectFile();
                _roleMappingPanel.SaveData(engineId);
            }
        }

        #endregion

        #region 角色和情绪指定Panel操作

        /// <summary>
        /// 初始化角色和情绪指定面板。
        /// </summary>
        private void init角色和情绪指定Panel()
        {
            _roleEmotionPanel = new RoleEmotionPanel();
            _roleEmotionPanel.Dock = DockStyle.Fill;
            _roleEmotionPanel.TtlSchemeController = _ttlSchemeController;
            _roleEmotionPanel.GetRoleNamesFunc = () => _roleMappingPanel?.GetRoleNames();

            _roleEmotionPanel.ProjectModified += (s, e) => markProjectModified();
            _roleEmotionPanel.SwitchToNextPageRequested += (s, e) => switchToNextPage(e.Text);

            this.pan_角色和情绪指定.Controls.Clear();
            this.pan_角色和情绪指定.Controls.Add(_roleEmotionPanel);

            _roleEmotionPanel.InitializePanel();
        }

        /// <summary>
        /// 加载角色和情绪指定面板数据。
        /// </summary>
        private void load角色和情绪指定PanelData()
        {
            if (_roleEmotionPanel != null)
            {
                _roleEmotionPanel.ProjectFile = getProjectFile();
                _roleEmotionPanel.CurrentEngineId = getCurrentEngineId();
                _roleEmotionPanel.LoadData();
            }
        }

        /// <summary>
        /// 保存角色和情绪指定面板数据。
        /// </summary>
        private void save角色和情绪指定PanelData()
        {
            if (_roleEmotionPanel != null)
            {
                _roleEmotionPanel.ProjectFile = getProjectFile();
                _roleEmotionPanel.CurrentEngineId = getCurrentEngineId();
                _roleEmotionPanel.SaveData();
            }
        }

        /// <summary>
        /// 保存角色和情绪指定面板数据到指定引擎。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        private void save角色和情绪指定PanelData(string engineId)
        {
            if (_roleEmotionPanel != null)
            {
                _roleEmotionPanel.ProjectFile = getProjectFile();
                _roleEmotionPanel.SaveData(engineId);
            }
        }

        #endregion

        #region 业务操作

        #region TTL引擎操作

        /// <summary>
        /// 初始化TTL引擎连接管理器
        /// </summary>
        private void initTtlEngineConnectionManager()
        {
            _ttlEngineConnectionTimer = new System.Windows.Forms.Timer();
            _ttlEngineConnectionTimer.Interval = 1000;
            _ttlEngineConnectionTimer.Tick += ttlEngineConnectionTimer_Tick;

            _ttlEngineConnectionTimer.Start();

            updateTtlEngineConnectionStatusLabel();
        }

        /// <summary>
        /// 定时器回调：处理TTL引擎连接状态
        /// </summary>
        private void ttlEngineConnectionTimer_Tick(object sender, EventArgs e)
        {
            switch (_ttlEngineConnectionStatus)
            {
                case TtlEngineConnectionStatus.连接中:
                    handle连接中状态();
                    break;
                case TtlEngineConnectionStatus.连接成功:
                    handle连接成功状态();
                    break;
                case TtlEngineConnectionStatus.连接失败:
                    handle连接失败状态();
                    break;
                case TtlEngineConnectionStatus.未连接:
                default:
                    break;
            }

            updateTtlEngineConnectionStatusLabel();
        }

        /// <summary>
        /// 处理连接中状态
        /// </summary>
        private void handle连接中状态()
        {
            _ttlEngineConnectionCountdown--;
            if (_ttlEngineConnectionCountdown <= 0)
            {
                _ttlEngineConnectionStatus = TtlEngineConnectionStatus.连接失败;
                _ttlEngineRetryCountdown = 连接失败_重试间隔秒数;
                stop连接任务();
            }
        }

        /// <summary>
        /// 处理连接成功状态
        /// </summary>
        private void handle连接成功状态()
        {
            _ttlEngineRetryCountdown--;
            if (_ttlEngineRetryCountdown <= 0)
            {
                start连接TTL引擎();
            }
        }

        /// <summary>
        /// 处理连接失败状态
        /// </summary>
        private void handle连接失败状态()
        {
            _ttlEngineRetryCountdown--;
            if (_ttlEngineRetryCountdown <= 0)
            {
                start连接TTL引擎();
            }
        }

        /// <summary>
        /// 开始连接TTL引擎
        /// </summary>
        private async void start连接TTL引擎()
        {
            var currentEngine = _ttlSchemeController.CurrentEngineConnector;
            if (currentEngine == null)
            {
                _ttlEngineConnectionStatus = TtlEngineConnectionStatus.未连接;
                updateTtlEngineConnectionStatusLabel();
                _ttlSchemePanel?.RefreshUi();
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
            _ttlSchemePanel?.RefreshUi();

            try
            {
                await currentEngine.ConnectAsync();

                _ttlEngineConnectionStatus = TtlEngineConnectionStatus.连接成功;
                _ttlEngineRetryCountdown = 连接成功_验证间隔秒数;

                _ttlSchemePanel?.RefreshSpeakers();

                _voiceGenerationPanel?.TryResumeQueue();
            }
            catch
            {
                _ttlEngineConnectionStatus = TtlEngineConnectionStatus.连接失败;
                _ttlEngineRetryCountdown = 连接失败_重试间隔秒数;
            }
            finally
            {
                _connectionCancellationTokenSource?.Dispose();
                _connectionCancellationTokenSource = null;
            }

            updateTtlEngineConnectionStatusLabel();
            _ttlSchemePanel?.RefreshUi();
        }

        /// <summary>
        /// 停止连接任务
        /// </summary>
        private void stop连接任务()
        {
            _connectionCancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// 更新TTL引擎连接状态标签
        /// </summary>
        private void updateTtlEngineConnectionStatusLabel()
        {
            Action action = () =>
            {
                string statusText = string.Empty;
                Color statusColor = SystemColors.ControlText;

                var currentEngine = _ttlSchemeController.CurrentEngineConnector;
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

                this.tssl_TTL连接状态显示.Text = statusText;
                this.tssl_TTL连接状态显示.ForeColor = statusColor;
            };

            UiHelper.UpdateUi(this, action);
        }

        /// <summary>
        /// 当有任务需要执行时，检查并尝试连接TTL引擎
        /// </summary>
        private void checkAndConnectTtlEngineWhenTaskStarts()
        {
            if (_ttlEngineConnectionStatus == TtlEngineConnectionStatus.未连接 ||
                _ttlEngineConnectionStatus == TtlEngineConnectionStatus.连接失败)
            {
                if (_ttlEngineConnectionTimer != null && !_ttlEngineConnectionTimer.Enabled)
                {
                    _ttlEngineConnectionTimer.Start();
                }
                start连接TTL引擎();
            }
        }

        #endregion

        #region 辅助操作

        /// <summary>
        /// 临时工作目录。
        /// </summary>
        private string _tempFolder = 临时_工作目录;

        /// <summary>
        /// 通过角色名或源名称查找SpeakerInfo对象。
        /// <para>首先在角色映射中查找角色名对应的源名称，然后从TTL引擎的朗读者列表中查找匹配的SpeakerInfo。</para>
        /// <para>如果角色映射中没有找到，则直接在TTL引擎的朗读者列表中查找源名称。</para>
        /// </summary>
        /// <param name="roleName">要查找的角色名或源名称</param>
        /// <returns>找到的SpeakerInfo对象，如果未找到则返回null</returns>
        private SpeakerInfo findSpeakerByShowName(string roleName)
        {
            var currentEngine = _ttlSchemeController.CurrentEngineConnector;
            if (string.IsNullOrEmpty(roleName) || currentEngine == null)
            {
                return null;
            }

            SpeakerInfo outValue = null;

            var speakers = currentEngine.Speakers;
            if (speakers != null && speakers.Length > 0)
            {
                var speaker = speakers.FirstOrDefault(s => s.SourceName.Equals(roleName, StringComparison.OrdinalIgnoreCase));
                if (speaker != null)
                {
                    outValue = speaker.Clone();
                }
            }

            return outValue;
        }

        #endregion

        #endregion

        #endregion
    }
}
