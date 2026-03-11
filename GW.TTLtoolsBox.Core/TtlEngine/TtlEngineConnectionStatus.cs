namespace GW.TTLtoolsBox.Core.TtlEngine
{
    /// <summary>
    /// TTL引擎连接状态枚举
    /// </summary>
    /// <remarks>
    /// 用于表示TTL引擎的当前连接状态
    /// 状态流转：
    /// - 未连接 → 连接中 → 连接成功
    /// - 连接中 → 连接失败
    /// - 连接成功 → 未连接（断开）
    /// - 连接失败 → 连接中（重试）
    /// </remarks>
    public enum TtlEngineConnectionStatus
    {
        /// <summary>
        /// 引擎未连接
        /// </summary>
        未连接 = 0,

        /// <summary>
        /// 正在连接
        /// </summary>
        连接中 = 1,

        /// <summary>
        /// 连接成功
        /// </summary>
        连接成功 = 2,

        /// <summary>
        /// 连接失败
        /// </summary>
        连接失败 = 3
    }
}
