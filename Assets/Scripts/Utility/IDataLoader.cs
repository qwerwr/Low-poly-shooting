using System.Collections.Generic;
using QFramework;

namespace Game
{
    /// <summary>
    /// 数据加载器接口
    /// 定义从数据源加载游戏数据的方法
    /// </summary>
    public interface IDataLoader : IUtility
    {
        /// <summary>
        /// 加载所有物品数据
        /// </summary>
        Dictionary<string, ItemData> LoadItems();
        
        /// <summary>
        /// 加载所有角色数据
        /// </summary>
        Dictionary<string, CharacterData> LoadCharacters();
        
        /// <summary>
        /// 加载所有地图数据
        /// </summary>
        Dictionary<int, MapData> LoadMaps();
        
        /// <summary>
        /// 加载商店数据
        /// </summary>
        ShopData LoadShop();
    }
}