using System;

namespace GW.TTLtoolsBox.WinFormUi.Base
{
    /// <summary>
    /// 呈现器接口，定义呈现器的基本功能。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 提供呈现器的基本操作接口
    /// - 管理视图和模型之间的交互
    /// 
    /// 使用场景：
    /// - 所有面板的呈现器都应实现此接口
    /// - 用于MVP模式中的呈现层
    /// 
    /// 依赖关系：
    /// - 依赖IView接口
    /// </remarks>
    public interface IPresenter
    {
        /// <summary>
        /// 获取或设置关联的视图。
        /// </summary>
        IView View { get; set; }

        /// <summary>
        /// 初始化呈现器。
        /// </summary>
        void Initialize();

        /// <summary>
        /// 释放呈现器资源。
        /// </summary>
        void Dispose();
    }
}
