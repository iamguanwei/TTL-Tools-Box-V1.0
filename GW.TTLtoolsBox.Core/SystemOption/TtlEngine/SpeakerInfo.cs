using System;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine
{
    /// <summary>
    /// 朗读者信息类，用于存储TTL引擎的朗读者基本信息。
    /// </summary>
    public class SpeakerInfo
    {
        /// <summary>
        /// 源名称（从TTL引擎获取，只读）。
        /// </summary>
        public string SourceName { get; }

        /// <summary>
        /// 声音样本存储位置（当前exe文件夹下的某个文件夹下，如果没有表示尚未创建）。
        /// </summary>
        public string VoiceSamplePath { get; set; }

        /// <summary>
        /// 初始化朗读者信息类的新实例。
        /// </summary>
        /// <param name="sourceName">源名称</param>
        public SpeakerInfo(string sourceName)
        {
            SourceName = sourceName;
            VoiceSamplePath = string.Empty;
        }

        /// <summary>
        /// 初始化朗读者信息类的新实例。
        /// </summary>
        /// <param name="sourceName">源名称</param>
        /// <param name="voiceSamplePath">声音样本存储位置</param>
        public SpeakerInfo(string sourceName, string voiceSamplePath)
        {
            SourceName = sourceName;
            VoiceSamplePath = voiceSamplePath ?? string.Empty;
        }

        /// <summary>
        /// 将类输出成字符串，仅返回源名称用于显示。
        /// </summary>
        /// <returns>源名称</returns>
        public override string ToString()
        {
            return SourceName;
        }

        /// <summary>
        /// 尝试从字符串还原属性。
        /// </summary>
        /// <param name="data">要还原的字符串</param>
        /// <returns>是否还原成功</returns>
        public bool TryFromString(string data)
        {
            try
            {
                var parts = data.Split('|');
                if (parts.Length != 2)
                {
                    return false;
                }

                string sourceNameFromData = parts[0];
                if (sourceNameFromData != SourceName)
                {
                    return false;
                }

                VoiceSamplePath = parts[1];

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 创建当前对象的浅表克隆体。
        /// </summary>
        /// <returns>当前对象的浅表克隆体</returns>
        public SpeakerInfo Clone()
        {
            return new SpeakerInfo(SourceName, VoiceSamplePath);
        }
    }
}
