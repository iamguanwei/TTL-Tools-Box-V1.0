using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GW.TTLtoolsBox.Core.Entity
{
    /// <summary>
    /// 语音生成任务项数据类（具体执行的任务项）。
    /// </summary>
    [DataContract]
    public class VoiceGenerationTaskItem
    {
        #region public

        #region 属性

        /// <summary>
        /// 获取或设置朗读者名称（SourceName）。
        /// </summary>
        [DataMember(Name = "speaker")]
        public string Speaker { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置文本内容。
        /// </summary>
        [DataMember(Name = "text")]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置临时文件路径。
        /// </summary>
        [DataMember(Name = "tempFile")]
        public string TempFile { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置结尾的换行符数量。
        /// </summary>
        [DataMember(Name = "endNewLine")]
        public int EndNewLine { get; set; } = 0;

        /// <summary>
        /// 获取或设置特性选择字典。
        /// </summary>
        /// <remarks>
        /// 键为特性名称，值为选中的枚举值（存储为int）。
        /// 用于保存TTL引擎支持的扩展参数选择。
        /// </remarks>
        [DataMember(Name = "featureSelections")]
        public Dictionary<string, int> FeatureSelections { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// 获取或设置语速值（0表示使用各自朗读者的语速，>0表示实际语速）。
        /// </summary>
        [DataMember(Name = "speed")]
        public int Speed { get; set; } = 0;

        /// <summary>
        /// 获取或设置音量值（0表示使用各自朗读者的音量，>0表示实际音量）。
        /// </summary>
        [DataMember(Name = "volume")]
        public int Volume { get; set; } = 0;

        #endregion

        #endregion
    }
}
