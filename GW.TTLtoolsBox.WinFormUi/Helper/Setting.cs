using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using GW.TTLtoolsBox.Core.Entity;
using GW.TTLtoolsBox.Core.FileAccesser;

namespace GW.TTLtoolsBox.WinFormUi.Helper
{
    /// <summary>
    /// 配置文件的静态工具类，继承自IniFileHelper。
    /// </summary>
    public static class Setting
    {
        #region 常量

        /// <summary>
        /// 配置文件名称（含路径）。
        /// </summary>
        public static string 设置_文件_全名 = $@"{AppDomain.CurrentDomain.BaseDirectory}\Setting.ini";

        #endregion

        #region private

        /// <summary>
        /// 内部使用的IniFileAccesser实例。
        /// </summary>
        private static IniFileAccesser _iniFileAccesser = null;

        /// <summary>
        /// 获取IniFileHelper实例（延迟初始化）。
        /// </summary>
        private static IniFileAccesser getIniFileHelper()
        {
            if (_iniFileAccesser == null)
            {
                _iniFileAccesser = new IniFileAccesser(设置_文件_全名);
            }
            return _iniFileAccesser;
        }

        #endregion

        #region public

        #region 属性

        /// <summary>
        /// 获取或设置一个值，表示是否在SetValue()之后立刻保存，而不需要调用Save()方法。
        /// </summary>
        public static bool IsAutoSave
        {
            get { return getIniFileHelper().IsAutoSave; }
            set { getIniFileHelper().IsAutoSave = value; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 保存配置。
        /// </summary>
        public static void Save()
        {
            getIniFileHelper().Save();
        }

        /// <summary>
        /// 获取指定名称的配置值。
        /// </summary>
        /// <param name="name">指定配置的名称。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>配置值。</returns>
        public static string GetValue(string name, string defaultValue)
        {
            return getIniFileHelper().GetValue(name, defaultValue);
        }

        /// <summary>
        /// 设置指定名称的配置值。
        /// </summary>
        /// <param name="name">指定配置的名称。</param>
        /// <param name="value">配置值。</param>
        public static void SetValue(string name, string value)
        {
            getIniFileHelper().SetValue(name, value);
        }

        /// <summary>
        /// 设置指定名称的配置值。
        /// </summary>
        /// <param name="name">指定配置的名称。</param>
        /// <param name="value">配置值。</param>
        public static void SetValue(string name, object value)
        {
            getIniFileHelper().SetValue(name, value);
        }

        /// <summary>
        /// 检查指定名称的配置是否存在。
        /// </summary>
        /// <param name="name">指定配置的名称。</param>
        /// <returns>是否存在</returns>
        public static bool ContainsKey(string name)
        {
            return getIniFileHelper().ContainsKey(name);
        }

        /// <summary>
        /// 删除指定名称的配置。
        /// </summary>
        /// <param name="name">指定配置的名称。</param>
        public static void Remove(string name)
        {
            getIniFileHelper().Remove(name);
        }

        /// <summary>
        /// 清空所有配置。
        /// </summary>
        public static void Clear()
        {
            getIniFileHelper().Clear();
        }

        /// <summary>
        /// 获取所有配置键名。
        /// </summary>
        /// <returns>所有配置键名的数组。</returns>
        public static string[] GetAllKeys()
        {
            return getIniFileHelper().GetAllKeys();
        }

        /// <summary>
        /// 保存语音生成任务列表到配置文件。
        /// </summary>
        /// <param name="engineId">TTL引擎ID。</param>
        /// <param name="tasks">任务列表。</param>
        public static void SaveVoiceGenerationTasks(string engineId, List<VoiceGenerationTask> tasks)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return;
            }

            try
            {
                string key = $"VoiceGenerationTasks_{engineId}";
                if (tasks == null || tasks.Count == 0)
                {
                    Remove(key);
                    return;
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<VoiceGenerationTask>));
                    serializer.WriteObject(ms, tasks);
                    string json = Encoding.UTF8.GetString(ms.ToArray());
                    SetValue(key, json);
                }

                if (IsAutoSave)
                {
                    Save();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 从配置文件加载语音生成任务列表。
        /// </summary>
        /// <param name="engineId">TTL引擎ID。</param>
        /// <returns>任务列表。</returns>
        public static List<VoiceGenerationTask> LoadVoiceGenerationTasks(string engineId)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return new List<VoiceGenerationTask>();
            }

            try
            {
                string key = $"VoiceGenerationTasks_{engineId}";
                string json = GetValue(key, string.Empty);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<VoiceGenerationTask>();
                }

                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<VoiceGenerationTask>));
                    List<VoiceGenerationTask> tasks = serializer.ReadObject(ms) as List<VoiceGenerationTask>;
                    return tasks ?? new List<VoiceGenerationTask>();
                }
            }
            catch
            {
                return new List<VoiceGenerationTask>();
            }
        }

        /// <summary>
        /// 获取文本拆分的拆分长度。
        /// </summary>
        /// <param name="engineId">TTL引擎ID。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>拆分长度。</returns>
        public static int GetTextSplit_SplitLength(string engineId, int defaultValue = 100)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return defaultValue;
            }

            string key = $"TextSplit_SplitLength_{engineId}";
            string value = GetValue(key, defaultValue.ToString());
            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        /// <summary>
        /// 设置文本拆分的拆分长度。
        /// </summary>
        /// <param name="engineId">TTL引擎ID。</param>
        /// <param name="value">拆分长度。</param>
        public static void SetTextSplit_SplitLength(string engineId, int value)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return;
            }

            string key = $"TextSplit_SplitLength_{engineId}";
            SetValue(key, value);
            Save();
        }

        /// <summary>
        /// 获取文本拆分的忽略换行符设置。
        /// </summary>
        /// <param name="engineId">TTL引擎ID。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>是否忽略换行符。</returns>
        public static bool GetTextSplit_IgnoreLineBreaks(string engineId, bool defaultValue = true)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return defaultValue;
            }

            string key = $"TextSplit_IgnoreLineBreaks_{engineId}";
            string value = GetValue(key, defaultValue.ToString());
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        /// <summary>
        /// 设置文本拆分的忽略换行符设置。
        /// </summary>
        /// <param name="engineId">TTL引擎ID。</param>
        /// <param name="value">是否忽略换行符。</param>
        public static void SetTextSplit_IgnoreLineBreaks(string engineId, bool value)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return;
            }

            string key = $"TextSplit_IgnoreLineBreaks_{engineId}";
            SetValue(key, value);
            Save();
        }

        /// <summary>
        /// 获取文本拆分的拆分方式（true=按句子拆分，false=按对话拆分）。
        /// </summary>
        /// <param name="engineId">TTL引擎ID。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>是否按句子拆分。</returns>
        public static bool GetTextSplit_SplitBySentence(string engineId, bool defaultValue = true)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return defaultValue;
            }

            string key = $"TextSplit_SplitBySentence_{engineId}";
            string value = GetValue(key, defaultValue.ToString());
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        /// <summary>
        /// 设置文本拆分的拆分方式（true=按句子拆分，false=按对话拆分）。
        /// </summary>
        /// <param name="engineId">TTL引擎ID。</param>
        /// <param name="value">是否按句子拆分。</param>
        public static void SetTextSplit_SplitBySentence(string engineId, bool value)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return;
            }

            string key = $"TextSplit_SplitBySentence_{engineId}";
            SetValue(key, value);
            Save();
        }

        #endregion

        #endregion
    }
}
