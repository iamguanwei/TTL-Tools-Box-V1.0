using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GW.TTLtoolsBox.WinFormUi
{
    /// <summary>
    /// 应用程序的主入口点。
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool createdNew;
            string exePath = Application.ExecutablePath;
            string mutexKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(exePath))
                .Replace('/', '_')
                .Replace('+', '-')
                .Replace('=', '_');
            string mutexName = $"Global\\TTLToolsBox_{mutexKey}";

            using (Mutex mutex = new Mutex(true, mutexName, out createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show(
                        "程序已在运行中，请勿重复启动。",
                        "提示",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(args));
            }
        }
    }
}
