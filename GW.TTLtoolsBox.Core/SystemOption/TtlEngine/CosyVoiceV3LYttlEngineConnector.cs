using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using GW.TTLtoolsBox.Core.PolyReplace;
using GW.TTLtoolsBox.Core.SystemOption.TtlEngine.Features;

namespace GW.TTLtoolsBox.Core.SystemOption.TtlEngine
{
    /// <summary>
    /// CosyVoice V3 TTL引擎连接器，继承自ATtlEngineConnector
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 连接CosyVoice V3 TTL引擎
    /// - 构建适合CosyVoice V3 TTL引擎的请求URL
    /// - 处理CosyVoice V3 TTL引擎的文本转语音请求
    /// - 通过HTTP请求获取朗读者列表
    /// 
    /// 使用场景：
    /// - 连接运行在CosyVoice V3的TTL引擎
    /// - 适用于单机部署的TTL引擎服务
    /// 
    /// 依赖关系：
    /// - 继承自ANetworkTtlEngineConnector抽象类
    /// - 依赖System.Runtime.Serialization进行JSON反序列化
    /// </remarks>
    public class CosyVoiceV3LYttlEngineConnector : ANetworkTtlEngineConnector
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
        /// 初始化CosyVoiceV3TtlEngineConnector类的新实例
        /// </summary>
        public CosyVoiceV3LYttlEngineConnector()
            : base()
        {
        }

        #endregion

        #region ATtlEngineConnector 实现

        /// <summary>
        /// 获取引擎的唯一标识符
        /// </summary>
        public override string Id { get; } = "a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d";

        /// <summary>
        /// 获取连接器的名称
        /// </summary>
        public override string Name
        {
            get { return "CosyVoice V3（刘悦）"; }
        }

        /// <summary>
        /// 获取连接器的说明或详情
        /// </summary>
        public override string Description
        {
            get { return "刘悦的 CosyVoice V3 TTL引擎服务（刘悦的技术博客(B站/Youtube 同名) https://t.zsxq.com/IrQPr）\n支持特性：方言、情感风格、场景"; }
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
                    "连接地址，格式：http://ip:port，范例：http://localhost:9880"
                };
            }
        }

        /// <summary>
        /// 获取当前引擎所使用的多音字方案
        /// </summary>
        public override IPolyphonicScheme PolyphonicScheme
        {
            get { return new TonePinyinPolyphonicScheme(); }
        }

        /// <summary>
        /// 获取引擎支持的特性定义列表
        /// </summary>
        public override TtlEngineFeatureDefinition[] FeatureDefinitions
        {
            get { return _featureDefinitions; }
        }

        /// <summary>
        /// CosyVoice V3支持的特性定义数组
        /// </summary>
        private static readonly TtlEngineFeatureDefinition[] _featureDefinitions = new[]
        {
            new TtlEngineFeatureDefinition("方言", typeof(DialectFeature), "请用{0}表达。"),
            new TtlEngineFeatureDefinition("情感风格", typeof(EmotionFeature), "请用{0}表达。"),
            new TtlEngineFeatureDefinition("场景", typeof(SceneFeature), "请用{0}表达。")
        };

        /// <summary>
        /// 异步连接到TTL引擎，通过获取朗读者列表验证连接
        /// </summary>
        /// <returns>表示异步操作的任务</returns>
        /// <exception cref="Exception">连接失败时抛出异常</exception>
        public override async Task ConnectAsync()
        {
            if (GetConnectionStatus() == ConnectionStatus.Connected)
            {
                return;
            }

            OnConnectionStatusChanged(ConnectionStatus.Connecting, "正在连接CosyVoice V3引擎...");

            try
            {
                // 通过获取朗读者列表来验证连接
                var speakerList = await FetchSpeakersAsync();

                // 更新Speakers属性
                UpdateSpeakers(speakerList);

                OnConnectionStatusChanged(ConnectionStatus.Connected, "CosyVoice V3引擎连接成功");
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
                var speakerList = FetchSpeakersAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
                UpdateSpeakers(speakerList);
            }
            catch
            {
                // 发生异常时保持原有列表不变
            }
        }

        #endregion

        #endregion

        #region private

        #region 方法

        /// <summary>
        /// 异步获取朗读者列表，使用默认超时时间
        /// </summary>
        /// <returns>朗读者列表</returns>
        /// <exception cref="Exception">获取失败时抛出异常</exception>
        private Task<List<SpeakerInfo>> FetchSpeakersAsync()
        {
            return FetchSpeakersAsync(ConnectionTimeout);
        }

        /// <summary>
        /// 异步获取朗读者列表
        /// </summary>
        /// <param name="timeout">请求超时时间</param>
        /// <returns>朗读者列表</returns>
        /// <exception cref="Exception">获取失败时抛出异常</exception>
        private async Task<List<SpeakerInfo>> FetchSpeakersAsync(TimeSpan timeout)
        {
            var speakerList = new List<SpeakerInfo>();

            // 检查参数是否设置
            if (Parameters == null || Parameters.Length < 1)
            {
                throw new InvalidOperationException("未设置连接参数");
            }

            string baseUrl = Parameters[0];
            string speakersUrl = $"{baseUrl}/speakers";

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = timeout;
                var response = await httpClient.GetAsync(speakersUrl);
                response.EnsureSuccessStatusCode();

                var jsonStream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(SpeakerData[]));
                var speakerDataArray = (SpeakerData[])serializer.ReadObject(jsonStream);

                if (speakerDataArray != null)
                {
                    foreach (var speakerData in speakerDataArray)
                    {
                        if (!string.IsNullOrEmpty(speakerData.Name))
                        {
                            var speaker = new SpeakerInfo(speakerData.Name);
                            speakerList.Add(speaker);
                        }
                    }
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

            // 获取文本和说话人参数
            var text = parameters.Text ?? string.Empty;
            var speaker = parameters.Speaker;

            // 直接拼接查询字符串，不进行URL编码
            string baseUrl = this.Parameters[0];
            string url = $"{baseUrl}{_Url_Query_Path}{_Param_Text_Name}={text}&{_Param_Speaker_Name}={speaker}";

            // 构建instruct参数
            var instructParts = new List<string>();
            foreach (var featureDef in _featureDefinitions)
            {
                if (parameters.FeatureSelections != null &&
                    parameters.FeatureSelections.TryGetValue(featureDef.Name, out int selectedValue))
                {
                    object enumValue = featureDef.GetEnumValueByInt(selectedValue);
                    string instruction = featureDef.BuildInstruction(enumValue);
                    if (!string.IsNullOrEmpty(instruction))
                    {
                        instructParts.Add(instruction);
                    }
                }
            }

            if (instructParts.Count > 0)
            {
                string instruct = string.Join("", instructParts);
                url += $"&instruct={Uri.EscapeDataString(instruct)}";
            }

            return url;
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

            if (parameters.Length < 1)
            {
                throw new ArgumentException("Parameters must contain at least one item: connection address", nameof(parameters));
            }

            // 验证并处理第一个参数（网址）：确保末尾没有/
            string baseUrl = parameters[0];
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentException("Connection address cannot be null or empty", nameof(parameters));
            }

            // 简单验证URL格式
            if (!baseUrl.StartsWith("http://") && !baseUrl.StartsWith("https://"))
            {
                throw new ArgumentException("Connection address must start with http:// or https://", nameof(parameters));
            }

            // 移除末尾的/
            if (baseUrl.EndsWith("/"))
            {
                parameters[0] = baseUrl.TrimEnd('/');
            }
        }

        /// <summary>
        /// 执行任务并获取音频文件路径
        /// </summary>
        /// <param name="parameters">TTL引擎参数</param>
        /// <returns>音频文件路径</returns>
        protected override async Task<string> ExecuteTaskAsync(TtlEngineParameters parameters)
        {
            // 执行网络任务获取音频数据
            var audioData = await base.ExecuteNetworkTaskAsync(parameters);

            // 确定输出文件路径
            string outputFilePath;
            if (!string.IsNullOrWhiteSpace(parameters.OutputFilePath))
            {
                // 使用指定的输出路径
                outputFilePath = parameters.OutputFilePath;
            }
            else
            {
                // 使用默认临时文件路径
                outputFilePath = Path.GetTempFileName() + ".wav";
            }

            // 确保目录存在
            string directory = Path.GetDirectoryName(outputFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 保存音频数据到文件
            await Task.Run(() => File.WriteAllBytes(outputFilePath, audioData));
            return outputFilePath;
        }

        #endregion

        #endregion

        #region 嵌套类型

        /// <summary>
        /// 朗读者数据契约类，用于JSON反序列化
        /// </summary>
        [DataContract]
        private class SpeakerData
        {
            /// <summary>
            /// 获取或设置朗读者名称
            /// </summary>
            [DataMember(Name = "name")]
            public string Name { get; set; }

            /// <summary>
            /// 获取或设置朗读者ID
            /// </summary>
            [DataMember(Name = "voice_id")]
            public string VoiceId { get; set; }
        }

        #endregion

    }
}