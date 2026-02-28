using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using GW.TTLtoolsBox.Core.SystemOption.Helper;
using GW.TTLtoolsBox.WinFormUi.Manager;

namespace GW.TTLtoolsBox.WinFormUi.Manager
{
    /// <summary>
    /// 语音生成任务数据类。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 存储语音生成任务的数据
    /// - 实现INotifyPropertyChanged接口支持属性变更通知
    /// 
    /// 使用场景：
    /// - 作为语音生成任务队列的任务项
    /// - 绑定到UI显示任务状态
    /// 
    /// 依赖关系：
    /// - 依赖VoiceGenerationTaskItem类
    /// - 依赖VoiceGenerationTaskStatus枚举
    /// - 依赖MD5Helper生成唯一ID
    /// </remarks>
    public class VoiceGenerationTask : INotifyPropertyChanged
    {
        #region public

        #region 事件

        /// <summary>
        /// 属性变更事件。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置任务状态。
        /// </summary>
        public VoiceGenerationTaskStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        /// <summary>
        /// 获取或设置任务进度百分比。
        /// </summary>
        public decimal Progress
        {
            get => _progress;
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged(nameof(Progress));
                    OnPropertyChanged(nameof(ShowProgress));
                }
            }
        }

        /// <summary>
        /// 获取任务进度百分比（显示）。
        /// </summary>
        public string ShowProgress => $"{Math.Round(Progress * 100, 0)}%";

        /// <summary>
        /// 获取或设置任务进度详情。
        /// </summary>
        public string ProgressDetail
        {
            get => _progressDetail;
            set
            {
                if (_progressDetail != value)
                {
                    _progressDetail = value;
                    OnPropertyChanged(nameof(ProgressDetail));
                }
            }
        }

        /// <summary>
        /// 获取或设置保存文件路径。
        /// </summary>
        public string SaveFile { get; set; } = string.Empty;

        /// <summary>
        /// 获取保存的文件名（包含最后一层文件夹）。
        /// </summary>
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(SaveFile))
                {
                    return string.Empty;
                }

                string directoryName = Path.GetFileName(Path.GetDirectoryName(SaveFile));
                string fileName = Path.GetFileName(SaveFile);

                if (string.IsNullOrEmpty(directoryName))
                {
                    return "\\" + fileName;
                }

                return Path.Combine(directoryName, fileName);
            }
        }

        /// <summary>
        /// 获取文本内容。
        /// </summary>
        public string Text => Items == null || Items.Length == 0 ? string.Empty : string.Concat(Items.Select(i => i.Text));

        /// <summary>
        /// 获取或设置语音速度。
        /// </summary>
        public int Speed { get; set; } = 100;

        /// <summary>
        /// 获取显示语音速度。
        /// </summary>
        public string ShowSpeed => $"{Speed}%";

        /// <summary>
        /// 获取或设置空白时长（单位：秒）。
        /// </summary>
        public float SpaceTime { get; set; } = 1f;

        /// <summary>
        /// 获取显示空白时长。
        /// </summary>
        public string ShowSpaceTime => SpaceTime == Math.Truncate(SpaceTime) ? $"{(int)SpaceTime} 秒" : $"{SpaceTime} 秒";

        /// <summary>
        /// 获取或设置具体的任务项。
        /// </summary>
        public VoiceGenerationTaskItem[] Items { get; set; } = Array.Empty<VoiceGenerationTaskItem>();

        /// <summary>
        /// 获取或设置当前实例的ID。
        /// </summary>
        public string Id { get; set; } = MD5Helper.GetShortMd5(Guid.NewGuid().ToString());

        /// <summary>
        /// 获取或设置是否为预览任务。
        /// </summary>
        public bool IsPreview { get; set; } = false;

        /// <summary>
        /// 获取或设置预览任务对应的源名称（用于刷新UI）。
        /// </summary>
        public string PreviewSourceName { get; set; } = string.Empty;

        #endregion

        #endregion

        #region protected

        #region 方法

        /// <summary>
        /// 触发属性变更事件。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion

        #region private

        #region 字段

        private VoiceGenerationTaskStatus _status = VoiceGenerationTaskStatus.未开始;
        private decimal _progress = 0;
        private string _progressDetail = string.Empty;

        #endregion

        #endregion
    }
}
