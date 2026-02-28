using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GW.TTLtoolsBox.Core.PolyReplace;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine
{
    /// <summary>
    /// Index TTL V2 TTL引擎连接器，继承自ATtlEngineConnector
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 连接Index TTL V2 TTL引擎
    /// - 构建适合Index TTL V2 TTL引擎的请求URL
    /// - 处理Index TTL V2 TTL引擎的文本转语音请求
    /// - 通过HTTP请求获取朗读者列表
    /// 
    /// 使用场景：
    /// - 连接运行在Index TTL V2的TTL引擎
    /// - 适用于单机部署的TTL引擎服务
    /// 
    /// 依赖关系：
    /// - 继承自ANetworkTtlEngineConnector抽象类
    /// - 依赖System.Runtime.Serialization进行JSON反序列化
    /// </remarks>
    public class IndexTTLv2LYttlEngineConnector : ANetworkTtlEngineConnector
    {
        #region 常量

        /// <summary>
        /// URL查询路径，用于构建TTS请求
        /// </summary>
        private const string _Url_Query_Path = "/?";

        /// <summary>
        /// 文本参数名称
        /// </summary>
        private const string _Param_Text_Name = "text";

        /// <summary>
        /// 说话人参数名称
        /// </summary>
        private const string _Param_Speaker_Name = "speaker";

        #endregion

        #region public

        #region 构造函数

        /// <summary>
        /// 初始化IndexTTLv2LYttlEngineConnector类的新实例
        /// </summary>
        public IndexTTLv2LYttlEngineConnector()
            : base()
        {
        }

        #endregion

        #region ATtlEngineConnector 实现

        /// <summary>
        /// 获取引擎的唯一标识符
        /// </summary>
        public override string Id { get; } = "b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e";

        /// <summary>
        /// 获取连接器的名称
        /// </summary>
        public override string Name
        {
            get { return "Index TTL V2（刘悦）"; }
        }

        /// <summary>
        /// 获取连接器的说明或详情
        /// </summary>
        public override string Description
        {
            get { return "刘悦的 Index TTL V2 TTL引擎服务（刘悦的技术博客(B站/Youtube 同名) https://t.zsxq.com/IrQPr）"; }
        }

        /// <summary>
        /// 获取各个参数的意义和格式说明
        /// </summary>
        public override string[] ParameterDescriptions
        {
            get
            {
                return new string[]
                {
                    "连接地址，格式：http://ip:port，范例：http://localhost:9880",
                    "Index TTL V2角色文件所在路径，例如c:\\index-ttl-v2\\voices"
                };
            }
        }

        /// <summary>
        /// 获取当前引擎所使用的多音字方案
        /// </summary>
        public override IPolyphonicScheme PolyphonicScheme
        {
            get { return new NumberTonePinyinPolyphonicScheme(); }
        }

        /// <summary>
        /// 异步连接到TTL引擎，通过验证URL和获取朗读者列表
        /// </summary>
        /// <returns>表示异步操作的任务</returns>
        /// <exception cref="Exception">连接失败时抛出异常</exception>
        public override async Task ConnectAsync()
        {
            if (GetConnectionStatus() == ConnectionStatus.Connected)
            {
                return;
            }

            OnConnectionStatusChanged(ConnectionStatus.Connecting, "正在连接Index TTL V2引擎...");

            try
            {
                await ValidateConnectionAsync();

                var speakerList = FetchSpeakersFromFolder();

                UpdateSpeakers(speakerList);

                OnConnectionStatusChanged(ConnectionStatus.Connected, "Index TTL V2引擎连接成功");
            }
            catch (Exception ex)
            {
                OnConnectionStatusChanged(ConnectionStatus.Failed, $"连接失败: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// 刷新角色名单
        /// </summary>
        public override void RefreshSpeakers()
        {
            try
            {
                var speakerList = FetchSpeakersFromFolder();
                UpdateSpeakers(speakerList);
            }
            catch
            {
            }
        }

        #endregion

        #endregion

        #region private

        #region 方法

        /// <summary>
        /// 异步验证连接，检查URL是否能返回JSON格式响应
        /// </summary>
        /// <returns>表示异步操作的任务</returns>
        /// <exception cref="Exception">验证失败时抛出异常</exception>
        private async Task ValidateConnectionAsync()
        {
            if (Parameters == null || Parameters.Length < 1)
            {
                throw new InvalidOperationException("未设置连接参数");
            }

            string baseUrl = Parameters[0];
            string speakersUrl = $"{baseUrl}/Speakers";

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = ConnectionTimeout;
                var response = await httpClient.GetAsync(speakersUrl);

                var jsonString = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    throw new Exception("服务器返回空响应");
                }

                jsonString = jsonString.Trim();
                if (!jsonString.StartsWith("[") && !jsonString.StartsWith("{"))
                {
                    throw new Exception($"服务器返回非JSON格式响应: {jsonString.Substring(0, Math.Min(100, jsonString.Length))}");
                }
            }
        }

        /// <summary>
        /// 从角色文件夹获取朗读者列表
        /// </summary>
        /// <returns>朗读者列表</returns>
        /// <exception cref="Exception">获取失败时抛出异常</exception>
        private List<SpeakerInfo> FetchSpeakersFromFolder()
        {
            var speakerList = new List<SpeakerInfo>();

            if (Parameters == null || Parameters.Length < 2)
            {
                throw new InvalidOperationException("未设置角色文件路径参数");
            }

            string voicesPath = Parameters[1];

            if (!Directory.Exists(voicesPath))
            {
                throw new InvalidOperationException($"角色文件路径不存在: {voicesPath}");
            }

            var files = Directory.GetFiles(voicesPath);
            foreach (var file in files)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                if (!string.IsNullOrEmpty(fileNameWithoutExtension))
                {
                    var speaker = new SpeakerInfo(fileNameWithoutExtension);
                    speakerList.Add(speaker);
                }
            }

            return speakerList;
        }

        /// <summary>
        /// 更新Speakers属性，保留已修改的部分
        /// </summary>
        /// <param name="newSpeakerList">新的朗读者列表</param>
        private void UpdateSpeakers(List<SpeakerInfo> newSpeakerList)
        {
            var oldSpeakerArray = Speakers;
            for (int i = 0; i < newSpeakerList.Count; i++)
            {
                var newSpeaker = newSpeakerList[i];
                var oldSpeaker = oldSpeakerArray.FirstOrDefault(s => s.SourceName.Equals(newSpeaker.SourceName));
                if (oldSpeaker != null)
                {
                    newSpeaker.TryFromString(oldSpeaker.ToString());
                }
            }

            Speakers = newSpeakerList.ToArray();
        }

        #endregion

        #endregion

        #region protected

        #region 方法

        /// <summary>
        /// 构建请求URL
        /// </summary>
        /// <param name="parameters">TTL引擎参数</param>
        /// <returns>构建好的请求URL</returns>
        /// <exception cref="ArgumentNullException">parameters为null时抛出异常</exception>
        protected override string BuildRequestUrl(TtlEngineParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters), "TTL engine parameters cannot be null");
            }

            var text = parameters.Text ?? string.Empty;
            var speaker = parameters.Speaker;

            string baseUrl = this.Parameters[0];
            return $"{baseUrl}{_Url_Query_Path}{_Param_Text_Name}={text}&{_Param_Speaker_Name}={speaker}";
        }

        /// <summary>
        /// 验证参数是否合法
        /// </summary>
        /// <param name="parameters">要验证的参数数组</param>
        protected override void ValidateParameters(string[] parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters), "Parameters cannot be null");
            }

            if (parameters.Length < 2)
            {
                throw new ArgumentException("Parameters must contain at least two items: connection address and output folder path", nameof(parameters));
            }

            string baseUrl = parameters[0];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentException("Connection address cannot be null or empty", nameof(parameters));
            }

            if (!baseUrl.StartsWith("http://") && !baseUrl.StartsWith("https://"))
            {
                throw new ArgumentException("Connection address must start with http:// or https://", nameof(parameters));
            }

            if (baseUrl.EndsWith("/"))
            {
                parameters[0] = baseUrl.TrimEnd('/');
            }

            string outputPath = parameters[1];
            if (string.IsNullOrEmpty(outputPath))
            {
                throw new ArgumentException("Output folder path cannot be null or empty", nameof(parameters));
            }
        }

        /// <summary>
        /// 执行任务并获取音频文件路径
        /// </summary>
        /// <param name="parameters">TTL引擎参数</param>
        /// <returns>音频文件路径</returns>
        protected override async Task<string> ExecuteTaskAsync(TtlEngineParameters parameters)
        {
            var audioData = await base.ExecuteNetworkTaskAsync(parameters);

            string outputFilePath;
            if (!string.IsNullOrWhiteSpace(parameters.OutputFilePath))
            {
                outputFilePath = parameters.OutputFilePath;
            }
            else
            {
                string outputFolder = Parameters[1];
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                string fileName = $"{Guid.NewGuid()}.wav";
                outputFilePath = Path.Combine(outputFolder, fileName);
            }

            string directory = Path.GetDirectoryName(outputFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await Task.Run(() => File.WriteAllBytes(outputFilePath, audioData));
            return outputFilePath;
        }

        #endregion

        #endregion

    }
}
