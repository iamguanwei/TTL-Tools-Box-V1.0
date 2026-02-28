using GW.TTLtoolsBox.Core.PolyReplace.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GW.TTLtoolsBox.Core.PolyReplace
{
    /// <summary>
    /// 采用音调拼音标注的多音字方案的实体类。
    /// </summary>
    public class TonePinyinPolyphonicScheme : APolyphonicScheme
    {
        #region public

        #region APolyphonicScheme 实现

        /// <summary>
        /// 获取方案名称。
        /// </summary>
        public override string Name => "音调拼音标注";

        /// <summary>
        /// 获取方案范例（形如：张叁 → 张三、击中 → 击中[ZHONG4]）。
        /// </summary>
        public override string Description => "击中 → 击 [zh][òng] ";

        /// <summary>
        /// 根据给定的原文本自动生成替换字符串。
        /// </summary>
        /// <param name="originalText">给定的原文本。</param>
        public override void BuildDefaultReplaceStrings(string originalText)
        {
            if (string.IsNullOrWhiteSpace(originalText) == false)
            {
                List<string> replaceStrList = base.ReplaceStrings.ToList();

                List<List<string>> charPinyinList = new List<List<string>>();
                foreach (char ch in originalText)
                {
                    string[] pinyinArray = PinyinHelper.GetAllPinyinWithToneMark(ch);
                    if (pinyinArray == null || pinyinArray.Length == 0)
                    {
                        charPinyinList.Add(new List<string> { ch.ToString() });
                    }
                    else
                    {
                        var distinctPinyins = pinyinArray.Distinct().ToList();
                        if (distinctPinyins.Count == 1)
                        {
                            charPinyinList.Add(new List<string> { ch.ToString() });
                        }
                        else
                        {
                            charPinyinList.Add(distinctPinyins);
                        }
                    }
                }

                List<string> allCombinations = generatePinyinCombinations(charPinyinList, originalText);

                replaceStrList.AddRange(allCombinations);

                base.ReplaceStrings = replaceStrList.Distinct().ToArray();
            }
        }

        /// <summary>
        /// 创建当前实例的深拷贝。
        /// </summary>
        /// <returns>新的实例副本。</returns>
        public override IPolyphonicScheme Clone()
        {
            TonePinyinPolyphonicScheme clone = new TonePinyinPolyphonicScheme();
            CopyTo(clone);
            return clone;
        }

        #endregion

        #endregion

        #region private

        #region 辅助方法

        /// <summary>
        /// 生成拼音的全排列组合（笛卡尔积）
        /// </summary>
        /// <param name="charPinyinList">每个字的读音列表</param>
        /// <param name="originalText">原始文本</param>
        /// <returns>所有组合的字符串（如重[CHONG2]重[CHONG2]）</returns>
        private List<string> generatePinyinCombinations(List<List<string>> charPinyinList, string originalText)
        {
            List<string> combinations = new List<string>();

            // 递归生成笛卡尔积
            generateCombinationsRecursive(charPinyinList, 0, "", originalText, combinations);

            return combinations;
        }

        /// <summary>
        /// 递归核心方法：生成所有拼音组合
        /// </summary>
        /// <param name="charPinyinList">每个字的读音列表</param>
        /// <param name="index">当前处理的字索引</param>
        /// <param name="currentCombination">当前已拼接的组合</param>
        /// <param name="originalText">原始文本</param>
        /// <param name="result">最终结果列表</param>
        private void generateCombinationsRecursive(List<List<string>> charPinyinList, int index, string currentCombination, string originalText, List<string> result)
        {
            // 递归终止条件：处理完所有字
            if (index >= charPinyinList.Count)
            {
                if (!string.IsNullOrEmpty(currentCombination))
                {
                    result.Add(currentCombination);
                }
                return;
            }

            // 获取当前字的所有读音
            List<string> pinyins = charPinyinList[index];
            char currentChar = originalText[index];

            // 遍历当前字的每个读音，拼接组合
            foreach (string pinyin in pinyins)
            {
                string newCombination;
                if (pinyin == currentChar.ToString())
                {
                    // 非中文字符：直接拼接原字符（无拼音标注）
                    newCombination = currentCombination + currentChar;
                }
                else
                {
                    // 中文字符：拼接 [声母][韵母] 格式
                    var py = PinyinHelper.SplitPinyin(pinyin);
                    newCombination = currentCombination + $" [{py.Shengmu}][{py.Yunmu}] ";
                }

                // 递归处理下一个字
                generateCombinationsRecursive(charPinyinList, index + 1, newCombination, originalText, result);
            }
        }

        #endregion

        #endregion
    }
}