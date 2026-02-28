using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GW.TTLtoolsBox.Core.PolyReplace
{
    /// <summary>
    /// 替换项实体类。
    /// </summary>
    public class ReplaceItem
    {
        #region 常量

        /// <summary>
        /// 上下文的最大长度。
        /// </summary>
        private const int _context_Max_Length = 15;

        /// <summary>
        /// 不替换的文本。
        /// </summary>
        private const string _none_Select_Text = "[不替换]";

        /// <summary>
        /// 表示更多省略文本。
        /// </summary>
        private const string _more_Ellipsis_Text = "……";

        #endregion

        #region public

        #region 属性

        /// <summary>
        /// 获取原始文本。
        /// </summary>
        public string OriginalText => UsedPolyphonicItem?.OriginalText ?? string.Empty;

        /// <summary>
        /// 获取替换文本。
        /// </summary>
        public string ReplaceText => getSelectedReplaceString() ?? TargetText;

        /// <summary>
        /// 获取一个值，表示当前实例是否执行过UpdateReplaceText()，即是否指定过替换文本。
        /// </summary>
        public bool IsReplaceTextUpdated { get; private set; } = false;

        /// <summary>
        /// 获取备选文本。
        /// </summary>
        public string[] AlternativeTexts
        {
            get
            {
                List<string> outValue = new List<string>();

                outValue.Add(_none_Select_Text);
                
                var ps = getSelectedPolyphonicScheme();
                if (ps != null) outValue.AddRange(ps.ReplaceStrings);

                return outValue.ToArray();
            }
        }

        /// <summary>
        /// 获取替换方案名称。
        /// </summary>
        public string SchemeName
        {
            get
            {
                if (UsedPolyphonicItem == null || _selectedSchemeIndex < 0)
                {
                    return string.Empty;
                }

                var schemes = UsedPolyphonicItem.PolyphonicSchemes;
                if (_selectedSchemeIndex >= schemes.Length)
                {
                    return string.Empty;
                }

                return schemes[_selectedSchemeIndex].Name;
            }
        }

        /// <summary>
        /// 获取多音字在目标文本中的位置索引。
        /// </summary>
        public int PositionIndex { get; private set; } = -1;

        /// <summary>
        /// 获取使用的多音字替换项。
        /// </summary>
        public PolyphonicItem UsedPolyphonicItem { get; private set; } = null;

        /// <summary>
        /// 获取上文文本。
        /// </summary>
        public string PreContextText { get; private set; } = string.Empty;

        /// <summary>
        /// 获取待替换文本。
        /// </summary>
        public string TargetText { get; private set; } = string.Empty;

        /// <summary>
        /// 获取下文文本。
        /// </summary>
        public string PostContextText { get; private set; } = string.Empty;

        #endregion

        #region 方法

        /// <summary>
        /// 更新替换的文本。
        /// </summary>
        /// <param name="newReplaceText">新的替换文本。</param>
        public void UpdateReplaceText(string newReplaceText)
        {
            if (newReplaceText != null)
            {
                var ps = getSelectedPolyphonicScheme();
                var index = Array.FindIndex(ps.ReplaceStrings, s => s.Equals(newReplaceText) == true);
                if (index >= 0) _selectedReplaceStringIndex = index;
                else _selectedReplaceStringIndex = -1;

                IsReplaceTextUpdated = true;
            }
        }

        #endregion

        #region 静态方法

        /// <summary>
        /// 根据目标文本和多音字替换项生成ReplaceItem数组。
        /// </summary>
        /// <param name="targetText">目标文本。</param>
        /// <param name="polyphonicItems">多音字替换项数组。</param>
        /// <param name="currentSchemeName">当前使用的多音字方案名称。</param>
        /// <returns>生成的ReplaceItem数组。</returns>
        public static ReplaceItem[] GenerateFromText(
            string targetText,
            PolyphonicItem[] polyphonicItems,
            string currentSchemeName)
        {
            if (targetText == null) throw new ArgumentNullException(nameof(targetText));
            if (polyphonicItems == null || polyphonicItems.Length == 0) throw new ArgumentNullException(nameof(polyphonicItems));
            if (currentSchemeName == null) throw new ArgumentNullException(nameof(currentSchemeName));

            List <ReplaceItem> replaceItems = new List<ReplaceItem>();

            // 获取多音字替换方案索引
            int psIndex = -1;
            {
                PolyphonicItem pi = new PolyphonicItem();
                psIndex = Array.FindIndex(pi.PolyphonicSchemes, p => p.Name.Equals(currentSchemeName) == true);
                if (psIndex < 0) throw new Exception($"无法找到名称为 {currentSchemeName} 的多音字方案");
            }

            // 生成替换项
            {
                foreach (var pi in polyphonicItems)
                {
                    if (string.IsNullOrWhiteSpace(pi.OriginalText)) continue;

                    int startIndex = 0;
                    while (startIndex < targetText.Length)
                    {
                        int positionIndex = targetText.IndexOf(pi.OriginalText, startIndex);
                        if (positionIndex == -1) break;

                        // 创建ReplaceItem对象
                        ReplaceItem replaceItem = new ReplaceItem()
                        {
                            UsedPolyphonicItem = pi,
                            PositionIndex = positionIndex,
                            _selectedSchemeIndex = psIndex,
                        };

                        // 提取上文、中文、下文
                        int preContextStart = Math.Max(0, positionIndex - _context_Max_Length);
                        int preContextEnd = positionIndex;
                        int targetEnd = positionIndex + pi.OriginalText.Length;
                        int postContextStart = targetEnd;
                        int postContextEnd = Math.Min(targetText.Length, targetEnd + _context_Max_Length);

                        // 处理上文
                        string preContext = targetText.Substring(preContextStart, preContextEnd - preContextStart);
                        // 检查上文是否有换行符
                        int preNewlineIndex = preContext.LastIndexOfAny(new char[] { '\n', '\r' });
                        if (preNewlineIndex >= 0)
                        {
                            preContext = preContext.Substring(preNewlineIndex + 1);
                        }
                        // 检查上文长度是否超过上下文长度
                        if (preContextStart > 0 && preContext.Length == _context_Max_Length)
                        {
                            preContext = _more_Ellipsis_Text + preContext;
                        }
                        replaceItem.PreContextText = preContext;

                        // 处理目标文本
                        replaceItem.TargetText = targetText.Substring(positionIndex, targetEnd - positionIndex);

                        // 处理下文
                        string postContext = targetText.Substring(postContextStart, postContextEnd - postContextStart);
                        // 检查下文是否有换行符
                        int postNewlineIndex = postContext.IndexOfAny(new char[] { '\n', '\r' });
                        if (postNewlineIndex >= 0)
                        {
                            postContext = postContext.Substring(0, postNewlineIndex);
                        }
                        // 检查下文长度是否超过上下文长度
                        if (postContextEnd < targetText.Length && postContext.Length == _context_Max_Length)
                        {
                            postContext = postContext + _more_Ellipsis_Text;
                        }
                        replaceItem.PostContextText = postContext;

                        // 添加到列表
                        replaceItems.Add(replaceItem);

                        // 继续查找下一个匹配位置
                        startIndex = positionIndex + pi.OriginalText.Length;
                    }
                }
            }

            return replaceItems.ToArray();
        }

        /// <summary>
        /// 执行替换操作，将ReplaceItem数组中的替换应用到目标文本。
        /// </summary>
        /// <param name="targetText">目标文本。</param>
        /// <param name="replaceItems">替换项数组。</param>
        /// <returns>替换后的文本。</returns>
        public static string ApplyReplacements(string targetText, ReplaceItem[] replaceItems)
        {
            if (string.IsNullOrEmpty(targetText)) return targetText;
            if (replaceItems == null || replaceItems.Length == 0) return targetText;

            // 按位置倒序排序，避免替换影响后续位置
            var sortedItems = replaceItems.OrderByDescending(item => item.PositionIndex).ToArray();
            StringBuilder sb = new StringBuilder(targetText);

            foreach (var item in sortedItems)
            {
                string replaceText = item.ReplaceText;
                if (replaceText != item.TargetText) // 只有当替换文本与原文本不同时才替换
                {
                    sb.Remove(item.PositionIndex, item.TargetText.Length);
                    sb.Insert(item.PositionIndex, replaceText);
                }
            }

            return sb.ToString();
        }

        #endregion

        #endregion

        #region private

        #region 构造函数

        /// <summary>
        /// 构造函数。
        /// </summary>
        private ReplaceItem()
        {
            // 无代码
        }

        #endregion

        #region 替换操作

        /// <summary>
        /// 选中的方案索引。
        /// </summary>
        private int _selectedSchemeIndex = -1;

        /// <summary>
        /// 选中的替换字符串索引。
        /// </summary>
        private int _selectedReplaceStringIndex = -1;

        /// <summary>
        /// 获取当前选中的多音字方案。
        /// </summary>
        /// <returns>当前选中的多音字方案，如果没有选中则返回null。</returns>
        private IPolyphonicScheme getSelectedPolyphonicScheme()
        {
            IPolyphonicScheme outValue = null;

            if (UsedPolyphonicItem != null &&
                _selectedSchemeIndex >= 0 &&
                _selectedSchemeIndex < UsedPolyphonicItem.PolyphonicSchemes.Length)
            {
                outValue = UsedPolyphonicItem.PolyphonicSchemes[_selectedSchemeIndex];
            }

            return outValue;
        }

        /// <summary>
        /// 获取当前选中的替换字符串。
        /// </summary>
        /// <returns>选中的替换字符串，如果没有选中则返回null。</returns>
        private string getSelectedReplaceString()
        {
            string outValue = null;

            var ps = getSelectedPolyphonicScheme();
            if (ps != null &&
                _selectedReplaceStringIndex >= 0 &&
                _selectedReplaceStringIndex < ps.ReplaceStrings.Length)
            {
                outValue = ps.ReplaceStrings[_selectedReplaceStringIndex];
            }

            return outValue;
        }

        #endregion

        #endregion
    }
}