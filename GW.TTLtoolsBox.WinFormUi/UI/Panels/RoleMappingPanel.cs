using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GW.TTLtoolsBox.Core.Entity;
using GW.TTLtoolsBox.Core.FileAccesser;
using GW.TTLtoolsBox.Core.SystemOption.TtlEngine;
using GW.TTLtoolsBox.WinFormUi.Base;
using GW.TTLtoolsBox.WinFormUi.Helper;
using GW.TTLtoolsBox.WinFormUi.Manager;

namespace GW.TTLtoolsBox.WinFormUi.UI.Panels
{
    /// <summary>
    /// 角色映射面板，提供角色映射配置功能。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 管理角色名称到TTL引擎朗读者源名称的映射
    /// - 支持声音预览功能
    /// - 支持映射数据的加载和保存
    /// 
    /// 使用场景：
    /// - 在TTL工具箱中配置角色与朗读者的对应关系
    /// - 作为工作流的一部分，为后续语音生成提供角色映射
    /// 
    /// 依赖关系：
    /// - 依赖TtlSchemeController获取当前引擎信息
    /// - 依赖ProjectFile保存/加载映射数据
    /// - 依赖PreviewVoiceManager进行声音预览
    /// </remarks>
    public partial class RoleMappingPanel : ViewBase
    {
        #region 常量

        /// <summary>
        /// 已丢失标识后缀。
        /// </summary>
        private const string 已丢失_标识后缀 = Constants.已丢失_标识后缀;

        #endregion

        #region public

        #region 事件

        /// <summary>
        /// 项目内容被修改时触发的事件。
        /// </summary>
        public event EventHandler ProjectModified;

        /// <summary>
        /// 角色映射列表改变时触发的事件。
        /// </summary>
        public event EventHandler RoleMappingListChanged;

        /// <summary>
        /// 请求预览声音时触发的事件。
        /// </summary>
        public event EventHandler<PreviewVoiceRequestEventArgs> PreviewVoiceRequested;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化RoleMappingPanel类的新实例。
        /// </summary>
        public RoleMappingPanel()
        {
            InitializeComponent();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取角色映射数据列表。
        /// </summary>
        public BindingList<RoleMappingItem> RoleMappingItems => _roleMappingItems;

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
        /// 初始化面板。
        /// </summary>
        public void InitializePanel()
        {
            init角色映射Ui();
        }

        /// <summary>
        /// 刷新面板UI状态。
        /// </summary>
        public override void RefreshUi()
        {
            refresh角色映射Ui();
        }

        /// <summary>
        /// 加载角色映射数据。
        /// </summary>
        public void LoadData()
        {
            load角色映射Data();
        }

        /// <summary>
        /// 加载指定引擎的角色映射数据。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        public void LoadData(string engineId)
        {
            load角色映射Data(engineId);
        }

        /// <summary>
        /// 保存角色映射数据。
        /// </summary>
        public void SaveData()
        {
            save角色映射Data();
        }

        /// <summary>
        /// 保存角色映射数据到指定引擎。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        public void SaveData(string engineId)
        {
            save角色映射Data(engineId);
        }

        /// <summary>
        /// 清除缓存的角色名称列表。
        /// </summary>
        public void ClearCachedRoleNames()
        {
            _cachedRoleNames.Clear();
        }

        /// <summary>
        /// 刷新声音预览按钮状态。
        /// </summary>
        public void RefreshVoicePreviewButtons()
        {
            if (this.dgv_角色映射 != null && this.dgv_角色映射.IsHandleCreated)
            {
                this.dgv_角色映射.Invalidate();
            }
        }

        /// <summary>
        /// 获取所有角色名称列表。
        /// </summary>
        /// <returns>角色名称列表。</returns>
        public List<string> GetRoleNames()
        {
            List<string> roleNames = new List<string>();
            if (_roleMappingItems != null && _roleMappingItems.Count > 0)
            {
                foreach (var item in _roleMappingItems)
                {
                    if (!string.IsNullOrWhiteSpace(item.RoleName))
                    {
                        roleNames.Add(item.RoleName);
                    }
                }
            }
            return roleNames;
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
        /// 触发角色映射列表改变事件。
        /// </summary>
        protected void OnRoleMappingListChanged()
        {
            RoleMappingListChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 触发预览声音请求事件。
        /// </summary>
        /// <param name="sourceName">朗读者源名称。</param>
        protected void OnPreviewVoiceRequested(string sourceName)
        {
            PreviewVoiceRequested?.Invoke(this, new PreviewVoiceRequestEventArgs(sourceName));
        }

        #endregion

        #endregion

        #region private

        #region 字段

        /// <summary>
        /// 角色映射数据列表，用于绑定到角色映射表格。
        /// </summary>
        private BindingList<RoleMappingItem> _roleMappingItems = new BindingList<RoleMappingItem>();

        /// <summary>
        /// 缓存的角色名称列表。
        /// </summary>
        private List<string> _cachedRoleNames = new List<string>();

        /// <summary>
        /// 预览声音状态获取委托。
        /// </summary>
        private Func<string, PreviewVoiceStatus> _getPreviewVoiceStatus;

        #endregion

        #region 公共方法设置

        /// <summary>
        /// 设置预览声音状态获取委托。
        /// </summary>
        /// <param name="getPreviewVoiceStatus">获取预览声音状态的委托。</param>
        public void SetPreviewVoiceStatusGetter(Func<string, PreviewVoiceStatus> getPreviewVoiceStatus)
        {
            _getPreviewVoiceStatus = getPreviewVoiceStatus;
        }

        #endregion

        #region UI初始化

        /// <summary>
        /// 初始化角色映射表格UI。
        /// </summary>
        private void init角色映射Ui()
        {
            setup角色映射GridColumns();

            this.dgv_角色映射.CellClick -= dgv_角色映射_CellClick;
            this.dgv_角色映射.CellClick += dgv_角色映射_CellClick;

            this.dgv_角色映射.CellFormatting -= dgv_角色映射_CellFormatting;
            this.dgv_角色映射.CellFormatting += dgv_角色映射_CellFormatting;

            this.dgv_角色映射.DataError -= dgv_角色映射_DataError;
            this.dgv_角色映射.DataError += dgv_角色映射_DataError;

            this.dgv_角色映射.CurrentCellDirtyStateChanged -= dgv_角色映射_CurrentCellDirtyStateChanged;
            this.dgv_角色映射.CurrentCellDirtyStateChanged += dgv_角色映射_CurrentCellDirtyStateChanged;

            this.dgv_角色映射.MouseDown -= dgv_角色映射_MouseDown;
            this.dgv_角色映射.MouseDown += dgv_角色映射_MouseDown;

            _roleMappingItems.ListChanged -= _roleMappingItems_ListChanged;
            _roleMappingItems.ListChanged += _roleMappingItems_ListChanged;

            refresh角色映射Ui();
        }

        #endregion

        #region UI操作

        /// <summary>
        /// 刷新角色映射UI状态。
        /// </summary>
        private void refresh角色映射Ui()
        {
            refresh角色映射SourceNameOptions();
            OnRoleMappingListChanged();
        }

        /// <summary>
        /// 设置角色映射表格的列属性。
        /// </summary>
        private void setup角色映射GridColumns()
        {
            this.dgv_角色映射.AutoGenerateColumns = false;

            this.dgv_角色映射.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgv_角色映射.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
            this.dgv_角色映射.DefaultCellStyle.SelectionForeColor = Color.Black;

            this.dgv_角色映射.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            UiControlHelper.SetupDataGridViewBasicStyle(this.dgv_角色映射);

            this.dgv_角色映射.EditMode = DataGridViewEditMode.EditOnEnter;

            this.dgv_角色映射.Columns.Clear();

            DataGridViewTextBoxColumn roleNameColumn = new DataGridViewTextBoxColumn();
            roleNameColumn.Name = roleNameColumn.DataPropertyName = "RoleName";
            roleNameColumn.HeaderText = "角色名";
            roleNameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            this.dgv_角色映射.Columns.Add(roleNameColumn);

            DataGridViewComboBoxColumn sourceNameColumn = new DataGridViewComboBoxColumn();
            sourceNameColumn.Name = "SourceName";
            sourceNameColumn.DataPropertyName = "SourceName";
            sourceNameColumn.HeaderText = "源名称";
            sourceNameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            sourceNameColumn.FlatStyle = FlatStyle.Flat;
            this.dgv_角色映射.Columns.Add(sourceNameColumn);

            DataGridViewButtonColumn voicePreviewColumn = new DataGridViewButtonColumn();
            voicePreviewColumn.Name = "VoicePreview";
            voicePreviewColumn.HeaderText = "声音预览";
            voicePreviewColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            voicePreviewColumn.MinimumWidth = 150;
            voicePreviewColumn.FlatStyle = FlatStyle.Flat;
            voicePreviewColumn.UseColumnTextForButtonValue = false;
            this.dgv_角色映射.Columns.Add(voicePreviewColumn);

            typeof(DataGridView).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null,
                this.dgv_角色映射,
                new object[] { true });

            this.dgv_角色映射.DataError -= dgv_角色映射_DataError;
            this.dgv_角色映射.DataError += dgv_角色映射_DataError;

            this.dgv_角色映射.DataSource = _roleMappingItems;
        }

        /// <summary>
        /// 刷新角色映射表格的源名称下拉列表选项。
        /// </summary>
        private void refresh角色映射SourceNameOptions()
        {
            var sourceNameColumn = this.dgv_角色映射.Columns["SourceName"] as DataGridViewComboBoxColumn;
            if (sourceNameColumn == null)
            {
                return;
            }

            this.dgv_角色映射.SuspendLayout();
            try
            {
                sourceNameColumn.Items.Clear();

                HashSet<string> availableSourceNames = new HashSet<string>();
                var currentEngine = TtlSchemeController?.CurrentEngineConnector;
                if (currentEngine != null)
                {
                    var speakers = currentEngine.Speakers;
                    if (speakers != null && speakers.Length > 0)
                    {
                        foreach (var speaker in speakers)
                        {
                            sourceNameColumn.Items.Add(speaker.SourceName);
                            availableSourceNames.Add(speaker.SourceName);
                        }
                    }
                }

                foreach (var item in _roleMappingItems)
                {
                    if (string.IsNullOrWhiteSpace(item.SourceName))
                    {
                        item.IsLost = false;
                        continue;
                    }

                    string currentSourceName = item.SourceName;
                    bool hasLostSuffix = currentSourceName.EndsWith(已丢失_标识后缀);
                    string baseSourceName = hasLostSuffix
                        ? currentSourceName.Substring(0, currentSourceName.Length - 已丢失_标识后缀.Length)
                        : currentSourceName;

                    if (availableSourceNames.Contains(baseSourceName))
                    {
                        if (hasLostSuffix)
                        {
                            item.SourceName = baseSourceName;
                        }
                        item.IsLost = false;
                    }
                    else
                    {
                        item.IsLost = true;
                        string lostName = baseSourceName + 已丢失_标识后缀;
                        if (!hasLostSuffix)
                        {
                            item.SourceName = lostName;
                        }
                        if (!sourceNameColumn.Items.Contains(lostName))
                        {
                            sourceNameColumn.Items.Add(lostName);
                        }
                    }
                }

                this.dgv_角色映射.AutoResizeColumn(this.dgv_角色映射.Columns["SourceName"].Index, DataGridViewAutoSizeColumnMode.AllCells);
            }
            finally
            {
                this.dgv_角色映射.ResumeLayout();
            }
        }

        /// <summary>
        /// 加载角色映射数据。
        /// </summary>
        private void load角色映射Data()
        {
            load角色映射Data(CurrentEngineId);
        }

        /// <summary>
        /// 加载角色映射数据。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        private void load角色映射Data(string engineId)
        {
            _roleMappingItems.Clear();
            _cachedRoleNames.Clear();

            if (!string.IsNullOrEmpty(engineId) && ProjectFile != null)
            {
                var data = ProjectFile.LoadRoleMappingData(engineId);
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        _roleMappingItems.Add(item);
                    }
                }
            }

            refresh角色映射Ui();
        }

        /// <summary>
        /// 保存角色映射数据。
        /// </summary>
        private void save角色映射Data()
        {
            save角色映射Data(CurrentEngineId);
        }

        /// <summary>
        /// 保存角色映射数据。
        /// </summary>
        /// <param name="engineId">引擎ID。</param>
        private void save角色映射Data(string engineId)
        {
            if (!string.IsNullOrEmpty(engineId) && ProjectFile != null)
            {
                var filteredItems = _roleMappingItems
                    .Where(item => !string.IsNullOrWhiteSpace(item.RoleName))
                    .ToList();
                ProjectFile.SaveRoleMappingData(engineId, filteredItems);
            }
        }

        /// <summary>
        /// 删除角色映射表格中选中的行。
        /// </summary>
        private void deleteSelected角色映射Rows()
        {
            if (dgv_角色映射.SelectedRows.Count == 0)
            {
                return;
            }

            List<int> rowIndexes = new List<int>();
            foreach (DataGridViewRow row in dgv_角色映射.SelectedRows)
            {
                if (row.Index >= 0)
                {
                    rowIndexes.Add(row.Index);
                }
            }

            rowIndexes.Sort((a, b) => b.CompareTo(a));
            foreach (int index in rowIndexes)
            {
                if (index < _roleMappingItems.Count)
                {
                    _roleMappingItems.RemoveAt(index);
                }
            }

            dgv_角色映射.Refresh();
            OnRoleMappingListChanged();
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 事件处理：角色映射表格单元格点击，用于处理声音预览按钮的点击事件。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_角色映射_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var voicePreviewColumn = this.dgv_角色映射.Columns["VoicePreview"];
            if (voicePreviewColumn != null && e.RowIndex >= 0 && e.ColumnIndex == voicePreviewColumn.Index)
            {
                if (e.RowIndex < _roleMappingItems.Count)
                {
                    var item = _roleMappingItems[e.RowIndex];
                    if (item.IsLost)
                    {
                        return;
                    }

                    string sourceName = item.SourceName;
                    if (sourceName.EndsWith(已丢失_标识后缀))
                    {
                        sourceName = sourceName.Substring(0, sourceName.Length - 已丢失_标识后缀.Length);
                    }

                    if (string.IsNullOrWhiteSpace(sourceName))
                    {
                        return;
                    }

                    OnPreviewVoiceRequested(sourceName);
                }
            }
        }

        /// <summary>
        /// 事件处理：角色映射表格单元格格式化，用于动态设置按钮文本。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_角色映射_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var voicePreviewColumn = this.dgv_角色映射.Columns["VoicePreview"];
            if (voicePreviewColumn != null && e.ColumnIndex == voicePreviewColumn.Index && e.RowIndex >= 0)
            {
                if (e.RowIndex < _roleMappingItems.Count)
                {
                    var item = _roleMappingItems[e.RowIndex];

                    if (item.IsLost)
                    {
                        e.Value = "不可用";
                        e.FormattingApplied = true;
                        return;
                    }

                    string sourceName = item.SourceName;
                    if (sourceName.EndsWith(已丢失_标识后缀))
                    {
                        sourceName = sourceName.Substring(0, sourceName.Length - 已丢失_标识后缀.Length);
                    }

                    if (string.IsNullOrWhiteSpace(sourceName))
                    {
                        e.Value = "不可用";
                        e.FormattingApplied = true;
                        return;
                    }

                    PreviewVoiceStatus status = PreviewVoiceStatus.未生成;
                    if (_getPreviewVoiceStatus != null)
                    {
                        status = _getPreviewVoiceStatus(sourceName);
                    }

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
        }

        /// <summary>
        /// 事件处理：角色映射表格数据错误，用于处理下拉列表的数据错误。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_角色映射_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        /// <summary>
        /// 事件处理：角色映射表格单元格脏状态变化，用于在下拉列表选择后立即刷新预览按钮状态。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_角色映射_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.dgv_角色映射.IsCurrentCellDirty)
            {
                if (this.dgv_角色映射.CurrentCell is DataGridViewComboBoxCell)
                {
                    this.dgv_角色映射.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    this.dgv_角色映射.InvalidateRow(this.dgv_角色映射.CurrentCell.RowIndex);
                }
            }
        }

        /// <summary>
        /// 事件处理：角色映射表格鼠标按下，用于处理右键菜单。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void dgv_角色映射_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo hitTest = dgv_角色映射.HitTest(e.X, e.Y);
                if (hitTest.Type == DataGridViewHitTestType.RowHeader && hitTest.RowIndex >= 0)
                {
                    if (dgv_角色映射.Rows[hitTest.RowIndex].Selected)
                    {
                        ContextMenuStrip cms = new ContextMenuStrip();
                        ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem("删除");
                        deleteMenuItem.Click += (s, args) =>
                        {
                            if (UiControlHelper.ShowConfirmDialog($"确定要删除选中的 {dgv_角色映射.SelectedRows.Count} 行吗？", "确认删除"))
                            {
                                deleteSelected角色映射Rows();
                            }
                        };
                        cms.Items.Add(deleteMenuItem);
                        cms.Show(dgv_角色映射, e.Location);
                    }
                    else
                    {
                        dgv_角色映射.ClearSelection();
                        dgv_角色映射.Rows[hitTest.RowIndex].Selected = true;
                    }
                }
            }
        }

        /// <summary>
        /// 事件处理：角色映射列表内容改变。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="e">事件参数。</param>
        private void _roleMappingItems_ListChanged(object sender, ListChangedEventArgs e)
        {
            _cachedRoleNames.Clear();
            OnProjectModified();
            OnRoleMappingListChanged();
        }

        #endregion

        #endregion
    }

    #region 事件参数类

    /// <summary>
    /// 预览声音请求事件参数。
    /// </summary>
    public class PreviewVoiceRequestEventArgs : EventArgs
    {
        /// <summary>
        /// 获取朗读者源名称。
        /// </summary>
        public string SourceName { get; }

        /// <summary>
        /// 初始化PreviewVoiceRequestEventArgs类的新实例。
        /// </summary>
        /// <param name="sourceName">朗读者源名称。</param>
        public PreviewVoiceRequestEventArgs(string sourceName)
        {
            SourceName = sourceName;
        }
    }

    #endregion
}
