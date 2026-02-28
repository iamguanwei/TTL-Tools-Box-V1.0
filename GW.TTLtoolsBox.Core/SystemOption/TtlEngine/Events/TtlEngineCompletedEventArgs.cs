using System;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine.Events
{
    /// <summary>
    /// 表示TTL引擎完成事件的参数
    /// </summary>
    public class TtlEngineCompletedEventArgs : EventArgs
    {
        #region public

        #region 构造函数

        /// <summary>
        /// 初始化TtlEngineCompletedEventArgs类的新实例
        /// </summary>
        /// <param name="audioFilePath">音频文件的保存位置</param>
        /// <param name="success">是否成功</param>
        /// <param name="errorMessage">错误信息（如果失败）</param>
        /// <param name="taskId">完成的任务ID</param>
        public TtlEngineCompletedEventArgs(string audioFilePath, bool success, string errorMessage, Guid taskId)
        {
            AudioFilePath = audioFilePath;
            Success = success;
            ErrorMessage = errorMessage;
            TaskId = taskId;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取音频文件的保存位置
        /// </summary>
        public string AudioFilePath { get; }

        /// <summary>
        /// 获取是否成功
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// 获取错误信息（如果失败）
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// 获取完成的任务ID
        /// </summary>
        public Guid TaskId { get; }

        #endregion

        #endregion

        #region private

        #endregion
    }
}
