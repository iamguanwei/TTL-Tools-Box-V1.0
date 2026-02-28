using System;

namespace GW.TTLtoolsBox.WinFormUi.Service
{
    /// <summary>
    /// 音频服务接口，提供音频播放和管理功能。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 播放音频文件
    /// - 停止音频播放
    /// - 获取播放状态
    /// 
    /// 使用场景：
    /// - 预览声音播放
    /// - 音频文件播放控制
    /// 
    /// 依赖关系：
    /// - 无外部依赖
    /// </remarks>
    public interface IAudioService
    {
        #region 属性

        /// <summary>
        /// 获取当前是否正在播放。
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// 获取当前播放的文件路径。
        /// </summary>
        string CurrentFile { get; }

        #endregion

        #region 方法

        /// <summary>
        /// 播放音频文件。
        /// </summary>
        /// <param name="filePath">音频文件路径。</param>
        void Play(string filePath);

        /// <summary>
        /// 停止播放。
        /// </summary>
        void Stop();

        /// <summary>
        /// 暂停播放。
        /// </summary>
        void Pause();

        /// <summary>
        /// 恢复播放。
        /// </summary>
        void Resume();

        #endregion

        #region 事件

        /// <summary>
        /// 播放完成事件。
        /// </summary>
        event EventHandler PlaybackCompleted;

        /// <summary>
        /// 播放错误事件。
        /// </summary>
        event EventHandler<AudioErrorEventArgs> PlaybackError;

        #endregion
    }

    /// <summary>
    /// 音频错误事件参数。
    /// </summary>
    public class AudioErrorEventArgs : EventArgs
    {
        /// <summary>
        /// 获取错误消息。
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 初始化AudioErrorEventArgs类的新实例。
        /// </summary>
        /// <param name="message">错误消息。</param>
        public AudioErrorEventArgs(string message)
        {
            Message = message;
        }
    }
}
