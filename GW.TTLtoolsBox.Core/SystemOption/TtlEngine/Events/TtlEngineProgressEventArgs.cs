using System;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine.Events
{
    /// <summary>
    /// 表示TTL引擎进度事件的参数
    /// </summary>
    public class TtlEngineProgressEventArgs : EventArgs
    {
        #region public

        #region 构造函数

        /// <summary>
        /// 初始化TtlEngineProgressEventArgs类的新实例
        /// </summary>
        /// <param name="elapsedTime">自调用开始经过的时间（秒）</param>
        /// <param name="taskId">当前任务ID</param>
        /// <param name="queuePosition">当前任务在队列中的位置</param>
        public TtlEngineProgressEventArgs(int elapsedTime, Guid taskId, int queuePosition)
        {
            ElapsedTime = elapsedTime;
            TaskId = taskId;
            QueuePosition = queuePosition;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取自调用开始经过的时间（秒）
        /// </summary>
        public int ElapsedTime { get; }

        /// <summary>
        /// 获取当前任务ID
        /// </summary>
        public Guid TaskId { get; }

        /// <summary>
        /// 获取当前任务在队列中的位置
        /// </summary>
        public int QueuePosition { get; }

        #endregion

        #endregion

        #region private

        #endregion
    }
}
