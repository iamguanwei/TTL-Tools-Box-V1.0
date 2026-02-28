namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine
{
    /// <summary>
    /// 表示TTL引擎的连接状态
    /// </summary>
    /// <remarks>
    /// 此枚举用于描述TTL引擎连接器的当前状态
    /// 状态流转：
    /// - Disconnected → Connecting → Connected
    /// - Connecting → Failed
    /// - Connected → Disconnected
    /// - Failed → Connecting
    /// </remarks>
    public enum ConnectionStatus : int
    {
        /// <summary>
        /// 未连接状态
        /// </summary>
        /// <remarks>初始状态，未与TTL引擎建立连接</remarks>
        Disconnected = 0,

        /// <summary>
        /// 连接中状态
        /// </summary>
        /// <remarks>正在尝试与TTL引擎建立连接</remarks>
        Connecting = 1,

        /// <summary>
        /// 已连接状态
        /// </summary>
        /// <remarks>成功与TTL引擎建立连接</remarks>
        Connected = 2,

        /// <summary>
        /// 连接失败状态
        /// </summary>
        /// <remarks>尝试连接TTL引擎失败</remarks>
        Failed = 3
    }
}