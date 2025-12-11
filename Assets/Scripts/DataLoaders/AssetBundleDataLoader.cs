using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Game
{
    /// <summary>
    /// AssetBundle数据加载器
    /// </summary>
    public class AssetBundleDataLoader : IDataLoader
    {
        private const string ASSETBUNDLE_PATH = "AssetBundles/";

        // AssetBundle缓存
        private Dictionary<string, AssetBundle> m_AssetBundleCache = new Dictionary<string, AssetBundle>();

        // 数据缓存
        private Dictionary<string, ItemData> m_ItemsCache;
        private Dictionary<string, CharacterData> m_CharactersCache;
        private Dictionary<int, MapData> m_MapsCache;
        private ShopData m_ShopCache;

        public AssetBundleDataLoader()
        {
            // 初始化缓存
            m_ItemsCache = new Dictionary<string, ItemData>();
            m_CharactersCache = new Dictionary<string, CharacterData>();
            m_MapsCache = new Dictionary<int, MapData>();
        }

        /// <summary>
        /// 加载物品数据
        /// </summary>
        public Dictionary<string, ItemData> LoadItems()
        {
            return LoadDataFromAssetBundle<ItemData>("items", m_ItemsCache, item => item.Id);
        }

        /// <summary>
        /// 加载角色数据
        /// </summary>
        public Dictionary<string, CharacterData> LoadCharacters()
        {
            return LoadDataFromAssetBundle<CharacterData>("characters", m_CharactersCache, character => character.Id);
        }

        /// <summary>
        /// 加载地图数据
        /// </summary>
        public Dictionary<int, MapData> LoadMaps()
        {
            if (m_MapsCache.Count > 0)
            {
                return m_MapsCache;
            }

            // 加载地图AssetBundle
            AssetBundle mapsBundle = LoadAssetBundle("maps");
            if (mapsBundle == null) return m_MapsCache;

            // 获取所有地图Asset名称
            string[] assetNames = mapsBundle.GetAllAssetNames();

            foreach (string assetName in assetNames)
            {
                // 加载地图ScriptableObject
                MapData mapData = mapsBundle.LoadAsset<MapData>(assetName);
                if (mapData != null)
                {
                    if (int.TryParse(mapData.MapId, out int mapId))
                    {
                        m_MapsCache[mapId] = mapData;
                    }
                }
            }

            Debug.Log($"从AssetBundle成功加载 {m_MapsCache.Count} 个地图");
            return m_MapsCache;
        }

        /// <summary>
        /// 加载商店数据
        /// </summary>
        public ShopData LoadShop()
        {
            if (m_ShopCache != null)
            {
                return m_ShopCache;
            }

            // 加载商店AssetBundle
            AssetBundle shopBundle = LoadAssetBundle("shop");
            if (shopBundle == null) return m_ShopCache;

            // 加载商店ScriptableObject
            string[] assetNames = shopBundle.GetAllAssetNames();
            if (assetNames.Length > 0)
            {
                m_ShopCache = shopBundle.LoadAsset<ShopData>(assetNames[0]);
            }

            Debug.Log("从AssetBundle成功加载商店数据");
            return m_ShopCache;
        }

        /// <summary>
        /// 从AssetBundle加载数据的通用方法
        /// </summary>
        private Dictionary<string, T> LoadDataFromAssetBundle<T>(string bundleName, Dictionary<string, T> cache, System.Func<T, string> getIdFunc) where T : ScriptableObject
        {
            if (cache.Count > 0)
            {
                return cache;
            }

            // 加载AssetBundle
            AssetBundle bundle = LoadAssetBundle(bundleName);
            if (bundle == null) return cache;

            // 获取所有Asset名称
            string[] assetNames = bundle.GetAllAssetNames();

            foreach (string assetName in assetNames)
            {
                // 加载ScriptableObject
                T data = bundle.LoadAsset<T>(assetName);
                if (data != null)
                {
                    string id = getIdFunc(data);
                    cache[id] = data;
                }
            }

            Debug.Log($"从AssetBundle成功加载 {cache.Count} 个{bundleName}");
            return cache;
        }

        /// <summary>
        /// 加载AssetBundle
        /// </summary>
        private AssetBundle LoadAssetBundle(string bundleName)
        {
            // 检查缓存中是否已存在
            if (m_AssetBundleCache.ContainsKey(bundleName))
            {
                return m_AssetBundleCache[bundleName];
            }

            // 构建AssetBundle路径
            string bundlePath = Path.Combine(Application.streamingAssetsPath, ASSETBUNDLE_PATH, bundleName);

            // 加载AssetBundle
            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);

            if (bundle != null)
            {
                // 缓存AssetBundle
                m_AssetBundleCache[bundleName] = bundle;
                Debug.Log($"成功加载AssetBundle：{bundleName}");
            }
            else
            {
                Debug.LogError($"无法加载AssetBundle：{bundlePath}");
            }

            return bundle;
        }
    }
}