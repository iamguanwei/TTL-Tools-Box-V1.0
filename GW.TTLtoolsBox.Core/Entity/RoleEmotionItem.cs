using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GW.TTLtoolsBox.Core.Entity
{
    /// <summary>
    /// 角色和情绪指定项数据类。
    /// </summary>
    [DataContract]
    public class RoleEmotionItem
    {
        #region public

        #region 属性

        /// <summary>
        /// 获取或设置角色标识。
        /// </summary>
        [DataMember(Name = "role")]
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置文本内容。
        /// </summary>
        [DataMember(Name = "text")]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置特性选择字典。
        /// </summary>
        /// <remarks>
        /// 键为特性名称，值为选中的枚举值（存储为int）。
        /// 用于保存TTL引擎支持的扩展参数选择。
        /// </remarks>
        [DataMember(Name = "featureSelections")]
        public Dictionary<string, int> FeatureSelections { get; set; } = new Dictionary<string, int>();

        #endregion

        #endregion
    }
}
