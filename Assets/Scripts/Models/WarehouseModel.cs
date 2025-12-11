using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 仓库数据模型
    /// 用于管理仓库中的物品
    /// </summary>
    public class WarehouseModel : AbstractModel
    {
        /// <summary>
        /// 仓库最大容量
        /// </summary>
        public int MaxCapacity { get; set; } = 40;

        /// <summary>
        /// 仓库中的物品列表
        /// </summary>
        public List<InventoryItemData> Items { get; private set; }

        /// <summary>
        /// 初始化仓库模型
        /// </summary>
        protected override void OnInit()
        {
            Items = new List<InventoryItemData>();
        }

        /// <summary>
        /// 添加物品到仓库
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="quantity">数量</param>
        /// <returns>是否添加成功</returns>
        public bool AddItem(string itemId, int quantity = 1)
        {
            // 查找是否已有相同物品
            InventoryItemData existingItem = Items.Find(item => item.ItemId == itemId);

            // 如果已有相同物品且可堆叠，增加数量
            if (existingItem != null && existingItem.ItemRef.CanStack)
            {
                existingItem.Quantity += quantity;
                return true;
            }

            // 如果仓库未满，添加新物品
            if (Items.Count < MaxCapacity)
            {
                // 获取物品数据
                GameDataModel gameDataModel = GameArchitecture.Interface.GetModel<GameDataModel>();
                if (!gameDataModel.Items.ContainsKey(itemId))
                {
                    Debug.LogError($"物品ID不存在：{itemId}");
                    return false;
                }

                ItemData itemData = gameDataModel.Items[itemId];

                // 创建新物品
                InventoryItemData newItem = new InventoryItemData
                {
                    ItemId = itemId,
                    Quantity = quantity,
                    ItemRef = itemData
                };

                Items.Add(newItem);
                return true;
            }

            // 仓库已满，添加失败
            return false;
        }

        /// <summary>
        /// 从仓库移除物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="quantity">数量</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveItem(string itemId, int quantity = 1)
        {
            // 查找物品
            InventoryItemData existingItem = Items.Find(item => item.ItemId == itemId);
            if (existingItem == null)
                return false;

            // 如果数量足够，减少数量
            if (existingItem.Quantity > quantity)
            {
                existingItem.Quantity -= quantity;
                return true;
            }

            // 如果数量正好，移除物品
            if (existingItem.Quantity == quantity)
            {
                Items.Remove(existingItem);
                return true;
            }

            // 数量不足，移除失败
            return false;
        }

        /// <summary>
        /// 从仓库移除物品（按索引）
        /// </summary>
        /// <param name="index">物品索引</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveItem(int index)
        {
            if (index >= 0 && index < Items.Count)
            {
                Items.RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 清空仓库
        /// </summary>
        public void ClearItems()
        {
            Items.Clear();
        }

        /// <summary>
        /// 检查仓库是否已满
        /// </summary>
        /// <returns>是否已满</returns>
        public bool IsFull()
        {
            return Items.Count >= MaxCapacity;
        }
    }
}