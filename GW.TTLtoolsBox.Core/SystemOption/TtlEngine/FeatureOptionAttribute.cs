using System;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine
{
    /// <summary>
    /// 特性选项特性，用于标注枚举项的指令片段
    /// </summary>
    /// <remarks>
    /// 此特性用于TTL引擎特性枚举，标注每个枚举项对应的指令文本片段。
    /// 例如：方言特性中的"上海话"选项，其指令片段为"上海话"，
    /// 最终会通过指令模板生成"请用上海话表达。"这样的完整指令。
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class FeatureOptionAttribute : Attribute
    {
        #region public

        #region 构造函数

        /// <summary>
        /// 初始化FeatureOptionAttribute类的新实例
        /// </summary>
        /// <param name="instructionFragment">指令文本片段</param>
        public FeatureOptionAttribute(string instructionFragment)
        {
            InstructionFragment = instructionFragment ?? string.Empty;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取指令文本片段
        /// </summary>
        /// <remarks>
        /// 指令片段是用于构建完整指令的文本片段。
        /// 例如："上海话"、"开心的语气"、"新闻播报的语气"等。
        /// 如果为空字符串，表示该选项不生成指令（如"不选择"选项）。
        /// </remarks>
        public string InstructionFragment { get; }

        #endregion

        #endregion
    }
}
