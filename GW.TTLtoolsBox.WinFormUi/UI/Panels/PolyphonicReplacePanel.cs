using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using GW.TTLtoolsBox.Core.PolyReplace;
using GW.TTLtoolsBox.Core.FileAccesser;
using GW.TTLtoolsBox.WinFormUi.Base;
using GW.TTLtoolsBox.WinFormUi.Helper;
using GW.TTLtoolsBox.WinFormUi.Manager;

namespace GW.TTLtoolsBox.WinFormUi.UI.Panels
{
    /// <summary>
    /// 多音字替换面板，提供多音字替换功能。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 管理多音字替换方案
    /// - 执行多音字替换操作
    /// - 支持多种替换方案选择
    /// 
    /// 使用场景：
    /// - 在文本中查找并替换多音字
    /// - 作为工作流的第二步处理文本拆分后的文本
    /// 
    /// 依赖关系：
    /// - 依赖Setting类保存用户配置
    /// - 依赖PolyphonicItem和ReplaceItem进行多音字处理
    /// - 依赖ProjectFile保存/加载项目数据
    /// - 依赖TtlSchemeController获取当前引擎信息
    /// </remarks>
    public partial class PolyphonicReplacePanel : ViewBase
    {
        #region 常量

        /// <summary>
        /// 存盘用的分隔符。
        /// </summary>
        private const string 存盘_分隔符 = Constants.存盘_分隔符;

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
        /// 初始化PolyphonicReplacePanel类的新实例。
        /// </summary>
        public PolyphonicReplacePanel()
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

        #endregion

        #region 方法

        /// <summary>
        /// 获取多音字方案表是否有未保存的更改。
        /// </summary>
        public bool IsSchemeTableChanged => _dgv多音字方案表Changed;

        /// <summary>
        /// 初始化面板。
        /// </summary>
        public void InitializePanel()
        {
            init多音字替换Ui();
        }

        /// <summary>
        /// 刷新面板UI状态。
        /// </summary>
        public override void RefreshUi()
        {
            refresh多音字替换Ui();
        }

        /// <summary>
        /// 重置更改标志（仅在第一次切换选项卡时调用）。
        /// </summary>
        public void ResetChangedFlag()
        {
            if (_isFirstSwitchToTab == true)
            {
                _isFirstSwitchToTab = false;
                _dgv多音字方案表Changed = false;
                refresh多音字替换Ui();
            }
        }   

        /// <summary>
        /// 清理面板内容。
        /// </summary>
        public void ClearPanel()
        {
            clear多音字替换Ui();
        }

        /// <summary>
        /// 保存面板数据到项目文件。
        /// </summary>
        public void SaveData()
        {
            save多音字替换Data();
        }

        /// <summary>
        /// 从项目文件加载面板数据。
        /// </summary>
        public void LoadData()
        {
            load多音字替换();
        }

        /// <summary>
        /// 设置最终文本内容。
        /// </summary>
        /// <param name="text">要设置的文本。</param>
        public void SetFinalText(string text)
        {
            UpdateUi(() =>
            {
                if (this.tb_最终文本.Enabled)
                {
                    this.tb_最终文本.Text = text;
                }
            });
        }

        /// <summary>
        /// 初始化多音字方案数据源。
        /// </summary>
        public void InitializeSchemeDataSource()
        {
            if (this.dgv_多音字方案.DataSource == null)
            {
                this.dgv_多音字方案.DataSource = PolyphonicItem.ConvertToDataTable(_savedPolyphonicItemArray);
                _dgv多音字方案表Changed = false;
            }
        }

        #endregion

        #endregion

        #region private

        #region 字段

        /// <summary>
        /// 多音字方案表是否被编辑。
        /// </summary>
        private bool _dgv多音字方案表Changed
        {
            get => _isFirstSwitchToTab == false && __dgv多音字方案表Changed;
            set => __dgv多音字方案表Changed = value;
        }
        private bool __dgv多音字方案表Changed = false;

        /// <summary>
        /// 多音字替换操作是否正在进行中。
        /// </summary>
        private bool _do多音字替换Working = false;

        /// <summary>
        /// 是否是首次切换到选项卡了（用于控制是否需要重置更改标志）。
        /// </summary>
        private bool _isFirstSwitchToTab = true;

        /// <summary>
        /// 已保存的多音字替换项清单。
        /// </summary>
        private PolyphonicItem[] _savedPolyphonicItemArray = Array.Empty<PolyphonicItem>();

        /// <summary>
        /// 记录多音字在目标文本中的位置的清单。
        /// </summary>
        private ReplaceItem[] _working多音字Array = new ReplaceItem[0];

        /// <summary>
        /// 当前正在操作的多音字索引。
        /// </summary>
        private int _work多音字Index = -1;

        #endregion

        #region UI初始化

        /// <summary>
        /// 初始化多音字替换UI。
        /// </summary>
        private void init多音字替换Ui()
        {
            this.tb_最终文本.MaxLength = int.MaxValue;

            this.dgv_多音字方案.EditMode = DataGridViewEditMode.EditOnEnter;

            this.dgv_多音字方案.MouseDown -= dgv_多音字方案_MouseDown;
            this.dgv_多音字方案.MouseDown += dgv_多音字方案_MouseDown;

            // 加载数据
            {
                // 多音字方案表
                {
                    var itemText = Setting.GetValue(this.dgv_多音字方案.Name, string.Empty);
                    List<PolyphonicItem> itemList = new List<PolyphonicItem>();
                    foreach (var itemStr in itemText.Split(new string[] { 存盘_分隔符 }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (string.IsNullOrWhiteSpace(itemStr) == false)
                        {
                            PolyphonicItem pi = new PolyphonicItem();
                            pi.LoadFromString(itemStr);

                            itemList.Add(pi);
                        }
                    }
                    _savedPolyphonicItemArray = itemList.ToArray();

                    // 设置表格数据源
                    this.dgv_多音字方案.DataSource = PolyphonicItem.ConvertToDataTable(_savedPolyphonicItemArray);
                }
            }

            // 可选多音字方案列表
            {
                var selectedItem = Setting.GetValue(this.cb_选择多音字方案.Name, string.Empty);
                foreach (var ps in (new PolyphonicItem().PolyphonicSchemes))
                {
                    this.cb_选择多音字方案.Items.Add(ps.ShowName);
                }

                if (this.cb_选择多音字方案.Items.Count > 0)
                {
                    if (this.cb_选择多音字方案.Items.Contains(selectedItem) == true) this.cb_选择多音字方案.SelectedItem = selectedItem;
                    else this.cb_选择多音字方案.SelectedIndex = 0;
                }
                else this.cb_选择多音字方案.Enabled = false;
            }

            refresh多音字替换Ui();
        }

        #endregion

        #region UI操作

        /// <summary>
        /// 刷新多音字替换UI。
        /// </summary>
        private void refresh多音字替换Ui()
        {
            Action action = () =>
            {
                string outputText = this.tb_最终文本.Text;

                // 最终文本
                {
                    // 保存文本框的选择状态
                    int selectionStart = this.tb_最终文本.SelectionStart;
                    int selectionLength = this.tb_最终文本.SelectionLength;
                    bool isFocused = this.tb_最终文本.Focused;

                    this.tb_最终文本.Enabled = !_do多音字替换Working;
                    this.bt_复制最终文本.Enabled = this.bt_发送最终文本到下一个步骤.Enabled = this.tb_最终文本.Enabled && outputText.Length > 0;
                    this.bt_清理最终文本.Enabled = this.tb_最终文本.Enabled && outputText.Length > 0;

                    // 恢复文本框的选择状态
                    if (isFocused)
                    {
                        this.tb_最终文本.Focus();
                        this.tb_最终文本.SelectionStart = selectionStart;
                        this.tb_最终文本.SelectionLength = selectionLength;
                    }
                    else
                    {
                        this.tb_最终文本.SelectionStart = selectionStart;
                        this.tb_最终文本.SelectionLength = selectionLength;
                    }
                }

                // 多音字方案
                {
                    this.dgv_多音字方案.Enabled = !_do多音字替换Working;
                    this.bt_保存多音字方案.Enabled = this.bt_还原多音字方案.Enabled = this.dgv_多音字方案.Enabled && _dgv多音字方案表Changed;
                }

                // 工作区域
                {
                    var currentEngine = TtlSchemeController?.CurrentEngineConnector;
                    if (currentEngine != null) this.cb_选择多音字方案.SelectedItem = currentEngine.PolyphonicScheme.ShowName;
                    this.cb_选择多音字方案.Enabled = !_do多音字替换Working && this.cb_选择多音字方案.Items.Count > 0 && currentEngine == null;
                    this.bt_开始替换最终文本中的多音字.Enabled =
                        !_do多音字替换Working &&
                        this.cb_选择多音字方案.Items.Count > 0 &&
                        this.cb_选择多音字方案.SelectedItem != null &&
                        outputText.Length > 0 &&
                        _savedPolyphonicItemArray.Length > 0;

                    this.pan_实施工作面板.Visible = outputText.Length > 0 && _do多音字替换Working && _working多音字Array.Length > 0 && _work多音字Index >= 0;
                    if (this.pan_实施工作面板.Visible)
                    {
                        _work多音字Index = _work多音字Index >= _working多音字Array.Length ? _working多音字Array.Length - 1 : _work多音字Index;

                        this.lab_上文.Text = _working多音字Array[_work多音字Index].PreContextText;
                        this.lab_替换目标.Text = _working多音字Array[_work多音字Index].TargetText;
                        this.lab_下文.Text = _working多音字Array[_work多音字Index].PostContextText;

                        this.bt_上一条替换目标.Enabled = _work多音字Index > 0;
                        this.bt_下一条替换目标.Enabled = _work多音字Index < _working多音字Array.Length - 1;
                        this.bt_下一组替换目标.Enabled = _working多音字Array.Last().UsedPolyphonicItem != _working多音字Array[_work多音字Index].UsedPolyphonicItem;
                        this.bt_应用替换结果.Enabled = this.bt_取消应用替换结果.Enabled = true;

                        // 工作统计信息
                        {
                            this.lab_替换工作信息.Text = BuildWorkStatisticsText();
                        }

                        // 选项按钮
                        {
                            ClearAlternativePanelControls();

                            // 获取当前替换项的备选文本
                            var alternativeTexts = _working多音字Array[_work多音字Index].AlternativeTexts;

                            // 复制模板控件并添加到面板
                            int yOffset = this.rb_待选词模板.Top;
                            int height = this.rb_待选词模板.Height + 5;

                            // 获取当前ReplaceItem
                            var currentReplaceItem = _working多音字Array[_work多音字Index];
                            var previousReplaceItem = _work多音字Index > 0 ? _working多音字Array[_work多音字Index - 1] : null;

                            // 确定选中索引
                            int selectedIndex = CalculateSelectedIndex(currentReplaceItem, alternativeTexts, previousReplaceItem);

                            for (int i = 0; i < alternativeTexts.Length; i++)
                            {
                                RadioButton rb = CreateAlternativeRadioButton(alternativeTexts, i, selectedIndex, yOffset, height);
                                this.pan_待选词.Controls.Add(rb);
                            }

                            // 确保模板控件隐藏
                            this.rb_待选词模板.Visible = false;
                        }
                    }
                }
            };

            UpdateUi(action);
        }

        /// <summary>
        /// 清理多音字替换UI。
        /// </summary>
        private void clear多音字替换Ui()
        {
            Action action = () =>
            {
                this.tb_最终文本.Text = string.Empty;
            };

            UpdateUi(action);

            refresh多音字替换Ui();
        }

        #endregion

        #region 业务操作

        /// <summary>
        /// 填充工作目标。
        /// </summary>
        private void fill多音字WorkingArray()
        {
            _working多音字Array = new ReplaceItem[0];

            string targetText = this.tb_最终文本.Text;
            if (string.IsNullOrWhiteSpace(targetText) == false)
            {
                var pi = new PolyphonicItem();
                var psIndex = this.cb_选择多音字方案.SelectedIndex;
                if (psIndex >= 0 && psIndex < pi.PolyphonicSchemes.Length)
                {
                    var psName = pi.PolyphonicSchemes[psIndex].Name;
                    _working多音字Array = ReplaceItem.GenerateFromText(targetText, _savedPolyphonicItemArray, psName).OrderByDescending(r => r.OriginalText.Length).ToArray();
                }
            }
        }

        /// <summary>
        /// 将用户选中的文本保存到多音字替换项。
        /// </summary>
        /// <param name="replaceItem">要保存的多音字替换项。</param>
        private void save多音字Working(ReplaceItem replaceItem)
        {
            RadioButton selectedRadioButton = this.pan_待选词.Controls
                .OfType<RadioButton>()
                .FirstOrDefault(rb => rb.Checked && rb != this.rb_待选词模板);

            if (selectedRadioButton != null)
            {
                string selectedText = selectedRadioButton.Text;
                replaceItem.UpdateReplaceText(selectedText);
            }
        }

        /// <summary>
        /// 保存多音字替换工作内容到项目文件对象。
        /// </summary>
        private void save多音字替换Data()
        {
            if (!string.IsNullOrEmpty(CurrentEngineId) && ProjectFile != null)
            {
                ProjectFile.SetPolyReplace_FinalText(CurrentEngineId, this.tb_最终文本.Text);
            }
        }

        /// <summary>
        /// 加载多音字替换工作内容。
        /// </summary>
        private void load多音字替换()
        {
            Action act = () =>
            {
                if (!string.IsNullOrEmpty(CurrentEngineId) && ProjectFile != null)
                {
                    this.tb_最终文本.Text = ProjectFile.GetPolyReplace_FinalText(CurrentEngineId);
                }
                else
                {
                    this.tb_最终文本.Text = string.Empty;
                }
            };

            UpdateUi(act);
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
            SwitchToNextPageRequested?.Invoke(this, new SwitchToNextPageEventArgs(text));
        }

        /// <summary>
        /// 获取DataGridView中选中的行索引集合。
        /// </summary>
        /// <returns>选中的行索引集合。</returns>
        private HashSet<int> GetSelectedRowIndexes()
        {
            HashSet<int> selectedRowIndexes = new HashSet<int>();
            foreach (DataGridViewCell cell in dgv_多音字方案.SelectedCells)
            {
                selectedRowIndexes.Add(cell.RowIndex);
            }
            foreach (DataGridViewRow row in dgv_多音字方案.SelectedRows)
            {
                selectedRowIndexes.Add(row.Index);
            }
            return selectedRowIndexes;
        }

        /// <summary>
        /// 检查选中行是否存在非空的首列单元格。
        /// </summary>
        /// <param name="rowIndexes">行索引集合。</param>
        /// <returns>如果存在非空首列返回true，否则返回false。</returns>
        private bool HasNonEmptyFirstColumn(HashSet<int> rowIndexes)
        {
            foreach (int idx in rowIndexes)
            {
                if (idx >= 0 && idx < dgv_多音字方案.Rows.Count)
                {
                    DataGridViewCell firstCell = dgv_多音字方案.Rows[idx].Cells[0];
                    string originalText = firstCell.Value?.ToString() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(originalText))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 构建工作统计信息文本。
        /// </summary>
        /// <returns>统计信息文本。</returns>
        private string BuildWorkStatisticsText()
        {
            var subArray = _working多音字Array.Where(a => a.UsedPolyphonicItem.Equals(_working多音字Array[_work多音字Index].UsedPolyphonicItem) == true).ToArray();
            return string.Format(
                "共有 {0} 个多音词需要替换，正在替换第 {1} 个，剩余 {2} 个。本组共有 {3} 个，剩余 {4} 个。",
                $"{_working多音字Array.Length}",
                $"{_work多音字Index + 1}",
                $"{_working多音字Array.Length - (_work多音字Index + 1)}",
                $"{subArray.Length}",
                $"{subArray.Length - Array.IndexOf(subArray, _working多音字Array[_work多音字Index]) - 1}");
        }

        /// <summary>
        /// 清理待选词面板中的现有控件（保留模板）。
        /// </summary>
        private void ClearAlternativePanelControls()
        {
            var existingControls = this.pan_待选词.Controls.OfType<RadioButton>().Where(c => c != this.rb_待选词模板).ToList();
            foreach (var control in existingControls)
            {
                this.pan_待选词.Controls.Remove(control);
                control.Dispose();
            }
        }

        /// <summary>
        /// 计算当前备选文本的选中索引。
        /// </summary>
        /// <param name="currentReplaceItem">当前替换项。</param>
        /// <param name="alternativeTexts">备选文本数组。</param>
        /// <param name="previousReplaceItem">前一个替换项。</param>
        /// <returns>选中的索引。</returns>
        private int CalculateSelectedIndex(ReplaceItem currentReplaceItem, string[] alternativeTexts, ReplaceItem previousReplaceItem)
        {
            string currentReplaceText = currentReplaceItem.ReplaceText;
            bool hasUpdated = currentReplaceItem.IsReplaceTextUpdated;

            if (hasUpdated)
            {
                for (int i = 0; i < alternativeTexts.Length; i++)
                {
                    if ((i == 0 && currentReplaceText == currentReplaceItem.TargetText) ||
                        (i > 0 && alternativeTexts[i] == currentReplaceText))
                    {
                        return i;
                    }
                }
            }
            else if (previousReplaceItem != null)
            {
                bool isSamePolyphonicItem = currentReplaceItem.UsedPolyphonicItem != null &&
                                           previousReplaceItem.UsedPolyphonicItem != null &&
                                           currentReplaceItem.UsedPolyphonicItem.Equals(previousReplaceItem.UsedPolyphonicItem);

                if (isSamePolyphonicItem)
                {
                    string previousReplaceText = previousReplaceItem.ReplaceText;
                    for (int i = 0; i < alternativeTexts.Length; i++)
                    {
                        if ((i == 0 && previousReplaceText == previousReplaceItem.TargetText) ||
                            (i > 0 && alternativeTexts[i] == previousReplaceText))
                        {
                            return i;
                        }
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// 创建备选词的RadioButton控件。
        /// </summary>
        /// <param name="alternativeTexts">备选文本数组。</param>
        /// <param name="index">当前索引。</param>
        /// <param name="selectedIndex">选中的索引。</param>
        /// <param name="yOffset">Y轴偏移量。</param>
        /// <param name="height">控件高度。</param>
        /// <returns>创建的RadioButton控件。</returns>
        private RadioButton CreateAlternativeRadioButton(string[] alternativeTexts, int index, int selectedIndex, int yOffset, int height)
        {
            RadioButton rb = new RadioButton();
            rb.Text = alternativeTexts[index];
            rb.Location = new System.Drawing.Point(this.rb_待选词模板.Left, yOffset + index * height);
            rb.Size = this.rb_待选词模板.Size;
            rb.Font = this.rb_待选词模板.Font;
            rb.ForeColor = this.rb_待选词模板.ForeColor;
            rb.AutoSize = true;
            rb.Checked = index == selectedIndex;

            rb.Click += (sender, e) =>
            {
                if (sender is RadioButton radioButton && radioButton.Checked)
                {
                    int idx = Array.IndexOf(alternativeTexts, radioButton.Text);
                    if (idx > 0)
                    {
                        _working多音字Array[_work多音字Index].UpdateReplaceText(alternativeTexts[idx]);
                    }
                    else
                    {
                        _working多音字Array[_work多音字Index].UpdateReplaceText(null);
                    }
                }
            };

            return rb;
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 事件处理：点击"清理最终文本"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_清理最终文本_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("清理后无法恢复，确定清理最终文本吗？", "确定清理吗？", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                clear多音字替换Ui();
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
        /// 事件处理：点击"发送最终文本到下一个步骤"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_发送最终文本到下一个步骤_Click(object sender, EventArgs e)
        {
            onSwitchToNextPage(this.tb_最终文本.Text);
        }

        /// <summary>
        /// 事件处理：点击"替换最终文本中的多音字"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_替换最终文本中的多音字_Click(object sender, EventArgs e)
        {
            if (this.cb_选择多音字方案.SelectedItem != null)
            {
                // 多音字方案必须保存才允许替换操作
                if (_dgv多音字方案表Changed == false ||
                    MessageBox.Show("多音字方案没有保存！点击“确定”立即保存并开始替换操作，点击“取消”中止操作。", "保存多音字方案吗？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    // 保存相关配置
                    {
                        if (_dgv多音字方案表Changed == true) bt_保存多音字方案_Click(sender, e);
                        Setting.SetValue(this.cb_选择多音字方案.Name, this.cb_选择多音字方案.SelectedItem.ToString());
                    }

                    // 启动替换工作
                    {
                        fill多音字WorkingArray();
                        if (_working多音字Array.Length > 0)
                        {
                            _work多音字Index = 0;
                            _do多音字替换Working = true;

                            refresh多音字替换Ui();
                        }
                        else MessageBox.Show("没有任何需要替换的多音字。", "提示", MessageBoxButtons.OK);
                    }
                }

            }
        }

        /// <summary>
        /// 事件处理：点击"还原多音字方案"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_还原多音字方案_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("如果选择“确定”，那么未保存的内容将会丢失。确定要立即重新加载吗？", "重新加载吗？", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.dgv_多音字方案.DataSource = PolyphonicItem.ConvertToDataTable(_savedPolyphonicItemArray);

                _dgv多音字方案表Changed = false;
                refresh多音字替换Ui();
            }
        }

        /// <summary>
        /// 事件处理：点击"保存多音字方案"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_保存多音字方案_Click(object sender, EventArgs e)
        {
            _savedPolyphonicItemArray = PolyphonicItem.ConvertFromDataTable((DataTable)this.dgv_多音字方案.DataSource);
            Setting.SetValue(this.dgv_多音字方案.Name, string.Join(存盘_分隔符, _savedPolyphonicItemArray.Select(p => p.ToString())));

            _dgv多音字方案表Changed = false;
            refresh多音字替换Ui();
        }

        /// <summary>
        /// 事件处理：最终文本内容改变。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void tb_最终文本_TextChanged(object sender, EventArgs e)
        {
            refresh多音字替换Ui();
            onProjectModified();
        }

        /// <summary>
        /// 事件处理：多音字方案表鼠标按下，用于处理右键菜单。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_多音字方案_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo hitTest = this.dgv_多音字方案.HitTest(e.X, e.Y);
                if (hitTest.Type == DataGridViewHitTestType.RowHeader && hitTest.RowIndex >= 0)
                {
                    if (!dgv_多音字方案.Rows[hitTest.RowIndex].Selected)
                    {
                        dgv_多音字方案.ClearSelection();
                        dgv_多音字方案.Rows[hitTest.RowIndex].Selected = true;
                    }

                    showRowHeaderContextMenu(hitTest.RowIndex);
                }
            }
        }

        /// <summary>
        /// 显示行首上下文菜单。
        /// </summary>
        /// <param name="rowIndex">行索引。</param>
        private void showRowHeaderContextMenu(int rowIndex)
        {
            var selectedRowIndexes = GetSelectedRowIndexes();
            if (selectedRowIndexes.Count == 0) return;

            bool hasNonEmptyFirstColumn = HasNonEmptyFirstColumn(selectedRowIndexes);

            ContextMenuStrip cms = new ContextMenuStrip();

            ToolStripMenuItem autoGenerateMenuItem = new ToolStripMenuItem("自动生成");
            autoGenerateMenuItem.Enabled = hasNonEmptyFirstColumn;
            autoGenerateMenuItem.Click += (s, args) =>
            {
                autoGenerateForSelectedRows();
            };
            cms.Items.Add(autoGenerateMenuItem);

            ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem("删除");
            deleteMenuItem.Click += (s, args) =>
            {
                if (MessageBox.Show($"确定要删除选中的 {selectedRowIndexes.Count} 行吗？", "确认删除", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    deleteSelectedRows(selectedRowIndexes);
                }
            };
            cms.Items.Add(deleteMenuItem);

            cms.Show(dgv_多音字方案, dgv_多音字方案.GetCellDisplayRectangle(0, rowIndex, false).Location);
        }

        /// <summary>
        /// 对选中的行执行自动生成替换文本。
        /// </summary>
        private void autoGenerateForSelectedRows()
        {
            var selectedRowIndexes = GetSelectedRowIndexes();
            if (selectedRowIndexes.Count == 0) return;

            var pi = new PolyphonicItem();
            int schemeCount = pi.PolyphonicSchemes.Length;
            if (schemeCount == 0)
            {
                MessageBox.Show("没有可用的多音字方案。", "提示", MessageBoxButtons.OK);
                return;
            }

            foreach (int rowIdx in selectedRowIndexes)
            {
                if (rowIdx < 0 || rowIdx >= dgv_多音字方案.Rows.Count) continue;

                DataGridViewRow row = dgv_多音字方案.Rows[rowIdx];
                DataGridViewCell firstCell = row.Cells[0];
                string originalText = firstCell.Value?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(originalText)) continue;

                PolyphonicItem rowPi = new PolyphonicItem { OriginalText = originalText };

                for (int schemeIndex = 0; schemeIndex < schemeCount && (schemeIndex + 1) < row.Cells.Count; schemeIndex++)
                {
                    rowPi.PolyphonicSchemes[schemeIndex].BuildDefaultReplaceStrings(originalText);
                    string replaceText = string.Join("\r\n", rowPi.PolyphonicSchemes[schemeIndex].ReplaceStrings);
                    row.Cells[schemeIndex + 1].Value = replaceText;
                }
            }

            _dgv多音字方案表Changed = true;
            refresh多音字替换Ui();
        }

        /// <summary>
        /// 删除选中的行。
        /// </summary>
        /// <param name="selectedRowIndexes">选中的行索引集合。</param>
        private void deleteSelectedRows(HashSet<int> selectedRowIndexes)
        {
            if (selectedRowIndexes.Count == 0) return;

            List<int> rowIndexes = selectedRowIndexes.ToList();
            rowIndexes.Sort((a, b) => b.CompareTo(a));
            foreach (int index in rowIndexes)
            {
                if (index >= 0 && index < dgv_多音字方案.Rows.Count && !dgv_多音字方案.Rows[index].IsNewRow)
                {
                    dgv_多音字方案.Rows.RemoveAt(index);
                }
            }

            _dgv多音字方案表Changed = true;
            refresh多音字替换Ui();
        }

        /// <summary>
        /// 事件处理：多音字方案表结束编辑。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_多音字方案_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            _dgv多音字方案表Changed = true;
            refresh多音字替换Ui();
        }

        /// <summary>
        /// 事件处理：多音字方案表用户删除行。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_多音字方案_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            _dgv多音字方案表Changed = true;
            refresh多音字替换Ui();
        }

        /// <summary>
        /// 事件处理：多音字方案表新添加行。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_多音字方案_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            _dgv多音字方案表Changed = true;
            refresh多音字替换Ui();
        }

        /// <summary>
        /// 事件处理：点击"自动生成"菜单项。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void 自动生成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 获取当前选中的单元格
            if (this.dgv_多音字方案.SelectedCells.Count > 0)
            {
                DataGridViewCell selectedCell = this.dgv_多音字方案.SelectedCells[0];
                int rowIndex = selectedCell.RowIndex;
                int colIndex = selectedCell.ColumnIndex - 1; // 转换为0-based的方案索引

                if (colIndex >= 0 && rowIndex >= 0)
                {
                    // 获取该行第一个单元格的内容
                    DataGridViewCell firstCell = this.dgv_多音字方案.Rows[rowIndex].Cells[0];
                    string originalText = firstCell.Value?.ToString() ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(originalText))
                    {
                        // 生成替换文本
                        PolyphonicItem pi = new PolyphonicItem() { OriginalText = originalText };
                        if (colIndex < pi.PolyphonicSchemes.Length)
                        {
                            pi.PolyphonicSchemes[colIndex].BuildDefaultReplaceStrings(originalText);
                            string replaceText = string.Join("\r\n", pi.PolyphonicSchemes[colIndex].ReplaceStrings);

                            // 填充替换文本到选中的单元格
                            selectedCell.Value = replaceText;

                            // 标记方案表已更改
                            _dgv多音字方案表Changed = true;
                            refresh多音字替换Ui();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 事件处理：点击"上一条替换目标"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_上一条替换目标_Click(object sender, EventArgs e)
        {
            if (_do多音字替换Working == true && _work多音字Index > 0)
            {
                save多音字Working(_working多音字Array[_work多音字Index]);
                _work多音字Index--;

                refresh多音字替换Ui();
            }
        }

        /// <summary>
        /// 事件处理：点击"下一条替换目标"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_下一条替换目标_Click(object sender, EventArgs e)
        {
            if (_do多音字替换Working == true && _work多音字Index < _working多音字Array.Length - 1)
            {
                save多音字Working(_working多音字Array[_work多音字Index]);
                _work多音字Index++;

                refresh多音字替换Ui();
            }
        }

        /// <summary>
        /// 事件处理：点击"下一组替换目标"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_下一组替换目标_Click(object sender, EventArgs e)
        {
            if (_do多音字替换Working == true)
            {
                var subArray = _working多音字Array.Where(a => a.UsedPolyphonicItem.Equals(_working多音字Array[_work多音字Index].UsedPolyphonicItem) == true).ToArray();
                int index = Array.IndexOf(subArray, _working多音字Array[_work多音字Index]);
                for (int i = index; i < subArray.Length; i++)
                {
                    save多音字Working(_working多音字Array[i]);
                }

                _work多音字Index += subArray.Length - index;
                refresh多音字替换Ui();
            }
        }

        /// <summary>
        /// 事件处理：点击"应用替换结果"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_应用替换结果_Click(object sender, EventArgs e)
        {
            if (_do多音字替换Working == true)
            {
                save多音字Working(_working多音字Array[_work多音字Index]);
                this.tb_最终文本.Text = ReplaceItem.ApplyReplacements(this.tb_最终文本.Text, _working多音字Array);

                bt_取消应用替换结果_Click(sender, e);
            }
        }

        /// <summary>
        /// 事件处理：点击"取消应用替换结果"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_取消应用替换结果_Click(object sender, EventArgs e)
        {
            if (_do多音字替换Working == true)
            {
                _do多音字替换Working = false;
                refresh多音字替换Ui();
            }
        }

        #endregion

        #endregion
    }
}
