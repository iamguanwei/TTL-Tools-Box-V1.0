using System.ComponentModel;

namespace GW.TTLtoolsBox.Core.TtlEngine.Features
{
    /// <summary>
    /// 语速特性枚举，定义TTL引擎支持的语速选项
    /// </summary>
    /// <remarks>
    /// 用于CosyVoice等支持语速调节的TTL引擎。
    /// 每个语速选项对应一个指令片段，用于构建完整的语速指令。
    /// </remarks>
    public enum SpeedFeature
    {
        /// <summary>
        /// 正常语速
        /// </summary>
        [Description("正常")]
        [FeatureOption("")]
        正常 = 0,

        /// <summary>
        /// 快速语速
        /// </summary>
        [Description("快速")]
        [FeatureOption("快速的语速")]
        快速 = 1,

        /// <summary>
        /// 慢速语速
        /// </summary>
        [Description("慢速")]
        [FeatureOption("慢速的语速")]
        慢速 = 2
    }
}
