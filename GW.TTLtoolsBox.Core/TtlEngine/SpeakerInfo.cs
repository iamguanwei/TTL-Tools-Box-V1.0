using System;

namespace GW.TTLtoolsBox.Core.TtlEngine
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
        /// 获取或设置朗读速度（百分比，默认100）。
        /// </summary>
        public int Speed { get; set; } = 100;

        /// <summary>
        /// 获取或设置音量（百分比，默认100）。
        /// </summary>
        public int Volume { get; set; } = 100;

        /// <summary>
        /// 获取或设置备注信息。
        /// </summary>
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// 初始化朗读者信息类的新实例。
        /// </summary>
        /// <param name="sourceName">源名称</param>
        public SpeakerInfo(string sourceName)
        {
            SourceName = sourceName;
            VoiceSamplePath = string.Empty;
            Speed = 100;
            Volume = 100;
            Remark = string.Empty;
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
            Speed = 100;
            Volume = 100;
            Remark = string.Empty;
        }

        /// <summary>
        /// 初始化朗读者信息类的新实例。
        /// </summary>
        /// <param name="sourceName">源名称</param>
        /// <param name="voiceSamplePath">声音样本存储位置</param>
        /// <param name="speed">朗读速度</param>
        public SpeakerInfo(string sourceName, string voiceSamplePath, int speed)
        {
            SourceName = sourceName;
            VoiceSamplePath = voiceSamplePath ?? string.Empty;
            Speed = speed;
            Volume = 100;
            Remark = string.Empty;
        }

        /// <summary>
        /// 初始化朗读者信息类的新实例。
        /// </summary>
        /// <param name="sourceName">源名称</param>
        /// <param name="voiceSamplePath">声音样本存储位置</param>
        /// <param name="speed">朗读速度</param>
        /// <param name="volume">音量</param>
        public SpeakerInfo(string sourceName, string voiceSamplePath, int speed, int volume)
        {
            SourceName = sourceName;
            VoiceSamplePath = voiceSamplePath ?? string.Empty;
            Speed = speed;
            Volume = volume;
            Remark = string.Empty;
        }

        /// <summary>
        /// 初始化朗读者信息类的新实例。
        /// </summary>
        /// <param name="sourceName">源名称</param>
        /// <param name="voiceSamplePath">声音样本存储位置</param>
        /// <param name="speed">朗读速度</param>
        /// <param name="volume">音量</param>
        /// <param name="remark">备注信息</param>
        public SpeakerInfo(string sourceName, string voiceSamplePath, int speed, int volume, string remark)
        {
            SourceName = sourceName;
            VoiceSamplePath = voiceSamplePath ?? string.Empty;
            Speed = speed;
            Volume = volume;
            Remark = remark ?? string.Empty;
        }

        /// <summary>
        /// 将类输出成字符串，用于序列化存储。
        /// </summary>
        /// <returns>序列化字符串</returns>
        public override string ToString()
        {
            string escapedRemark = Remark.Replace("|", "&#124;");
            return $"{SourceName}|{VoiceSamplePath}|{Speed}|{Volume}|{escapedRemark}";
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
                if (string.IsNullOrEmpty(data))
                {
                    return false;
                }

                var parts = data.Split('|');
                if (parts.Length < 2)
                {
                    return false;
                }

                string sourceNameFromData = parts[0];
                if (sourceNameFromData != SourceName)
                {
                    return false;
                }

                VoiceSamplePath = parts[1];

                if (parts.Length >= 3 && int.TryParse(parts[2], out int speed))
                {
                    Speed = speed;
                }
                else
                {
                    Speed = 100;
                }

                if (parts.Length >= 4 && int.TryParse(parts[3], out int volume))
                {
                    Volume = volume;
                }
                else
                {
                    Volume = 100;
                }

                if (parts.Length >= 5)
                {
                    Remark = parts[4].Replace("&#124;", "|");
                }
                else
                {
                    Remark = string.Empty;
                }

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
            return new SpeakerInfo(SourceName, VoiceSamplePath, Speed, Volume, Remark);
        }
    }
}
