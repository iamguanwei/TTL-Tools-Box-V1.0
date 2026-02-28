using System;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine.Events
{
    /// <summary>
    /// 表示TTL引擎连接状态变化事件的参数
    /// </summary>
    public class TtlEngineConnectionEventArgs : EventArgs
    {
        #region public

        #region 构造函数

        /// <summary>
        /// 初始化TtlEngineConnectionEventArgs类的新实例
        /// </summary>
        /// <param name="status">连接状态</param>
        /// <param name="message">状态消息</param>
        /// <param name="error">错误信息（如果有）</param>
        public TtlEngineConnectionEventArgs(GW.TTLtoolsBox.Core.SystemOption.TtlEngine.ConnectionStatus status, string message, Exception error = null)
        {
            Status = status;
            Message = message;
            Error = error;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取连接状态
        /// </summary>
        public GW.TTLtoolsBox.Core.SystemOption.TtlEngine.ConnectionStatus Status { get; }

        /// <summary>
        /// 获取状态消息
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// 获取错误信息（如果有）
        /// </summary>
        public Exception Error { get; }

        #endregion

        #endregion

        #region private

        #endregion
    }
}
