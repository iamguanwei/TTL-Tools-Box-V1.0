namespace GW.TTLtoolsBox.WinFormUi.Manager
{
    /// <summary>
    /// TTL引擎连接状态枚举。
    /// </summary>
    /// <remarks>
    /// 用于表示TTL引擎的当前连接状态。
    /// </remarks>
    public enum TtlEngineConnectionStatus
    {
        /// <summary>
        /// 引擎未连接。
        /// </summary>
        未连接,

        /// <summary>
        /// 正在连接。
        /// </summary>
        连接中,

        /// <summary>
        /// 连接成功。
        /// </summary>
        连接成功,

        /// <summary>
        /// 连接失败。
        /// </summary>
        连接失败
    }
}
