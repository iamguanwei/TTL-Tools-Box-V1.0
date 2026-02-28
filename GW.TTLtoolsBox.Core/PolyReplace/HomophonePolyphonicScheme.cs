using GW.TTLtoolsBox.Core.PolyReplace.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GW.TTLtoolsBox.Core.PolyReplace
{
    /// <summary>
    /// 采用同音字替换的多音字方案的实体类。
    /// </summary>
    public class HomophonePolyphonicScheme : APolyphonicScheme
    {
        #region public

        #region APolyphonicScheme 实现

        /// <summary>
        /// 获取方案名称。
        /// </summary>
        public override string Name => "同音字替换";

        /// <summary>
        /// 获取方案范例（形如：张叁 → 张三、击中 → 击中[ZHONG4]）。
        /// </summary>
        public override string Description => "张叁 → 章三";

        /// <summary>
        /// 根据给定的原文本自动生成替换字符串（直接使用硬编码的常用同音字映射）。
        /// </summary>
        /// <param name="originalText">给定的原文本。</param>
        public override void BuildDefaultReplaceStrings(string originalText)
        {
            if (string.IsNullOrWhiteSpace(originalText))
                return;

            // 1. 初始化替换列表（保留原有内容）
            List<string> replaceStrList = base.ReplaceStrings?.ToList() ?? new List<string>();

            // 2. 解析每个字的常用同音非多音字（直接使用硬编码映射）
            List<List<string>> charHomophoneList = new List<List<string>>();
            foreach (char ch in originalText)
            {
                // 获取当前字的所有带数字声调的拼音
                string[] pinyins = getPinyinWithToneNumber(ch);
                if (pinyins == null || pinyins.Length == 0)
                {
                    // 非中文字符/无拼音：保留原字
                    charHomophoneList.Add(new List<string> { ch.ToString() });
                    continue;
                }

                // 为每个读音直接取硬编码的最常用字
                List<string> homophones = new List<string>();
                foreach (string pinyin in pinyins)
                {
                    string mostCommonChar = getMostCommonCharByPinyin(pinyin);
                    if (!string.IsNullOrEmpty(mostCommonChar))
                    {
                        homophones.Add(mostCommonChar);
                    }
                }

                // 无匹配的同音字则保留原字
                charHomophoneList.Add(homophones.Count > 0 ? homophones.Distinct().ToList() : new List<string> { ch.ToString() });
            }

            // 3. 生成所有同音词组合（笛卡尔积）
            List<string> allCombinations = generateHomophoneCombinations(charHomophoneList);

            // 4. 添加组合到替换列表（去重）
            replaceStrList.AddRange(allCombinations);
            base.ReplaceStrings = replaceStrList.Distinct().ToArray();
        }

        /// <summary>
        /// 创建当前实例的深拷贝。
        /// </summary>
        /// <returns>新的实例副本。</returns>
        public override IPolyphonicScheme Clone()
        {
            HomophonePolyphonicScheme clone = new HomophonePolyphonicScheme();
            CopyTo(clone);
            return clone;
        }

        #endregion

        #endregion

        #region private 核心辅助方法

        // 核心字典：每个拼音仅对应1个最常见、非多音字、非生僻字（按你的修改调整）
        private static readonly Dictionary<string, string> _highFrequencyCharMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // 基础数字
            { "yi1", "一" },
            { "er4", "二" },
            { "san1", "三" },
            { "si4", "四" },
            { "wu3", "五" },
            { "liu4", "六" },
            { "qi1", "七" },
            { "ba1", "八" },
            { "jiu3", "九" },
            { "shi2", "十" },

            // 核心常用拼音（已按你的修改调整：di4=弟、cheng1=撑）
            { "chong2", "虫" },
            { "zhong4", "众" },
            { "zhong1", "钟" },
            { "zhang1", "章" },
            { "zhang3", "掌" },
            { "chang2", "常" },
            { "xing2", "星" },
            { "hang2", "航" },
            { "le4", "勒" },
            { "yue4", "月" },
            { "ji1", "机" },
            { "ji4", "计" },
            { "he2", "河" },
            { "he4", "贺" },
            { "li3", "里" },
            { "li4", "力" },
            { "di4", "弟" },
            { "ti2", "提" },
            { "ta1", "他" },
            { "duo1", "多" },
            { "guo4", "过" },
            { "huo3", "火" },
            { "ge1", "哥" },
            { "ke1", "科" },

            // 扩展常用拼音
            { "tian1", "天" },
            { "tian2", "田" },
            { "ren2", "人" },
            { "ming2", "明" },
            { "hong2", "红" },
            { "huang2", "黄" },
            { "shui3", "水" },
            { "shan1", "山" },
            { "feng1", "风" },
            { "feng4", "凤" },
            { "guang1", "光" },
            { "nian2", "年" },
            { "nian4", "念" },
            { "dong1", "东" },
            { "dong4", "动" },
            { "nan2", "南" },
            { "bei3", "北" },
            { "xi1", "西" },
            { "xi4", "细" },
            { "jin1", "金" },
            { "jin4", "进" },
            { "qian2", "前" },
            { "hou4", "后" },
            { "zuo3", "左" },
            { "you4", "右" },
            { "shang4", "上" },
            { "xia4", "下" },
            { "da4", "大" },
            { "xiao3", "小" },
            { "gao1", "高" },
            { "di1", "低" },
            { "kuai4", "快" },
            { "man4", "慢" },
            { "xin1", "心" },
            { "xin4", "信" },
            { "sheng1", "生" },
            { "sheng4", "胜" },
            { "cheng2", "成" },
            { "cheng1", "撑" }
        };

        /// <summary>
        /// 获取单个字符的所有带数字声调的拼音
        /// </summary>
        /// <param name="ch">目标字符</param>
        /// <returns>拼音数组（如重 → ["chong2", "zhong4"]）</returns>
        private string[] getPinyinWithToneNumber(char ch)
        {
            try
            {
                // 核心API：获取带数字声调的拼音（小写统一格式）
                return PinyinHelper.GetAllPinyinWithToneMark(ch)?
                    .Select(p => p.ToLower().Trim())
                    .ToArray() ?? Array.Empty<string>();
            }
            catch
            {
                // 非中文字符/无拼音返回空
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// 直接从硬编码字典获取指定拼音对应的最常用字（移除多音字检查）
        /// </summary>
        /// <param name="targetPinyin">带数字声调的目标拼音</param>
        /// <returns>硬编码的最常用字</returns>
        private string getMostCommonCharByPinyin(string targetPinyin)
        {
            if (string.IsNullOrWhiteSpace(targetPinyin))
                return string.Empty;

            // 直接返回硬编码字典的映射结果，不做任何多音字验证
            _highFrequencyCharMap.TryGetValue(targetPinyin, out string result);
            return result ?? string.Empty;
        }

        /// <summary>
        /// 生成同音词的全排列组合（笛卡尔积）
        /// </summary>
        /// <param name="charHomophoneList">每个字的同音字列表（每个拼音仅1个）</param>
        /// <returns>所有组合结果</returns>
        private List<string> generateHomophoneCombinations(List<List<string>> charHomophoneList)
        {
            List<string> combinations = new List<string>();
            generateCombinationsRecursive(charHomophoneList, 0, "", combinations);
            return combinations;
        }

        /// <summary>
        /// 递归生成组合的核心方法
        /// </summary>
        private void generateCombinationsRecursive(List<List<string>> charHomophoneList, int index, string currentCombination, List<string> result)
        {
            if (index >= charHomophoneList.Count)
            {
                if (!string.IsNullOrEmpty(currentCombination))
                {
                    result.Add(currentCombination);
                }
                return;
            }

            foreach (string homophone in charHomophoneList[index])
            {
                generateCombinationsRecursive(charHomophoneList, index + 1, currentCombination + homophone, result);
            }
        }

        #endregion
    }
}