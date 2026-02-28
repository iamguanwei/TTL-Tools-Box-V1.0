using System;
using GW.TTLtoolsBox.Core.FileAccesser;

namespace GW.TTLtoolsBox.WinFormUi.Helper
{
    /// <summary>
    /// 配置文件的静态工具类，继承自IniFileHelper。
    /// </summary>
    public static class Setting
    {
        #region 常量

        /// <summary>
        /// 配置文件名称（含路径）。
        /// </summary>
        public static string Setting_File_Full_Name = $@"{AppDomain.CurrentDomain.BaseDirectory}\Setting.ini";

        #endregion

        #region private

        /// <summary>
        /// 内部使用的IniFileAccesser实例。
        /// </summary>
        private static IniFileAccesser _iniFileAccesser = null;

        /// <summary>
        /// 获取IniFileHelper实例（延迟初始化）。
        /// </summary>
        private static IniFileAccesser getIniFileHelper()
        {
            if (_iniFileAccesser == null)
            {
                _iniFileAccesser = new IniFileAccesser(Setting_File_Full_Name);
            }
            return _iniFileAccesser;
        }

        #endregion

        #region public

        #region 属性

        /// <summary>
        /// 获取或设置一个值，表示是否在SetValue()之后立刻保存，而不需要调用Save()方法。
        /// </summary>
        public static bool IsAutoSave
        {
            get { return getIniFileHelper().IsAutoSave; }
            set { getIniFileHelper().IsAutoSave = value; }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 保存配置。
        /// </summary>
        public static void Save()
        {
            getIniFileHelper().Save();
        }

        /// <summary>
        /// 获取指定名称的配置值。
        /// </summary>
        /// <param name="name">指定配置的名称。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>配置值。</returns>
        public static string GetValue(string name, string defaultValue)
        {
            return getIniFileHelper().GetValue(name, defaultValue);
        }

        /// <summary>
        /// 设置指定名称的配置值。
        /// </summary>
        /// <param name="name">指定配置的名称。</param>
        /// <param name="value">配置值。</param>
        public static void SetValue(string name, string value)
        {
            getIniFileHelper().SetValue(name, value);
        }

        /// <summary>
        /// 设置指定名称的配置值。
        /// </summary>
        /// <param name="name">指定配置的名称。</param>
        /// <param name="value">配置值。</param>
        public static void SetValue(string name, object value)
        {
            getIniFileHelper().SetValue(name, value);
        }

        /// <summary>
        /// 检查指定名称的配置是否存在。
        /// </summary>
        /// <param name="name">指定配置的名称。</param>
        /// <returns>是否存在</returns>
        public static bool ContainsKey(string name)
        {
            return getIniFileHelper().ContainsKey(name);
        }

        /// <summary>
        /// 删除指定名称的配置。
        /// </summary>
        /// <param name="name">指定配置的名称。</param>
        public static void Remove(string name)
        {
            getIniFileHelper().Remove(name);
        }

        /// <summary>
        /// 清空所有配置。
        /// </summary>
        public static void Clear()
        {
            getIniFileHelper().Clear();
        }

        #endregion

        #endregion
    }
}
