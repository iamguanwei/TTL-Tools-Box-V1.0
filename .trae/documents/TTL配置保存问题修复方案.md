# TTL配置保存问题修复方案

## 一、问题分析

### 根本原因
经过代码搜索，确认**原来的保存和加载逻辑在重构时丢失了**。

### 原有格式
根据用户反馈，原来的 `setting.ini` 文件格式如下：
```
Params_b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e = http://localhost:9880#换行#H:\AI\IndexTTS\index-tts-2\voices
```

- **Key格式**: `Params_{engineId}`
- **Value格式**: 多个参数用 `#换行#` 分隔

### 当前问题
1. `SaveEngineParameters` 方法只设置内存参数，没有持久化到配置文件
2. 没有从配置文件加载保存的参数的逻辑

---

## 二、修改方案

### 2.1 修改 `TtlEngineConnectionManager.SaveEngineParameters` 方法
**文件**: `TtlEngineConnectionManager.cs`

**修改内容**:
```csharp
/// <summary>
/// 保存引擎参数
/// </summary>
/// <param name="parameters">参数数组</param>
public void SaveEngineParameters(string[] parameters)
{
    if (_currentEngine == null) return;
    
    // 设置到引擎对象内存
    _currentEngine.SetParameters(parameters);
    
    // 持久化到配置文件（使用原有格式：Params_{engineId}）
    string key = $"Params_{_currentEngine.Id}";
    string value = string.Join("#换行#", parameters);
    Setting.SetValue(key, value);
    Setting.Save();
}
```

### 2.2 新增 `LoadEngineParameters` 方法
**文件**: `TtlEngineConnectionManager.cs`

**新增方法**:
```csharp
/// <summary>
/// 从配置文件加载引擎参数
/// </summary>
/// <returns>参数数组，如果没有保存的参数则返回null</returns>
public string[] LoadEngineParameters()
{
    if (_currentEngine == null) return null;
    
    string key = $"Params_{_currentEngine.Id}";
    string value = Setting.GetValue(key, string.Empty);
    
    if (string.IsNullOrEmpty(value))
    {
        return null;
    }
    
    return value.Split(new string[] { "#换行#" }, StringSplitOptions.None);
}
```

### 2.3 修改 `TtlSchemePanel` 加载参数的逻辑
**文件**: `TtlSchemePanel.cs`

**修改位置**: `cb_TTL方案_当前方案名称_SelectedIndexChanged` 方法中加载参数的部分（约第1254行）

**修改内容**:
```csharp
// 优先从配置文件加载保存的参数
var savedParameters = ConnectionManager?.LoadEngineParameters();
if (savedParameters != null && savedParameters.Length > 0)
{
    this.tb_TTL方案_连接参数配置.Text = string.Join("\r\n", savedParameters);
}
else
{
    // 如果没有保存的参数，使用引擎的当前参数或默认参数描述
    this.tb_TTL方案_连接参数配置.Text = ttl.Parameters == null || ttl.Parameters.Length == 0
        ? string.Join("\r\n", ttl.ParameterDescriptions)
        : string.Join("\r\n", ttl.Parameters);
}
```

### 2.4 添加引用
**文件**: `TtlEngineConnectionManager.cs`

**添加引用**:
```csharp
using GW.TTLtoolsBox.WinFormUi.Helper;
```

---

## 三、修改文件清单

| 文件 | 修改内容 |
|------|----------|
| `TtlEngineConnectionManager.cs` | 1. 添加 `using GW.TTLtoolsBox.WinFormUi.Helper;` 引用<br>2. 修改 `SaveEngineParameters` 方法添加持久化逻辑<br>3. 新增 `LoadEngineParameters` 方法 |
| `TtlSchemePanel.cs` | 修改 `cb_TTL方案_当前方案名称_SelectedIndexChanged` 方法中加载参数的逻辑 |

---

## 四、实施步骤

1. 在 `TtlEngineConnectionManager.cs` 添加 `using GW.TTLtoolsBox.WinFormUi.Helper;` 引用
2. 修改 `TtlEngineConnectionManager.SaveEngineParameters` 方法添加持久化逻辑（使用原有格式）
3. 在 `TtlEngineConnectionManager.cs` 新增 `LoadEngineParameters` 方法
4. 修改 `TtlSchemePanel.cb_TTL方案_当前方案名称_SelectedIndexChanged` 方法中加载参数的逻辑

---

## 五、兼容性说明

此方案完全兼容原有的配置文件格式：
- Key: `Params_{engineId}`
- Value: 参数用 `#换行#` 分隔

`IniFileAccesser` 类已经内置了换行符处理逻辑，会将 `\r\n` 自动替换为 `#换行#`，所以直接使用 `Setting.SetValue` 和 `Setting.GetValue` 即可。
