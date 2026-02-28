using System;
using System.Threading.Tasks;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine
{
    /// <summary>
    /// 表示一个TTL引擎任务
    /// </summary>
    public class TtlEngineTask
    {
        #region 常量

        #endregion

        #region public

        #region 构造函数

        /// <summary>
        /// 使用指定的参数初始化TtlEngineTask类的新实例
        /// </summary>
        /// <param name="parameters">任务参数</param>
        public TtlEngineTask(TtlEngineParameters parameters)
        {
            TaskId = Guid.NewGuid();
            Parameters = parameters;
            CreationTime = DateTime.Now;
            TaskCompletionSource = new TaskCompletionSource<string>();
        }

        /// <summary>
        /// 使用指定的文本初始化TtlEngineTask类的新实例
        /// </summary>
        /// <param name="text">要转换的文本</param>
        public TtlEngineTask(string text)
            : this(new TtlEngineParameters(text))
        {
        }

        /// <summary>
        /// 使用指定的文本和角色初始化TtlEngineTask类的新实例
        /// </summary>
        /// <param name="text">要转换的文本</param>
        /// <param name="speaker">角色名称</param>
        public TtlEngineTask(string text, string speaker)
            : this(new TtlEngineParameters(text, speaker))
        {
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取任务唯一标识
        /// </summary>
        public Guid TaskId { get; }

        /// <summary>
        /// 获取任务参数
        /// </summary>
        public TtlEngineParameters Parameters { get; }

        /// <summary>
        /// 获取任务创建时间
        /// </summary>
        public DateTime CreationTime { get; }

        /// <summary>
        /// 获取用于异步任务完成通知的TaskCompletionSource
        /// </summary>
        public TaskCompletionSource<string> TaskCompletionSource { get; }

        #endregion

        #endregion

        #region protected

        #endregion

        #region private

        #endregion
    }
}