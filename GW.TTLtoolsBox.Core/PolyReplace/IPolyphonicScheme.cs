using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GW.TTLtoolsBox.Core.PolyReplace
{
    /// <summary>
    /// 多音字方案实体类的接口。
    /// </summary>
    public interface IPolyphonicScheme
    {
        /// <summary>
        /// 获取方案名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取方案范例（形如：张叁 → 张三、击中 → 击中[ZHONG4]）。
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 获取用于显示的名称+范例组合。
        /// </summary>
        string ShowName { get; }

        /// <summary>
        /// 获取或设置替换的字符串。
        /// </summary>
        string[] ReplaceStrings { get; set; }

        /// <summary>
        /// 根据给定的原文本自动生成替换字符串。
        /// </summary>
        /// <param name="originalText">给定的原文本。</param>
        void BuildDefaultReplaceStrings(string originalText);

        /// <summary>
        /// 尝试从字符串读取数据填充实例。
        /// </summary>
        /// <param name="str">字符串。</param>
        /// <returns>是否成功。</returns>
        bool TryLoadFromString(string str);

        /// <summary>
        /// 替换文本中的多音字。
        /// </summary>
        /// <param name="text">要替换的文本。</param>
        /// <returns>替换后的文本。</returns>
        string Replace(string text);

        /// <summary>
        /// 创建当前实例的深拷贝。
        /// </summary>
        /// <returns>新的实例副本。</returns>
        IPolyphonicScheme Clone();
    }
}
