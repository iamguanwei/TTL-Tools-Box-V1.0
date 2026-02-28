namespace GW.TTLtoolsBox.WinFormUi.Manager
{
    /// <summary>
    /// 语音生成任务状态枚举。
    /// </summary>
    /// <remarks>
    /// 用于表示语音生成任务的当前状态。
    /// </remarks>
    public enum VoiceGenerationTaskStatusUi
    {
        /// <summary>
        /// 任务未开始。
        /// </summary>
        未开始,

        /// <summary>
        /// 任务排队中。
        /// </summary>
        排队中,

        /// <summary>
        /// 任务正在生成。
        /// </summary>
        正在生成,

        /// <summary>
        /// 任务已完成。
        /// </summary>
        已完成,

        /// <summary>
        /// 任务生成失败。
        /// </summary>
        生成失败
    }
}
