using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine
{
    /// <summary>
    /// 网络TTL引擎连接器抽象类，用于网络连接的TTL引擎
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 提供网络连接相关的通用实现
    /// - 处理HTTP请求和响应
    /// - 管理网络连接状态
    /// 
    /// 使用场景：
    /// - 连接基于网络的TTL引擎服务
    /// - 适用于通过HTTP/HTTPS协议访问的TTL引擎
    /// 
    /// 依赖关系：
    /// - 继承自ATtlEngineConnector抽象类
    /// - 依赖HttpClient进行HTTP请求
    /// </remarks>
    public abstract class ANetworkTtlEngineConnector : ATtlEngineConnector
    {
        #region public

        #region 构造函数

        /// <summary>
        /// 初始化ANetworkTtlEngineConnector类的新实例
        /// </summary>
        public ANetworkTtlEngineConnector()
            : base()
        {
        }

        #endregion

        #region 方法

        /// <summary>
        /// 异步连接到TTL引擎
        /// </summary>
        /// <returns>表示异步操作的任务</returns>
        /// <exception cref="Exception">连接失败时抛出异常</exception>
        public override async Task ConnectAsync()
        {
            if (GetConnectionStatus() == ConnectionStatus.Connected)
            {
                return;
            }

            OnConnectionStatusChanged(ConnectionStatus.Connecting, "Connecting to network TTL engine...");

            try
            {
                using (var cts = new CancellationTokenSource(ConnectionTimeout))
                {
                    // 从Parameters中获取基础URL
                    string baseUrl = GetBaseUrlFromParameters();
                    // 只要能访问服务器即可，不检查返回的状态码
                    await _httpClient.GetAsync(baseUrl, cts.Token);

                    OnConnectionStatusChanged(ConnectionStatus.Connected, "Connected to network TTL engine successfully");
                }
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged(ConnectionStatus.Failed, $"Failed to connect: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// 异步断开与TTL引擎的连接
        /// </summary>
        /// <returns>表示异步操作的任务</returns>
        /// <exception cref="Exception">断开连接失败时抛出异常</exception>
        public override async Task DisconnectAsync()
        {
            if (GetConnectionStatus() == ConnectionStatus.Disconnected)
            {
                return;
            }

            try
            {
                OnConnectionStatusChanged(ConnectionStatus.Disconnected, "Disconnected from network TTL engine");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged(ConnectionStatus.Disconnected, $"Error during disconnect: {ex.Message}", ex);
                throw;
            }
        }

        #endregion

        #endregion

        #region protected

        #region 方法

        /// <summary>
        /// 从参数中获取基础URL
        /// </summary>
        /// <returns>基础URL</returns>
        /// <exception cref="InvalidOperationException">当参数为空或不包含URL时抛出异常</exception>
        protected virtual string GetBaseUrlFromParameters()
        {
            if (Parameters == null || Parameters.Length == 0)
            {
                throw new InvalidOperationException("Parameters cannot be null or empty. Please set parameters using SetParameters method.");
            }

            return Parameters[0];
        }

        #endregion

        #endregion

        #region protected

        #region 方法

        /// <summary>
        /// 执行任务并获取音频数据
        /// </summary>
        /// <param name="parameters">TTL引擎参数</param>
        /// <returns>音频数据</returns>
        protected virtual async Task<byte[]> ExecuteNetworkTaskAsync(TtlEngineParameters parameters)
        {
            using (var cts = new CancellationTokenSource(RequestTimeout))
            {
                var requestUrl = BuildRequestUrl(parameters);
                var response = await _httpClient.GetAsync(requestUrl, cts.Token);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsByteArrayAsync();
            }
        }

        #endregion

        #endregion

        #region private

        /// <summary>
        /// HTTP客户端
        /// </summary>
        private readonly HttpClient _httpClient = new HttpClient();

        #endregion
    }
}