using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GW.TTLtoolsBox.Core.TtlEngine.Helper
{
    /// <summary>
    /// MD5相关操作的静态工具类。
    /// </summary>
    public static class MD5Helper
    {
        #region public

        /// <summary>
        /// 生成字符串的短MD5值（16位小写）
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <returns>16位小写的短MD5字符串，输入为null时返回空字符串</returns>
        public static string GetShortMd5(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            try
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                    byte[] hashBytes = md5Hash.ComputeHash(inputBytes);

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("x2"));
                    }
                    string fullMd5 = sb.ToString();

                    string shortMd5 = fullMd5.Substring(8, 16);

                    return shortMd5;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"生成MD5时出错: {ex.Message}");
                return string.Empty;
            }
        }

        #endregion
    }
}
