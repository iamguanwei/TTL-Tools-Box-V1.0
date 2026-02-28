using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GW.TTLtoolsBox.Core.SystemOption.Helper
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
            // 处理空输入
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            try
            {
                // 创建MD5加密服务实例
                using (MD5 md5Hash = MD5.Create())
                {
                    // 将字符串转换为UTF8编码的字节数组
                    byte[] inputBytes = Encoding.UTF8.GetBytes(input);

                    // 计算MD5哈希值
                    byte[] hashBytes = md5Hash.ComputeHash(inputBytes);

                    // 将哈希字节数组转换为32位十六进制字符串
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("x2")); // x2表示两位小写十六进制
                    }
                    string fullMd5 = sb.ToString();

                    // 截取中间16位作为短MD5（也可以根据需求截取前16位或后16位）
                    string shortMd5 = fullMd5.Substring(8, 16);

                    return shortMd5;
                }
            }
            catch (Exception ex)
            {
                // 异常处理，实际项目中可根据需要记录日志
                Console.WriteLine($"生成MD5时出错: {ex.Message}");
                return string.Empty;
            }
        }

        #endregion
    }
}
