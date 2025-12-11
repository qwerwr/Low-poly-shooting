using UnityEngine;
using System.Collections;
using QFramework;

namespace Game
{
    /// <summary>
    /// 数据加载器测试脚本
    /// </summary>
    public class DataLoaderTest : MonoBehaviour
    {
        /// <summary>
        /// 测试开始
        /// </summary>
        private void Start()
        {
            Debug.Log("=== 数据加载器测试开始 ===");

            // 直接创建数据加载器进行测试，避免依赖Architecture
            TestDataLoaderDirectly(new XMLDataLoader(), "XML");
            TestDataLoaderDirectly(new AssetBundleDataLoader(), "AssetBundle");

            Debug.Log("=== 数据加载器测试结束 ===");
        }

        /// <summary>
        /// 直接测试数据加载器，不依赖Architecture
        /// </summary>
        /// <param name="dataLoader">数据加载器实例</param>
        /// <param name="loaderName">数据加载器名称</param>
        private void TestDataLoaderDirectly(IDataLoader dataLoader, string loaderName)
        {
            Debug.Log($"\n--- 测试{loaderName}数据加载器 ---");

            // 测试加载物品数据
            TestLoadItems(dataLoader);

            // 测试加载角色数据
            TestLoadCharacters(dataLoader);

            // 测试加载地图数据
            TestLoadMaps(dataLoader);

            // 测试加载商店数据
            TestLoadShop(dataLoader);
        }

        // 以下测试方法保持不变...
        private void TestLoadItems(IDataLoader dataLoader)
        {
            Debug.Log("测试加载物品数据...");

            var items = dataLoader.LoadItems();
            if (items != null && items.Count > 0)
            {
                Debug.Log($"成功加载 {items.Count} 个物品");

                // 打印前5个物品信息
                int count = 0;
                foreach (var item in items)
                {
                    Debug.Log($"  物品: {item.Value.Name} (ID: {item.Value.Id}, 类型: {item.Value.Type})");
                    count++;
                    if (count >= 5)
                        break;
                }
            }
            else
            {
                Debug.LogWarning("未加载到物品数据");
            }
        }

        private void TestLoadCharacters(IDataLoader dataLoader)
        {
            Debug.Log("测试加载角色数据...");

            var characters = dataLoader.LoadCharacters();
            if (characters != null && characters.Count > 0)
            {
                Debug.Log($"成功加载 {characters.Count} 个角色");

                // 打印所有角色信息
                foreach (var character in characters)
                {
                    Debug.Log($"  角色: {character.Value.Name} (ID: {character.Value.Id}, 类型: {character.Value.Type})");
                }
            }
            else
            {
                Debug.LogWarning("未加载到角色数据");
            }
        }

        private void TestLoadMaps(IDataLoader dataLoader)
        {
            Debug.Log("测试加载地图数据...");

            var maps = dataLoader.LoadMaps();
            if (maps != null && maps.Count > 0)
            {
                Debug.Log($"成功加载 {maps.Count} 个地图");

                // 打印所有地图信息
                foreach (var map in maps)
                {
                    Debug.Log($"  地图: {map.Value.MapName} (ID: {map.Value.MapId})");
                }
            }
            else
            {
                Debug.LogWarning("未加载到地图数据");
            }
        }

        private void TestLoadShop(IDataLoader dataLoader)
        {
            Debug.Log("测试加载商店数据...");

            var shop = dataLoader.LoadShop();
            if (shop != null)
            {
                Debug.Log($"成功加载商店数据: {shop.ShopName} (ID: {shop.ShopId})");

                if (shop.Items != null && shop.Items.Count > 0)
                {
                    Debug.Log($"商店包含 {shop.Items.Count} 个物品");

                    // 打印前5个商店物品信息
                    int count = 0;
                    foreach (var shopItem in shop.Items)
                    {
                        Debug.Log($"  商店物品: {shopItem.ItemId} (价格: {shopItem.Price})");
                        count++;
                        if (count >= 5)
                            break;
                    }
                }
            }
            else
            {
                Debug.LogWarning("未加载到商店数据");
            }
        }
    }
}