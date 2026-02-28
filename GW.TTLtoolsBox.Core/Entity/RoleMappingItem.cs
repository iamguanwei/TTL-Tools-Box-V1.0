using System.Runtime.Serialization;

namespace GW.TTLtoolsBox.Core.Entity
{
    /// <summary>
    /// 角色映射项数据类，用于将项目中的角色映射到TTL引擎的朗读者。
    /// </summary>
    [DataContract]
    public class RoleMappingItem
    {
        #region public

        #region 属性

        /// <summary>
        /// 获取或设置角色名称。
        /// </summary>
        [DataMember(Name = "roleName")]
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置源名称（TTL引擎朗读者名称）。
        /// </summary>
        [DataMember(Name = "sourceName")]
        public string SourceName { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置声音预览文件路径。
        /// </summary>
        [DataMember(Name = "voiceSamplePath")]
        public string VoiceSamplePath { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置一个值，表示该映射的源名称是否已丢失（在TTL引擎中找不到）。
        /// </summary>
        [IgnoreDataMember]
        public bool IsLost { get; set; } = false;

        #endregion

        #endregion
    }
}
