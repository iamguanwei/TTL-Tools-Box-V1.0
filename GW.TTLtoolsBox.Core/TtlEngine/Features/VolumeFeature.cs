using System.ComponentModel;

namespace GW.TTLtoolsBox.Core.TtlEngine.Features
{
    /// <summary>
    /// 音量特性枚举，定义TTL引擎支持的音量选项
    /// </summary>
    /// <remarks>
    /// 用于CosyVoice等支持音量调节的TTL引擎。
    /// 每个音量选项对应一个指令片段，用于构建完整的音量指令。
    /// </remarks>
    public enum VolumeFeature
    {
        /// <summary>
        /// 正常音量
        /// </summary>
        [Description("正常")]
        [FeatureOption("")]
        正常 = 0,

        /// <summary>
        /// 大声音量
        /// </summary>
        [Description("大声")]
        [FeatureOption("大声的音量")]
        大声 = 1,

        /// <summary>
        /// 轻声音量
        /// </summary>
        [Description("轻声")]
        [FeatureOption("轻声的音量")]
        轻声 = 2
    }
}
