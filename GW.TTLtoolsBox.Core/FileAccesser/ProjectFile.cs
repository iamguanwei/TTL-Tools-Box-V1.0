using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using GW.TTLtoolsBox.Core.Entity;

namespace GW.TTLtoolsBox.Core.FileAccesser
{
    /// <summary>
    /// 项目文件管理类，用于保存和加载项目相关的文本内容，支持JSON格式存储和多引擎数据管理。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 管理项目相关的文本内容存储
    /// - 支持多引擎数据的独立存储
    /// - 使用JSON格式进行数据持久化
    /// - 保持与旧版INI格式的向后兼容性
    /// 
    /// 使用场景：
    /// - 保存和加载项目文件
    /// - 管理不同TTL引擎的专属数据
    /// 
    /// 依赖关系：
    /// - 依赖System.Runtime.Serialization进行JSON序列化
    /// - 依赖GW.TTLtoolsBox.Core.Entity中的数据实体类
    /// </remarks>
    public class ProjectFile
    {
        #region 常量

        /// <summary>
        /// 项目文件默认名称。
        /// </summary>
        public const string Default_Project_File_Name = "_默认项目_文件.json";

        /// <summary>
        /// 旧版项目文件默认名称（用于向后兼容）。
        /// </summary>
        private const string _legacy_Project_File_Name = "_默认项目_文件.ini";

        /// <summary>
        /// 回车系统标识（用于旧格式兼容）。
        /// </summary>
        private const string _carriage_Return_Marker = "#回车#";

        /// <summary>
        /// 存盘分隔符（用于旧格式兼容）。
        /// </summary>
        private const string _save_Split_Marker = "#SAVESPLIT#";

        /// <summary>
        /// 段落分隔符（用于旧格式兼容）。
        /// </summary>
        private const string _paragraph_Split_Marker = "#PARAGRAPH#";

        #endregion

        #region public

        #region 属性

        /// <summary>
        /// 获取或设置文本拆分的原始文本。
        /// </summary>
        public string TextSplit_OriginalText
        {
            get { return _projectData.TextSplit_OriginalText; }
            set { _projectData.TextSplit_OriginalText = value ?? string.Empty; }
        }

        /// <summary>
        /// 获取或设置文本拆分的最终文本。
        /// </summary>
        public string TextSplit_FinalText
        {
            get { return _projectData.TextSplit_FinalText; }
            set { _projectData.TextSplit_FinalText = value ?? string.Empty; }
        }

        /// <summary>
        /// 获取保存的文件名。
        /// </summary>
        public string FileName { get; private set; } = string.Empty;

        /// <summary>
        /// 获取或设置一个值，表示是否在保存数据后立刻写入文件。
        /// </summary>
        public bool IsAutoSave { get; set; } = false;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化ProjectFile类的新实例，使用指定的文件路径。
        /// </summary>
        /// <param name="fileFullName">项目文件完整路径</param>
        public ProjectFile(string fileFullName)
        {
            if (string.IsNullOrWhiteSpace(fileFullName))
            {
                throw new ArgumentException("文件路径不能为空", nameof(fileFullName));
            }

            FileName = fileFullName;
            Load();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 获取默认项目文件路径。
        /// </summary>
        /// <returns>默认项目文件完整路径</returns>
        public static string GetDefaultFilePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Default_Project_File_Name);
        }

        /// <summary>
        /// 获取所有引擎ID列表。
        /// </summary>
        /// <returns>所有引擎ID的列表</returns>
        public List<string> GetAllEngineIds()
        {
            if (_projectData?.EngineDataDic == null)
            {
                return new List<string>();
            }
            return _projectData.EngineDataDic.Keys.ToList();
        }

        /// <summary>
        /// 获取多音字替换的最终文本。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <returns>多音字替换的最终文本</returns>
        public string GetPolyReplace_FinalText(string engineId)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return string.Empty;
            }

            if (_projectData.EngineDataDic.TryGetValue(engineId, out EngineProjectData engineData))
            {
                return engineData.PolyReplace_FinalText;
            }

            return string.Empty;
        }

        /// <summary>
        /// 设置多音字替换的最终文本。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <param name="text">多音字替换的最终文本</param>
        public void SetPolyReplace_FinalText(string engineId, string text)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return;
            }

            EnsureEngineData(engineId);
            _projectData.EngineDataDic[engineId].PolyReplace_FinalText = text ?? string.Empty;

            if (IsAutoSave)
            {
                Save();
            }
        }

        /// <summary>
        /// 获取语音生成预处理的最终文本。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <returns>语音生成预处理的最终文本</returns>
        public string GetVoicePreprocess_FinalText(string engineId)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return string.Empty;
            }

            if (_projectData.EngineDataDic.TryGetValue(engineId, out EngineProjectData engineData))
            {
                return engineData.VoicePreprocess_FinalText;
            }

            return string.Empty;
        }

        /// <summary>
        /// 设置语音生成预处理的最终文本。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <param name="text">语音生成预处理的最终文本</param>
        public void SetVoicePreprocess_FinalText(string engineId, string text)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return;
            }

            EnsureEngineData(engineId);
            _projectData.EngineDataDic[engineId].VoicePreprocess_FinalText = text ?? string.Empty;

            if (IsAutoSave)
            {
                Save();
            }
        }

        /// <summary>
        /// 保存角色和情绪指定表格的所有段落数据。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <param name="allParagraphsData">所有段落的数据列表</param>
        public void SaveRoleEmotionData(string engineId, List<List<RoleEmotionItem>> allParagraphsData)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return;
            }

            EnsureEngineData(engineId);
            _projectData.EngineDataDic[engineId].RoleEmotionData = allParagraphsData ?? new List<List<RoleEmotionItem>>();

            if (IsAutoSave)
            {
                Save();
            }
        }

        /// <summary>
        /// 加载角色和情绪指定表格的所有段落数据。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <returns>所有段落的数据列表</returns>
        public List<List<RoleEmotionItem>> LoadRoleEmotionData(string engineId)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return new List<List<RoleEmotionItem>>();
            }

            if (_projectData.EngineDataDic.TryGetValue(engineId, out EngineProjectData engineData))
            {
                return engineData.RoleEmotionData ?? new List<List<RoleEmotionItem>>();
            }

            return new List<List<RoleEmotionItem>>();
        }

        /// <summary>
        /// 保存语音生成任务清单。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <param name="tasks">任务列表</param>
        public void SaveVoiceGenerationTasks(string engineId, List<VoiceGenerationTask> tasks)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return;
            }

            EnsureEngineData(engineId);
            _projectData.EngineDataDic[engineId].VoiceGenerationTasks = tasks ?? new List<VoiceGenerationTask>();

            if (IsAutoSave)
            {
                Save();
            }
        }

        /// <summary>
        /// 加载语音生成任务清单。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <returns>任务列表</returns>
        public List<VoiceGenerationTask> LoadVoiceGenerationTasks(string engineId)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return new List<VoiceGenerationTask>();
            }

            if (_projectData.EngineDataDic.TryGetValue(engineId, out EngineProjectData engineData))
            {
                return engineData.VoiceGenerationTasks ?? new List<VoiceGenerationTask>();
            }

            return new List<VoiceGenerationTask>();
        }

        /// <summary>
        /// 保存角色映射数据。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <param name="roleMappingData">角色映射数据列表</param>
        public void SaveRoleMappingData(string engineId, List<RoleMappingItem> roleMappingData)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return;
            }

            EnsureEngineData(engineId);
            _projectData.EngineDataDic[engineId].RoleMappingData = roleMappingData ?? new List<RoleMappingItem>();

            if (IsAutoSave)
            {
                Save();
            }
        }

        /// <summary>
        /// 加载角色映射数据。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        /// <returns>角色映射数据列表</returns>
        public List<RoleMappingItem> LoadRoleMappingData(string engineId)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return new List<RoleMappingItem>();
            }

            if (_projectData.EngineDataDic.TryGetValue(engineId, out EngineProjectData engineData))
            {
                return engineData.RoleMappingData ?? new List<RoleMappingItem>();
            }

            return new List<RoleMappingItem>();
        }

        /// <summary>
        /// 保存项目数据到文件。
        /// </summary>
        public void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(FileName);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                _projectData.Version = ProjectData.Current_Version;

                using (FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ProjectData));
                    serializer.WriteObject(fs, _projectData);
                }
            }
            catch
            {
                // 错误不处理
            }
        }

        /// <summary>
        /// 清空所有项目数据（保留项目结构）。
        /// </summary>
        public void ClearAllData()
        {
            _projectData = new ProjectData();
        }

        /// <summary>
        /// 从文件加载项目数据。
        /// </summary>
        public void Load()
        {
            _projectData = new ProjectData();

            // 尝试加载JSON格式文件
            if (File.Exists(FileName))
            {
                try
                {
                    using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ProjectData));
                        ProjectData loadedData = serializer.ReadObject(fs) as ProjectData;
                        if (loadedData != null)
                        {
                            _projectData = loadedData;
                            return;
                        }
                    }
                }
                catch
                {
                    // JSON加载失败，尝试加载旧版INI格式
                }
            }

            // 尝试加载旧版INI格式文件
            string oldFilePath = Path.Combine(Path.GetDirectoryName(FileName), _legacy_Project_File_Name);
            if (File.Exists(oldFilePath))
            {
                try
                {
                    LoadFromIniFormat(oldFilePath);
                }
                catch
                {
                    // 旧版加载失败，使用空数据
                }
            }
        }

        #endregion

        #endregion

        #region private

        #region 字段

        /// <summary>
        /// 项目数据对象。
        /// </summary>
        private ProjectData _projectData;

        #endregion

        #region 方法

        /// <summary>
        /// 确保指定引擎的数据存在。
        /// </summary>
        /// <param name="engineId">TTL引擎ID</param>
        private void EnsureEngineData(string engineId)
        {
            if (!_projectData.EngineDataDic.ContainsKey(engineId))
            {
                _projectData.EngineDataDic[engineId] = new EngineProjectData();
            }
        }

        /// <summary>
        /// 从旧版INI格式文件加载数据。
        /// </summary>
        /// <param name="iniFilePath">INI文件路径</param>
        private void LoadFromIniFormat(string iniFilePath)
        {
            Dictionary<string, string> valueDic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (FileStream fs = new FileStream(iniFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        int equalIndex = line.IndexOf('=');
                        if (equalIndex > 0)
                        {
                            string key = line.Substring(0, equalIndex).Trim();
                            string value = equalIndex < line.Length - 1 ? line.Substring(equalIndex + 1).Trim() : string.Empty;
                            valueDic[key] = value;
                        }
                    }
                }
            }

            string GetValue(string key, string defaultValue)
            {
                return valueDic.TryGetValue(key, out string value) ? value : defaultValue;
            }

            // 加载基础文本数据
            _projectData.TextSplit_OriginalText = GetValue(nameof(TextSplit_OriginalText), string.Empty).Replace(_carriage_Return_Marker, "\r\n");
            _projectData.TextSplit_FinalText = GetValue(nameof(TextSplit_FinalText), string.Empty).Replace(_carriage_Return_Marker, "\r\n");

            // 加载旧版引擎数据（迁移到默认引擎ID）
            string defaultEngineId = "legacy-default";
            EngineProjectData engineData = new EngineProjectData();

            string polyReplaceText = GetValue("PolyReplace_FinalText", string.Empty).Replace(_carriage_Return_Marker, "\r\n");
            if (!string.IsNullOrEmpty(polyReplaceText))
            {
                engineData.PolyReplace_FinalText = polyReplaceText;
            }

            string voicePreprocessText = GetValue("VoicePreprocess_FinalText", string.Empty).Replace(_carriage_Return_Marker, "\r\n");
            if (!string.IsNullOrEmpty(voicePreprocessText))
            {
                engineData.VoicePreprocess_FinalText = voicePreprocessText;
            }

            // 加载角色和情绪数据
            string roleEmotionData = GetValue("RoleEmotionData", string.Empty);
            if (!string.IsNullOrEmpty(roleEmotionData))
            {
                engineData.RoleEmotionData = ParseRoleEmotionData(roleEmotionData);
            }

            // 加载语音生成任务
            string voiceTasksData = GetValue("VoiceGenerationTasks", string.Empty);
            if (!string.IsNullOrEmpty(voiceTasksData))
            {
                engineData.VoiceGenerationTasks = ParseVoiceGenerationTasks(voiceTasksData);
            }

            _projectData.EngineDataDic[defaultEngineId] = engineData;
        }

        /// <summary>
        /// 解析角色和情绪数据。
        /// </summary>
        /// <param name="savedData">保存的数据字符串</param>
        /// <returns>角色和情绪数据列表</returns>
        private List<List<RoleEmotionItem>> ParseRoleEmotionData(string savedData)
        {
            List<List<RoleEmotionItem>> result = new List<List<RoleEmotionItem>>();

            string[] paragraphs = savedData.Split(new string[] { _paragraph_Split_Marker }, StringSplitOptions.None);
            foreach (string paragraph in paragraphs)
            {
                List<RoleEmotionItem> paragraphItems = new List<RoleEmotionItem>();
                string[] rows = paragraph.Split(new string[] { _carriage_Return_Marker }, StringSplitOptions.None);
                foreach (string row in rows)
                {
                    if (string.IsNullOrWhiteSpace(row))
                    {
                        continue;
                    }

                    string[] parts = row.Split(new string[] { _save_Split_Marker }, StringSplitOptions.None);
                    if (parts.Length >= 2)
                    {
                        paragraphItems.Add(new RoleEmotionItem
                        {
                            Role = parts[0],
                            Text = parts[1]
                        });
                    }
                }
                result.Add(paragraphItems);
            }

            return result;
        }

        /// <summary>
        /// 解析语音生成任务数据。
        /// </summary>
        /// <param name="savedData">保存的数据字符串</param>
        /// <returns>语音生成任务列表</returns>
        private List<VoiceGenerationTask> ParseVoiceGenerationTasks(string savedData)
        {
            List<VoiceGenerationTask> result = new List<VoiceGenerationTask>();

            string[] tasks = savedData.Split(new string[] { _paragraph_Split_Marker }, StringSplitOptions.None);
            foreach (string task in tasks)
            {
                if (string.IsNullOrWhiteSpace(task))
                {
                    continue;
                }

                string[] parts = task.Split(new string[] { _save_Split_Marker }, StringSplitOptions.None);
                if (parts.Length >= 5)
                {
                    VoiceGenerationTask voiceTask = new VoiceGenerationTask();

                    if (parts.Length >= 8)
                    {
                        voiceTask.Id = parts[0];
                        voiceTask.Status = parts[1];
                        voiceTask.Progress = decimal.TryParse(parts[2], out decimal progress) ? progress : 0;
                        voiceTask.ProgressDetail = parts[3];
                        voiceTask.SaveFile = parts[4];
                        voiceTask.Speed = int.TryParse(parts[5], out int speed) ? speed : 100;
                        voiceTask.SpaceTime = float.TryParse(parts[6], out float spaceTime) ? spaceTime : 1f;

                        if (!string.IsNullOrEmpty(parts[7]))
                        {
                            ParseTaskItems(voiceTask, parts[7]);
                        }
                    }
                    else if (parts.Length >= 6)
                    {
                        voiceTask.Id = parts[0];
                        voiceTask.Status = parts[1];
                        voiceTask.SaveFile = parts[2];
                        voiceTask.Speed = int.TryParse(parts[3], out int speed) ? speed : 100;
                        voiceTask.SpaceTime = float.TryParse(parts[4], out float spaceTime) ? spaceTime : 1f;

                        if (!string.IsNullOrEmpty(parts[5]))
                        {
                            ParseTaskItems(voiceTask, parts[5]);
                        }
                    }
                    else
                    {
                        voiceTask.Status = parts[0];
                        voiceTask.SaveFile = parts[1];
                        voiceTask.Speed = int.TryParse(parts[2], out int speed) ? speed : 100;
                        voiceTask.SpaceTime = float.TryParse(parts[3], out float spaceTime) ? spaceTime : 1f;

                        if (!string.IsNullOrEmpty(parts[4]))
                        {
                            ParseTaskItems(voiceTask, parts[4]);
                        }
                    }

                    result.Add(voiceTask);
                }
            }

            return result;
        }

        /// <summary>
        /// 解析任务项数据。
        /// </summary>
        /// <param name="voiceTask">任务对象</param>
        /// <param name="itemsData">任务项数据字符串</param>
        private void ParseTaskItems(VoiceGenerationTask voiceTask, string itemsData)
        {
            string[] items = itemsData.Split(new string[] { "#ITEM#" }, StringSplitOptions.None);
            foreach (string itemStr in items)
            {
                if (string.IsNullOrWhiteSpace(itemStr))
                {
                    continue;
                }

                string[] itemParts = itemStr.Split('|');
                if (itemParts.Length >= 3)
                {
                    voiceTask.Items.Add(new VoiceGenerationTaskItem
                    {
                        TempFile = itemParts[0],
                        Speaker = itemParts[1],
                        Text = itemParts[2].Replace(_carriage_Return_Marker, "\r\n"),
                        EndNewLine = itemParts.Length >= 4 ? int.TryParse(itemParts[3], out int endNewLine) ? endNewLine : 0 : 0
                    });
                }
            }
        }

        #endregion

        #endregion
    }
}
