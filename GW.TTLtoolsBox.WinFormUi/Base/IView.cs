namespace GW.TTLtoolsBox.WinFormUi.Base
{
    /// <summary>
    /// 视图接口，定义视图的基本功能。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 提供视图的基本操作接口
    /// 
    /// 使用场景：
    /// - 所有用户控件面板都应实现此接口
    /// - 用于MVP模式中的视图层
    /// 
    /// 依赖关系：
    /// - 无外部依赖
    /// </remarks>
    public interface IView
    {
        /// <summary>
        /// 获取或设置关联的呈现器。
        /// </summary>
        IPresenter Presenter { get; set; }

        /// <summary>
        /// 刷新视图状态。
        /// </summary>
        void RefreshUi();
    }
}
