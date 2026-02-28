using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GW.TTLtoolsBox.Core.PolyReplace
{
    /// <summary>
    /// 多音字方案实体类的抽象类。
    /// </summary>
    public abstract class APolyphonicScheme : IPolyphonicScheme
    {
        #region 常量

        /// <summary>
        /// 字符串分隔符。
        /// </summary>
        private const string _to_String_Split = "***";

        #endregion

        #region public

        #region IPolyphonicScheme 实现

        /// <summary>
        /// 获取方案名称。
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 获取方案范例（形如：张叁 → 张三、击中 → 击中[ZHONG4]）。
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 获取或设置替换的字符串。
        /// </summary>
        public string[] ReplaceStrings
        {
            get => _replaceStrings;
            set => _replaceStrings = value == null ? Array.Empty<string>() : value;
        }

        /// <summary>
        /// 获取用于显示的名称+范例组合。
        /// </summary>
        public string ShowName => $"{Name} [{Description}]";

        /// <summary>
        /// 根据给定的原文本自动生成替换字符串。
        /// </summary>
        /// <param name="originalText">给定的原文本。</param>
        public abstract void BuildDefaultReplaceStrings(string originalText);

        /// <summary>
        /// 尝试从字符串读取数据填充实例。
        /// </summary>
        /// <param name="str">字符串。</param>
        /// <returns>是否成功。</returns>
        public bool TryLoadFromString(string str)
        {
            bool outValue = false;

            if (str != null)
            {
                List<string> strList = new List<string>();
                bool isFirst = true;
                foreach (var rs in str.Split(new string[] { _to_String_Split }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (string.IsNullOrWhiteSpace(rs) == false)
                    {
                        if (isFirst == true)
                        {
                            if (rs.Equals(Name) == true) isFirst = false;
                            else break;
                        }
                        else
                        {
                            strList.Add(rs);
                        }
                    }
                }

                if (isFirst == false)
                {
                    ReplaceStrings = strList.ToArray();
                    outValue = true;
                }
            }

            return outValue;
        }

        #endregion

        #region override

        /// <summary>
        /// （已重写）将实例输出为字符串。
        /// </summary>
        /// <returns>字符串。</returns>
        public override string ToString()
        {
            string outValue = $" {_to_String_Split}{Name}";
            foreach (var str in ReplaceStrings) outValue += $"{_to_String_Split}{(str == string.Empty ? " " : str)}";

            return outValue;
        }

        /// <summary>
        /// 替换文本中的多音字。
        /// </summary>
        /// <param name="text">要替换的文本。</param>
        /// <returns>替换后的文本。</returns>
        public virtual string Replace(string text)
        {
            if (string.IsNullOrEmpty(text) || ReplaceStrings == null || ReplaceStrings.Length == 0)
            {
                return text;
            }

            string result = text;
            foreach (var replaceString in ReplaceStrings)
            {
                if (string.IsNullOrEmpty(replaceString))
                {
                    continue;
                }

                string originalWord = ExtractOriginalWord(replaceString);
                if (!string.IsNullOrEmpty(originalWord) && text.Contains(originalWord))
                {
                    result = result.Replace(originalWord, replaceString);
                }
            }

            return result;
        }

        /// <summary>
        /// 从替换字符串中提取原词。
        /// 子类可重写此方法以支持不同的格式。
        /// </summary>
        /// <param name="replaceString">替换字符串。</param>
        /// <returns>原词。</returns>
        protected virtual string ExtractOriginalWord(string replaceString)
        {
            if (string.IsNullOrEmpty(replaceString))
            {
                return string.Empty;
            }

            int spaceIndex = replaceString.IndexOf(' ');
            if (spaceIndex > 0)
            {
                return replaceString.Substring(0, spaceIndex).Trim();
            }

            return replaceString;
        }

        /// <summary>
        /// 创建当前实例的深拷贝。
        /// </summary>
        /// <returns>新的实例副本。</returns>
        public abstract IPolyphonicScheme Clone();

        /// <summary>
        /// 将当前实例的数据复制到目标实例。
        /// </summary>
        /// <param name="target">目标实例。</param>
        protected void CopyTo(APolyphonicScheme target)
        {
            if (target == null) return;
            target._replaceStrings = (string[])_replaceStrings?.Clone() ?? Array.Empty<string>();
        }

        #endregion

        #endregion

        #region private

        #region 属性访问器

        private string[] _replaceStrings = Array.Empty<string>();

        #endregion

        #endregion
    }
}
