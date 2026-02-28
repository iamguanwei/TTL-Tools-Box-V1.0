using System;
using System.Collections.Generic;
using System.IO;

namespace GW.TTLtoolsBox.Core.FileAccesser
{
    /// <summary>
    /// INI格式存盘文件的读写类。
    /// </summary>
    public class IniFileAccesser
    {
        #region 常量

        /// <summary>
        /// 换行的替换符。
        /// </summary>
        protected const string _newLine_Replacement = "#换行#";

        #endregion

        #region public

        #region 属性

        /// <summary>
        /// 获取或设置一个值，表示是否在SetValue()之后立刻保存，而不需要调用Save()方法。
        /// </summary>
        public bool IsAutoSave { get; set; } = false;

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化IniFileHelper类的新实例。
        /// </summary>
        /// <param name="fileFullName">配置文件完整路径</param>
        public IniFileAccesser(string fileFullName)
        {
            if (string.IsNullOrWhiteSpace(fileFullName))
            {
                throw new ArgumentException("文件路径不能为空", nameof(fileFullName));
            }
            FileName = fileFullName;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取保存的文件名。
        /// </summary>
        public string FileName { get; private set; } = string.Empty;

        #endregion

        #region 方法

        /// <summary>
        /// 保存配置到文件。
        /// </summary>
        public virtual void Save()
        {
            if (_valueDic != null)
            {
                lock (_valueDic)
                {
                    try
                    {
                        string directory = Path.GetDirectoryName(FileName);
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        using (FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                foreach (var kv in _valueDic)
                                {
                                    sw.WriteLine($"{kv.Key} = {kv.Value}");
                                }
                            }
                        }
                    }
                    catch
                    {
                        // 错误不处理
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定名称的配置值。
        /// </summary>
        /// <param name="name">指定配置的名称。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>配置值。</returns>
        public virtual string GetValue(string name, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"{nameof(name)}不得为null或者空白");
            }

            string outValue = defaultValue;

            load();
            lock (_valueDic)
            {
                if (_valueDic.ContainsKey(name))
                {
                    outValue = _valueDic[name].Replace(_newLine_Replacement, "\r\n");
                }
                else
                {
                    SetValue(name, defaultValue);
                }
            }

            return outValue;
        }

        /// <summary>
        /// 设置指定名称的配置值。
        /// </summary>
        /// <param name="name">指定配置的名称。</param>
        /// <param name="value">配置值。</param>
        public virtual void SetValue(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"{nameof(name)}不得为null或者空白");
            }

            load();
            lock (_valueDic)
            {
                _valueDic[name] = (value ?? string.Empty).Replace("\r\n", _newLine_Replacement);
            }

            if (IsAutoSave)
            {
                Save();
            }
        }

        /// <summary>
        /// 设置指定名称的配置值。
        /// </summary>
        /// <param name="name">指定配置的名称。</param>
        /// <param name="value">配置值。</param>
        public virtual void SetValue(string name, object value)
        {
            SetValue(name, value == null ? string.Empty : value.ToString());
        }

        /// <summary>
        /// 检查指定名称的配置是否存在。
        /// </summary>
        /// <param name="name">指定配置的名称。</param>
        /// <returns>是否存在</returns>
        public virtual bool ContainsKey(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            load();
            lock (_valueDic)
            {
                return _valueDic.ContainsKey(name);
            }
        }

        /// <summary>
        /// 删除指定名称的配置。
        /// </summary>
        /// <param name="name">指定配置的名称。</param>
        public virtual void Remove(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            load();
            lock (_valueDic)
            {
                if (_valueDic.ContainsKey(name))
                {
                    _valueDic.Remove(name);
                }
            }

            if (IsAutoSave)
            {
                Save();
            }
        }

        /// <summary>
        /// 清空所有配置。
        /// </summary>
        public virtual void Clear()
        {
            if (_valueDic != null)
            {
                lock (_valueDic)
                {
                    _valueDic.Clear();
                }
            }

            if (IsAutoSave)
            {
                Save();
            }
        }

        #endregion

        #endregion

        #region private

        #region 配置文件操作

        /// <summary>
        /// 配置值的字典。
        /// </summary>
        private Dictionary<string, string> _valueDic = null;

        /// <summary>
        /// 加载配置文件。
        /// </summary>
        private void load()
        {
            if (_valueDic == null)
            {
                _valueDic = new Dictionary<string, string>();
                lock (_valueDic)
                {
                    try
                    {
                        if (File.Exists(FileName))
                        {
                            using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                using (StreamReader sr = new StreamReader(fs))
                                {
                                    while (!sr.EndOfStream)
                                    {
                                        var lineStr = sr.ReadLine();
                                        if (!string.IsNullOrWhiteSpace(lineStr))
                                        {
                                            lineStr = lineStr.Trim();
                                            if (!string.IsNullOrWhiteSpace(lineStr) &&
                                                !lineStr.StartsWith("#"))
                                            {
                                                var strArray = lineStr.Split('=');
                                                if (strArray.Length == 2)
                                                {
                                                    var name = strArray[0].Trim();
                                                    var value = strArray[1].Trim();
                                                    if (!string.IsNullOrWhiteSpace(name))
                                                    {
                                                        _valueDic[name] = value;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        // 错误不处理
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
