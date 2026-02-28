using System;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine.Events
{
    /// <summary>
    /// 表示TTL引擎重连倒秒事件的参数
    /// </summary>
    public class TtlEngineReconnectEventArgs : EventArgs
    {
        #region public

        #region 构造函数

        /// <summary>
        /// 初始化TtlEngineReconnectEventArgs类的新实例
        /// </summary>
        /// <param name="remainingSeconds">剩余重连等待时间（秒）</param>
        /// <param name="attemptCount">重连尝试次数</param>
        public TtlEngineReconnectEventArgs(int remainingSeconds, int attemptCount)
        {
            RemainingSeconds = remainingSeconds;
            AttemptCount = attemptCount;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取剩余重连等待时间（秒）
        /// </summary>
        public int RemainingSeconds { get; }

        /// <summary>
        /// 获取重连尝试次数
        /// </summary>
        public int AttemptCount { get; }

        #endregion

        #endregion

        #region private

        #endregion
    }
}
