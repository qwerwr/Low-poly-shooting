using QFramework;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 背包物品数据
    /// </summary>
    [System.Serializable]
    public class InventoryItemData
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public string ItemId;
        
        /// <summary>
        /// 物品数量
        /// </summary>
        public int Quantity;
        
        /// <summary>
        /// 槽位索引
        /// </summary>
        public int SlotIndex;
        
        /// <summary>
        /// 物品引用
        /// </summary>
        public ItemData ItemRef;
    }

    /// <summary>
    /// 背包数据模型
    /// </summary>
    public class InventoryModel : AbstractModel
    {
        /// <summary>
        /// 背包最大容量
        /// </summary>
        public int MaxCapacity = 20;
        
        /// <summary>
        /// 背包物品列表
        /// </summary>
        private List<InventoryItemData> m_Items = new List<InventoryItemData>();
        
        /// <summary>
        /// 背包物品列表
        /// </summary>
        public List<InventoryItemData> Items
        {
            get { return m_Items; }
            set { m_Items = value; }
        }
        
        /// <summary>
        /// 初始化模型
        /// </summary>
        protected override void OnInit()
        {
            InitializeInventory();
        }
        
        /// <summary>
        /// 初始化背包
        /// </summary>
        private void InitializeInventory()
        {
            m_Items = new List<InventoryItemData>();
        }
        
        /// <summary>
        /// 添加物品到背包
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="quantity">数量</param>
        /// <returns>是否添加成功</returns>
        public bool AddItem(string itemId, int quantity = 1)
        {
            // 获取物品数据
            GameDataModel gameDataModel = this.GetModel<GameDataModel>();
            if (!gameDataModel.Items.ContainsKey(itemId))
            {
                Debug.LogError($"物品ID不存在：{itemId}");
                return false;
            }
            
            ItemData itemData = gameDataModel.Items[itemId];
            
            // 检查物品是否可堆叠
            if (itemData.CanStack)
            {
                // 查找已有堆叠
                foreach (var invItem in m_Items)
                {
                    if (invItem.ItemId == itemId)
                    {
                        invItem.Quantity += quantity;
                        return true;
                    }
                }
            }
            
            // 检查背包容量
            if (m_Items.Count >= MaxCapacity)
            {
                Debug.Log("背包已满");
                return false;
            }
            
            // 添加新物品
            InventoryItemData newItem = new InventoryItemData
            {
                ItemId = itemId,
                Quantity = quantity,
                SlotIndex = m_Items.Count,
                ItemRef = itemData
            };
            m_Items.Add(newItem);
            return true;
        }
        
        /// <summary>
        /// 从背包移除物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="quantity">数量</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveItem(string itemId, int quantity = 1)
        {
            for (int i = m_Items.Count - 1; i >= 0; i--)
            {
                var invItem = m_Items[i];
                if (invItem.ItemId == itemId)
                {
                    if (invItem.Quantity <= quantity)
                    {
                        m_Items.RemoveAt(i);
                    }
                    else
                    {
                        invItem.Quantity -= quantity;
                    }
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 交换物品槽位
        /// </summary>
        /// <param name="slotIndex1">槽位1索引</param>
        /// <param name="slotIndex2">槽位2索引</param>
        public void SwapItems(int slotIndex1, int slotIndex2)
        {
            if (slotIndex1 < 0 || slotIndex1 >= m_Items.Count || slotIndex2 < 0 || slotIndex2 >= m_Items.Count)
                return;
            
            // 交换槽位索引
            int tempIndex = m_Items[slotIndex1].SlotIndex;
            m_Items[slotIndex1].SlotIndex = m_Items[slotIndex2].SlotIndex;
            m_Items[slotIndex2].SlotIndex = tempIndex;
            
            // 交换列表中的位置
            InventoryItemData tempItem = m_Items[slotIndex1];
            m_Items[slotIndex1] = m_Items[slotIndex2];
            m_Items[slotIndex2] = tempItem;
        }
        
        /// <summary>
        /// 获取背包中物品的数量
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品数量</returns>
        public int GetItemQuantity(string itemId)
        {
            int quantity = 0;
            foreach (var invItem in m_Items)
            {
                if (invItem.ItemId == itemId)
                {
                    quantity += invItem.Quantity;
                }
            }
            return quantity;
        }
        
        /// <summary>
        /// 清空背包
        /// </summary>
        public void ClearInventory()
        {
            m_Items.Clear();
        }
    }
}