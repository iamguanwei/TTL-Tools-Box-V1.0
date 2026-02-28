using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GW.TTLtoolsBox.Core.Entity
{
    /// <summary>
    /// 语音生成任务数据类（任务列表中显示的任务）。
    /// </summary>
    [DataContract]
    public class VoiceGenerationTask
    {
        #region public

        #region 属性

        /// <summary>
        /// 获取或设置任务ID。
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置任务状态。
        /// </summary>
        [DataMember(Name = "status")]
        public string Status { get; set; } = "未开始";

        /// <summary>
        /// 获取或设置任务进度百分比。
        /// </summary>
        [DataMember(Name = "progress")]
        public decimal Progress { get; set; } = 0;

        /// <summary>
        /// 获取或设置任务进度详情。
        /// </summary>
        [DataMember(Name = "progressDetail")]
        public string ProgressDetail { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置保存文件路径。
        /// </summary>
        [DataMember(Name = "saveFile")]
        public string SaveFile { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置语音速度。
        /// </summary>
        [DataMember(Name = "speed")]
        public int Speed { get; set; } = 100;

        /// <summary>
        /// 获取或设置音量。
        /// </summary>
        [DataMember(Name = "volume")]
        public int Volume { get; set; } = 100;

        /// <summary>
        /// 获取或设置空白时长（单位：秒）。
        /// </summary>
        [DataMember(Name = "spaceTime")]
        public float SpaceTime { get; set; } = 1f;

        /// <summary>
        /// 获取或设置是否为预览任务。
        /// </summary>
        [DataMember(Name = "isPreview")]
        public bool IsPreview { get; set; } = false;

        /// <summary>
        /// 获取或设置预览任务对应的源名称。
        /// </summary>
        [DataMember(Name = "previewSourceName")]
        public string PreviewSourceName { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置具体的任务项列表。
        /// </summary>
        [DataMember(Name = "items")]
        public List<VoiceGenerationTaskItem> Items { get; set; } = new List<VoiceGenerationTaskItem>();

        #endregion

        #endregion
    }
}
