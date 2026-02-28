using System;
using System.IO;
using System.Windows.Forms;

namespace GW.TTLtoolsBox.WinFormUi.Helper
{
    /// <summary>
    /// 提供文件和通用辅助操作的静态工具类。
    /// </summary>
    internal static class FileHelper
    {
        #region public

        #region 文件操作

        /// <summary>
        /// 确保指定文件夹存在，如果不存在则自动创建（支持多层父文件夹）。
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <returns>是否成功创建或文件夹已存在</returns>
        public static bool EnsureFolderExists(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建文件夹失败：{folderPath}\n错误信息：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion

        #region 数字处理

        /// <summary>
        /// 将给定数字扩充到指定位数，前面补零。
        /// </summary>
        /// <param name="number">要扩充的数字</param>
        /// <param name="length">目标位数</param>
        /// <returns>扩充后的字符串</returns>
        /// <example>
        /// PadNumber(1, 2) 返回 "01"
        /// PadNumber(1, 3) 返回 "001"
        /// PadNumber(25, 3) 返回 "025"
        /// </example>
        public static string PadNumber(int number, int length)
        {
            return number.ToString().PadLeft(length, '0');
        }

        /// <summary>
        /// 判断给定字符串的末尾是否为数字，如果是则分割为前缀和数字两部分。
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <returns>
        /// 如果末尾是数字，返回包含两个元素的数组：[前缀, 数字]；
        /// 如果末尾不是数字，返回包含一个元素的数组：[原字符串]
        /// </returns>
        /// <example>
        /// SplitTrailingNumber("file123") 返回 ["file", "123"]
        /// SplitTrailingNumber("test01") 返回 ["test", "01"]
        /// SplitTrailingNumber("abc") 返回 ["abc"]
        /// SplitTrailingNumber("123") 返回 ["", "123"]
        /// </example>
        public static string[] SplitTrailingNumber(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new string[] { input };
            }

            int i = input.Length - 1;
            while (i >= 0 && char.IsDigit(input[i]))
            {
                i--;
            }

            if (i == input.Length - 1)
            {
                return new string[] { input };
            }

            string prefix = input.Substring(0, i + 1);
            string number = input.Substring(i + 1);

            return new string[] { prefix, number };
        }

        #endregion

        #endregion
    }
}
