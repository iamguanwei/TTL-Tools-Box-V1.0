using System.ComponentModel;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine.Features
{
    /// <summary>
    /// 方言特性枚举，定义TTL引擎支持的方言选项
    /// </summary>
    /// <remarks>
    /// 用于CosyVoice等支持方言的TTL引擎。
    /// 每个方言选项对应一个指令片段，用于构建完整的方言指令。
    /// </remarks>
    public enum DialectFeature
    {
        /// <summary>
        /// 不选择方言
        /// </summary>
        [Description("不选择")]
        [FeatureOption("")]
        不选择 = 0,

        /// <summary>
        /// 上海话
        /// </summary>
        [Description("上海话")]
        [FeatureOption("上海话")]
        上海话 = 1,

        /// <summary>
        /// 粤语
        /// </summary>
        [Description("粤语")]
        [FeatureOption("粤语")]
        粤语 = 2,

        /// <summary>
        /// 四川话
        /// </summary>
        [Description("四川话")]
        [FeatureOption("四川话")]
        四川话 = 3,

        /// <summary>
        /// 东北话
        /// </summary>
        [Description("东北话")]
        [FeatureOption("东北话")]
        东北话 = 4
    }
}
