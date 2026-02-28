using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GW.TTLtoolsBox.WinFormUi.Helper
{
    /// <summary>
    /// TextBox扩展方法类。
    /// </summary>
    public static class TextBoxExtension
    {
        #region Windows API

        /// <summary>
        /// 垂直滚动消息常量。
        /// </summary>
        private const int WM_VSCROLL = 0x115;

        /// <summary>
        /// 滚动条拇指位置常量。
        /// </summary>
        private const int SB_THUMBPOSITION = 4;

        /// <summary>
        /// 垂直滚动条标识。
        /// </summary>
        private const int SB_VERT = 1;

        /// <summary>
        /// 发送Windows消息。
        /// </summary>
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        /// <summary>
        /// 获取滚动条位置。
        /// </summary>
        [DllImport("user32.dll")]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);

        #endregion

        #region 扩展方法

        /// <summary>
        /// 获取TextBox的垂直滚动位置。
        /// </summary>
        /// <param name="textBox">TextBox控件</param>
        /// <returns>滚动位置</returns>
        public static int GetScrollPosition(this TextBox textBox)
        {
            return GetScrollPos(textBox.Handle, SB_VERT);
        }

        /// <summary>
        /// 设置TextBox的垂直滚动位置。
        /// </summary>
        /// <param name="textBox">TextBox控件</param>
        /// <param name="position">滚动位置</param>
        public static void SetScrollPosition(this TextBox textBox, int position)
        {
            SendMessage(textBox.Handle, WM_VSCROLL, SB_THUMBPOSITION + 0x10000 * position, 0);
        }

        #endregion
    }
}
