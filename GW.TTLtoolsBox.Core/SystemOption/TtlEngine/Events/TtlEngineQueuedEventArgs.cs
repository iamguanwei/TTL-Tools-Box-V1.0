using System;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine.Events
{
    /// <summary>
    /// 表示TTL引擎任务排队事件的参数
    /// </summary>
    public class TtlEngineQueuedEventArgs : EventArgs
    {
        #region public

        #region 构造函数

        /// <summary>
        /// 初始化TtlEngineQueuedEventArgs类的新实例
        /// </summary>
        /// <param name="taskId">排队的任务ID</param>
        /// <param name="queuePosition">任务在队列中的位置</param>
        /// <param name="queueCount">当前队列总数</param>
        public TtlEngineQueuedEventArgs(Guid taskId, int queuePosition, int queueCount)
        {
            TaskId = taskId;
            QueuePosition = queuePosition;
            QueueCount = queueCount;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取排队的任务ID
        /// </summary>
        public Guid TaskId { get; }

        /// <summary>
        /// 获取任务在队列中的位置
        /// </summary>
        public int QueuePosition { get; }

        /// <summary>
        /// 获取当前队列总数
        /// </summary>
        public int QueueCount { get; }

        #endregion

        #endregion

        #region private

        #endregion
    }
}
