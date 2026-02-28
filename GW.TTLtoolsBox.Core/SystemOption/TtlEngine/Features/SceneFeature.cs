using System.ComponentModel;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine.Features
{
    /// <summary>
    /// 场景特性枚举，定义TTL引擎支持的场景选项
    /// </summary>
    /// <remarks>
    /// 用于CosyVoice等支持场景的TTL引擎。
    /// 每个场景选项对应一个指令片段，用于构建完整的场景指令。
    /// </remarks>
    public enum SceneFeature
    {
        /// <summary>
        /// 不选择场景
        /// </summary>
        [Description("不选择")]
        [FeatureOption("")]
        不选择 = 0,

        /// <summary>
        /// 新闻播报
        /// </summary>
        [Description("新闻播报")]
        [FeatureOption("新闻播报的语气")]
        新闻播报 = 1,

        /// <summary>
        /// 讲故事
        /// </summary>
        [Description("讲故事")]
        [FeatureOption("讲故事的语气")]
        讲故事 = 2,

        /// <summary>
        /// 客服
        /// </summary>
        [Description("客服")]
        [FeatureOption("客服的语气")]
        客服 = 3,

        /// <summary>
        /// 朗诵
        /// </summary>
        [Description("朗诵")]
        [FeatureOption("朗诵的语气")]
        朗诵 = 4
    }
}
