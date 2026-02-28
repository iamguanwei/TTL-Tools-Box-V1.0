using System;
using GW.TTLtoolsBox.WinFormUi.Helper;

namespace GW.TTLtoolsBox.WinFormUi.Base
{
    /// <summary>
    /// 视图基类，提供视图的通用功能。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 提供UI线程安全操作
    /// - 实现IView接口的基本功能
    /// 
    /// 使用场景：
    /// - 所有用户控件面板都应继承此基类
    /// - 用于MVP模式中的视图层
    /// 
    /// 依赖关系：
    /// - 依赖IView和IPresenter接口
    /// - 依赖UiHelper进行UI线程操作
    /// </remarks>
    public class ViewBase : System.Windows.Forms.UserControl, IView
    {
        #region public

        #region 构造函数

        /// <summary>
        /// 初始化ViewBase类的新实例。
        /// </summary>
        public ViewBase()
        {
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置关联的呈现器。
        /// </summary>
        public IPresenter Presenter { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 刷新视图状态。
        /// </summary>
        public virtual void RefreshUi()
        {
        }

        #endregion

        #endregion

        #region protected

        #region 方法

        /// <summary>
        /// 在UI线程上执行指定操作。
        /// </summary>
        /// <param name="action">要执行的操作。</param>
        protected void UpdateUi(Action action)
        {
            UiHelper.UpdateUi(this, action);
        }

        #endregion

        #endregion
    }
}
