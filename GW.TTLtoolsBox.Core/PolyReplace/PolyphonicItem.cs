using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace GW.TTLtoolsBox.Core.PolyReplace
{
    /// <summary>
    /// 多音字替换项实体类。
    /// </summary>
    public class PolyphonicItem
    {
        #region 常量

        /// <summary>
        /// 字符串分隔符。
        /// </summary>
        private const string _to_String_Split = "###";

        #endregion

        #region private

        /// <summary>
        /// 静态缓存的多音字方案集合，所有实例共享。
        /// </summary>
        private static IPolyphonicScheme[] _cachedPolyphonicSchemes = null;

        /// <summary>
        /// 用于线程安全的锁对象。
        /// </summary>
        private static readonly object _cacheLock = new object();

        /// <summary>
        /// 实例级别的多音字方案数组，用于存储每个实例的方案引用。
        /// </summary>
        private IPolyphonicScheme[] _polyphonicSchemes = null;

        #endregion

        #region public

        #region 属性

        /// <summary>
        /// 原始文本（需要替换的多音字/文本）
        /// </summary>
        public string OriginalText { get; set; } = string.Empty;

        /// <summary>
        /// 获取方案集合（每个实例拥有独立的副本）。
        /// </summary>
        public IPolyphonicScheme[] PolyphonicSchemes
        {
            get
            {
                if (_polyphonicSchemes == null)
                {
                    _polyphonicSchemes = GetOrCreateCachedSchemesClone();
                }

                return _polyphonicSchemes;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 生成默认的替换字符串。
        /// </summary>
        public void BuildDefaultReplaceStrings()
        {
            foreach (var ps in PolyphonicSchemes) ps.BuildDefaultReplaceStrings(OriginalText);
        }

        /// <summary>
        /// 从字符串读取数据填充实例。
        /// </summary>
        /// <param name="str">字符串。</param>
        public void LoadFromString(string str)
        {
            if (str != null)
            {
                var strArray = str.Split(new string[] { _to_String_Split }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length > 0) OriginalText = strArray[0];
                else OriginalText = string.Empty;

                var subStrArray = strArray.Skip(1).ToArray();
                for (int i = 0; i < PolyphonicSchemes.Length; i++)
                {
                    foreach (var subStr in subStrArray)
                    {
                        if (PolyphonicSchemes[i].TryLoadFromString(subStr) == true) break;
                    }
                }
            }
        }

        #endregion

        #region override

        /// <summary>
        /// （已重写）将实例输出为字符串。
        /// </summary>
        /// <returns>字符串。</returns>
        public override string ToString()
        {
            string outValue = OriginalText;
            foreach (var ps in PolyphonicSchemes) outValue += $"{_to_String_Split}{ps}";

            return outValue;
        }

        #endregion

        #region 静态

        /// <summary>
        /// 清除静态缓存的多音字方案集合。
        /// </summary>
        public static void ClearCache()
        {
            lock (_cacheLock)
            {
                _cachedPolyphonicSchemes = null;
            }
        }

        /// <summary>
        /// 从PolyphonicItem[]生成DataTable。
        /// </summary>
        /// <param name="polyphonicItems">PolyphonicItem[]。</param>
        /// <returns>适配的DataTable。</returns>
        public static DataTable ConvertToDataTable(PolyphonicItem[] polyphonicItems)
        {
            if (polyphonicItems == null) throw new ArgumentNullException(nameof(polyphonicItems));

            DataTable outValue = new DataTable();

            outValue.Columns.AddRange(getDataColumns());
            foreach (var pi in polyphonicItems)
            {
                var dr = outValue.NewRow();

                int colIndex = 0;
                dr[colIndex++] = pi.OriginalText;

                foreach (var ps in pi.PolyphonicSchemes)
                {
                    dr[colIndex++] = string.Join("\r\n", ps.ReplaceStrings);
                }

                outValue.Rows.Add(dr);
            }

            return outValue;
        }

        /// <summary>
        /// 从DataTable生成PolyphonicItem[]。
        /// </summary>
        /// <param name="dt">DataTable。</param>
        /// <returns>PolyphonicItem[]。</returns>
        public static PolyphonicItem[] ConvertFromDataTable(DataTable dt)
        {
            if (dt == null) throw new ArgumentNullException(nameof(dt));

            List<PolyphonicItem> outValue = new List<PolyphonicItem>();

            foreach (DataRow dr in dt.Rows)
            {
                PolyphonicItem pi = new PolyphonicItem();

                int colIndex = 0;
                pi.OriginalText = dr[colIndex++].ToString();

                if (string.IsNullOrWhiteSpace(pi.OriginalText) == false)
                {
                    foreach (var ps in pi.PolyphonicSchemes)
                    {
                        ps.ReplaceStrings = dr[colIndex++].ToString().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    outValue.Add(pi);
                }
            }

            return outValue.ToArray();
        }

        #endregion

        #endregion

        #region private

        #region 静态缓存方法

        /// <summary>
        /// 获取或创建缓存的多音字方案集合。
        /// </summary>
        /// <returns>多音字方案数组。</returns>
        private static IPolyphonicScheme[] GetOrCreateCachedSchemes()
        {
            if (_cachedPolyphonicSchemes != null)
            {
                return _cachedPolyphonicSchemes;
            }

            lock (_cacheLock)
            {
                if (_cachedPolyphonicSchemes != null)
                {
                    return _cachedPolyphonicSchemes;
                }

                List<IPolyphonicScheme> psList = new List<IPolyphonicScheme>();

                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                Type interfaceType = typeof(IPolyphonicScheme);

                foreach (Type type in currentAssembly.GetTypes())
                {
                    if (!type.IsInterface
                        && !type.IsAbstract
                        && !type.IsGenericTypeDefinition
                        && interfaceType.IsAssignableFrom(type))
                    {
                        try
                        {
                            psList.Add(Activator.CreateInstance(type) as IPolyphonicScheme);
                        }
                        catch
                        {
                            // 错误不处理
                        }
                    }
                }

                _cachedPolyphonicSchemes = psList.ToArray();
                return _cachedPolyphonicSchemes;
            }
        }

        /// <summary>
        /// 获取缓存方案集合的独立克隆副本。
        /// </summary>
        /// <returns>多音字方案数组的克隆副本。</returns>
        private static IPolyphonicScheme[] GetOrCreateCachedSchemesClone()
        {
            var cachedSchemes = GetOrCreateCachedSchemes();
            IPolyphonicScheme[] clones = new IPolyphonicScheme[cachedSchemes.Length];
            for (int i = 0; i < cachedSchemes.Length; i++)
            {
                clones[i] = cachedSchemes[i].Clone();
            }
            return clones;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取数据标题集合。
        /// </summary>
        /// <returns>数据标题集合。</returns>
        private static DataColumn[] getDataColumns()
        {
            List<DataColumn> outValue = new List<DataColumn>();

            outValue.Add(new DataColumn("原始文本", typeof(string)));

            foreach (var ps in GetOrCreateCachedSchemes())
            {
                outValue.Add(new DataColumn($"{ps.Name} [{ps.Description}]", typeof(string)));
            }

            return outValue.ToArray();
        }

        #endregion

        #endregion
    }
}
