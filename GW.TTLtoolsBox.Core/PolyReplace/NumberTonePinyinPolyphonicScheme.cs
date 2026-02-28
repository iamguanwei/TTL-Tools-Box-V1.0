using GW.TTLtoolsBox.Core.PolyReplace.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW.TTLtoolsBox.Core.PolyReplace
{
    /// <summary>
    /// 采用数字声调拼音标注的多音字方案的实体类。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 将多音字转换为数字声调拼音格式
    /// - 输出格式为方括号括起来的拼音，如[chong4 chong4]
    /// - 音调采用后续数字标识，轻声不需要数字
    /// 
    /// 使用场景：
    /// - 用于Index TTL V2等需要数字声调拼音标注的TTL引擎
    /// 
    /// 依赖关系：
    /// - 继承自APolyphonicScheme抽象类
    /// - 依赖PinyinHelper进行拼音转换
    /// </remarks>
    public class NumberTonePinyinPolyphonicScheme : APolyphonicScheme
    {
        #region public

        #region APolyphonicScheme 实现

        /// <summary>
        /// 获取方案名称。
        /// </summary>
        public override string Name => "数字声调拼音标注";

        /// <summary>
        /// 获取方案范例。
        /// </summary>
        public override string Description => "重重 → [zhong4 zhong4]";

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
                    string[] pinyinArray = PinyinHelper.GetAllPinyinWithToneNumber(ch);
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
            NumberTonePinyinPolyphonicScheme clone = new NumberTonePinyinPolyphonicScheme();
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
        /// <returns>所有组合的字符串</returns>
        private List<string> generatePinyinCombinations(List<List<string>> charPinyinList, string originalText)
        {
            List<string> combinations = new List<string>();

            generateCombinationsRecursive(charPinyinList, 0, "", originalText, new List<string>(), combinations);

            return combinations;
        }

        /// <summary>
        /// 递归核心方法：生成所有拼音组合
        /// </summary>
        /// <param name="charPinyinList">每个字的读音列表</param>
        /// <param name="index">当前处理的字索引</param>
        /// <param name="currentCombination">当前已拼接的组合</param>
        /// <param name="originalText">原始文本</param>
        /// <param name="currentPinyins">当前收集的拼音列表</param>
        /// <param name="result">最终结果列表</param>
        private void generateCombinationsRecursive(
            List<List<string>> charPinyinList,
            int index,
            string currentCombination,
            string originalText,
            List<string> currentPinyins,
            List<string> result)
        {
            if (index >= charPinyinList.Count)
            {
                if (!string.IsNullOrEmpty(currentCombination))
                {
                    result.Add(currentCombination);
                }
                return;
            }

            List<string> pinyins = charPinyinList[index];
            char currentChar = originalText[index];

            foreach (string pinyin in pinyins)
            {
                string newCombination;
                List<string> newPinyins = new List<string>(currentPinyins);

                if (pinyin == currentChar.ToString())
                {
                    if (currentPinyins.Count > 0)
                    {
                        string pinyinStr = string.Join(" ", currentPinyins);
                        newCombination = currentCombination + $"[{pinyinStr}]{currentChar}";
                        newPinyins.Clear();
                    }
                    else
                    {
                        newCombination = currentCombination + currentChar;
                    }
                }
                else
                {
                    newPinyins.Add(pinyin);
                    newCombination = currentCombination;
                }

                if (index == charPinyinList.Count - 1 && newPinyins.Count > 0)
                {
                    string pinyinStr = string.Join(" ", newPinyins);
                    newCombination = newCombination + $"[{pinyinStr}]";
                    result.Add(newCombination);
                }
                else
                {
                    generateCombinationsRecursive(charPinyinList, index + 1, newCombination, originalText, newPinyins, result);
                }
            }
        }

        #endregion

        #endregion
    }
}
