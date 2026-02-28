using System;

namespace GW.TTLtoolsBox.WinFormUi.Helper
{
    /// <summary>
    /// 切换到下一页事件参数。
    /// </summary>
    public class SwitchToNextPageEventArgs : EventArgs
    {
        /// <summary>
        /// 获取要传递的文本内容。
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// 初始化SwitchToNextPageEventArgs类的新实例。
        /// </summary>
        /// <param name="text">要传递的文本内容。</param>
        public SwitchToNextPageEventArgs(string text)
        {
            Text = text;
        }
    }
}
