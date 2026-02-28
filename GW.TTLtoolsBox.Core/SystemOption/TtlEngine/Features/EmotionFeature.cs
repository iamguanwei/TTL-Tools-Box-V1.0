using System.ComponentModel;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine.Features
{
    /// <summary>
    /// 情感风格特性枚举，定义TTL引擎支持的情感风格选项
    /// </summary>
    /// <remarks>
    /// 用于CosyVoice等支持情感风格的TTL引擎。
    /// 每个情感风格选项对应一个指令片段，用于构建完整的情感风格指令。
    /// </remarks>
    public enum EmotionFeature
    {
        /// <summary>
        /// 不选择情感风格
        /// </summary>
        [Description("不选择")]
        [FeatureOption("")]
        不选择 = 0,

        /// <summary>
        /// 温柔
        /// </summary>
        [Description("温柔")]
        [FeatureOption("温柔的语气")]
        温柔 = 1,

        /// <summary>
        /// 开心
        /// </summary>
        [Description("开心")]
        [FeatureOption("开心的语气")]
        开心 = 2,

        /// <summary>
        /// 严肃
        /// </summary>
        [Description("严肃")]
        [FeatureOption("严肃的语气")]
        严肃 = 3,

        /// <summary>
        /// 悲伤
        /// </summary>
        [Description("悲伤")]
        [FeatureOption("悲伤的语气")]
        悲伤 = 4,

        /// <summary>
        /// 生气
        /// </summary>
        [Description("生气")]
        [FeatureOption("生气的语气")]
        生气 = 5
    }
}
