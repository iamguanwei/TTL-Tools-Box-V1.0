# CosyVoice描述更新计划

## 任务目标
在CosyVoice V3 TTL引擎连接器的Description属性中添加支持的特性信息。

## 修改内容

### 修改文件
`GW.TTLtoolsBox.Core\SystemOption\TtlEngine\CosyVoiceV3LYttlEngineConnector.cs`

### 修改位置
第86-89行的Description属性

### 修改前
```csharp
public override string Description
{
    get { return "刘悦的 CosyVoice V3 TTL引擎服务（刘悦的技术博客(B站/Youtube 同名) https://t.zsxq.com/IrQPr）"; }
}
```

### 修改后
```csharp
public override string Description
{
    get { return "刘悦的 CosyVoice V3 TTL引擎服务（刘悦的技术博客(B站/Youtube 同名) https://t.zsxq.com/IrQPr）\n支持特性：方言、情感风格、场景"; }
}
```

## 实现步骤
1. 修改CosyVoiceV3LYttlEngineConnector.cs中的Description属性
2. 编译验证
