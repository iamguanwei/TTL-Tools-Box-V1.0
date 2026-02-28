using System.Collections.Generic;

namespace GW.TTLtoolsBox.WinFormUi.Manager
{
    /// <summary>
    /// 角色文本项，用于绑定到角色和情绪指定表格。
    /// </summary>
    /// <remarks>
    /// 核心功能：
    /// - 存储角色和对应的文本
    /// - 存储特性选择信息
    /// 
    /// 使用场景：
    /// - 绑定到角色和情绪指定DataGridView
    /// - 用于角色映射和情绪指定功能
    /// 
    /// 依赖关系：
    /// - 无外部依赖
    /// </remarks>
    public class RoleTextItem
    {
        #region public

        #region 构造函数

        /// <summary>
        /// 初始化RoleTextItem类的新实例。
        /// </summary>
        public RoleTextItem()
        {
            Role = string.Empty;
            Text = string.Empty;
            FeatureSelections = new Dictionary<string, int>();
        }

        /// <summary>
        /// 使用指定的角色和文本初始化RoleTextItem类的新实例。
        /// </summary>
        /// <param name="role">角色。</param>
        /// <param name="text">文本。</param>
        public RoleTextItem(string role, string text)
        {
            Role = role;
            Text = text;
            FeatureSelections = new Dictionary<string, int>();
        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取或设置角色。
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 获取或设置文本。
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 获取或设置特性选择字典，键为特性名称，值为选中的枚举值（存储为int便于序列化）。
        /// </summary>
        public Dictionary<string, int> FeatureSelections { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// 创建当前对象的浅表克隆体。
        /// </summary>
        /// <returns>当前对象的浅表克隆体。</returns>
        public RoleTextItem Clone()
        {
            var clone = new RoleTextItem(Role, Text);
            foreach (var kvp in FeatureSelections)
            {
                clone.FeatureSelections[kvp.Key] = kvp.Value;
            }
            return clone;
        }

        #endregion

        #endregion
    }
}
