using System.ComponentModel;

namespace GW.TTLtoolsBox.Core.TtlEngine.Features
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
        东北话 = 4,

        /// <summary>
        /// 天津话
        /// </summary>
        [Description("天津话")]
        [FeatureOption("天津话")]
        天津话 = 5,

        /// <summary>
        /// 山东话
        /// </summary>
        [Description("山东话")]
        [FeatureOption("山东话")]
        山东话 = 6,

        /// <summary>
        /// 河南话
        /// </summary>
        [Description("河南话")]
        [FeatureOption("河南话")]
        河南话 = 7,

        /// <summary>
        /// 陕西话
        /// </summary>
        [Description("陕西话")]
        [FeatureOption("陕西话")]
        陕西话 = 8,

        /// <summary>
        /// 山西话
        /// </summary>
        [Description("山西话")]
        [FeatureOption("山西话")]
        山西话 = 9,

        /// <summary>
        /// 甘肃话
        /// </summary>
        [Description("甘肃话")]
        [FeatureOption("甘肃话")]
        甘肃话 = 10,

        /// <summary>
        /// 云南话
        /// </summary>
        [Description("云南话")]
        [FeatureOption("云南话")]
        云南话 = 11,

        /// <summary>
        /// 贵州话
        /// </summary>
        [Description("贵州话")]
        [FeatureOption("贵州话")]
        贵州话 = 12,

        /// <summary>
        /// 湖北话
        /// </summary>
        [Description("湖北话")]
        [FeatureOption("湖北话")]
        湖北话 = 13,

        /// <summary>
        /// 湖南话
        /// </summary>
        [Description("湖南话")]
        [FeatureOption("湖南话")]
        湖南话 = 14,

        /// <summary>
        /// 江西话
        /// </summary>
        [Description("江西话")]
        [FeatureOption("江西话")]
        江西话 = 15,

        /// <summary>
        /// 闽南话
        /// </summary>
        [Description("闽南话")]
        [FeatureOption("闽南话")]
        闽南话 = 16,

        /// <summary>
        /// 宁夏话
        /// </summary>
        [Description("宁夏话")]
        [FeatureOption("宁夏话")]
        宁夏话 = 17
    }
}
