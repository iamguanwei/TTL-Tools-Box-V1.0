using System;
using System.Threading.Tasks;
using GW.TTLtoolsBox.Core.PolyReplace;
using GW.TTLtoolsBox.Core.SystemOption.TtlEngine.Events;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine
{
    /// <summary>
    /// TTL引擎连接器接口，定义了与TTL引擎交互的核心方法和事件
    /// </summary>
    /// <remarks>
    /// 此接口用于抽象TTL引擎的连接和操作，支持不同类型的TTL引擎实现
    /// 主要功能包括：连接管理、文本发送、任务队列管理、状态监控等
    /// </remarks>
    public interface ITtlEngineConnector
    {
        #region 方法

        /// <summary>
        /// 异步连接到TTL引擎
        /// </summary>
        /// <returns>表示异步操作的任务</returns>
        /// <exception cref="Exception">连接失败时抛出异常</exception>
        Task ConnectAsync();

        /// <summary>
        /// 异步断开与TTL引擎的连接
        /// </summary>
        /// <returns>表示异步操作的任务</returns>
        /// <exception cref="Exception">断开连接失败时抛出异常</exception>
        Task DisconnectAsync();

        /// <summary>
        /// 异步发送文本到TTL引擎
        /// </summary>
        /// <param name="text">要发送的文本</param>
        /// <param name="parameters">TTL引擎参数，可为null</param>
        /// <returns>返回音频文件的保存路径</returns>
        /// <exception cref="Exception">发送失败时抛出异常</exception>
        Task<string> SendTextAsync(string text, TtlEngineParameters parameters = null);

        /// <summary>
        /// 获取当前连接状态
        /// </summary>
        /// <returns>当前连接状态</returns>
        ConnectionStatus GetConnectionStatus();

        /// <summary>
        /// 获取当前队列中的任务数
        /// </summary>
        /// <returns>队列中的任务数</returns>
        int GetQueueCount();

        /// <summary>
        /// 启动自动重连
        /// </summary>
        /// <param name="intervalSeconds">重连间隔（秒）</param>
        void StartAutoReconnect(int intervalSeconds);

        /// <summary>
        /// 停止自动重连
        /// </summary>
        void StopAutoReconnect();

        #endregion

        #region 属性

        /// <summary>
        /// 获取引擎的唯一标识符
        /// </summary>
        string Id { get; }

        /// <summary>
        /// 获取或设置连接超时时间
        /// </summary>
        TimeSpan ConnectionTimeout { get; set; }

        /// <summary>
        /// 获取或设置请求超时时间
        /// </summary>
        TimeSpan RequestTimeout { get; set; }

        /// <summary>
        /// 获取当前引擎所使用的多音字方案
        /// </summary>
        IPolyphonicScheme PolyphonicScheme { get; }

        /// <summary>
        /// 获取连接器的名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取连接器的说明或详情
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 获取保存的多个参数
        /// </summary>
        string[] Parameters { get; }

        /// <summary>
        /// 获取各个参数的意义和格式说明
        /// </summary>
        string[] ParameterDescriptions { get; }

        /// <summary>
        /// 获取朗读者名单
        /// </summary>
        SpeakerInfo[] Speakers { get; }

        /// <summary>
        /// 获取引擎支持的特性定义列表
        /// </summary>
        /// <remarks>
        /// 如果引擎不支持任何特性，返回空数组。
        /// 特性定义用于描述TTL引擎支持的扩展参数，如方言、情感风格、场景等。
        /// </remarks>
        TtlEngineFeatureDefinition[] FeatureDefinitions { get; }

        #endregion

        #region 方法

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="parameters">要设置的参数数组</param>
        void SetParameters(string[] parameters);

        /// <summary>
        /// 刷新朗读者名单
        /// </summary>
        void RefreshSpeakers();

        #endregion

        #region 事件

        /// <summary>
        /// 进度变化事件，每秒触发一次
        /// </summary>
        event EventHandler<TtlEngineProgressEventArgs> ProgressChanged;

        /// <summary>
        /// 任务完成事件
        /// </summary>
        event EventHandler<TtlEngineCompletedEventArgs> Completed;

        /// <summary>
        /// 任务排队事件
        /// </summary>
        event EventHandler<TtlEngineQueuedEventArgs> TaskQueued;

        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        event EventHandler<TtlEngineConnectionEventArgs> ConnectionStatusChanged;

        /// <summary>
        /// 重连倒秒事件
        /// </summary>
        event EventHandler<TtlEngineReconnectEventArgs> ReconnectCountdown;

        #endregion
    }
}