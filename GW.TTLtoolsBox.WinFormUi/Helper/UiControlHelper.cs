using System;
using System.Drawing;
using System.Windows.Forms;

namespace GW.TTLtoolsBox.WinFormUi.Helper
{
    /// <summary>
    /// 提供UI控件辅助操作的静态工具类。
    /// </summary>
    internal static class UiControlHelper
    {
        #region public

        #region 剪贴板操作

        /// <summary>
        /// 复制给定文本到剪贴板。
        /// </summary>
        /// <param name="text">要复制的文本</param>
        /// <param name="showSuccessMessage">是否显示成功消息</param>
        /// <returns>是否成功复制</returns>
        public static bool CopyToClipboard(string text, bool showSuccessMessage = true)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("没有内容可复制", "提示", MessageBoxButtons.OK);
                return false;
            }

            try
            {
                Clipboard.SetText(text);
                if (showSuccessMessage)
                {
                    MessageBox.Show("最终文本已经复制到剪贴板", "提示", MessageBoxButtons.OK);
                }
                return true;
            }
            catch
            {
                MessageBox.Show("剪贴板目前无法访问，请稍后再试", "错误", MessageBoxButtons.OK);
                return false;
            }
        }

        #endregion

        #region DataGridView操作

        /// <summary>
        /// 配置DataGridView的基本样式。
        /// </summary>
        /// <param name="grid">要配置的表格</param>
        /// <param name="fontSize">字体大小，默认10.5</param>
        public static void SetupDataGridViewBasicStyle(DataGridView grid, float fontSize = 10.5f)
        {
            grid.DefaultCellStyle.Font = new Font("微软雅黑", fontSize);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", fontSize, FontStyle.Bold);
            grid.RowTemplate.Height = 28;
            grid.ColumnHeadersHeight = 28;

            grid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        }

        #endregion

        #region 文本框操作

        /// <summary>
        /// 在文本框的光标位置插入分隔符。
        /// </summary>
        /// <param name="textBox">文本框控件</param>
        /// <param name="separator">要插入的分隔符</param>
        public static void InsertSeparatorAtCursor(TextBox textBox, string separator)
        {
            if (textBox == null || string.IsNullOrEmpty(separator))
            {
                return;
            }

            int selectionIndex = textBox.SelectionStart;
            int scrollPosition = textBox.GetScrollPosition();
            textBox.Text = textBox.Text.Insert(selectionIndex, separator);
            textBox.SelectionStart = selectionIndex + separator.Length;
            textBox.SetScrollPosition(scrollPosition);
            textBox.Focus();
        }

        #endregion

        #region 对话框操作

        /// <summary>
        /// 显示确认对话框。
        /// </summary>
        /// <param name="message">确认消息</param>
        /// <param name="title">对话框标题</param>
        /// <returns>用户是否确认</returns>
        public static bool ShowConfirmDialog(string message, string title = "确认")
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo) == DialogResult.Yes;
        }

        #endregion

        #endregion
    }
}
