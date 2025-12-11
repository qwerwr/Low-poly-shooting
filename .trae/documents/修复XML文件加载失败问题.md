## 问题分析

1. **错误信息**：无法加载XML文件: StreamingAssets/GameData.xml
2. **根本原因**：XMLDataLoader.cs中使用了`Resources.Load<TextAsset>()`来加载StreamingAssets文件夹下的文件，这是错误的
3. **技术限制**：
   - `Resources.Load()`只能加载Resources文件夹下的资源
   - StreamingAssets文件夹下的资源需要使用不同的加载方式

## 解决方案

修改XMLDataLoader.cs文件，使用正确的方式加载StreamingAssets文件夹下的XML文件：

1. **修改加载逻辑**：根据不同平台使用不同的加载方式
2. **Editor和Standalone平台**：使用`System.IO.File.ReadAllText()`直接读取文件
3. **保持兼容性**：确保代码在不同平台下都能正常工作

## 具体修改步骤

1. 打开`Assets/Scripts/DataLoaders/XMLDataLoader.cs`文件
2. 修改`LoadXMLData()`方法，替换错误的`Resources.Load()`调用
3. 使用`Application.streamingAssetsPath`获取正确的文件路径
4. 使用`System.IO.File.ReadAllText()`读取文件内容
5. 解析XML内容并初始化数据

## 预期效果

- 游戏能够成功加载StreamingAssets/GameData.xml文件
- 不再出现"无法加载XML文件"的错误
- 游戏数据能够正常初始化
- 角色控制功能能够正常工作

## 验证方法

1. 在Unity编辑器中运行游戏
2. 查看Console窗口，确认没有XML加载错误
3. 测试角色控制功能，确认游戏能够正常运行
4. 查看OnGUI调试信息，确认游戏数据已正确加载