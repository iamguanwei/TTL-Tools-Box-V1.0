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

        #endregion

        #endregion
    }
}
