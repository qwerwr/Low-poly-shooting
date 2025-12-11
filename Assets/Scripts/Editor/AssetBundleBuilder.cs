using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace Game.Editor
{
    /// <summary>
    /// AssetBundle构建脚本
    /// </summary>
    public class AssetBundleBuilder
    {
        private const string ASSETS_PATH = "Assets/Resources/GameData/";
        private const string ASSETBUNDLE_OUTPUT_PATH = "Assets/StreamingAssets/AssetBundles/";

        /// <summary>
        /// 编辑器菜单选项：构建所有AssetBundle
        /// </summary>
        [MenuItem("GameTools/Build AssetBundles")]
        public static void BuildAllAssetBundles()
        {
            // 创建输出目录
            EnsureOutputDirectoryExists();

            // 设置AssetBundle标签
            SetAssetBundleLabels();

            // 构建AssetBundle
            BuildAssetBundles();

            Debug.Log("AssetBundle构建完成！");
        }

        /// <summary>
        /// 确保输出目录存在
        /// </summary>
        private static void EnsureOutputDirectoryExists()
        {
            // 输出详细日志
            Debug.Log($"检查AssetBundle输出目录：{ASSETBUNDLE_OUTPUT_PATH}");

            if (!Directory.Exists(ASSETBUNDLE_OUTPUT_PATH))
            {
                Debug.Log($"创建AssetBundle输出目录：{ASSETBUNDLE_OUTPUT_PATH}");
                Directory.CreateDirectory(ASSETBUNDLE_OUTPUT_PATH);

                // 验证目录是否创建成功
                if (Directory.Exists(ASSETBUNDLE_OUTPUT_PATH))
                {
                    Debug.Log($"AssetBundle输出目录创建成功：{ASSETBUNDLE_OUTPUT_PATH}");
                }
                else
                {
                    Debug.LogError($"AssetBundle输出目录创建失败：{ASSETBUNDLE_OUTPUT_PATH}");
                }
            }
            else
            {
                Debug.Log($"AssetBundle输出目录已存在：{ASSETBUNDLE_OUTPUT_PATH}");
            }
        }

        /// <summary>
        /// 设置AssetBundle标签
        /// </summary>
        private static void SetAssetBundleLabels()
        {
            // 刷新AssetDatabase，确保所有资源都被正确识别
            AssetDatabase.Refresh();

            // 设置物品AssetBundle标签
            SetAssetBundleLabelsForDirectory(Path.Combine(ASSETS_PATH, "Items"), "items");

            // 设置角色AssetBundle标签
            SetAssetBundleLabelsForDirectory(Path.Combine(ASSETS_PATH, "Characters"), "characters");

            // 设置地图AssetBundle标签
            SetAssetBundleLabelsForDirectory(Path.Combine(ASSETS_PATH, "Maps"), "maps");

            // 设置商店AssetBundle标签
            SetAssetBundleLabelsForDirectory(Path.Combine(ASSETS_PATH, "Shop"), "shop");

            // 再次刷新AssetDatabase，确保标签设置生效
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 为指定目录下的所有资产设置AssetBundle标签
        /// </summary>
        /// <param name="directoryPath">目录路径</param>
        /// <param name="bundleName">AssetBundle名称</param>
        private static void SetAssetBundleLabelsForDirectory(string directoryPath, string bundleName)
        {
            Debug.Log($"设置AssetBundle标签，目录：{directoryPath}，bundleName：{bundleName}");

            if (!Directory.Exists(directoryPath))
            {
                Debug.LogWarning($"目录不存在，跳过设置标签：{directoryPath}");
                return;
            }

            // 获取目录下所有.asset文件
            string[] assetFiles = Directory.GetFiles(directoryPath, "*.asset", SearchOption.AllDirectories);

            Debug.Log($"找到 {assetFiles.Length} 个.asset文件");

            foreach (string assetFile in assetFiles)
            {
                // 获取相对路径
                string relativePath = assetFile.Replace(Application.dataPath, "Assets");

                // 统一路径格式为Unity标准的正斜杠
                relativePath = relativePath.Replace('\\', '/');

                Debug.Log($"处理文件：{relativePath}");

                // 获取AssetImporter
                AssetImporter importer = AssetImporter.GetAtPath(relativePath);
                if (importer != null)
                {
                    // 设置AssetBundle标签
                    importer.assetBundleName = bundleName;
                    importer.assetBundleVariant = "";
                    Debug.Log($"设置AssetBundle标签：{relativePath} -> {bundleName}");
                }
                else
                {
                    Debug.LogWarning($"无法获取AssetImporter：{relativePath}");
                }
            }
        }

        /// <summary>
        /// 构建AssetBundle
        /// </summary>
        private static void BuildAssetBundles()
        {
            // 输出构建目标
            Debug.Log($"=== 开始构建AssetBundle ===");
            Debug.Log($"构建目标平台：{EditorUserBuildSettings.activeBuildTarget}");

            // 使用绝对路径确保路径正确
            string absoluteOutputPath = Path.GetFullPath(ASSETBUNDLE_OUTPUT_PATH);
            Debug.Log($"使用绝对路径构建：{absoluteOutputPath}");

            // 构建前检查目录状态
            Debug.Log($"构建前，输出目录存在：{Directory.Exists(absoluteOutputPath)}");
            if (Directory.Exists(absoluteOutputPath))
            {
                string[] preBuildFiles = Directory.GetFiles(absoluteOutputPath);
                Debug.Log($"构建前，输出目录包含 {preBuildFiles.Length} 个文件");
                foreach (string file in preBuildFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    Debug.Log($"  - {fileInfo.Name} ({fileInfo.Length} bytes)");
                }
            }

            // 构建AssetBundle，使用LZ4压缩算法
            Debug.Log("开始执行BuildPipeline.BuildAssetBundles...");
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(
                absoluteOutputPath,
                BuildAssetBundleOptions.ChunkBasedCompression,
                EditorUserBuildSettings.activeBuildTarget
            );
            Debug.Log($"BuildPipeline.BuildAssetBundles执行完成，返回manifest：{manifest != null}");

            // 检查构建结果
            if (manifest != null)
            {
                Debug.Log("AssetBundle构建成功！");

                // 输出构建的AssetBundle列表
                Debug.Log("构建的AssetBundle文件：");
                string[] bundleNames = manifest.GetAllAssetBundles();
                Debug.Log($"manifest返回了 {bundleNames.Length} 个AssetBundle名称");

                foreach (string bundleName in bundleNames)
                {
                    string bundlePath = Path.Combine(absoluteOutputPath, bundleName);
                    if (File.Exists(bundlePath))
                    {
                        FileInfo fileInfo = new FileInfo(bundlePath);
                        Debug.Log($"  - {bundleName} ({fileInfo.Length} bytes) 路径：{bundlePath}");
                    }
                    else
                    {
                        Debug.LogWarning($"  - {bundleName} (文件不存在) 预期路径：{bundlePath}");

                        // 检查是否有带后缀的版本
                        string[] possibleExtensions = { ".unity3d", ".bundle" };
                        foreach (string ext in possibleExtensions)
                        {
                            string extBundlePath = bundlePath + ext;
                            if (File.Exists(extBundlePath))
                            {
                                FileInfo fileInfo = new FileInfo(extBundlePath);
                                Debug.Log($"    发现带后缀的版本：{bundleName}{ext} ({fileInfo.Length} bytes) 路径：{extBundlePath}");
                            }
                        }
                    }
                }

                // 输出所有文件列表
                Debug.Log("输出目录文件列表：");
                if (Directory.Exists(absoluteOutputPath))
                {
                    string[] files = Directory.GetFiles(absoluteOutputPath);
                    Debug.Log($"输出目录包含 {files.Length} 个文件");
                    foreach (string file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        Debug.Log($"  - {fileInfo.Name} ({fileInfo.Length} bytes) 完整路径：{file}");
                    }
                }
                else
                {
                    Debug.LogError($"输出目录不存在：{absoluteOutputPath}");
                }
            }
            else
            {
                Debug.LogError("AssetBundle构建失败！BuildPipeline.BuildAssetBundles返回null");

                // 构建失败后检查目录状态
                Debug.Log($"构建失败后，输出目录存在：{Directory.Exists(absoluteOutputPath)}");
                if (Directory.Exists(absoluteOutputPath))
                {
                    string[] postBuildFiles = Directory.GetFiles(absoluteOutputPath);
                    Debug.Log($"构建失败后，输出目录包含 {postBuildFiles.Length} 个文件");
                    foreach (string file in postBuildFiles)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        Debug.Log($"  - {fileInfo.Name} ({fileInfo.Length} bytes)");
                    }
                }
            }

            Debug.Log("=== AssetBundle构建流程结束 ===");
        }

        /// <summary>
        /// 编辑器菜单选项：清除所有AssetBundle标签
        /// </summary>
        [MenuItem("GameTools/Clear AssetBundle Labels")]
        public static void ClearAllAssetBundleLabels()
        {
            Debug.Log("=== 开始清除AssetBundle标签 ===");

            // 清除前刷新AssetDatabase
            AssetDatabase.Refresh();

            // 清除物品AssetBundle标签
            ClearAssetBundleLabelsForDirectory(Path.Combine(ASSETS_PATH, "Items"));

            // 清除角色AssetBundle标签
            ClearAssetBundleLabelsForDirectory(Path.Combine(ASSETS_PATH, "Characters"));

            // 清除地图AssetBundle标签
            ClearAssetBundleLabelsForDirectory(Path.Combine(ASSETS_PATH, "Maps"));

            // 清除商店AssetBundle标签
            ClearAssetBundleLabelsForDirectory(Path.Combine(ASSETS_PATH, "Shop"));

            // 清除后刷新AssetDatabase
            AssetDatabase.Refresh();

            Debug.Log("AssetBundle标签清除完成！");
        }

        /// <summary>
        /// 清除指定目录下所有资产的AssetBundle标签
        /// </summary>
        /// <param name="directoryPath">目录路径</param>
        private static void ClearAssetBundleLabelsForDirectory(string directoryPath)
        {
            Debug.Log($"清除目录 {directoryPath} 下的AssetBundle标签");

            if (!Directory.Exists(directoryPath))
            {
                Debug.LogWarning($"目录不存在，跳过清除：{directoryPath}");
                return;
            }

            // 获取目录下所有.asset文件
            string[] assetFiles = Directory.GetFiles(directoryPath, "*.asset", SearchOption.AllDirectories);
            Debug.Log($"找到 {assetFiles.Length} 个.asset文件");

            int clearedCount = 0;

            foreach (string assetFile in assetFiles)
            {
                try
                {
                    // 获取相对路径
                    string relativePath = assetFile.Replace(Application.dataPath, "Assets");

                    // 统一路径格式为Unity标准的正斜杠
                    relativePath = relativePath.Replace('\\', '/');

                    Debug.Log($"处理文件：{relativePath}");

                    // 获取AssetImporter
                    AssetImporter importer = AssetImporter.GetAtPath(relativePath);
                    if (importer != null)
                    {
                        // 清除AssetBundle标签
                        importer.assetBundleName = "";
                        importer.assetBundleVariant = "";
                        clearedCount++;
                        Debug.Log($"  清除成功");
                    }
                    else
                    {
                        Debug.LogWarning($"  无法获取AssetImporter");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"  处理文件时出错：{ex.Message}");
                }
            }

            Debug.Log($"清除完成，共处理 {clearedCount} 个文件");
        }
    }
}