using System;
using System.Collections.Generic;
using GW.TTLtoolsBox.Core.TtlEngine;

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
        #region 常量

        /// <summary>
        /// 语音生成临时文件前缀。
        /// </summary>
        private const string _临时文件_前缀_语音生成 = "temp_";

        #endregion

        #region public

        #region 属性

        /// <summary>
        /// 获取保存临时文件路径。
        /// </summary>
        public string TempFile { get; private set; } = string.Empty;

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

        /// <summary>
        /// 获取或设置语速值（0表示使用各自朗读者的语速，>0表示实际语速）。
        /// </summary>
        public int Speed { get; set; } = 0;

        /// <summary>
        /// 获取或设置音量值（0表示使用各自朗读者的音量，>0表示实际音量）。
        /// </summary>
        public int Volume { get; set; } = 0;

        #endregion

        #region 方法

        /// <summary>
        /// 设置临时文件路径，文件名包含前缀和唯一标识符，确保每个任务项的临时文件路径唯一。
        /// </summary>
        /// <param name="tempDirectory">临时文件夹。</param>
        /// <param name="taskId">任务ID。</param>
        /// <param name="number">文件序号。</param>
        public void SetTempFile(string tempDirectory, string taskId, uint number)
        {
            if (string.IsNullOrWhiteSpace(tempDirectory) == true) throw new ArgumentNullException(nameof(tempDirectory));
            if (string.IsNullOrWhiteSpace(taskId) == true) throw new ArgumentNullException(nameof(taskId));

            TempFile = System.IO.Path.Combine(tempDirectory, $"{_临时文件_前缀_语音生成}{taskId}_{number}.wav");
        }

        /// <summary>
        /// 设置临时文件路径，文件名包含前缀和唯一标识符，确保每个任务项的临时文件路径唯一。
        /// </summary>
        /// <param name="tempDirectory">临时文件夹。</param>
        /// <param name="taskId">任务ID。</param>
        public void SetTempFile(string tempDirectory, string taskId)
        {
            if (string.IsNullOrWhiteSpace(tempDirectory) == true) throw new ArgumentNullException(nameof(tempDirectory));
            if (string.IsNullOrWhiteSpace(taskId) == true) throw new ArgumentNullException(nameof(taskId));

            TempFile = System.IO.Path.Combine(tempDirectory, $"{_临时文件_前缀_语音生成}{taskId}.wav");
        }

        /// <summary>
        /// 设置临时文件路径，文件名包含前缀和唯一标识符，确保每个任务项的临时文件路径唯一。
        /// </summary>
        /// <param name="tempFile">临时文件。</param>
        public void SetTempFile(string tempFile)
        {
            if (string.IsNullOrWhiteSpace(tempFile) == true) throw new ArgumentNullException(nameof(tempFile));
            TempFile = tempFile;
        }

        /// <summary>
        /// 创建一个当前实例的浅表克隆体，确保新实例与原实例具有相同的数据，但在内存中是独立的对象。
        /// </summary>
        /// <returns>浅表克隆体。</returns>
        public VoiceGenerationTaskItem Clone()
        {
            return new VoiceGenerationTaskItem
            {
                TempFile = this.TempFile,
                Speaker = this.Speaker,
                Text = this.Text,
                EndNewLine = this.EndNewLine,
                FeatureSelections = new Dictionary<string, int>(this.FeatureSelections),
                Speed = this.Speed,
                Volume = this.Volume
            };
        }

        #endregion

        #endregion
    }
}
