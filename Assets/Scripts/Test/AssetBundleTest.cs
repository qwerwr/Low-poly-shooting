using UnityEngine;
using QFramework;
using System.Collections.Generic;

namespace Game.Test
{
    /// <summary>
    /// AssetBundle测试脚本
    /// 用于验证AssetBundle是否成功加载
    /// </summary>
    public class AssetBundleTest : MonoBehaviour
    {
        /// <summary>
        /// 测试AssetBundle加载
        /// </summary>
        [ContextMenu("Test AssetBundle Loading")]
        public void TestAssetBundleLoading()
        {
            Debug.Log("=== 开始AssetBundle加载测试 ===");

            // 1. 确保使用AssetBundle数据加载器
            GameArchitecture.CurrentDataLoaderType = GameArchitecture.DataLoaderType.AssetBundle;
            Debug.Log($"当前数据加载器类型：{GameArchitecture.CurrentDataLoaderType}");

            // 2. 获取GameArchitecture实例，触发初始化
            IArchitecture architecture = GameArchitecture.Interface;
            Debug.Log("GameArchitecture实例获取成功");

            // 3. 获取IDataLoader实例
            IDataLoader dataLoader = architecture.GetUtility<IDataLoader>();
            if (dataLoader == null)
            {
                Debug.LogError("无法获取IDataLoader实例");
                return;
            }

            Debug.Log($"获取到数据加载器：{dataLoader.GetType().Name}");

            // 4. 测试加载不同类型的数据
            TestLoadItems(dataLoader);
            TestLoadCharacters(dataLoader);
            TestLoadMaps(dataLoader);
            TestLoadShop(dataLoader);

            Debug.Log("=== AssetBundle加载测试完成 ===");
        }

        /// <summary>
        /// 测试加载物品数据
        /// </summary>
        private void TestLoadItems(IDataLoader dataLoader)
        {
            Debug.Log("\n--- 测试加载物品数据 ---");

            Dictionary<string, ItemData> items = dataLoader.LoadItems();
            if (items != null && items.Count > 0)
            {
                Debug.Log($"成功加载 {items.Count} 个物品");

                // 打印前5个物品信息
                int count = 0;
                foreach (var item in items)
                {
                    Debug.Log($"物品ID：{item.Key}，名称：{item.Value.Name}，类型：{item.Value.Type}");
                    count++;
                    if (count >= 5) break;
                }
            }
            else
            {
                Debug.LogError("物品数据加载失败");
            }
        }

        /// <summary>
        /// 测试加载角色数据
        /// </summary>
        private void TestLoadCharacters(IDataLoader dataLoader)
        {
            Debug.Log("\n--- 测试加载角色数据 ---");

            Dictionary<string, CharacterData> characters = dataLoader.LoadCharacters();
            if (characters != null && characters.Count > 0)
            {
                Debug.Log($"成功加载 {characters.Count} 个角色");

                // 打印所有角色信息
                foreach (var character in characters)
                {
                    Debug.Log($"角色ID：{character.Key}，名称：{character.Value.Name}，类型：{character.Value.Type}");
                }
            }
            else
            {
                Debug.LogError("角色数据加载失败");
            }
        }

        /// <summary>
        /// 测试加载地图数据
        /// </summary>
        private void TestLoadMaps(IDataLoader dataLoader)
        {
            Debug.Log("\n--- 测试加载地图数据 ---");

            Dictionary<int, MapData> maps = dataLoader.LoadMaps();
            if (maps != null && maps.Count > 0)
            {
                Debug.Log($"成功加载 {maps.Count} 个地图");

                // 打印所有地图信息
                foreach (var map in maps)
                {
                    Debug.Log($"地图ID：{map.Key}，名称：{map.Value.MapName}，场景名：{map.Value.SceneName}");
                }
            }
            else
            {
                Debug.LogError("地图数据加载失败");
            }
        }

        /// <summary>
        /// 测试加载商店数据
        /// </summary>
        private void TestLoadShop(IDataLoader dataLoader)
        {
            Debug.Log("\n--- 测试加载商店数据 ---");

            ShopData shop = dataLoader.LoadShop();
            if (shop != null)
            {
                Debug.Log($"成功加载商店数据：{shop.ShopName}");
                Debug.Log($"初始金币：{shop.InitialGold}");
                Debug.Log($"商店物品数量：{shop.Items.Count}");

                // 打印商店物品信息
                foreach (var shopItem in shop.Items)
                {
                    Debug.Log($"商店物品：{shopItem.ItemId}，价格：{shopItem.Price}");
                }
            }
            else
            {
                Debug.LogError("商店数据加载失败");
            }
        }

        /// <summary>
        /// 切换数据加载器类型
        /// </summary>
        [ContextMenu("Switch to AssetBundle Loader")]
        public void SwitchToAssetBundleLoader()
        {
            GameArchitecture.CurrentDataLoaderType = GameArchitecture.DataLoaderType.AssetBundle;
            Debug.Log("已切换到AssetBundle数据加载器");
        }

        [ContextMenu("Switch to XML Loader")]
        public void SwitchToXMLLoader()
        {
            GameArchitecture.CurrentDataLoaderType = GameArchitecture.DataLoaderType.XML;
            Debug.Log("已切换到XML数据加载器");
        }


    }
}