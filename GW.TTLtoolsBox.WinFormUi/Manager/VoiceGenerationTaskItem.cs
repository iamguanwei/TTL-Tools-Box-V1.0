using System.Collections.Generic;
using GW.TTLtoolsBox.Core.SystemOption.TtlEngine;

namespace GW.TTLtoolsBox.WinFormUi.Manager
{
    /// <summary>
    /// 语音生成任务项数据类。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 存储单个语音生成任务项的数据
    /// 
    /// 使用场景：
    /// - 作为VoiceGenerationTask的子项
    /// - 包含朗读者、文本、临时文件等信息
    /// 
    /// 依赖关系：
    /// - 依赖SpeakerInfo类
    /// </remarks>
    public class VoiceGenerationTaskItem
    {
        /// <summary>
        /// 获取或设置保存临时文件路径。
        /// </summary>
        public string TempFile { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置朗读者信息。
        /// </summary>
        public SpeakerInfo Speaker { get; set; } = null;

        /// <summary>
        /// 获取或设置文本内容。
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置结尾的换行符数量，这将关系到延迟多长时间。
        /// </summary>
        public int EndNewLine { get; set; } = 0;

        /// <summary>
        /// 获取或设置特性选择字典，键为特性名称，值为选中的枚举值。
        /// </summary>
        public Dictionary<string, int> FeatureSelections { get; set; } = new Dictionary<string, int>();
    }
}
