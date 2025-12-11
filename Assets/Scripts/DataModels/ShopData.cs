using UnityEngine;
using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// 商店物品数据类
    /// </summary>
    [System.Serializable]
    public class ShopItemData
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public string ItemId;
        
        /// <summary>
        /// 物品价格
        /// </summary>
        public int Price;
        
        /// <summary>
        /// 物品引用（运行时设置）
        /// </summary>
        [System.NonSerialized]
        public ItemData ItemRef;
    }
    
    /// <summary>
    /// 商店数据类
    /// </summary>
    [CreateAssetMenu(fileName = "NewShop", menuName = "GameData/Shop")]
    public class ShopData : ScriptableObject
    {
        /// <summary>
        /// 商店ID
        /// </summary>
        public string ShopId;
        
        /// <summary>
        /// 商店名称
        /// </summary>
        public string ShopName;
        
        /// <summary>
        /// 初始金币
        /// </summary>
        public int InitialGold;
        
        /// <summary>
        /// 商店物品列表
        /// </summary>
        public List<ShopItemData> Items = new List<ShopItemData>();
        
        /// <summary>
        /// 添加商店物品
        /// </summary>
        /// <param name="item">商店物品数据</param>
        public void AddItem(ShopItemData item)
        {
            Items.Add(item);
        }
    }
}