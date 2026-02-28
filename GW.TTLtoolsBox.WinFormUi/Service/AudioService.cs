using System;
using System.IO;
using System.Media;
using System.Threading;

namespace GW.TTLtoolsBox.WinFormUi.Service
{
    /// <summary>
    /// 音频服务实现类，提供音频播放和管理功能。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 使用SoundPlayer播放WAV格式音频
    /// - 支持播放、停止、暂停、恢复操作
    /// 
    /// 使用场景：
    /// - 预览声音播放
    /// - 音频文件播放控制
    /// 
    /// 依赖关系：
    /// - 实现IAudioService接口
    /// </remarks>
    public class AudioService : IAudioService, IDisposable
    {
        #region public

        #region 构造函数

        /// <summary>
        /// 初始化AudioService类的新实例。
        /// </summary>
        public AudioService()
        {
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取当前是否正在播放。
        /// </summary>
        public bool IsPlaying => _isPlaying;

        /// <summary>
        /// 获取当前播放的文件路径。
        /// </summary>
        public string CurrentFile => _currentFile;

        #endregion

        #region 方法

        /// <summary>
        /// 播放音频文件。
        /// </summary>
        /// <param name="filePath">音频文件路径。</param>
        public void Play(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                OnPlaybackError("文件不存在或路径无效");
                return;
            }

            Stop();

            try
            {
                _currentFile = filePath;
                _soundPlayer = new SoundPlayer(filePath);
                _soundPlayer.Load();
                _soundPlayer.PlaySync();
                
                _isPlaying = false;
                OnPlaybackCompleted();
            }
            catch (Exception ex)
            {
                _isPlaying = false;
                OnPlaybackError(ex.Message);
            }
        }

        /// <summary>
        /// 停止播放。
        /// </summary>
        public void Stop()
        {
            if (_soundPlayer != null)
            {
                _soundPlayer.Stop();
                _soundPlayer.Dispose();
                _soundPlayer = null;
            }
            _isPlaying = false;
            _currentFile = null;
        }

        /// <summary>
        /// 暂停播放。
        /// </summary>
        public void Pause()
        {
            if (_soundPlayer != null && _isPlaying)
            {
                _soundPlayer.Stop();
                _isPlaying = false;
            }
        }

        /// <summary>
        /// 恢复播放。
        /// </summary>
        public void Resume()
        {
            if (_soundPlayer != null && !_isPlaying && !string.IsNullOrEmpty(_currentFile))
            {
                try
                {
                    _soundPlayer.Play();
                    _isPlaying = true;
                }
                catch (Exception ex)
                {
                    OnPlaybackError(ex.Message);
                }
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 播放完成事件。
        /// </summary>
        public event EventHandler PlaybackCompleted;

        /// <summary>
        /// 播放错误事件。
        /// </summary>
        public event EventHandler<AudioErrorEventArgs> PlaybackError;

        #endregion

        #endregion

        #region private

        #region 字段

        private SoundPlayer _soundPlayer;
        private bool _isPlaying = false;
        private string _currentFile = null;

        #endregion

        #region 方法

        private void OnPlaybackCompleted()
        {
            PlaybackCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void OnPlaybackError(string message)
        {
            PlaybackError?.Invoke(this, new AudioErrorEventArgs(message));
        }

        #endregion

        #endregion

        #region IDisposable

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        #endregion
    }
}
