using QFramework;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 物品箱模型
    /// 管理物品箱物品和刷新逻辑
    /// </summary>
    public class ItemBoxModel : AbstractModel
    {
        /// <summary>
        /// 物品箱物品列表
        /// </summary>
        private List<InventoryItemData> m_Items = new List<InventoryItemData>();
        
        /// <summary>
        /// 物品箱物品列表
        /// </summary>
        public List<InventoryItemData> Items
        {
            get { return m_Items; }
            set { m_Items = value; }
        }
        
        /// <summary>
        /// 物品箱最大容量
        /// </summary>
        public int MaxCapacity = 8;
        
        /// <summary>
        /// 初始化模型
        /// </summary>
        protected override void OnInit()
        {
            // 初始化物品列表
            m_Items = new List<InventoryItemData>();
            
            // 初始刷新物品
            RefreshItems();
        }
        
        /// <summary>
        /// 刷新物品箱物品
        /// 随机生成2-4个物品
        /// </summary>
        public void RefreshItems()
        {
            // 清空现有物品
            m_Items.Clear();
            
            // 随机生成2-4个物品
            int itemCount = Random.Range(2, 5);
            
            // 获取游戏数据模型
            GameDataModel gameDataModel = this.GetModel<GameDataModel>();
            
            // 随机选择物品
            List<string> itemIds = new List<string>(gameDataModel.Items.Keys);
            
            for (int i = 0; i < itemCount && i < MaxCapacity; i++)
            {
                // 随机选择物品ID
                string randomItemId = itemIds[Random.Range(0, itemIds.Count)];
                ItemData itemData = gameDataModel.Items[randomItemId];
                
                // 创建物品数据
                InventoryItemData inventoryItemData = new InventoryItemData
                {
                    ItemId = randomItemId,
                    Quantity = 1,
                    SlotIndex = i,
                    ItemRef = itemData
                };
                
                m_Items.Add(inventoryItemData);
            }
        }
        
        /// <summary>
        /// 添加物品到物品箱
        /// </summary>
        public bool AddItem(InventoryItemData itemData)
        {
            // 检查物品箱是否已满
            if (m_Items.Count >= MaxCapacity)
                return false;
            
            // 设置物品槽位索引
            itemData.SlotIndex = m_Items.Count;
            
            // 添加物品
            m_Items.Add(itemData);
            
            return true;
        }
        
        /// <summary>
        /// 从物品箱移除物品
        /// </summary>
        public bool RemoveItem(int slotIndex)
        {
            // 检查索引是否有效
            if (slotIndex < 0 || slotIndex >= m_Items.Count)
                return false;
            
            // 移除物品
            m_Items.RemoveAt(slotIndex);
            
            // 更新剩余物品的槽位索引
            for (int i = slotIndex; i < m_Items.Count; i++)
            {
                m_Items[i].SlotIndex = i;
            }
            
            return true;
        }
    }
}