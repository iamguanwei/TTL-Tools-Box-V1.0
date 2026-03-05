using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GW.TTLtoolsBox.Core.Entity
{
    /// <summary>
    /// 引擎项目数据类，包含特定引擎的相关数据。
    /// </summary>
    [DataContract]
    public class EngineProjectData
    {
        #region public

        #region 属性

        /// <summary>
        /// 获取或设置多音字替换的最终文本。
        /// </summary>
        [DataMember(Name = "polyReplace_finalText")]
        public string PolyReplace_FinalText { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置语音生成预处理的最终文本。
        /// </summary>
        [DataMember(Name = "voicePreprocess_finalText")]
        public string VoicePreprocess_FinalText { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置角色和情绪指定数据。
        /// </summary>
        [DataMember(Name = "roleEmotionData")]
        public List<List<RoleEmotionItem>> RoleEmotionData { get; set; } = new List<List<RoleEmotionItem>>();

        /// <summary>
        /// 获取或设置语音生成任务数据。
        /// </summary>
        [DataMember(Name = "voiceGenerationTasks")]
        public List<VoiceGenerationTask> VoiceGenerationTasks { get; set; } = new List<VoiceGenerationTask>();

        /// <summary>
        /// 获取或设置角色映射数据。
        /// </summary>
        [DataMember(Name = "roleMappingData")]
        public List<RoleMappingItem> RoleMappingData { get; set; } = new List<RoleMappingItem>();

        /// <summary>
        /// 获取或设置默认朗读者名称。
        /// </summary>
        [DataMember(Name = "voicePreprocess_defaultSpeaker")]
        public string VoicePreprocess_DefaultSpeaker { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置全局语速值。
        /// </summary>
        [DataMember(Name = "voicePreprocess_globalSpeed")]
        public int VoicePreprocess_GlobalSpeed { get; set; } = 100;

        /// <summary>
        /// 获取或设置是否启用全局语速。
        /// </summary>
        [DataMember(Name = "voicePreprocess_useGlobalSpeed")]
        public bool VoicePreprocess_UseGlobalSpeed { get; set; } = false;

        /// <summary>
        /// 获取或设置全局音量值。
        /// </summary>
        [DataMember(Name = "voicePreprocess_globalVolume")]
        public int VoicePreprocess_GlobalVolume { get; set; } = 100;

        /// <summary>
        /// 获取或设置是否启用全局音量。
        /// </summary>
        [DataMember(Name = "voicePreprocess_useGlobalVolume")]
        public bool VoicePreprocess_UseGlobalVolume { get; set; } = false;

        /// <summary>
        /// 获取或设置段间空白时长（秒）。
        /// </summary>
        [DataMember(Name = "voicePreprocess_blankDuration")]
        public decimal VoicePreprocess_BlankDuration { get; set; } = 1;

        #endregion

        #endregion
    }
}
