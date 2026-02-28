using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GW.TTLtoolsBox.Core.FileAccesser;
using GW.TTLtoolsBox.Core.SystemOption.TtlEngine;
using GW.TTLtoolsBox.WinFormUi.Base;
using GW.TTLtoolsBox.WinFormUi.Helper;
using GW.TTLtoolsBox.WinFormUi.Manager;
using static GW.TTLtoolsBox.WinFormUi.Helper.Constants;

namespace GW.TTLtoolsBox.WinFormUi.UI.Panels
{
    /// <summary>
    /// 语音生成预处理面板，提供语音生成前的文本预处理功能。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 管理最终文本的编辑和格式化
    /// - 支持插入段落分隔符
    /// - 支持发送文本到语音生成任务队列
    /// - 管理默认角色设置
    /// 
    /// 使用场景：
    /// - 在TTL工具箱中进行语音生成前的文本预处理
    /// - 作为工作流的一部分，将处理后的文本发送到语音生成
    /// 
    /// 依赖关系：
    /// - 依赖TtlSchemeController获取当前引擎信息
    /// - 依赖ProjectFile保存/加载预处理数据
    /// - 通过事件与MainForm通信
    /// </remarks>
    public partial class VoicePreprocessPanel : ViewBase
    {
        #region public

        #region 事件

        /// <summary>
        /// 项目内容被修改时触发的事件。
        /// </summary>
        public event EventHandler ProjectModified;

        /// <summary>
        /// 请求切换到下一页时触发的事件。
        /// </summary>
        public event EventHandler<SwitchToNextPageEventArgs> SwitchToNextPageRequested;

        /// <summary>
        /// 请求插入分隔符时触发的事件。
        /// </summary>
        public event EventHandler<InsertSeparatorEventArgs> InsertSeparatorRequested;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化VoicePreprocessPanel类的新实例。
        /// </summary>
        public VoicePreprocessPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region 属性

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
        /// 获取或设置角色映射面板引用，用于获取角色名称列表。
        /// </summary>
        public RoleMappingPanel RoleMappingPanel { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 初始化面板。
        /// </summary>
        public void InitializePanel()
        {
            init语音生成预处理Ui();
        }

        /// <summary>
        /// 刷新面板UI状态。
        /// </summary>
        public override void RefreshUi()
        {
            refresh语音生成预处理Ui();
        }

        /// <summary>
        /// 加载语音生成预处理数据。
        /// </summary>
        public void LoadData()
        {
            load语音生成预处理();
        }

        /// <summary>
        /// 加载指定引擎的语音生成预处理数据。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        public void LoadData(string engineId)
        {
            load语音生成预处理(engineId);
        }

        /// <summary>
        /// 保存语音生成预处理数据。
        /// </summary>
        public void SaveData()
        {
            save语音生成预处理Data();
        }

        /// <summary>
        /// 保存语音生成预处理数据到指定引擎。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        public void SaveData(string engineId)
        {
            save语音生成预处理Data(engineId);
        }

        /// <summary>
        /// 加载默认角色设置。
        /// </summary>
        public void LoadDefaultSpeakerSettings()
        {
            load语音生成预处理默认角色设置();
        }

        /// <summary>
        /// 清空最终文本。
        /// </summary>
        public void ClearFinalText()
        {
            clear语音生成预处理Ui();
        }

        /// <summary>
        /// 设置最终文本内容。
        /// </summary>
        /// <param name="text">文本内容。</param>
        public void SetFinalText(string text)
        {
            UpdateUi(() =>
            {
                this.tb_语音生成预处理_最终文本.Text = text;
            });
        }

        /// <summary>
        /// 获取最终文本内容。
        /// </summary>
        /// <returns>最终文本内容。</returns>
        public string GetFinalText()
        {
            return this.tb_语音生成预处理_最终文本.Text;
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
        /// 触发切换到下一页请求事件。
        /// </summary>
        /// <param name="text">要传递的文本。</param>
        protected void OnSwitchToNextPageRequested(string text)
        {
            SwitchToNextPageRequested?.Invoke(this, new SwitchToNextPageEventArgs(text));
        }

        /// <summary>
        /// 触发插入分隔符请求事件。
        /// </summary>
        /// <param name="textBox">文本框控件。</param>
        /// <param name="separator">分隔符。</param>
        protected void OnInsertSeparatorRequested(TextBox textBox, string separator)
        {
            InsertSeparatorRequested?.Invoke(this, new InsertSeparatorEventArgs(textBox, separator));
        }

        #endregion

        #endregion

        #region private

        #region UI初始化

        /// <summary>
        /// 初始化语音生成预处理UI。
        /// </summary>
        private void init语音生成预处理Ui()
        {
            this.tb_语音生成预处理_最终文本.MaxLength = int.MaxValue;

            this.nud_语音生成预处理_语速设置.Minimum = 1;
            this.nud_语音生成预处理_语速设置.Maximum = 100;
            this.nud_语音生成预处理_语速设置.Value = decimal.Parse(Setting.GetValue(this.nud_语音生成预处理_语速设置.Name, "100"));

            this.nud_语音生成预处理_空白时长.Value = decimal.Parse(Setting.GetValue(this.nud_语音生成预处理_空白时长.Name, "1"));

            this.cb_语音生成预处理_默认角色设置.DropDownStyle = ComboBoxStyle.DropDownList;

            this.nud_语音生成预处理_语速设置.ValueChanged += nud_语音生成预处理_语速设置_ValueChanged;
            this.cb_语音生成预处理_默认角色设置.SelectedIndexChanged += cb_语音生成预处理_默认角色设置_SelectedIndexChanged;

            // 加载默认角色
            load语音生成预处理默认角色设置();

            refresh语音生成预处理Ui();
        }

        #endregion

        #region UI操作

        /// <summary>
        /// 刷新语音生成预处理UI。
        /// </summary>
        private void refresh语音生成预处理Ui()
        {
            Action action = () =>
            {
                bool hasData = !string.IsNullOrWhiteSpace(this.tb_语音生成预处理_最终文本.Text);
                this.bt_语音生成预处理_清理文本.Enabled = hasData;
                this.bt_语音生成预处理_插入段落分隔符.Enabled = hasData;

                var currentEngine = TtlSchemeController?.CurrentEngineConnector;
                bool hasSpeakers = currentEngine != null && currentEngine.Speakers != null && currentEngine.Speakers.Length > 0;
                this.bt_语音生成预处理_发送到语音生成.Enabled = hasData && hasSpeakers;

                string text = this.tb_语音生成预处理_最终文本.Text;
                int taskCount = 0;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    taskCount = lines.Length;
                }
                this.lab_语音生成与处理_最终文本.Text = string.Format("最终文本：将分为 {0} 个语音生成任务", taskCount);
            };

            UpdateUi(action);
        }

        /// <summary>
        /// 清理语音生成预处理UI。
        /// </summary>
        private void clear语音生成预处理Ui()
        {
            Action action = () =>
            {
                this.tb_语音生成预处理_最终文本.Text = string.Empty;
            };

            UpdateUi(action);

            refresh语音生成预处理Ui();
        }

        /// <summary>
        /// 保存语音生成预处理工作内容到项目文件对象。
        /// </summary>
        private void save语音生成预处理Data()
        {
            save语音生成预处理Data(CurrentEngineId);
        }

        /// <summary>
        /// 保存语音生成预处理工作内容到项目文件对象。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        private void save语音生成预处理Data(string engineId)
        {
            if (!string.IsNullOrEmpty(engineId) && ProjectFile != null)
            {
                ProjectFile.SetVoicePreprocess_FinalText(engineId, this.tb_语音生成预处理_最终文本.Text);
            }
        }

        /// <summary>
        /// 加载语音生成预处理工作内容。
        /// </summary>
        private void load语音生成预处理()
        {
            load语音生成预处理(CurrentEngineId);
        }

        /// <summary>
        /// 加载语音生成预处理工作内容。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        private void load语音生成预处理(string engineId)
        {
            Action act = () =>
            {
                if (!string.IsNullOrEmpty(engineId) && ProjectFile != null)
                {
                    this.tb_语音生成预处理_最终文本.Text = ProjectFile.GetVoicePreprocess_FinalText(engineId);
                }
                else
                {
                    this.tb_语音生成预处理_最终文本.Text = string.Empty;
                }
            };

            UpdateUi(act);
        }

        /// <summary>
        /// 加载语音生成预处理默认角色设置。
        /// </summary>
        private void load语音生成预处理默认角色设置()
        {
            Action act = () =>
            {
                this.cb_语音生成预处理_默认角色设置.Items.Clear();

                var currentEngine = TtlSchemeController?.CurrentEngineConnector;
                if (currentEngine != null)
                {
                    var speakers = currentEngine.Speakers;
                    if (speakers != null && speakers.Length > 0)
                    {
                        foreach (var speaker in speakers)
                        {
                            this.cb_语音生成预处理_默认角色设置.Items.Add(speaker);
                        }

                        string key = $"{this.cb_语音生成预处理_默认角色设置.Name}_{currentEngine.Id}";
                        string savedSourceName = Setting.GetValue(key, string.Empty);
                        if (string.IsNullOrEmpty(savedSourceName))
                        {
                            key = $"{this.cb_语音生成预处理_默认角色设置.Name}_{currentEngine.Name}";
                            savedSourceName = Setting.GetValue(key, string.Empty);
                        }

                        if (!string.IsNullOrWhiteSpace(savedSourceName))
                        {
                            var savedSpeaker = speakers.FirstOrDefault(s => s.SourceName == savedSourceName);
                            if (savedSpeaker != null)
                            {
                                this.cb_语音生成预处理_默认角色设置.SelectedItem = savedSpeaker;
                            }
                            else
                            {
                                this.cb_语音生成预处理_默认角色设置.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            this.cb_语音生成预处理_默认角色设置.SelectedIndex = 0;
                        }
                    }
                }
            };

            UpdateUi(act);
        }

        #endregion

        #region 业务逻辑

        /// <summary>
        /// 检查角色是否存在。
        /// </summary>
        /// <param name="roleName">角色名称。</param>
        /// <returns>角色是否存在。</returns>
        private bool isRoleExists(string roleName)
        {
            var roleNames = RoleMappingPanel?.GetRoleNames();
            if (roleNames == null || roleNames.Count == 0)
            {
                return false;
            }

            return roleNames.Any(name => name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 检查源名称是否存在于TTL引擎的朗读者列表中。
        /// </summary>
        /// <param name="sourceName">源名称。</param>
        /// <returns>源名称是否存在。</returns>
        private bool isSourceNameExists(string sourceName)
        {
            var currentEngine = TtlSchemeController?.CurrentEngineConnector;
            if (currentEngine == null || currentEngine.Speakers == null || string.IsNullOrWhiteSpace(sourceName))
            {
                return false;
            }

            return currentEngine.Speakers.Any(s => s.SourceName.Equals(sourceName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 根据角色名获取源名称。
        /// </summary>
        /// <param name="roleName">角色名称。</param>
        /// <returns>源名称，如果找不到则返回空字符串。</returns>
        private string getSourceNameByRoleName(string roleName)
        {
            var roleMappingItems = RoleMappingPanel?.RoleMappingItems;
            if (roleMappingItems == null || string.IsNullOrWhiteSpace(roleName))
            {
                return string.Empty;
            }

            var item = roleMappingItems.FirstOrDefault(i => i.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            return item?.SourceName ?? string.Empty;
        }

        /// <summary>
        /// 统计字符串中的换行标识符的数量。
        /// </summary>
        /// <param name="input">待统计的字符串。</param>
        /// <returns>换行标识符的数量。</returns>
        private int countNewVoice(string input)
        {
            int outValue = 0;

            if (string.IsNullOrWhiteSpace(input) == false)
            {
                string[] parts = input.Split(new[] { 生成段落_分隔符 }, StringSplitOptions.RemoveEmptyEntries);
                outValue = parts.Length;
            }

            return outValue;
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 事件处理：最终文本内容改变。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void tb_语音生成预处理_最终文本_TextChanged(object sender, EventArgs e)
        {
            refresh语音生成预处理Ui();
            OnProjectModified();
        }

        /// <summary>
        /// 事件处理：点击"插入段落分隔符"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_语音生成预处理_插入段落分隔符_Click(object sender, EventArgs e)
        {
            OnInsertSeparatorRequested(this.tb_语音生成预处理_最终文本, 生成段落_分隔符);
        }

        /// <summary>
        /// 事件处理：点击"清理文本"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_语音生成预处理_清理文本_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("清理后无法恢复，确定清理最终文本吗？", "确定清理吗？", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                clear语音生成预处理Ui();
            }
        }

        /// <summary>
        /// 事件处理：点击"发送到语音生成"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_语音生成预处理_发送到语音生成_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                if (string.IsNullOrWhiteSpace(this.tb_语音生成预处理_最终文本.Text) == false)
                {
                    if (this.cb_语音生成预处理_默认角色设置.SelectedItem is SpeakerInfo defaultSpeaker)
                    {
                        string defaultRole = defaultSpeaker.SourceName;

                        saveFileDialog.Title = "选择保存文件";
                        saveFileDialog.Filter = "wav文件|*.wav|mp3文件|*.mp3";
                        saveFileDialog.CheckFileExists = false;
                        saveFileDialog.CheckPathExists = false;
                        saveFileDialog.ValidateNames = false;
                        saveFileDialog.FileName = FileHelper.PadNumber(1, countNewVoice(this.tb_语音生成预处理_最终文本.Text).ToString().Length);
                        string lastFolder = Setting.GetValue(this.bt_语音生成预处理_发送到语音生成.Name + "_保存文件夹", string.Empty);
                        if (!string.IsNullOrWhiteSpace(lastFolder))
                        {
                            saveFileDialog.InitialDirectory = lastFolder;
                        }

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string saveFile = saveFileDialog.FileName;
                            Setting.SetValue(this.bt_语音生成预处理_发送到语音生成.Name + "_保存文件夹", Path.GetDirectoryName(saveFile));

                            string[] paragraphArray = this.tb_语音生成预处理_最终文本.Text.Split(new string[] { 生成段落_分隔符 }, StringSplitOptions.RemoveEmptyEntries);
                            string sendProcessedText = string.Empty;

                            foreach (var section in paragraphArray)
                            {
                                string originalText = section;
                                string[] lines = originalText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                                string processedText = string.Empty;

                                foreach (string line in lines)
                                {
                                    string currentRole = string.Empty;
                                    string currentText = string.Empty;
                                    var featureParts = new List<string>();

                                    string[] parts = line.Split(new string[] { 角色_文本分隔符 }, StringSplitOptions.None);
                                    if (parts.Length >= 2)
                                    {
                                        string role = parts[0].Trim();
                                        if (string.IsNullOrWhiteSpace(role) || role == 默认_角色标识)
                                        {
                                            currentRole = defaultRole;
                                        }
                                        else if (isRoleExists(role))
                                        {
                                            currentRole = getSourceNameByRoleName(role);
                                        }
                                        else if (isSourceNameExists(role))
                                        {
                                            currentRole = role;
                                        }
                                        else
                                        {
                                            currentRole = defaultRole;
                                        }

                                        // 解析中间的特性键值对和文本
                                        for (int i = 1; i < parts.Length; i++)
                                        {
                                            string part = parts[i];
                                            // 检查是否是特性键值对
                                            bool isFeature = false;
                                            if (part.Contains("="))
                                            {
                                                var kv = part.Split(new char[] { '=' }, 2);
                                                if (kv.Length == 2)
                                                {
                                                    string fname = kv[0];
                                                    string[] knownFeatures = { "方言", "情感风格", "场景" };
                                                    if (knownFeatures.Contains(fname) && int.TryParse(kv[1], out _))
                                                    {
                                                        isFeature = true;
                                                        featureParts.Add(part);
                                                    }
                                                }
                                            }

                                            if (!isFeature)
                                            {
                                                // 不是特性键值对，作为文本内容
                                                currentText = string.IsNullOrEmpty(currentText) ? part : currentText + 角色_文本分隔符 + part;
                                            }
                                        }

                                        // 如果没有解析到文本，取最后一个非特性部分
                                        if (string.IsNullOrEmpty(currentText) && parts.Length > 1)
                                        {
                                            currentText = parts[parts.Length - 1];
                                        }
                                    }
                                    else if (parts.Length >= 1)
                                    {
                                        currentRole = defaultRole;
                                        currentText = parts[0].Trim();
                                    }

                                    processedText += processedText == string.Empty ? string.Empty : Environment.NewLine;
                                    if (string.IsNullOrWhiteSpace(currentText) == true)
                                    {
                                        processedText += string.Empty;
                                    }
                                    else
                                    {
                                        if (string.IsNullOrWhiteSpace(currentRole) == true)
                                        {
                                            currentRole = defaultRole;
                                        }
                                        // 重新构建行：角色::特性1=值1::特性2=值2::文本
                                        var allParts = new List<string>();
                                        allParts.Add(currentRole);
                                        allParts.AddRange(featureParts);
                                        allParts.Add(currentText.Trim());
                                        processedText += string.Join(角色_文本分隔符, allParts);
                                    }
                                }

                                sendProcessedText += sendProcessedText == string.Empty ? string.Empty : 生成段落_分隔符;
                                sendProcessedText += processedText;
                            }

                            // 完成最终的文本
                            string textToSend =
                                $"{角色_文本分隔符}{saveFile}" +
                                $"{角色_文本分隔符}{this.nud_语音生成预处理_语速设置.Value}" +
                                $"{角色_文本分隔符}{this.nud_语音生成预处理_空白时长.Value}" +
                                $"{Environment.NewLine}" +
                                $"{sendProcessedText}";
                            OnSwitchToNextPageRequested(textToSend);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 事件处理：语速设置被改变。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void nud_语音生成预处理_语速设置_ValueChanged(object sender, EventArgs e)
        {
            Setting.SetValue(this.nud_语音生成预处理_语速设置.Name, this.nud_语音生成预处理_语速设置.Value.ToString());
        }

        /// <summary>
        /// 事件处理：更改空白时长值。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void nud_语音生成预处理_空白时长_ValueChanged(object sender, EventArgs e)
        {
            Setting.SetValue(this.nud_语音生成预处理_空白时长.Name, this.nud_语音生成预处理_空白时长.Value.ToString());
        }

        /// <summary>
        /// 事件处理：默认角色设置被改变。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void cb_语音生成预处理_默认角色设置_SelectedIndexChanged(object sender, EventArgs e)
        {
            var currentEngine = TtlSchemeController?.CurrentEngineConnector;
            if (currentEngine != null && this.cb_语音生成预处理_默认角色设置.SelectedItem is SpeakerInfo speaker)
            {
                string key = $"{this.cb_语音生成预处理_默认角色设置.Name}_{currentEngine.Id}";
                Setting.SetValue(key, speaker.SourceName);
            }
        }

        #endregion

        #endregion
    }

    #region 事件参数类

    /// <summary>
    /// 插入分隔符事件参数。
    /// </summary>
    public class InsertSeparatorEventArgs : EventArgs
    {
        /// <summary>
        /// 获取文本框控件。
        /// </summary>
        public TextBox TextBox { get; }

        /// <summary>
        /// 获取分隔符。
        /// </summary>
        public string Separator { get; }

        /// <summary>
        /// 初始化InsertSeparatorEventArgs类的新实例。
        /// </summary>
        /// <param name="textBox">文本框控件。</param>
        /// <param name="separator">分隔符。</param>
        public InsertSeparatorEventArgs(TextBox textBox, string separator)
        {
            TextBox = textBox;
            Separator = separator;
        }
    }

    #endregion
}
