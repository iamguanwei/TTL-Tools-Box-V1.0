using System;
using System.Windows.Forms;

namespace GW.TTLtoolsBox.WinFormUi.Helper
{
    /// <summary>
    /// 为UI提供服务的静态工具类。
    /// </summary>
    internal static class UiHelper
    {
        #region public

        #region 跨线程操作

        /// <summary>
        /// 更新UI的跨线程调用方法。
        /// </summary>
        /// <param name="control">相关的UI部件。</param>
        /// <param name="action">具体执行更新UI的方法。</param>
        public static void UpdateUi(Control control, Action action)
        {
            if (control == null) throw new NullReferenceException($"{nameof(control)}");
            if (action == null) throw new NullReferenceException($"{nameof(action)}");

            if (control.InvokeRequired) // 跨线程
            {
                if (control.Disposing == false && control.IsDisposed == false)
                {
                    try { control.Invoke(action, null); } catch { }
                }
            }
            else // 当前线程
            {
                action();
            }
        }

        #endregion

        #endregion
    }
}
