using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GW.TTLtoolsBox.WinFormUi.UI
{
    /// <summary>
    /// 自动填充数据事件参数
    /// </summary>
    public class AutoFillDataEventArgs : EventArgs
    {
        /// <summary>
        /// 获取行索引
        /// </summary>
        public int RowIndex { get; }

        /// <summary>
        /// 获取列索引
        /// </summary>
        public int ColumnIndex { get; }

        /// <summary>
        /// 获取行数据
        /// </summary>
        public object RowData { get; }

        /// <summary>
        /// 获取或设置填充内容
        /// </summary>
        public string FillContent { get; set; } = string.Empty;

        /// <summary>
        /// 初始化AutoFillDataEventArgs类的新实例
        /// </summary>
        /// <param name="rowIndex">行索引</param>
        /// <param name="columnIndex">列索引</param>
        /// <param name="rowData">行数据</param>
        public AutoFillDataEventArgs(int rowIndex, int columnIndex, object rowData)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            RowData = rowData;
        }
    }

    /// <summary>
    /// 自定义多行文本编辑控件
    /// </summary>
    public class MultiLineTextBoxEditingControl : DataGridViewTextBoxEditingControl
    {
        #region private

        private HashSet<string> _disallowNewLineColumnNames = new HashSet<string>();
        private HashSet<int> _disallowNewLineColumnIndexes = new HashSet<int>();

        #endregion

        #region public

        /// <summary>
        /// 设置不允许换行的列
        /// </summary>
        /// <param name="columnNames">列名集合</param>
        /// <param name="columnIndexes">列索引集合</param>
        public void SetDisallowNewLineColumns(HashSet<string> columnNames, HashSet<int> columnIndexes)
        {
            _disallowNewLineColumnNames = columnNames ?? new HashSet<string>();
            _disallowNewLineColumnIndexes = columnIndexes ?? new HashSet<int>();
        }

        #endregion

        #region protected

        /// <summary>
        /// 判断当前列是否允许换行
        /// </summary>
        /// <returns>是否允许换行</returns>
        protected bool IsCurrentColumnAllowNewLine()
        {
            if (this.EditingControlDataGridView == null)
                return true;

            DataGridViewCell currentCell = this.EditingControlDataGridView.CurrentCell;
            if (currentCell == null)
                return true;

            int colIndex = currentCell.ColumnIndex;
            string colName = this.EditingControlDataGridView.Columns[colIndex]?.Name;

            return !_disallowNewLineColumnIndexes.Contains(colIndex) &&
                   !_disallowNewLineColumnNames.Contains(colName);
        }

        /// <summary>
        /// 重写EditingControlWantsInputKey方法
        /// </summary>
        /// <param name="keyData">按键数据</param>
        /// <param name="dataGridViewWantsInputKey">DataGridView是否需要输入键</param>
        /// <returns>是否需要输入键</returns>
        public override bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            if (IsCurrentColumnAllowNewLine() && (keyData & Keys.KeyCode) == Keys.Enter)
            {
                return true;
            }
            return base.EditingControlWantsInputKey(keyData, dataGridViewWantsInputKey);
        }

        /// <summary>
        /// 重写OnKeyDown方法
        /// </summary>
        /// <param name="e">按键事件参数</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (IsCurrentColumnAllowNewLine())
            {
                if (e.KeyCode == Keys.Enter && !e.Shift && !e.Control && !e.Alt)
                {
                    int pos = this.SelectionStart;
                    this.Text = this.Text.Insert(pos, Environment.NewLine);
                    this.SelectionStart = pos + Environment.NewLine.Length;
                    e.Handled = true;
                    e.SuppressKeyPress = true;

                    var dgv = this.EditingControlDataGridView as CustomNewLineDataGridView;
                    dgv?.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
                else if (e.KeyCode == Keys.Enter && e.Shift)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;

                    var dgv = this.EditingControlDataGridView as CustomNewLineDataGridView;
                    dgv?.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    dgv?.EndEdit();

                    if (dgv != null)
                    {
                        int currentRowIndex = dgv.CurrentCell.RowIndex;
                        int nextRowIndex = currentRowIndex + 1;
                        if (nextRowIndex < dgv.Rows.Count)
                        {
                            dgv.CurrentCell = dgv[dgv.CurrentCell.ColumnIndex, nextRowIndex];
                            dgv.BeginEdit(true);
                        }
                    }
                }
                else
                {
                    base.OnKeyDown(e);
                }
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        #endregion
    }

    /// <summary>
    /// 自定义单元格
    /// </summary>
    public class MultiLineDataGridViewTextBoxCell : DataGridViewTextBoxCell
    {
        /// <summary>
        /// 获取编辑控件类型
        /// </summary>
        public override Type EditType => typeof(MultiLineTextBoxEditingControl);

        /// <summary>
        /// 初始化MultiLineDataGridViewTextBoxCell类的新实例
        /// </summary>
        public MultiLineDataGridViewTextBoxCell()
        {
            this.Style.WrapMode = DataGridViewTriState.True;
        }
    }

    /// <summary>
    /// 自定义列
    /// </summary>
    public class MultiLineDataGridViewTextBoxColumn : DataGridViewTextBoxColumn
    {
        /// <summary>
        /// 初始化MultiLineDataGridViewTextBoxColumn类的新实例
        /// </summary>
        public MultiLineDataGridViewTextBoxColumn()
        {
            this.CellTemplate = new MultiLineDataGridViewTextBoxCell();
            this.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }
    }

    /// <summary>
    /// 自定义DataGridView，支持多行文本编辑
    /// </summary>
    public class CustomNewLineDataGridView : DataGridView
    {
        #region private

        private bool _isInitialized = false;

        #endregion

        #region public

        /// <summary>
        /// 不允许换行的列名集合
        /// </summary>
        public HashSet<string> DisallowNewLineColumnNames { get; set; } = new HashSet<string>();

        /// <summary>
        /// 不允许换行的列索引集合
        /// </summary>
        public HashSet<int> DisallowNewLineColumnIndexes { get; set; } = new HashSet<int>();

        /// <summary>
        /// 单元格激活编辑且内容为空时触发，请求自动填充数据
        /// </summary>
        public event EventHandler<AutoFillDataEventArgs> NeedAutoFillData;

        #endregion

        #region protected

        /// <summary>
        /// 触发NeedAutoFillData事件
        /// </summary>
        /// <param name="e">事件参数</param>
        protected virtual void OnNeedAutoFillData(AutoFillDataEventArgs e)
        {
            NeedAutoFillData?.Invoke(this, e);
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化CustomNewLineDataGridView类的新实例
        /// </summary>
        public CustomNewLineDataGridView()
        {
            this.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.DefaultCellStyle.SelectionBackColor = this.DefaultCellStyle.BackColor;
            this.DefaultCellStyle.SelectionForeColor = this.DefaultCellStyle.ForeColor;
            this.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            this.EditMode = DataGridViewEditMode.EditOnEnter;
            this.RowTemplate.MinimumHeight = 40;
            this.ScrollBars = ScrollBars.Both;
            this.RowHeadersVisible = false;
            this.DoubleBuffered = true;

            this.EditingControlShowing += (s, e) =>
            {
                if (e.Control is MultiLineTextBoxEditingControl editCtrl)
                {
                    editCtrl.SetDisallowNewLineColumns(DisallowNewLineColumnNames, DisallowNewLineColumnIndexes);
                }
            };

            this.EditingControlShowing += CustomNewLineDataGridView_EditingControlShowing;

            this.CellBeginEdit += (s, e) =>
            {
                if (!_isInitialized)
                {
                    _isInitialized = true;
                }
            };

            this.CellClick += (s, e) =>
            {
                if (!_isInitialized && e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    _isInitialized = true;
                }
            };

            this.ColumnAdded += (s, e) =>
            {
                if (e.Column is DataGridViewTextBoxColumn && !(e.Column is MultiLineDataGridViewTextBoxColumn))
                {
                    int index = this.Columns.IndexOf(e.Column);
                    this.Columns.Remove(e.Column);
                    var newCol = new MultiLineDataGridViewTextBoxColumn
                    {
                        Name = e.Column.Name,
                        HeaderText = e.Column.HeaderText,
                        DataPropertyName = e.Column.DataPropertyName,
                        Width = e.Column.Width
                    };
                    this.Columns.Insert(index, newCol);
                }
            };
        }

        #endregion

        #region private

        private void CustomNewLineDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.CurrentCell == null)
                return;

            int rowIndex = this.CurrentCell.RowIndex;
            int colIndex = this.CurrentCell.ColumnIndex;

            if (rowIndex < 0 || colIndex < 0)
                return;

            var cell = this[colIndex, rowIndex];

            if (!IsCellContentEmpty(cell))
                return;

            var rowData = this.Rows[rowIndex].DataBoundItem;
            var args = new AutoFillDataEventArgs(rowIndex, colIndex, rowData);

            OnNeedAutoFillData(args);

            if (!string.IsNullOrEmpty(args.FillContent))
            {
                if (e.Control is TextBox txt)
                {
                    txt.Text = args.FillContent;
                    txt.SelectionStart = txt.Text.Length;

                    this.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    this.Rows[rowIndex].Height = GetAutoRowHeight(colIndex, args.FillContent);
                    this.Refresh();
                    this.PerformLayout();
                    this.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                }
            }
        }

        private int GetAutoRowHeight(int colIndex, string content)
        {
            Size textSize = TextRenderer.MeasureText(
                content,
                this.Font,
                new Size(this.Columns[colIndex].Width, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl
            );

            return Math.Max(textSize.Height + 10, this.RowTemplate.MinimumHeight);
        }

        private bool IsCellContentEmpty(DataGridViewCell cell)
        {
            if (cell.Value == null || cell.Value == DBNull.Value || string.IsNullOrEmpty(cell.Value.ToString()))
            {
                return true;
            }

            string content = cell.Value.ToString()
                .Replace(" ", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\t", "");
            return string.IsNullOrEmpty(content);
        }

        #endregion
    }
}
