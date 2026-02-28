using System.Collections.Generic;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine
{
    /// <summary>
    /// TTL引擎参数类，用于封装不同TTL引擎的参数
    /// </summary>
    public class TtlEngineParameters
    {
        #region 常量

        #endregion

        #region public

        #region 构造函数

        /// <summary>
        /// 初始化TtlEngineParameters类的新实例
        /// </summary>
        public TtlEngineParameters()
        {
        }

        /// <summary>
        /// 使用指定的文本初始化TtlEngineParameters类的新实例
        /// </summary>
        /// <param name="text">要转换的文本</param>
        public TtlEngineParameters(string text)
        {
            Text = text;
        }

        /// <summary>
        /// 使用指定的文本和角色初始化TtlEngineParameters类的新实例
        /// </summary>
        /// <param name="text">要转换的文本</param>
        /// <param name="speaker">角色名称</param>
        public TtlEngineParameters(string text, string speaker)
            : this(text)
        {
            Speaker = speaker;
        }

        /// <summary>
        /// 使用指定的文本和朗读者信息初始化TtlEngineParameters类的新实例
        /// </summary>
        /// <param name="text">要转换的文本</param>
        /// <param name="speakerInfo">朗读者信息</param>
        public TtlEngineParameters(string text, GW.TTLtoolsBox.Core.SystemOption.TtlEngine.SpeakerInfo speakerInfo)
            : this(text)
        {
            SpeakerInfo = speakerInfo;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置要转换的文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 获取或设置角色名称
        /// </summary>
        public string Speaker
        {
            get { return SpeakerInfo?.SourceName; }
            set { SpeakerInfo = new GW.TTLtoolsBox.Core.SystemOption.TtlEngine.SpeakerInfo(value); }
        }

        /// <summary>
        /// 获取或设置朗读者信息
        /// </summary>
        public GW.TTLtoolsBox.Core.SystemOption.TtlEngine.SpeakerInfo SpeakerInfo { get; set; }

        /// <summary>
        /// 获取或设置输出文件路径，用于指定音频文件的保存位置
        /// </summary>
        public string OutputFilePath { get; set; }

        /// <summary>
        /// 获取或设置特性选择字典
        /// </summary>
        /// <remarks>
        /// 键为特性名称（如"方言"、"情感风格"、"场景"），值为选中的枚举值（存储为int）。
        /// 用于传递TTL引擎支持的扩展参数。
        /// </remarks>
        public Dictionary<string, int> FeatureSelections { get; set; }

        #endregion

        #endregion

        #region protected

        #endregion

        #region private

        #endregion
    }
}