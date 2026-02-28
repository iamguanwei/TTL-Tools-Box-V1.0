using System;
using System.IO;

namespace GW.TTLtoolsBox.WinFormUi.Helper
{
    /// <summary>
    /// 音频播放器，提供简单的音频播放和停止功能。
    /// <para>支持 wav 和 mp3 格式。</para>
    /// </summary>
    internal class AudioPlayer : IDisposable
    {
        #region private

        #region 字段

        /// <summary>
        /// 当前音频播放器实例。
        /// </summary>
        private System.Windows.Media.MediaPlayer _mediaPlayer = null;

        /// <summary>
        /// 是否已释放。
        /// </summary>
        private bool _disposed = false;

        #endregion

        #endregion

        #region public

        #region 属性

        /// <summary>
        /// 获取当前是否正在播放。
        /// </summary>
        public bool IsPlaying => _mediaPlayer != null;

        #endregion

        #region 方法

        /// <summary>
        /// 播放指定的音频文件。
        /// <para>如果已有音频正在播放，将立即停止并播放新的音频。</para>
        /// </summary>
        /// <param name="filePath">音频文件路径。</param>
        public void Play(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return;
            }

            Stop();

            try
            {
                _mediaPlayer = new System.Windows.Media.MediaPlayer();
                _mediaPlayer.MediaOpened += (s, e) =>
                {
                    if (_mediaPlayer != null)
                    {
                        _mediaPlayer.Play();
                    }
                };
                _mediaPlayer.Open(new Uri(filePath));
            }
            catch
            {
                _mediaPlayer?.Close();
                _mediaPlayer = null;
            }
        }

        /// <summary>
        /// 停止当前音频播放。
        /// </summary>
        public void Stop()
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Stop();
                _mediaPlayer.Close();
                _mediaPlayer = null;
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Stop();
                }
                _disposed = true;
            }
        }

        #endregion

        #endregion
    }
}
