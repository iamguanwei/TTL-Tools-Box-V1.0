using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GW.TTLtoolsBox.Core.FileAccesser;
using GW.TTLtoolsBox.Core.TextSplit;
using GW.TTLtoolsBox.WinFormUi.Base;
using GW.TTLtoolsBox.WinFormUi.Helper;

namespace GW.TTLtoolsBox.WinFormUi.UI.Panels
{
    /// <summary>
    /// 文本拆分面板，提供文本拆分功能。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 按句子或对话拆分文本
    /// - 支持设置拆分长度
    /// - 支持插入段落分隔符
    /// 
    /// 使用场景：
    /// - 将长文本拆分为适合语音生成的段落
    /// - 作为工作流的第一步处理原始文本
    /// 
    /// 依赖关系：
    /// - 依赖Setting类保存用户配置
    /// - 依赖TextSplitHelper进行文本拆分
    /// - 依赖ProjectFile保存/加载项目数据
    /// </remarks>
    public partial class TextSplitPanel : ViewBase
    {
        #region 常量

        /// <summary>
        /// 整句分隔符号。
        /// </summary>
        private const string 整句_分割符号 = Constants.整句_分割符号;

        /// <summary>
        /// 半句分隔符号。
        /// </summary>
        private const string 半句_分割符号 = Constants.半句_分割符号;

        /// <summary>
        /// 拆分文本时最小的文本长度。
        /// </summary>
        private const uint 分割_最小长度 = Constants.分割_最小长度;

        /// <summary>
        /// 对话分隔符号。
        /// </summary>
        private const string 对话_分割符号 = Constants.对话_分割符号;

        /// <summary>
        /// 生成段落分隔符。
        /// </summary>
        private const string 生成段落_分隔符 = Constants.生成段落_分隔符;

        #endregion

        #region public

        #region 事件

        /// <summary>
        /// 项目内容被修改时触发的事件。
        /// </summary>
        public event EventHandler ProjectModified;

        /// <summary>
        /// 请求切换到下一个选项卡时触发的事件。
        /// </summary>
        public event EventHandler<SwitchToNextPageEventArgs> SwitchToNextPageRequested;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化TextSplitPanel类的新实例。
        /// </summary>
        public TextSplitPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 初始化面板。
        /// </summary>
        public void InitializePanel()
        {
            init文本拆分Ui();
        }

        /// <summary>
        /// 刷新面板UI状态。
        /// </summary>
        public override void RefreshUi()
        {
            refresh文本拆分Ui();
        }

        /// <summary>
        /// 清理面板内容。
        /// </summary>
        public void ClearPanel()
        {
            clear文本拆分Ui();
        }

        /// <summary>
        /// 保存面板数据到项目文件。
        /// </summary>
        /// <param name="projectFile">项目文件实例。</param>
        public void SaveData(ProjectFile projectFile)
        {
            if (projectFile != null)
            {
                projectFile.TextSplit_OriginalText = this.tb_原始文本.Text;
                projectFile.TextSplit_FinalText = this.tb_最终文本.Text;
            }
        }

        /// <summary>
        /// 从项目文件加载面板数据。
        /// </summary>
        /// <param name="projectFile">项目文件实例。</param>
        public void LoadData(ProjectFile projectFile)
        {
            if (projectFile != null)
            {
                UpdateUi(() =>
                {
                    this.tb_原始文本.Text = projectFile.TextSplit_OriginalText;
                    this.tb_最终文本.Text = projectFile.TextSplit_FinalText;
                });
            }
        }

        #endregion

        #endregion

        #region private

        #region 字段

        /// <summary>
        /// 最后一次查找到的最终文本中的最长文本段落。
        /// </summary>
        private string _lastMaxFinishedTextSubstring = string.Empty;

        #endregion

        #region UI初始化

        /// <summary>
        /// 初始化文本拆分UI。
        /// </summary>
        private void init文本拆分Ui()
        {
            this.tb_原始文本.MaxLength = int.MaxValue;
            this.tb_最终文本.MaxLength = int.MaxValue;

            this.rb_拆分方式_按句子拆分.Text += $" [ 整句切分 {整句_分割符号}  半句切分 {半句_分割符号} ]";
            this.rb_拆分方式_按句子拆分.Checked = bool.Parse(Setting.GetValue(this.rb_拆分方式_按句子拆分.Name, "true"));
            this.nud_拆分长度.Value = decimal.Parse(Setting.GetValue(this.nud_拆分长度.Name, $"{分割_最小长度}"));

            this.rb_拆分参数_按对话拆分.Text += $" [ {对话_分割符号} ]";
            this.rb_拆分参数_按对话拆分.Checked = bool.Parse(Setting.GetValue(this.rb_拆分参数_按对话拆分.Name, "false"));

            this.nud_拆分长度.Minimum = 分割_最小长度;
            this.nud_拆分长度.Maximum = 100000;

            this.rb_拆分方式_按句子拆分.CheckedChanged += rb_拆分方式_CheckedChanged;
            this.rb_拆分参数_按对话拆分.CheckedChanged += rb_拆分方式_CheckedChanged;

            refresh文本拆分Ui();
        }

        #endregion

        #region UI操作

        /// <summary>
        /// 刷新文本拆分UI。
        /// </summary>
        private void refresh文本拆分Ui()
        {
            Action action = () =>
            {
                string head = "字数：";

                // 原始文本
                string inputText = this.tb_原始文本.Text.Replace("\r\n", string.Empty);

                this.lab_原始文本字数.Text =
                    $"{head}" +
                    $"{inputText.Length}";

                // 最终文本
                string outputText = this.tb_最终文本.Text.Replace("\r\n", "\n");
                string[] strArray = outputText.Split('\n');
                outputText = String.Join(string.Empty, strArray); // 彻底排除换行符

                int maxLength = strArray.Length == 0 ? 0 : strArray.Max(s => s.Replace(" ", string.Empty).Length);

                var maxStr = maxLength == 0 ? "-" : strArray.First(s => s.Replace(" ", string.Empty).Length == maxLength);
                _lastMaxFinishedTextSubstring = maxLength == 0 ? string.Empty : maxStr;

                if (maxLength > 30) maxStr = $"{maxStr.Substring(0, 30)}...";

                this.lab_最终文本字数.Text =
                    $"{head}" +
                    $"{outputText.Length}" +
                    $"，最长段落字数：{maxLength}（{maxStr}）";

                this.bt_发送最终文本到下一个步骤.Enabled = this.bt_复制最终文本.Enabled = outputText.Length > 0;
                this.bt_在最终文本中标示.Enabled = _lastMaxFinishedTextSubstring != string.Empty;

                // 参数
                this.lab_拆分长度.Enabled = this.rb_拆分方式_按句子拆分.Checked;
                this.nud_拆分长度.Enabled = this.rb_拆分方式_按句子拆分.Checked;

                this.bt_开始拆分.Enabled = inputText.Length > 0;
                this.bt_插入段落分隔符.Enabled = inputText.Length > 0;
                this.bt_清理所有文本.Enabled = outputText.Length > 0 || inputText.Length > 0;
            };

            UpdateUi(action);
        }

        /// <summary>
        /// 清理文本拆分UI。
        /// </summary>
        private void clear文本拆分Ui()
        {
            Action action = () =>
            {
                this.tb_原始文本.Text = string.Empty;
                this.tb_最终文本.Text = string.Empty;
            };

            UpdateUi(action);

            refresh文本拆分Ui();
        }

        #endregion

        #region 业务操作

        /// <summary>
        /// 执行文本拆分操作。
        /// </summary>
        private void do拆分文本Job()
        {
            string sText = this.tb_原始文本.Text;
            if (string.IsNullOrWhiteSpace(sText) == false)
            {
                string dText;

                // 先用段落分隔符拆分整个文本
                string[] paragraphs = sText.Split(new string[] { 生成段落_分隔符 }, StringSplitOptions.None);
                List<string> processedParagraphs = new List<string>();

                // 对每一段文本采用之前的逻辑处理
                foreach (string paragraph in paragraphs)
                {
                    string processedText = paragraph;

                    if (this.rb_拆分方式_按句子拆分.Checked == true) // 按长度拆分
                        processedText = TextSplitHelper.SplitText(
                            paragraph,
                            (int)分割_最小长度,
                            (int)this.nud_拆分长度.Value,
                            整句_分割符号.ToCharArray(),
                            半句_分割符号.ToCharArray());
                    else if (this.rb_拆分参数_按对话拆分.Checked == true) // 按对话拆分
                        processedText = TextSplitHelper.SplitText(paragraph, 对话_分割符号.ToCharArray());

                    processedParagraphs.Add(processedText);
                }

                // 把每一段文本重新拼接起来，中间用"回车换行+段落分隔符"连接
                dText = string.Join(生成段落_分隔符, processedParagraphs);

                // 写入最终结果
                this.tb_最终文本.Text = dText;
            }
        }

        /// <summary>
        /// 触发项目修改事件。
        /// </summary>
        private void onProjectModified()
        {
            ProjectModified?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 触发切换到下一个选项卡事件。
        /// </summary>
        /// <param name="text">要传递的文本。</param>
        private void onSwitchToNextPage(string text)
        {
            SwitchToNextPageRequested?.Invoke(this, new global::GW.TTLtoolsBox.WinFormUi.Helper.SwitchToNextPageEventArgs(text));
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 事件处理：原始文本内容改变。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void tb_原始文本_TextChanged(object sender, EventArgs e)
        {
            refresh文本拆分Ui();
            onProjectModified();
        }

        /// <summary>
        /// 事件处理：最终文本内容改变。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void tb_最终文本_TextChanged(object sender, EventArgs e)
        {
            refresh文本拆分Ui();
            onProjectModified();
        }

        /// <summary>
        /// 事件处理：点击"清理所有文本"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_清理所有文本_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("清理后无法恢复，确定清理所有输入输出文本吗？", "确定清理吗？", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                clear文本拆分Ui();
            }
        }

        /// <summary>
        /// 事件处理：点击"复制最终文本"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_复制最终文本_Click(object sender, EventArgs e)
        {
            UiControlHelper.CopyToClipboard(this.tb_最终文本.Text);
        }

        /// <summary>
        /// 事件处理：点击"在最终文本中标示"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_在最终文本中标示_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_lastMaxFinishedTextSubstring) == false)
            {
                int index = this.tb_最终文本.Text.IndexOf(_lastMaxFinishedTextSubstring);
                this.tb_最终文本.SelectionStart = index;
                this.tb_最终文本.SelectionLength = _lastMaxFinishedTextSubstring.Length;
                this.tb_最终文本.ScrollToCaret();
                this.tb_最终文本.Focus();
            }
        }

        /// <summary>
        /// 事件处理：点击"开始拆分"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_开始拆分_Click(object sender, EventArgs e)
        {
            Setting.SetValue(this.rb_拆分方式_按句子拆分.Name, this.rb_拆分方式_按句子拆分.Checked.ToString());
            Setting.SetValue(this.nud_拆分长度.Name, this.nud_拆分长度.Value.ToString());

            Setting.SetValue(this.rb_拆分参数_按对话拆分.Name, this.rb_拆分参数_按对话拆分.Checked.ToString());

            do拆分文本Job();
            refresh文本拆分Ui();
        }

        /// <summary>
        /// 事件处理：点击"插入段落分隔符"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_插入段落分隔符_Click(object sender, EventArgs e)
        {
            UiControlHelper.InsertSeparatorAtCursor(this.tb_原始文本, 生成段落_分隔符);
        }

        /// <summary>
        /// 事件处理：拆分参数复选框值被改变。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void cb_拆分文本参数_CheckedChanged(object sender, EventArgs e)
        {
            refresh文本拆分Ui();
        }

        /// <summary>
        /// 事件处理：点击"发送最终文本到下一个步骤"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_发送最终文本到下一个步骤_Click(object sender, EventArgs e)
        {
            onSwitchToNextPage(this.tb_最终文本.Text);
        }

        /// <summary>
        /// 事件处理：拆分参数改选。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void rb_拆分方式_CheckedChanged(object sender, EventArgs e)
        {
            refresh文本拆分Ui();
        }

        #endregion

        #endregion
    }
}
