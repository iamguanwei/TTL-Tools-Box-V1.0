using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GW.TTLtoolsBox.WinFormUi.Helper
{
    /// <summary>
    /// 提供DataGridView辅助操作的静态工具类。
    /// </summary>
    internal static class DataGridViewHelper
    {
        #region public

        #region 行操作

        /// <summary>
        /// 判断选中的行索引是否连续。
        /// </summary>
        /// <param name="indices">行索引列表</param>
        /// <returns>如果连续返回true，否则返回false</returns>
        public static bool IsConsecutiveRows(List<int> indices)
        {
            if (indices == null || indices.Count <= 1)
            {
                return true;
            }

            for (int i = 1; i < indices.Count; i++)
            {
                if (indices[i] != indices[i - 1] + 1)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 将选中的行索引分组，连续的行作为一组。
        /// </summary>
        /// <param name="indices">行索引列表</param>
        /// <returns>分组后的行索引列表</returns>
        public static List<List<int>> GroupConsecutiveRows(List<int> indices)
        {
            List<List<int>> groups = new List<List<int>>();

            if (indices == null || indices.Count == 0)
            {
                return groups;
            }

            List<int> currentGroup = new List<int> { indices[0] };

            for (int i = 1; i < indices.Count; i++)
            {
                if (indices[i] == indices[i - 1] + 1)
                {
                    currentGroup.Add(indices[i]);
                }
                else
                {
                    groups.Add(currentGroup);
                    currentGroup = new List<int> { indices[i] };
                }
            }

            groups.Add(currentGroup);
            return groups;
        }

        /// <summary>
        /// 刷新DataGridView并恢复选中状态。
        /// </summary>
        /// <typeparam name="T">数据项类型</typeparam>
        /// <param name="dataGridView">目标DataGridView</param>
        /// <param name="dataSource">数据源</param>
        /// <param name="selectedItems">要选中的数据项集合</param>
        public static void RefreshAndRestoreSelection<T>(DataGridView dataGridView, object dataSource, HashSet<T> selectedItems) where T : class
        {
            if (dataGridView == null) return;

            dataGridView.SuspendLayout();

            try
            {
                dataGridView.DataSource = null;
                dataGridView.DataSource = dataSource;

                dataGridView.ClearSelection();

                if (selectedItems != null)
                {
                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        if (row.DataBoundItem is T item && selectedItems.Contains(item))
                        {
                            row.Selected = true;
                        }
                    }
                }
            }
            finally
            {
                dataGridView.ResumeLayout();
            }
        }

        #endregion

        #endregion
    }
}
