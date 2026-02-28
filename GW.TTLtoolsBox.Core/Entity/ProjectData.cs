using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GW.TTLtoolsBox.Core.Entity
{
    /// <summary>
    /// 项目数据类，包含项目的所有数据。
    /// </summary>
    [DataContract]
    public class ProjectData
    {
        #region 常量

        /// <summary>
        /// 当前数据版本号。
        /// </summary>
        public const string Current_Version = "2.0";

        #endregion

        #region public

        #region 构造函数

        /// <summary>
        /// 初始化ProjectData类的新实例。
        /// </summary>
        public ProjectData()
        {
            EngineDataDic = new Dictionary<string, EngineProjectData>();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置数据版本号。
        /// </summary>
        [DataMember(Name = "version")]
        public string Version { get; set; } = Current_Version;

        /// <summary>
        /// 获取或设置文本拆分的原始文本。
        /// </summary>
        [DataMember(Name = "textSplit_originalText")]
        public string TextSplit_OriginalText { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置文本拆分的最终文本。
        /// </summary>
        [DataMember(Name = "textSplit_finalText")]
        public string TextSplit_FinalText { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置引擎数据字典，键为引擎ID。
        /// </summary>
        [DataMember(Name = "engineDataDic")]
        public Dictionary<string, EngineProjectData> EngineDataDic { get; set; }

        #endregion

        #endregion
    }
}
