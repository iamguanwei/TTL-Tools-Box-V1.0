using System.IO;
using System.Windows.Forms;

namespace GW.TTLtoolsBox.WinFormUi.Helper
{
    /// <summary>
    /// 应用程序常量定义。
    /// </summary>
    internal static class Constants
    {
        #region 文本拆分

        /// <summary>
        /// 拆分文本时使用的整句分隔符号。
        /// </summary>
        public const string 整句_分割符号 = "！!？?。：\"\"\"…；;";

        /// <summary>
        /// 拆分文本时使用的半句分隔符号。
        /// </summary>
        public const string 半句_分割符号 = "，";

        /// <summary>
        /// 拆分文本时最小的文本长度。
        /// </summary>
        public const uint 分割_最小长度 = 10;

        /// <summary>
        /// 拆分文本时使用的对话分隔符号。
        /// </summary>
        public const string 对话_分割符号 = "「」";

        #endregion

        #region 存盘相关

        /// <summary>
        /// 存盘用的分隔符。
        /// </summary>
        public const string 存盘_分隔符 = "#SAVESPLIT#";

        /// <summary>
        /// 该分隔符用于分割不同的音频，每一段音频保存在一个音频文件中。
        /// </summary>
        public const string 生成段落_分隔符 = "#NEWVOICE#";

        /// <summary>
        /// 项目文件的扩展名（包含.）。
        /// </summary>
        public const string 项目文件_扩展名 = ".TTB";

        #endregion

        #region 角色相关

        /// <summary>
        /// 默认角色标识
        /// </summary>
        public const string 默认_角色标识 = "默认";

        /// <summary>
        /// 角色和文本之间的分隔符。
        /// </summary>
        public const string 角色_文本分隔符 = "::";

        /// <summary>
        /// 已丢失标识后缀，用于标记在TTL引擎中找不到的朗读者。
        /// </summary>
        public const string 已丢失_标识后缀 = "（已丢失）";

        #endregion

        #region 路径相关

        /// <summary>
        /// 临时工作目录。
        /// </summary>
        public static readonly string 临时_工作目录 = Path.Combine(Application.StartupPath, "Temp");

        /// <summary>
        /// ffmpeg文件夹。
        /// </summary>
        public static readonly string Ffmpeg_文件夹 = Path.Combine(Application.StartupPath, @"ffmpeg");

        /// <summary>
        /// ffmpeg文件。
        /// </summary>
        public static readonly string Ffmpeg_文件 = Path.Combine(Ffmpeg_文件夹, @"ffmpeg.exe");

        /// <summary>
        /// TTL引擎的角色声音预览文件路径。
        /// </summary>
        public static readonly string TTL角色预览声音_文件夹 = Path.Combine(Application.StartupPath, @"RoldVoice");

        #endregion

        #region TTL引擎相关

        /// <summary>
        /// TTL角色预览声音的生成文本。
        /// </summary>
        public const string TTL角色预览声音_文本 = "你好，你喜欢我的声音吗？可能这样的声音不是很令你满意，不过你要知道，每种声音都有自己的用途。有些更适合于谈话的场景，而有些则适用于角色朗读。另外，选择声音时还需要注意甄别声音的情绪。";

        /// <summary>
        /// TTL引擎连接成功后验证间隔（秒）。
        /// </summary>
        public const int 连接成功_验证间隔秒数 = 60;

        /// <summary>
        /// TTL引擎连接失败后重试间隔（秒）。
        /// </summary>
        public const int 连接失败_重试间隔秒数 = 10;

        /// <summary>
        /// 表示"无"TTL方案的特定引擎ID。
        /// </summary>
        public const string None_Engine_Id = "__NONE__";

        #endregion
    }
}
