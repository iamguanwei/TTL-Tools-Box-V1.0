using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine
{
    /// <summary>
    /// TTL引擎特性定义类，描述一个特性类别及其所有可选值
    /// </summary>
    /// <remarks>
    /// 此类通过反射读取枚举类型的选项信息，用于定义TTL引擎支持的特性。
    /// 每个特性定义包含：
    /// - 特性名称：用于UI显示和标识
    /// - 枚举类型：定义所有可选值
    /// - 指令模板：用于生成完整的指令文本
    /// 
    /// 使用示例：
    /// <code>
    /// // 定义方言特性
    /// var dialectDef = new TtlEngineFeatureDefinition(
    ///     "方言", 
    ///     typeof(DialectFeature), 
    ///     "请用{0}表达。");
    /// 
    /// // 获取显示名称
    /// string displayName = dialectDef.GetDisplayName(DialectFeature.上海话);
    /// 
    /// // 构建指令
    /// string instruction = dialectDef.BuildInstruction(DialectFeature.上海话);
    /// </code>
    /// </remarks>
    public class TtlEngineFeatureDefinition
    {
        #region public

        #region 构造函数

        /// <summary>
        /// 初始化TtlEngineFeatureDefinition类的新实例
        /// </summary>
        /// <param name="name">特性名称，用于UI显示和标识</param>
        /// <param name="enumType">枚举类型，必须使用FeatureOptionAttribute和DescriptionAttribute标注</param>
        /// <param name="instructionTemplate">指令模板，{0}会被选项的指令片段替换，为null则直接使用指令片段</param>
        /// <exception cref="ArgumentException">enumType不是枚举类型时抛出异常</exception>
        public TtlEngineFeatureDefinition(string name, Type enumType, string instructionTemplate = null)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("enumType必须是枚举类型", nameof(enumType));
            }

            Name = name;
            EnumType = enumType;
            InstructionTemplate = instructionTemplate;
            EnumValues = Enum.GetValues(enumType);
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取特性的显示名称
        /// </summary>
        /// <remarks>
        /// 例如："方言"、"情感风格"、"场景"
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// 获取枚举类型
        /// </summary>
        public Type EnumType { get; }

        /// <summary>
        /// 获取指令模板
        /// </summary>
        /// <remarks>
        /// 例如："请用{0}表达。" 其中{0}会被选项的指令片段替换。
        /// 如果为null，则直接使用指令片段作为完整指令。
        /// </remarks>
        public string InstructionTemplate { get; }

        /// <summary>
        /// 获取枚举的所有值
        /// </summary>
        public Array EnumValues { get; }

        #endregion

        #region 方法

        /// <summary>
        /// 获取枚举值的显示名称
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns>显示名称，如果未标注DescriptionAttribute则返回枚举值名称</returns>
        public string GetDisplayName(object enumValue)
        {
            var field = EnumType.GetField(enumValue.ToString());
            if (field == null)
            {
                return enumValue.ToString();
            }

            var attr = field.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? enumValue.ToString();
        }

        /// <summary>
        /// 获取枚举值的指令片段
        /// </summary>
        /// <param name="enumValue">枚举值</param>
        /// <returns>指令片段，如果未标注FeatureOptionAttribute则返回空字符串</returns>
        public string GetInstructionFragment(object enumValue)
        {
            var field = EnumType.GetField(enumValue.ToString());
            if (field == null)
            {
                return string.Empty;
            }

            var attr = field.GetCustomAttribute<FeatureOptionAttribute>();
            return attr?.InstructionFragment ?? string.Empty;
        }

        /// <summary>
        /// 获取枚举值的显示名称数组（用于UI绑定）
        /// </summary>
        /// <returns>显示名称数组</returns>
        public string[] GetDisplayNames()
        {
            return EnumValues.Cast<object>()
                .Select(v => GetDisplayName(v))
                .ToArray();
        }

        /// <summary>
        /// 构建指令文本
        /// </summary>
        /// <param name="enumValue">选中的枚举值</param>
        /// <returns>完整的指令文本，如果指令片段为空则返回空字符串</returns>
        public string BuildInstruction(object enumValue)
        {
            string fragment = GetInstructionFragment(enumValue);
            if (string.IsNullOrEmpty(fragment))
            {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(InstructionTemplate))
            {
                return string.Format(InstructionTemplate, fragment);
            }

            return fragment;
        }

        /// <summary>
        /// 根据显示名称获取枚举值
        /// </summary>
        /// <param name="displayName">显示名称</param>
        /// <returns>枚举值，未找到则返回枚举的第一个值</returns>
        public object GetEnumValueByDisplayName(string displayName)
        {
            foreach (var value in EnumValues)
            {
                if (GetDisplayName(value) == displayName)
                {
                    return value;
                }
            }
            return EnumValues.GetValue(0);
        }

        /// <summary>
        /// 根据枚举值的int值获取枚举对象
        /// </summary>
        /// <param name="intValue">枚举的int值</param>
        /// <returns>枚举对象</returns>
        public object GetEnumValueByInt(int intValue)
        {
            return Enum.ToObject(EnumType, intValue);
        }

        #endregion

        #endregion
    }
}
