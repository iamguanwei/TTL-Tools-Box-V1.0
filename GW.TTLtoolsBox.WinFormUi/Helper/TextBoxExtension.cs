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

        private const int WM_VSCROLL = 0x115;
        private const int SB_THUMBPOSITION = 4;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);

        private const int SB_VERT = 1;

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
