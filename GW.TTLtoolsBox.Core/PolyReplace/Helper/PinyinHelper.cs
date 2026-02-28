using Pinyin4net;
using Pinyin4net.Format;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GW.TTLtoolsBox.Core.PolyReplace.Helper
{
    /// <summary>
    /// 为拼音操作提供服务的静态工具类。
    /// </summary>
    public static class PinyinHelper
    {
        #region 常量

        /// <summary>
        /// 定义所有拼音声母（按拼音规则）。
        /// </summary>
        private static readonly HashSet<string> _shengmu_Set = new HashSet<string>
        {
            "b", "p", "m", "f", "d", "t", "n", "l",
            "g", "k", "h", "j", "q", "x", "zh", "ch",
            "sh", "r", "z", "c", "s"
        };

        #endregion

        #region public

        /// <summary>
        /// 获取单个字符的所有带数字声调的拼音（适配hyjiacan.py4n库）
        /// </summary>
        /// <param name="ch">单个字符</param>
        /// <returns>拼音数组</returns>
        public static string[] GetAllPinyinWithToneMark(char ch)
        {
            try
            {
                HanyuPinyinOutputFormat format = new HanyuPinyinOutputFormat()
                {
                    CaseType = HanyuPinyinCaseType.LOWERCASE,
                    ToneType = HanyuPinyinToneType.WITH_TONE_MARK,
                    VCharType = HanyuPinyinVCharType.WITH_U_UNICODE,
                };
                var pinyinArray = Pinyin4net.PinyinHelper.ToHanyuPinyinStringArray(ch, format);
                if (pinyinArray == null)
                {
                    return Array.Empty<string>();
                }

                return pinyinArray;
            }
            catch
            {
                // 非中文字符返回空数组
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// 获取单个字符的所有带数字声调的拼音
        /// </summary>
        /// <param name="ch">单个字符</param>
        /// <returns>拼音数组，格式如chong2、zhong4，轻声无数字，ü替换为u</returns>
        public static string[] GetAllPinyinWithToneNumber(char ch)
        {
            try
            {
                HanyuPinyinOutputFormat format = new HanyuPinyinOutputFormat()
                {
                    CaseType = HanyuPinyinCaseType.LOWERCASE,
                    ToneType = HanyuPinyinToneType.WITH_TONE_NUMBER,
                    VCharType = HanyuPinyinVCharType.WITH_U_UNICODE,
                };
                var pinyinArray = Pinyin4net.PinyinHelper.ToHanyuPinyinStringArray(ch, format);
                if (pinyinArray == null)
                {
                    return Array.Empty<string>();
                }

                for (int i = 0; i < pinyinArray.Length; i++)
                {
                    //pinyinArray[i] = pinyinArray[i].Replace("ü", "u");
                    pinyinArray[i] = pinyinArray[i].Replace("ü", "v");
                }

                return pinyinArray;
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// 拆分完整拼音字符串为【声母+韵母】
        /// </summary>
        /// <param name="fullPinyin">带声调的完整拼音（如chǔ、nǚ、yú）</param>
        /// <returns>Tuple(声母, 韵母)</returns>
        public static (string Shengmu, string Yunmu) SplitPinyin(string fullPinyin)
        {
            if (string.IsNullOrEmpty(fullPinyin))
            {
                return ("", "");
            }

            // 2. 优先匹配双声母（zh/ch/sh）
            if (fullPinyin.Length >= 2)
            {
                string twoChar = fullPinyin.Substring(0, 2);
                if (_shengmu_Set.Contains(twoChar))
                {
                    string yunmu = fullPinyin.Substring(2);
                    return (twoChar, yunmu);
                }
            }

            // 3. 匹配单声母
            string oneChar = fullPinyin.Substring(0, 1);
            if (_shengmu_Set.Contains(oneChar))
            {
                string yunmu = fullPinyin.Substring(1);
                return (oneChar, yunmu);
            }

            // 4. 无声母（单韵母拼音）
            return ("", fullPinyin);
        }

        #endregion
    }
}
