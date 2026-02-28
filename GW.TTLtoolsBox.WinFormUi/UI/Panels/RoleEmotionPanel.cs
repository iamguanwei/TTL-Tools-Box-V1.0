using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GW.TTLtoolsBox.Core.Entity;
using GW.TTLtoolsBox.Core.FileAccesser;
using GW.TTLtoolsBox.Core.SystemOption.TtlEngine;
using GW.TTLtoolsBox.WinFormUi.Base;
using GW.TTLtoolsBox.WinFormUi.Helper;
using GW.TTLtoolsBox.WinFormUi.Manager;
using static GW.TTLtoolsBox.WinFormUi.Helper.Constants;

namespace GW.TTLtoolsBox.WinFormUi.UI.Panels
{
    /// <summary>
    /// 角色和情绪指定面板，提供角色和情绪指定配置功能。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 管理角色和情绪指定数据
    /// - 支持多段落编辑
    /// - 支持批量设置角色和特性
    /// - 支持文本粘贴和复制
    /// 
    /// 使用场景：
    /// - 在TTL工具箱中配置角色与文本的对应关系
    /// - 作为工作流的一部分，为后续语音生成提供角色和情绪指定
    /// 
    /// 依赖关系：
    /// - 依赖TtlSchemeController获取当前引擎信息
    /// - 依赖ProjectFile保存/加载角色情绪数据
    /// - 依赖RoleMappingPanel获取角色列表
    /// </remarks>
    public class RoleEmotionPanel : ViewBase
    {
        #region 常量

        /// <summary>
        /// 默认角色标识。
        /// </summary>
        private const string 默认_角色标识 = Constants.默认_角色标识;

        /// <summary>
        /// 角色和文本之间的分隔符。
        /// </summary>
        private const string 角色_文本分隔符 = Constants.角色_文本分隔符;

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
        /// 初始化RoleEmotionPanel类的新实例。
        /// </summary>
        public RoleEmotionPanel()
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
        /// 获取或设置角色名称列表获取委托。
        /// </summary>
        public Func<List<string>> GetRoleNamesFunc { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 初始化面板。
        /// </summary>
        public void InitializePanel()
        {
            init角色和情绪指定Ui();
        }

        /// <summary>
        /// 刷新面板UI状态。
        /// </summary>
        public override void RefreshUi()
        {
            refresh角色和情绪指定Ui();
        }

        /// <summary>
        /// 加载角色和情绪指定数据。
        /// </summary>
        public void LoadData()
        {
            load角色和情绪指定Data();
        }

        /// <summary>
        /// 加载指定引擎的角色和情绪指定数据。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        public void LoadData(string engineId)
        {
            load角色和情绪指定Data(engineId);
        }

        /// <summary>
        /// 保存角色和情绪指定数据。
        /// </summary>
        public void SaveData()
        {
            save角色和情绪指定Data();
        }

        /// <summary>
        /// 保存角色和情绪指定数据到指定引擎。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        public void SaveData(string engineId)
        {
            save角色和情绪指定Data(engineId);
        }

        /// <summary>
        /// 刷新角色清单。
        /// </summary>
        public void RefreshRoleList()
        {
            refresh角色和情绪指定角色清单();
        }

        /// <summary>
        /// 设置表格列（切换引擎时调用）。
        /// </summary>
        public void SetupDataGridViewColumns()
        {
            setup角色和情绪指定表格();
        }

        /// <summary>
        /// 填充表格数据。
        /// </summary>
        /// <param name="text">要填充的文本。</param>
        public void FillTable(string text)
        {
            fill角色和情绪指定表格(text);
        }

        /// <summary>
        /// 清除面板数据。
        /// </summary>
        public void ClearData()
        {
            clear角色和情绪指定Ui();
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
        /// 触发切换到下一个选项卡事件。
        /// </summary>
        /// <param name="text">要传递的文本。</param>
        protected void OnSwitchToNextPageRequested(string text)
        {
            SwitchToNextPageRequested?.Invoke(this, new SwitchToNextPageEventArgs(text));
        }

        #endregion

        #endregion

        #region private

        #region 字段

        /// <summary>
        /// 角色和情绪指定表格的数据源。
        /// </summary>
        private List<RoleTextItem> _roleTextItems = new List<RoleTextItem>();

        /// <summary>
        /// 所有段落的数据，每个元素是一个段落的角色文本项列表。
        /// </summary>
        private List<List<RoleTextItem>> _allParagraphsData = new List<List<RoleTextItem>>();

        /// <summary>
        /// 当前显示的段落索引。
        /// </summary>
        private int _currentParagraphIndex = 0;

        /// <summary>
        /// 缓存的角色名称列表，用于判断是否需要刷新下拉框。
        /// </summary>
        private List<string> _cachedRoleNames = new List<string>();

        #endregion

        #region UI初始化

        /// <summary>
        /// 初始化角色和情绪指定UI。
        /// </summary>
        private void init角色和情绪指定Ui()
        {
            // 设置表格属性（会保存并恢复数据源）
            setup角色和情绪指定表格();

            // 绑定按钮事件
            this.bt_清理文本.Click -= bt_清理文本_Click;
            this.bt_清理文本.Click += bt_清理文本_Click;

            this.bt_发送到下一步.Click -= bt_发送到下一步_Click;
            this.bt_发送到下一步.Click += bt_发送到下一步_Click;

            this.bt_复制文本.Click -= bt_复制文本_Click;
            this.bt_复制文本.Click += bt_复制文本_Click;

            this.bt_粘贴文本.Click -= bt_粘贴文本_Click;
            this.bt_粘贴文本.Click += bt_粘贴文本_Click;

            this.bt_上一段.Click -= bt_上一段_Click;
            this.bt_上一段.Click += bt_上一段_Click;

            this.bt_下一段.Click -= bt_下一段_Click;
            this.bt_下一段.Click += bt_下一段_Click;

            this.cb_语音段.SelectedIndexChanged -= cb_语音段_SelectedIndexChanged;
            this.cb_语音段.SelectedIndexChanged += cb_语音段_SelectedIndexChanged;

            this.cb_语音段.KeyDown -= cb_语音段_KeyDown;
            this.cb_语音段.KeyDown += cb_语音段_KeyDown;

            // 刷新UI
            refresh角色和情绪指定Ui();
        }

        #endregion

        #region UI操作

        /// <summary>
        /// 刷新角色和情绪指定UI。
        /// </summary>
        private void refresh角色和情绪指定Ui()
        {
            bool hasData = _roleTextItems.Count > 0;
            this.bt_清理文本.Enabled = hasData;
            this.bt_发送到下一步.Enabled = hasData;
            this.bt_复制文本.Enabled = hasData;
            this.bt_粘贴文本.Enabled = Clipboard.ContainsText();
        }

        /// <summary>
        /// 清理角色和情绪指定UI。
        /// </summary>
        private void clear角色和情绪指定Ui()
        {
            Action action = () =>
            {
                _roleTextItems.Clear();
                _allParagraphsData.Clear();
                _currentParagraphIndex = 0;

                dgv_角色和情绪指定.SuspendLayout();
                try
                {
                    dgv_角色和情绪指定.DataSource = null;
                    dgv_角色和情绪指定.DataSource = new BindingList<RoleTextItem>(_roleTextItems);
                }
                finally
                {
                    dgv_角色和情绪指定.ResumeLayout();
                }
            };

            UpdateUi(action);

            updateParagraphSelector();
            refresh角色和情绪指定Ui();
        }

        /// <summary>
        /// 设置角色和情绪指定表格的属性。
        /// </summary>
        private void setup角色和情绪指定表格()
        {
            // 保存当前数据源
            var savedDataSource = dgv_角色和情绪指定.DataSource;
            dgv_角色和情绪指定.DataSource = null;

            UiControlHelper.SetupDataGridViewBasicStyle(dgv_角色和情绪指定);

            dgv_角色和情绪指定.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_角色和情绪指定.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            dgv_角色和情绪指定.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgv_角色和情绪指定.AutoGenerateColumns = false;

            dgv_角色和情绪指定.Columns.Clear();

            DataGridViewComboBoxColumn roleColumn = new DataGridViewComboBoxColumn();
            roleColumn.Name = roleColumn.DataPropertyName = "Role";
            roleColumn.HeaderText = "角色";
            roleColumn.MinimumWidth = 200;
            roleColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            roleColumn.FlatStyle = FlatStyle.Flat;

            roleColumn.Items.Add(默认_角色标识);
            var roleMappingItems = GetRoleNamesFunc?.Invoke();
            if (roleMappingItems != null && roleMappingItems.Count > 0)
            {
                foreach (var roleName in roleMappingItems)
                {
                    if (!string.IsNullOrWhiteSpace(roleName))
                    {
                        roleColumn.Items.Add(roleName);
                    }
                }
            }

            dgv_角色和情绪指定.Columns.Add(roleColumn);

            // 动态添加特性列（在角色列和文本列之间）
            var currentEngine = TtlSchemeController?.CurrentEngineConnector;
            if (currentEngine != null)
            {
                foreach (var featureDef in currentEngine.FeatureDefinitions)
                {
                    var featureColumn = new DataGridViewComboBoxColumn();
                    featureColumn.Name = $"Feature_{featureDef.Name}";
                    featureColumn.HeaderText = featureDef.Name;
                    featureColumn.Tag = featureDef;
                    featureColumn.MinimumWidth = 100;
                    featureColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    featureColumn.FlatStyle = FlatStyle.Flat;

                    // 添加选项（显示名称）
                    foreach (var value in featureDef.EnumValues)
                    {
                        featureColumn.Items.Add(featureDef.GetDisplayName(value));
                    }

                    dgv_角色和情绪指定.Columns.Add(featureColumn);
                }
            }

            // 文本列放在最后
            DataGridViewTextBoxColumn textColumn = new DataGridViewTextBoxColumn();
            textColumn.Name = textColumn.DataPropertyName = "Text";
            textColumn.HeaderText = "文本";
            textColumn.MinimumWidth = 500;
            textColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            textColumn.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv_角色和情绪指定.Columns.Add(textColumn);

            dgv_角色和情绪指定.AllowUserToAddRows = false;
            dgv_角色和情绪指定.AllowUserToDeleteRows = false;

            dgv_角色和情绪指定.EditMode = DataGridViewEditMode.EditOnEnter;

            dgv_角色和情绪指定.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            dgv_角色和情绪指定.DataError -= dgv_角色和情绪指定_DataError;
            dgv_角色和情绪指定.DataError += dgv_角色和情绪指定_DataError;

            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null,
                dgv_角色和情绪指定,
                new object[] { true });

            if (savedDataSource != null)
            {
                dgv_角色和情绪指定.DataSource = savedDataSource;
            }
            else
            {
                dgv_角色和情绪指定.DataSource = new BindingList<RoleTextItem>(_roleTextItems);
            }

            // 绑定事件
            // 解绑旧事件，避免重复绑定
            dgv_角色和情绪指定.MouseDown -= dgv_角色和情绪指定_MouseDown;
            dgv_角色和情绪指定.CellBeginEdit -= dgv_角色和情绪指定_CellBeginEdit;
            dgv_角色和情绪指定.CellEndEdit -= dgv_角色和情绪指定_CellEndEdit;
            dgv_角色和情绪指定.CellFormatting -= dgv_角色和情绪指定_CellFormatting;
            dgv_角色和情绪指定.CellParsing -= dgv_角色和情绪指定_CellParsing;
            // 绑定新事件
            dgv_角色和情绪指定.DataError += dgv_角色和情绪指定_DataError;
            dgv_角色和情绪指定.MouseDown += dgv_角色和情绪指定_MouseDown;
            dgv_角色和情绪指定.CellBeginEdit += dgv_角色和情绪指定_CellBeginEdit;
            dgv_角色和情绪指定.CellEndEdit += dgv_角色和情绪指定_CellEndEdit;
            dgv_角色和情绪指定.CellFormatting += dgv_角色和情绪指定_CellFormatting;
            dgv_角色和情绪指定.CellParsing += dgv_角色和情绪指定_CellParsing;
        }

        /// <summary>
        /// 刷新角色和情绪指定选项卡中的角色清单。
        /// </summary>
        private void refresh角色和情绪指定角色清单()
        {
            if (dgv_角色和情绪指定.Columns.Count == 0)
            {
                return;
            }

            var roleColumn = dgv_角色和情绪指定.Columns[0] as DataGridViewComboBoxColumn;
            if (roleColumn == null)
            {
                return;
            }

            List<string> newRoleNames = new List<string>();
            newRoleNames.Add(默认_角色标识);
            var roleMappingItems = GetRoleNamesFunc?.Invoke();
            if (roleMappingItems != null && roleMappingItems.Count > 0)
            {
                foreach (var roleName in roleMappingItems)
                {
                    if (!string.IsNullOrWhiteSpace(roleName))
                    {
                        newRoleNames.Add(roleName);
                    }
                }
            }

            bool roleListChanged = !newRoleNames.SequenceEqual(_cachedRoleNames);

            if (roleListChanged)
            {
                _cachedRoleNames = newRoleNames;

                List<string> savedRoleNames = new List<string>();
                List<bool> savedSelectedStates = new List<bool>();
                for (int i = 0; i < dgv_角色和情绪指定.Rows.Count; i++)
                {
                    var cellValue = dgv_角色和情绪指定.Rows[i].Cells[0].Value;
                    string roleValue = cellValue != null ? cellValue.ToString() : string.Empty;
                    savedRoleNames.Add(roleValue);
                    savedSelectedStates.Add(dgv_角色和情绪指定.Rows[i].Selected);
                }

                if (savedRoleNames.Count == 0 && _roleTextItems.Count > 0)
                {
                    foreach (var item in _roleTextItems)
                    {
                        savedRoleNames.Add(item.Role);
                        savedSelectedStates.Add(false);
                    }
                }

                dgv_角色和情绪指定.SuspendLayout();

                try
                {
                    dgv_角色和情绪指定.DataSource = null;

                    roleColumn.Items.Clear();
                    foreach (string roleName in newRoleNames)
                    {
                        roleColumn.Items.Add(roleName);
                    }

                    for (int i = 0; i < _roleTextItems.Count && i < savedRoleNames.Count; i++)
                    {
                        string savedRole = savedRoleNames[i];
                        if (roleColumn.Items.Contains(savedRole))
                        {
                            _roleTextItems[i].Role = savedRole;
                        }
                        else
                        {
                            _roleTextItems[i].Role = 默认_角色标识;
                        }
                    }

                    dgv_角色和情绪指定.DataSource = new BindingList<RoleTextItem>(_roleTextItems);

                    if (dgv_角色和情绪指定.Rows.Count > 0)
                    {
                        dgv_角色和情绪指定.ClearSelection();

                        int firstSelectedIndex = -1;
                        for (int i = 0; i < dgv_角色和情绪指定.Rows.Count && i < savedSelectedStates.Count; i++)
                        {
                            if (savedSelectedStates[i])
                            {
                                dgv_角色和情绪指定.Rows[i].Selected = true;
                                if (firstSelectedIndex < 0)
                                {
                                    firstSelectedIndex = i;
                                    dgv_角色和情绪指定.CurrentCell = dgv_角色和情绪指定.Rows[firstSelectedIndex].Cells[0];
                                }
                            }
                        }

                        if (firstSelectedIndex < 0)
                        {
                            dgv_角色和情绪指定.CurrentCell = dgv_角色和情绪指定.Rows[0].Cells[0];
                        }
                    }
                }
                finally
                {
                    dgv_角色和情绪指定.ResumeLayout();
                }
            }
        }

        /// <summary>
        /// 填充表格数据。
        /// </summary>
        /// <param name="text">要填充的文本。</param>
        private void fill角色和情绪指定表格(string text)
        {
            clear角色和情绪指定Ui();

            if (string.IsNullOrWhiteSpace(text))
            {
                dgv_角色和情绪指定.DataSource = new BindingList<RoleTextItem>(_roleTextItems);
                updateParagraphSelector();
                refresh角色和情绪指定Ui();
                return;
            }

            string[] paragraphs = text.Split(new string[] { 生成段落_分隔符 }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string paragraph in paragraphs)
            {
                List<RoleTextItem> paragraphItems = new List<RoleTextItem>();

                string[] lines = paragraph.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) == false)
                    {
                        paragraphItems.Add(new RoleTextItem(默认_角色标识, line));
                    }
                    else
                    {
                        if (paragraphItems.Count > 0) paragraphItems.Last().Text += $"{Environment.NewLine}";
                    }
                }

                _allParagraphsData.Add(paragraphItems);
            }

            if (_allParagraphsData.Count == 0)
            {
                _allParagraphsData.Add(new List<RoleTextItem>());
            }

            _roleTextItems = new List<RoleTextItem>(_allParagraphsData[0]);

            dgv_角色和情绪指定.SuspendLayout();
            try
            {
                dgv_角色和情绪指定.DataSource = new BindingList<RoleTextItem>(_roleTextItems);

                if (_roleTextItems.Count > 0)
                {
                    dgv_角色和情绪指定.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    dgv_角色和情绪指定.CurrentCell = dgv_角色和情绪指定.Rows[0].Cells[0];
                }
            }
            finally
            {
                dgv_角色和情绪指定.ResumeLayout();
            }

            updateParagraphSelector();
            refresh角色和情绪指定Ui();
        }

        /// <summary>
        /// 更新段落选择器（下拉列表框）。
        /// </summary>
        private void updateParagraphSelector()
        {
            this.cb_语音段.Items.Clear();
            for (int i = 0; i < _allParagraphsData.Count; i++)
            {
                this.cb_语音段.Items.Add((i + 1).ToString());
            }

            if (_allParagraphsData.Count > 0)
            {
                this.cb_语音段.SelectedIndex = _currentParagraphIndex;
            }

            // 更新按钮状态
            this.bt_上一段.Enabled = _currentParagraphIndex > 0;
            this.bt_下一段.Enabled = _currentParagraphIndex < _allParagraphsData.Count - 1;
        }

        /// <summary>
        /// 显示指定索引的段落。
        /// </summary>
        /// <param name="index">段落索引。</param>
        private void displayParagraph(int index)
        {
            if (index < 0 || index >= _allParagraphsData.Count)
            {
                return;
            }

            saveCurrentParagraphData();

            _currentParagraphIndex = index;

            dgv_角色和情绪指定.SuspendLayout();
            try
            {
                _roleTextItems = new List<RoleTextItem>(_allParagraphsData[index]);
                dgv_角色和情绪指定.DataSource = new BindingList<RoleTextItem>(_roleTextItems);

                if (_roleTextItems.Count > 0)
                {
                    dgv_角色和情绪指定.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                }
            }
            finally
            {
                dgv_角色和情绪指定.ResumeLayout();
            }

            this.cb_语音段.SelectedIndex = index;
            this.bt_上一段.Enabled = index > 0;
            this.bt_下一段.Enabled = index < _allParagraphsData.Count - 1;
        }

        /// <summary>
        /// 保存当前段落的数据到_allParagraphsData。
        /// </summary>
        private void saveCurrentParagraphData()
        {
            if (_currentParagraphIndex >= 0 && _currentParagraphIndex < _allParagraphsData.Count)
            {
                _allParagraphsData[_currentParagraphIndex] = new List<RoleTextItem>(_roleTextItems);
            }
        }

        #endregion

        #region 业务操作

        /// <summary>
        /// 批量设置角色。
        /// </summary>
        /// <param name="role">要设置的角色。</param>
        private void batchSetRole(string role)
        {
            // 结束所有正在编辑的单元格，确保修改能正确应用
            dgv_角色和情绪指定.EndEdit();

            foreach (DataGridViewRow row in dgv_角色和情绪指定.SelectedRows)
            {
                if (row.DataBoundItem is RoleTextItem item)
                {
                    item.Role = role;
                }
            }
            // 刷新表格
            dgv_角色和情绪指定.Refresh();
        }

        /// <summary>
        /// 批量设置特性值。
        /// </summary>
        /// <param name="featureName">特性名称。</param>
        /// <param name="featureValue">特性枚举值。</param>
        /// <param name="displayName">显示名称。</param>
        private void batchSetFeature(string featureName, object featureValue, string displayName)
        {
            // 结束所有正在编辑的单元格，确保修改能正确应用
            dgv_角色和情绪指定.EndEdit();

            int intValue = Convert.ToInt32(featureValue);

            foreach (DataGridViewRow row in dgv_角色和情绪指定.SelectedRows)
            {
                if (row.DataBoundItem is RoleTextItem item)
                {
                    item.FeatureSelections[featureName] = intValue;
                }
            }
            // 刷新表格
            dgv_角色和情绪指定.Refresh();
            OnProjectModified();
        }

        /// <summary>
        /// 生成最终文本。
        /// </summary>
        /// <returns>生成的最终文本。</returns>
        private string generateFinalText()
        {
            string outValue = string.Empty;

            // 先保存当前段落的数据
            saveCurrentParagraphData();

            // 遍历所有段落
            for (int i = 0; i < _allParagraphsData.Count; i++)
            {
                StringBuilder finalText = new StringBuilder();
                List<RoleTextItem> paragraphItems = _allParagraphsData[i];

                // 添加段落分隔符（第一个段落前不加）
                if (i > 0)
                {
                    finalText.Append(生成段落_分隔符);
                }

                // 添加该段落的内容
                foreach (RoleTextItem item in paragraphItems)
                {
                    if (string.IsNullOrWhiteSpace(item.Text))
                    {
                        // 保留空行
                        finalText.AppendLine();
                    }
                    else
                    {
                        var parts = new List<string>();
                        parts.Add(item.Role);

                        // 添加特性选择信息（格式：特性名=值）
                        if (item.FeatureSelections != null && item.FeatureSelections.Count > 0)
                        {
                            foreach (var kvp in item.FeatureSelections)
                            {
                                if (kvp.Value != 0) // 0表示"不选择"，跳过
                                {
                                    parts.Add($"{kvp.Key}={kvp.Value}");
                                }
                            }
                        }

                        parts.Add(item.Text);

                        finalText.AppendLine(string.Join(角色_文本分隔符, parts));
                    }
                }

                // 移除最后一个换行符
                var text = finalText.ToString();
                outValue += text.Substring(0, text.Length - (Environment.NewLine).Length);
            }

            return outValue;
        }

        /// <summary>
        /// 保存角色和情绪指定表格内容到项目文件对象。
        /// </summary>
        private void save角色和情绪指定Data()
        {
            save角色和情绪指定Data(CurrentEngineId);
        }

        /// <summary>
        /// 保存角色和情绪指定表格内容到项目文件对象。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        private void save角色和情绪指定Data(string engineId)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return;
            }

            // 先保存当前段落的数据
            saveCurrentParagraphData();

            // 转换为ProjectFile需要的格式
            List<List<RoleEmotionItem>> allData = new List<List<RoleEmotionItem>>();
            foreach (List<RoleTextItem> paragraph in _allParagraphsData)
            {
                List<RoleEmotionItem> paragraphItems = new List<RoleEmotionItem>();
                foreach (RoleTextItem item in paragraph)
                {
                    paragraphItems.Add(new RoleEmotionItem
                    {
                        Role = item.Role,
                        Text = item.Text,
                        FeatureSelections = new Dictionary<string, int>(item.FeatureSelections)
                    });
                }
                allData.Add(paragraphItems);
            }

            // 保存到项目文件
            ProjectFile?.SaveRoleEmotionData(engineId, allData);
        }

        /// <summary>
        /// 加载角色和情绪指定表格内容。
        /// </summary>
        private void load角色和情绪指定Data()
        {
            load角色和情绪指定Data(CurrentEngineId);
        }

        /// <summary>
        /// 加载角色和情绪指定表格内容。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        private void load角色和情绪指定Data(string engineId)
        {
            List<List<RoleEmotionItem>> loadedData = string.IsNullOrEmpty(engineId)
                ? new List<List<RoleEmotionItem>>()
                : ProjectFile?.LoadRoleEmotionData(engineId) ?? new List<List<RoleEmotionItem>>();

            _allParagraphsData.Clear();
            _currentParagraphIndex = 0;
            _roleTextItems.Clear();

            dgv_角色和情绪指定.SuspendLayout();
            try
            {
                if (loadedData.Count == 0)
                {
                    _allParagraphsData.Add(new List<RoleTextItem>());
                    dgv_角色和情绪指定.DataSource = new BindingList<RoleTextItem>(_roleTextItems);
                }
                else
                {
                    foreach (List<RoleEmotionItem> paragraph in loadedData)
                    {
                        List<RoleTextItem> paragraphItems = new List<RoleTextItem>();
                        foreach (RoleEmotionItem item in paragraph)
                        {
                            var roleTextItem = new RoleTextItem(item.Role, item.Text);
                            // 加载特性选择
                            if (item.FeatureSelections != null)
                            {
                                foreach (var kvp in item.FeatureSelections)
                                {
                                    roleTextItem.FeatureSelections[kvp.Key] = kvp.Value;
                                }
                            }
                            paragraphItems.Add(roleTextItem);
                        }
                        _allParagraphsData.Add(paragraphItems);
                    }

                    _roleTextItems = new List<RoleTextItem>(_allParagraphsData[0]);
                    dgv_角色和情绪指定.DataSource = new BindingList<RoleTextItem>(_roleTextItems);

                    if (_roleTextItems.Count > 0)
                    {
                        dgv_角色和情绪指定.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                        dgv_角色和情绪指定.CurrentCell = dgv_角色和情绪指定.Rows[0].Cells[0];
                    }
                }
            }
            finally
            {
                dgv_角色和情绪指定.ResumeLayout();
            }

            refresh角色和情绪指定角色清单();
            updateParagraphSelector();
            refresh角色和情绪指定Ui();
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 事件处理：点击"清理文本"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_清理文本_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("清理后无法恢复，确定清理表格中的所有内容吗？", "确定清理吗？", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                clear角色和情绪指定Ui();
            }
        }

        /// <summary>
        /// 事件处理：点击"发送到下一步"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_发送到下一步_Click(object sender, EventArgs e)
        {
            string finalText = generateFinalText();
            // 发送到下一个选项卡
            OnSwitchToNextPageRequested(finalText);
        }

        /// <summary>
        /// 事件处理：点击"复制文本"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_复制文本_Click(object sender, EventArgs e)
        {
            string finalText = generateFinalText();
            copyToClipboard(finalText);
        }

        /// <summary>
        /// 事件处理：点击"粘贴文本"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_粘贴文本_Click(object sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText() == false)
                {
                    MessageBox.Show("剪贴板中没有文本内容", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string clipboardText = Clipboard.GetText();
                if (string.IsNullOrWhiteSpace(clipboardText))
                {
                    MessageBox.Show("剪贴板中的内容为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataGridViewComboBoxColumn roleColumn = dgv_角色和情绪指定.Columns["Role"] as DataGridViewComboBoxColumn;
                if (roleColumn == null)
                {
                    MessageBox.Show("角色列未正确初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_roleTextItems.Count > 0)
                {
                    if (MessageBox.Show("粘贴前将清空表格中的现有内容，选择“确定”将清空表格并粘贴。", "清空并粘贴吗？", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        clear角色和情绪指定Ui();

                        string[] lines = clipboardText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        foreach (string line in lines)
                        {
                            string role = string.Empty;
                            string text = string.Empty;

                            if (line.Contains(角色_文本分隔符))
                            {
                                string[] parts = line.Split(new string[] { 角色_文本分隔符 }, StringSplitOptions.None);
                                role = parts[0].Trim();
                                text = parts.Length > 1 ? parts[1] : string.Empty;

                                if (roleColumn.Items.Contains(role) == false)
                                {
                                    role = 默认_角色标识;
                                }
                            }
                            else
                            {
                                role = 默认_角色标识;
                                text = line;
                            }

                            _roleTextItems.Add(new RoleTextItem(role, text));
                        }

                        dgv_角色和情绪指定.SuspendLayout();
                        try
                        {
                            dgv_角色和情绪指定.DataSource = new BindingList<RoleTextItem>(_roleTextItems);
                        }
                        finally
                        {
                            dgv_角色和情绪指定.ResumeLayout();
                        }

                        refresh角色和情绪指定Ui();
                        MessageBox.Show("文本已成功粘贴到表格", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"粘贴文本时出错：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 事件处理：点击"上一段"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_上一段_Click(object sender, EventArgs e)
        {
            if (_currentParagraphIndex > 0)
            {
                displayParagraph(_currentParagraphIndex - 1);
            }
        }

        /// <summary>
        /// 事件处理：点击"下一段"按钮。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void bt_下一段_Click(object sender, EventArgs e)
        {
            if (_currentParagraphIndex < _allParagraphsData.Count - 1)
            {
                displayParagraph(_currentParagraphIndex + 1);
            }
        }

        /// <summary>
        /// 事件处理：语音段下拉列表框选择改变。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void cb_语音段_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cb_语音段.SelectedIndex >= 0 &&
                this.cb_语音段.SelectedIndex != _currentParagraphIndex)
            {
                displayParagraph(this.cb_语音段.SelectedIndex);
            }
        }

        /// <summary>
        /// 事件处理：语音段下拉列表框按键按下。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void cb_语音段_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                // 尝试解析输入的数字
                if (int.TryParse(this.cb_语音段.Text, out int paragraphNumber))
                {
                    int targetIndex = paragraphNumber - 1;
                    if (targetIndex >= 0 && targetIndex < _allParagraphsData.Count)
                    {
                        displayParagraph(targetIndex);
                    }
                }
            }
        }

        /// <summary>
        /// 事件处理：表格数据错误，用于处理下拉列表的数据错误。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_角色和情绪指定_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        /// <summary>
        /// 事件处理：表格单元格开始编辑。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_角色和情绪指定_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // 当多选时，阻止第一列进入编辑状态
            if (dgv_角色和情绪指定.SelectedRows.Count > 1 && e.ColumnIndex == 0)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 事件处理：表格单元格结束编辑。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_角色和情绪指定_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            OnProjectModified();
        }

        /// <summary>
        /// 事件处理：表格单元格格式化，用于显示特性列的值。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_角色和情绪指定_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            var column = dgv_角色和情绪指定.Columns[e.ColumnIndex];
            if (column.Tag is TtlEngineFeatureDefinition featureDef)
            {
                var row = dgv_角色和情绪指定.Rows[e.RowIndex];
                if (row == null || row.DataBoundItem == null)
                {
                    return;
                }

                var item = row.DataBoundItem as RoleTextItem;
                if (item != null && item.FeatureSelections.TryGetValue(featureDef.Name, out int enumValue))
                {
                    object enumObj = featureDef.GetEnumValueByInt(enumValue);
                    e.Value = featureDef.GetDisplayName(enumObj);
                    e.FormattingApplied = true;
                }
                else
                {
                    // 默认显示第一个选项
                    e.Value = featureDef.GetDisplayName(featureDef.EnumValues.GetValue(0));
                    e.FormattingApplied = true;
                }
            }
        }

        /// <summary>
        /// 事件处理：表格单元格解析，用于保存特性列的值。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_角色和情绪指定_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            var column = dgv_角色和情绪指定.Columns[e.ColumnIndex];
            if (column.Tag is TtlEngineFeatureDefinition featureDef)
            {
                var item = dgv_角色和情绪指定.Rows[e.RowIndex].DataBoundItem as RoleTextItem;
                if (item != null && e.Value is string displayName)
                {
                    object enumValue = featureDef.GetEnumValueByDisplayName(displayName);
                    item.FeatureSelections[featureDef.Name] = Convert.ToInt32(enumValue);
                    e.Value = displayName;
                    e.ParsingApplied = true;
                    OnProjectModified();
                }
            }
        }

        /// <summary>
        /// 事件处理：表格鼠标右键点击。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_角色和情绪指定_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo hitTest = dgv_角色和情绪指定.HitTest(e.X, e.Y);
                // 改为右键点击行首RowHeaders才弹出右键菜单
                if (hitTest.Type == DataGridViewHitTestType.RowHeader && hitTest.RowIndex >= 0)
                {
                    // 当右键点击的是已经选中的行首，弹出菜单时保持选中不变
                    // 当右键点击的是之前没有选中的行首，且之前有多个选中行，则不弹出菜单也不改变选择
                    if (dgv_角色和情绪指定.Rows[hitTest.RowIndex].Selected)
                    {
                        // 点击的是已经选中的行，保持选中不变，显示菜单
                        showContextMenu(e.Location);
                    }
                    else
                    {
                        // 点击的是未选中的行，选中该行
                        dgv_角色和情绪指定.ClearSelection();
                        dgv_角色和情绪指定.Rows[hitTest.RowIndex].Selected = true;

                        // 显示右键菜单
                        showContextMenu(e.Location);
                    }
                }
            }
        }

        /// <summary>
        /// 显示右键菜单。
        /// </summary>
        /// <param name="location">菜单位置。</param>
        private void showContextMenu(Point location)
        {
            ContextMenuStrip cms = new ContextMenuStrip();

            // 添加角色菜单项
            ToolStripMenuItem setRoleMenuItem = new ToolStripMenuItem("角色");

            // 添加角色子菜单
            DataGridViewComboBoxColumn roleColumn = dgv_角色和情绪指定.Columns["Role"] as DataGridViewComboBoxColumn;
            if (roleColumn != null && roleColumn.Items.Count > 0)
            {
                for (int i = 0; i < roleColumn.Items.Count; i++)
                {
                    string roleName = roleColumn.Items[i].ToString();
                    ToolStripMenuItem roleMenuItem = new ToolStripMenuItem(roleName);
                    roleMenuItem.Click += (s, args) => batchSetRole(roleName);
                    setRoleMenuItem.DropDownItems.Add(roleMenuItem);

                    // 在第一项（默认）与下面的项之间添加一条分割线（如果下面还有其他项）
                    if (i == 0 && roleColumn.Items.Count > 1)
                    {
                        setRoleMenuItem.DropDownItems.Add(new ToolStripSeparator());
                    }
                }
            }

            // 添加到上下文菜单
            cms.Items.Add(setRoleMenuItem);

            // 添加特性菜单项（如果有特性定义）
            var currentEngine = TtlSchemeController?.CurrentEngineConnector;
            if (currentEngine != null && currentEngine.FeatureDefinitions.Length > 0)
            {
                ToolStripMenuItem featureMenuItem = new ToolStripMenuItem("特性");

                foreach (var featureDef in currentEngine.FeatureDefinitions)
                {
                    // 二级菜单：特性类别
                    ToolStripMenuItem categoryMenuItem = new ToolStripMenuItem(featureDef.Name);

                    // 三级菜单：具体选项
                    foreach (var value in featureDef.EnumValues)
                    {
                        string displayName = featureDef.GetDisplayName(value);
                        ToolStripMenuItem optionMenuItem = new ToolStripMenuItem(displayName);
                        optionMenuItem.Click += (s, args) => batchSetFeature(featureDef.Name, value, displayName);
                        categoryMenuItem.DropDownItems.Add(optionMenuItem);
                    }

                    featureMenuItem.DropDownItems.Add(categoryMenuItem);
                }

                cms.Items.Add(featureMenuItem);
            }

            // 显示菜单
            cms.Show(dgv_角色和情绪指定, location);
        }

        /// <summary>
        /// 复制文本到剪贴板。
        /// </summary>
        /// <param name="text">要复制的文本。</param>
        private static void copyToClipboard(string text)
        {
            if (string.IsNullOrEmpty(text) == false)
            {
                Clipboard.SetText(text);
            }
        }

        #endregion

        #endregion

        #region Windows Form Designer generated code

        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 设计器支持所需的方法。
        /// </summary>
        private void InitializeComponent()
        {
            this.dgv_角色和情绪指定 = new System.Windows.Forms.DataGridView();
            this.bt_发送到下一步 = new System.Windows.Forms.Button();
            this.bt_清理文本 = new System.Windows.Forms.Button();
            this.bt_复制文本 = new System.Windows.Forms.Button();
            this.bt_粘贴文本 = new System.Windows.Forms.Button();
            this.bt_上一段 = new System.Windows.Forms.Button();
            this.bt_下一段 = new System.Windows.Forms.Button();
            this.cb_语音段 = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_角色和情绪指定)).BeginInit();
            this.SuspendLayout();
            //
            // dgv_角色和情绪指定
            //
            this.dgv_角色和情绪指定.AllowUserToAddRows = false;
            this.dgv_角色和情绪指定.AllowUserToDeleteRows = false;
            this.dgv_角色和情绪指定.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv_角色和情绪指定.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_角色和情绪指定.Location = new System.Drawing.Point(5, 45);
            this.dgv_角色和情绪指定.Name = "dgv_角色和情绪指定";
            this.dgv_角色和情绪指定.RowTemplate.Height = 23;
            this.dgv_角色和情绪指定.Size = new System.Drawing.Size(1291, 568);
            this.dgv_角色和情绪指定.TabIndex = 0;
            //
            // bt_发送到下一步
            //
            this.bt_发送到下一步.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_发送到下一步.Location = new System.Drawing.Point(1192, 619);
            this.bt_发送到下一步.Name = "bt_发送到下一步";
            this.bt_发送到下一步.Size = new System.Drawing.Size(104, 28);
            this.bt_发送到下一步.TabIndex = 1;
            this.bt_发送到下一步.Text = "发送到下一步";
            this.bt_发送到下一步.UseVisualStyleBackColor = true;
            //
            // bt_清理文本
            //
            this.bt_清理文本.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bt_清理文本.Location = new System.Drawing.Point(5, 619);
            this.bt_清理文本.Name = "bt_清理文本";
            this.bt_清理文本.Size = new System.Drawing.Size(104, 28);
            this.bt_清理文本.TabIndex = 2;
            this.bt_清理文本.Text = "清理文本";
            this.bt_清理文本.UseVisualStyleBackColor = true;
            //
            // bt_复制文本
            //
            this.bt_复制文本.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_复制文本.Location = new System.Drawing.Point(1082, 619);
            this.bt_复制文本.Name = "bt_复制文本";
            this.bt_复制文本.Size = new System.Drawing.Size(104, 28);
            this.bt_复制文本.TabIndex = 3;
            this.bt_复制文本.Text = "复制文本";
            this.bt_复制文本.UseVisualStyleBackColor = true;
            //
            // bt_粘贴文本
            //
            this.bt_粘贴文本.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_粘贴文本.Location = new System.Drawing.Point(972, 619);
            this.bt_粘贴文本.Name = "bt_粘贴文本";
            this.bt_粘贴文本.Size = new System.Drawing.Size(104, 28);
            this.bt_粘贴文本.TabIndex = 4;
            this.bt_粘贴文本.Text = "粘贴文本";
            this.bt_粘贴文本.UseVisualStyleBackColor = true;
            //
            // bt_上一段
            //
            this.bt_上一段.Location = new System.Drawing.Point(5, 16);
            this.bt_上一段.Name = "bt_上一段";
            this.bt_上一段.Size = new System.Drawing.Size(40, 23);
            this.bt_上一段.TabIndex = 5;
            this.bt_上一段.Text = "←";
            this.bt_上一段.UseVisualStyleBackColor = true;
            //
            // bt_下一段
            //
            this.bt_下一段.Location = new System.Drawing.Point(136, 16);
            this.bt_下一段.Name = "bt_下一段";
            this.bt_下一段.Size = new System.Drawing.Size(40, 23);
            this.bt_下一段.TabIndex = 6;
            this.bt_下一段.Text = "→";
            this.bt_下一段.UseVisualStyleBackColor = true;
            //
            // cb_语音段
            //
            this.cb_语音段.FormattingEnabled = true;
            this.cb_语音段.Location = new System.Drawing.Point(51, 18);
            this.cb_语音段.Name = "cb_语音段";
            this.cb_语音段.Size = new System.Drawing.Size(79, 20);
            this.cb_语音段.TabIndex = 7;
            //
            // RoleEmotionPanel
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cb_语音段);
            this.Controls.Add(this.bt_下一段);
            this.Controls.Add(this.bt_上一段);
            this.Controls.Add(this.bt_粘贴文本);
            this.Controls.Add(this.bt_复制文本);
            this.Controls.Add(this.bt_清理文本);
            this.Controls.Add(this.bt_发送到下一步);
            this.Controls.Add(this.dgv_角色和情绪指定);
            this.Name = "RoleEmotionPanel";
            this.Size = new System.Drawing.Size(1301, 652);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_角色和情绪指定)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgv_角色和情绪指定;
        private System.Windows.Forms.Button bt_发送到下一步;
        private System.Windows.Forms.Button bt_清理文本;
        private System.Windows.Forms.Button bt_复制文本;
        private System.Windows.Forms.Button bt_粘贴文本;
        private System.Windows.Forms.Button bt_上一段;
        private System.Windows.Forms.Button bt_下一段;
        private System.Windows.Forms.ComboBox cb_语音段;

        #endregion
    }
}
