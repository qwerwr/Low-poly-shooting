using QFramework;
using System.Collections.Generic;
using UnityEngine;


namespace Game.UI
{
    /// <summary>
    /// 背包管理器
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        /// <summary>
        /// 背包面板
        /// </summary>
        public InventoryPanel InventoryPanel;

        /// <summary>
        /// 人物面板
        /// </summary>
        public CharacterPanel CharacterPanel;

        /// <summary>
        /// 物品箱面板
        /// </summary>
        public ItemBoxPanel ItemBoxPanel;

        /// <summary>
        /// 仓库面板
        /// </summary>
        public WarehousePanel WarehousePanel;

        /// <summary>
        /// 物品悬浮信息
        /// </summary>
        public ItemTooltip ItemTooltip;

        /// <summary>
        /// WarehouseCanvas引用
        /// </summary>
        public Canvas WarehouseCanvas;

        /// <summary>
        /// ShopCanvas引用
        /// </summary>
        public Canvas ShopCanvas;

        /// <summary>
        /// 背包数据模型
        /// </summary>
        private InventoryModel m_InventoryModel;

        /// <summary>
        /// 人物模型
        /// </summary>
        private CharacterModel m_CharacterModel;

        /// <summary>
        /// 物品箱模型
        /// </summary>
        private ItemBoxModel m_ItemBoxModel;

        /// <summary>
        /// 游戏数据模型
        /// </summary>
        private GameDataModel m_GameDataModel;

        private void Start()
        {
            // 获取数据模型
            m_InventoryModel = GameArchitecture.Interface.GetModel<InventoryModel>();
            m_CharacterModel = GameArchitecture.Interface.GetModel<CharacterModel>();
            m_ItemBoxModel = GameArchitecture.Interface.GetModel<ItemBoxModel>();
            m_GameDataModel = GameArchitecture.Interface.GetModel<GameDataModel>();

            // 初始化背包面板
            if (InventoryPanel != null)
            {
                InventoryPanel.Initialize(this);
            }

            // 初始化人物面板
            if (CharacterPanel != null)
            {
                CharacterPanel.Initialize(this);
            }

            // 初始化物品箱面板
            if (ItemBoxPanel != null)
            {
                ItemBoxPanel.Initialize(this);
            }

            // 初始化仓库面板
            if (WarehousePanel != null)
            {
                WarehousePanel.Initialize(this);
            }

            // 测试添加物品
            TestAddItems();
        }

        /// <summary>
        /// 测试添加物品
        /// </summary>
        private void TestAddItems()
        {
            // 添加测试物品
            AddItem("weapon_pistol");
            AddItem("ammo_pistol_l1", 50);
            AddItem("helmet_l1");
            AddItem("armor_l1");
            AddItem("misc_gold", 100);
        }

        /// <summary>
        /// 添加物品到背包
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="quantity">数量</param>
        public void AddItem(string itemId, int quantity = 1)
        {
            // 调用数据模型添加物品
            bool success = m_InventoryModel.AddItem(itemId, quantity);
            if (success)
            {
                // 更新UI
                InventoryPanel.UpdateInventoryUI();
            }
        }

        /// <summary>
        /// 从背包移除物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="quantity">数量</param>
        public void RemoveItem(string itemId, int quantity = 1)
        {
            // 调用数据模型移除物品
            bool success = m_InventoryModel.RemoveItem(itemId, quantity);
            if (success)
            {
                // 更新UI
                InventoryPanel.UpdateInventoryUI();
            }
        }

        /// <summary>
        /// 处理物品拖拽到槽位
        /// 支持不同面板之间的物品拖拽
        /// </summary>
        /// <param name="draggedItem">拖拽的物品</param>
        /// <param name="targetSlot">目标槽位</param>
        public void HandleItemDrop(InventoryItem draggedItem, InventorySlot targetSlot)
        {
            if (draggedItem == null || targetSlot == null)
            {
                Debug.LogError($"[InventoryManager] HandleItemDrop - 物品或目标槽位为空");
                return;
            }

            // 日志：开始处理物品拖拽
            Debug.Log($"[InventoryManager] HandleItemDrop - 开始处理拖拽，物品: {draggedItem.ItemId}, 源槽位: {draggedItem.CurrentSlot.gameObject.name}, 目标槽位: {targetSlot.gameObject.name}");

            // 获取源槽位
            InventorySlot sourceSlot = draggedItem.CurrentSlot;

            Debug.Log($"[InventoryManager] HandleItemDrop - 源槽位父物体: {sourceSlot.transform.parent.name}, 目标槽位父物体: {targetSlot.transform.parent.name}");

            // 如果拖拽到空槽位
            if (targetSlot.CurrentItem == null)
            {
                Debug.Log($"[InventoryManager] HandleItemDrop - 目标槽位为空，移动物品到空槽位");

                // 移动物品到空槽位
                MoveItemToSlot(draggedItem, targetSlot);

                // 处理不同面板之间的物品转移
                HandlePanelTransfer(draggedItem, sourceSlot, targetSlot);
            }
            // 如果拖拽到有物品的槽位
            else
            {
                Debug.Log($"[InventoryManager] HandleItemDrop - 目标槽位已有物品: {targetSlot.CurrentItem.ItemId}");

                // 如果是相同物品且可堆叠
                if (draggedItem.ItemId == targetSlot.CurrentItem.ItemId && draggedItem.ItemData.CanStack)
                {
                    Debug.Log($"[InventoryManager] HandleItemDrop - 相同物品且可堆叠，合并物品");

                    // 合并物品
                    MergeItems(draggedItem, targetSlot.CurrentItem);

                    // 处理不同面板之间的物品转移
                    HandlePanelTransfer(draggedItem, sourceSlot, targetSlot);
                }
                // 否则交换物品
                else
                {
                    Debug.Log($"[InventoryManager] HandleItemDrop - 不同物品或不可堆叠，交换物品");

                    SwapItems(draggedItem, targetSlot.CurrentItem);
                }
            }

            // 更新所有面板UI
            Debug.Log($"[InventoryManager] HandleItemDrop - 更新所有面板UI");
            UpdateAllPanels();
        }

        /// <summary>
        /// 处理不同面板之间的物品转移
        /// 现在这个方法不再直接修改数据模型，而是通过调用各个面板的UpdateModel方法来同步数据
        /// </summary>
        private void HandlePanelTransfer(InventoryItem draggedItem, InventorySlot sourceSlot, InventorySlot targetSlot)
        {
            // 现在我们不再直接修改数据模型
            // 而是在MoveItemToSlot/MergeItems/SwapItems方法中更新UI后，调用各个面板的UpdateModel方法
            // 这样可以确保数据模型与UI保持一致
        }

        /// <summary>
        /// 检查是否是物品箱槽位
        /// </summary>
        private bool IsItemBoxSlot(InventorySlot slot)
        {
            // 简单判断：如果槽位索引小于物品箱最大容量，认为是物品箱槽位
            return slot.SlotIndex < m_ItemBoxModel.MaxCapacity;
        }

        /// <summary>
        /// 检查是否是背包槽位
        /// </summary>
        private bool IsBackpackSlot(InventorySlot slot)
        {
            // 简单判断：如果槽位索引小于背包最大容量，认为是背包槽位
            return slot.SlotIndex < m_InventoryModel.MaxCapacity;
        }

        /// <summary>
        /// 检查是否是人物面板普通槽位
        /// </summary>
        private bool IsCharacterNormalSlot(InventorySlot slot)
        {
            // 简单判断：如果槽位允许所有类型，认为是普通槽位
            return slot.AllowedItemType == ItemType.Misc;
        }

        /// <summary>
        /// 检查是否是人物面板特殊槽位
        /// </summary>
        private bool IsCharacterSpecialSlot(InventorySlot slot)
        {
            // 简单判断：如果槽位不允许所有类型，认为是特殊槽位
            return slot.AllowedItemType != ItemType.Misc;
        }

        /// <summary>
        /// 检查是否是仓库槽位
        /// </summary>
        private bool IsWarehouseSlot(InventorySlot slot)
        {
            // 简单判断：如果槽位索引小于仓库最大容量，认为是仓库槽位
            return slot.SlotIndex < GameArchitecture.Interface.GetModel<WarehouseModel>().MaxCapacity;
        }

        /// <summary>
        /// 更新所有面板UI
        /// </summary>
        private void UpdateAllPanels()
        {
            // 更新背包面板
            if (InventoryPanel != null)
            {
                InventoryPanel.UpdateInventoryUI();
            }

            // 更新人物面板
            if (CharacterPanel != null)
            {
                CharacterPanel.UpdateCharacterUI();
            }

            // 更新物品箱面板
            if (ItemBoxPanel != null)
            {
                ItemBoxPanel.UpdateItemBoxUI();
            }

            // 更新仓库面板
            if (WarehousePanel != null)
            {
                WarehousePanel.UpdateWarehouseUI();
            }
        }

        /// <summary>
        /// 移动物品到槽位
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="slot">槽位</param>
        private void MoveItemToSlot(InventoryItem item, InventorySlot slot)
        {
            // 日志：开始移动物品
            Debug.Log($"[InventoryManager] MoveItemToSlot - 开始移动物品: {item.ItemId}, 源槽位: {item.CurrentSlot.gameObject.name}, 目标槽位: {slot.gameObject.name}");

            // 获取源槽位
            InventorySlot sourceSlot = item.CurrentSlot;

            // 更新物品的当前槽位
            item.CurrentSlot = slot;
            Debug.Log($"[InventoryManager] MoveItemToSlot - 更新物品当前槽位为: {slot.gameObject.name}");

            // 清空源槽位
            if (sourceSlot != null)
            {
                sourceSlot.ClearSlot();
                Debug.Log($"[InventoryManager] MoveItemToSlot - 清空源槽位: {sourceSlot.gameObject.name}");
            }

            // 设置目标槽位物品
            slot.SetItem(item);
            Debug.Log($"[InventoryManager] MoveItemToSlot - 设置目标槽位物品: {slot.gameObject.name}");

            // 更新物品的父物体
            item.transform.SetParent(slot.transform);
            item.transform.localPosition = Vector3.zero;
            Debug.Log($"[InventoryManager] MoveItemToSlot - 更新物品父物体为: {slot.transform.name}");

            // 更新所有数据模型
            Debug.Log($"[InventoryManager] MoveItemToSlot - 更新所有数据模型");
            UpdateAllModels();
        }

        /// <summary>
        /// 合并物品
        /// </summary>
        /// <param name="sourceItem">源物品</param>
        /// <param name="targetItem">目标物品</param>
        private void MergeItems(InventoryItem sourceItem, InventoryItem targetItem)
        {
            // 合并数量
            targetItem.Quantity += sourceItem.Quantity;
            targetItem.UpdateItemDisplay();

            // 移除源物品
            InventorySlot sourceSlot = sourceItem.CurrentSlot;
            if (sourceSlot != null)
            {
                sourceSlot.ClearSlot();
            }

            // 销毁源物品
            Destroy(sourceItem.gameObject);

            // 更新所有数据模型
            UpdateAllModels();
        }

        /// <summary>
        /// 交换物品
        /// </summary>
        /// <param name="item1">物品1</param>
        /// <param name="item2">物品2</param>
        private void SwapItems(InventoryItem item1, InventoryItem item2)
        {
            // 保存源槽位
            InventorySlot slot1 = item1.CurrentSlot;
            InventorySlot slot2 = item2.CurrentSlot;

            // 交换物品的槽位引用
            item1.CurrentSlot = slot2;
            item2.CurrentSlot = slot1;

            // 交换槽位的物品引用
            slot1.SetItem(item2);
            slot2.SetItem(item1);

            // 交换物品的父物体
            Transform parent1 = item1.transform.parent;
            Transform parent2 = item2.transform.parent;

            item1.transform.SetParent(parent2);
            item1.transform.localPosition = Vector3.zero;

            item2.transform.SetParent(parent1);
            item2.transform.localPosition = Vector3.zero;

            // 更新所有数据模型
            UpdateAllModels();
        }

        /// <summary>
        /// 更新所有面板的数据模型
        /// 从UI同步数据到各个数据模型
        /// </summary>
        private void UpdateAllModels()
        {
            // 日志：开始更新所有数据模型
            Debug.Log($"[InventoryManager] UpdateAllModels - 开始更新所有数据模型");

            // 更新背包数据模型
            Debug.Log($"[InventoryManager] UpdateAllModels - 更新背包数据模型");
            UpdateInventoryModel();

            // 更新人物面板数据模型
            if (CharacterPanel != null)
            {
                Debug.Log($"[InventoryManager] UpdateAllModels - 更新人物面板数据模型");
                CharacterPanel.UpdateCharacterModel();
            }
            else
            {
                Debug.LogWarning($"[InventoryManager] UpdateAllModels - CharacterPanel为空");
            }

            // 更新物品箱数据模型
            if (ItemBoxPanel != null)
            {
                Debug.Log($"[InventoryManager] UpdateAllModels - 更新物品箱数据模型");
                ItemBoxPanel.UpdateItemBoxModel();
            }
            else
            {
                Debug.LogWarning($"[InventoryManager] UpdateAllModels - ItemBoxPanel为空");
            }

            // 更新仓库数据模型
            if (WarehousePanel != null)
            {
                Debug.Log($"[InventoryManager] UpdateAllModels - 更新仓库数据模型");
                WarehousePanel.UpdateWarehouseModel();
            }
            else
            {
                Debug.LogWarning($"[InventoryManager] UpdateAllModels - WarehousePanel为空");
            }

            Debug.Log($"[InventoryManager] UpdateAllModels - 所有数据模型更新完成");
        }

        /// <summary>
        /// 更新背包数据模型
        /// </summary>
        private void UpdateInventoryModel()
        {
            // 获取背包数据模型
            InventoryModel inventoryModel = GetInventoryModel();
            if (inventoryModel == null)
                return;

            // 清空现有物品
            inventoryModel.ClearInventory();

            // 遍历所有槽位，更新数据模型
            foreach (InventorySlot slot in InventoryPanel.GetSlots())
            {
                if (slot.CurrentItem != null)
                {
                    // 添加物品到数据模型
                    inventoryModel.AddItem(slot.CurrentItem.ItemId, slot.CurrentItem.Quantity);
                }
            }
        }

        /// <summary>
        /// 显示物品悬浮信息
        /// </summary>
        /// <param name="item">物品</param>
        /// <param name="position">位置</param>
        public void ShowItemTooltip(InventoryItem item, Vector3 position)
        {
            if (ItemTooltip != null)
            {
                ItemTooltip.ShowItemInfo(item, position);
            }
        }

        /// <summary>
        /// 显示物品悬浮信息
        /// </summary>
        /// <param name="itemData">物品数据</param>
        /// <param name="position">位置</param>
        public void ShowItemTooltip(ItemData itemData, Vector3 position)
        {
            if (ItemTooltip != null)
            {
                ItemTooltip.ShowItemInfo(itemData, position);
            }
        }

        /// <summary>
        /// 隐藏物品悬浮信息
        /// </summary>
        public void HideItemTooltip()
        {
            if (ItemTooltip != null)
            {
                ItemTooltip.HideItemInfo();
            }
        }

        /// <summary>
        /// 获取背包数据模型
        /// </summary>
        public InventoryModel GetInventoryModel()
        {
            return m_InventoryModel;
        }

        /// <summary>
        /// 获取游戏数据模型
        /// </summary>
        public GameDataModel GetGameDataModel()
        {
            return m_GameDataModel;
        }

        /// <summary>
        /// 显示WarehouseCanvas
        /// </summary>
        public void ShowWarehouseCanvas()
        {
            if (WarehouseCanvas != null)
            {
                WarehouseCanvas.gameObject.SetActive(true);
                // 更新WarehouseCanvas中的仓库UI
                UpdateWarehouseInCanvas(CanvasType.Warehouse);
            }
            if (ShopCanvas != null)
            {
                ShopCanvas.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 显示ShopCanvas
        /// </summary>
        public void ShowShopCanvas()
        {
            if (WarehouseCanvas != null)
            {
                WarehouseCanvas.gameObject.SetActive(false);
            }
            if (ShopCanvas != null)
            {
                ShopCanvas.gameObject.SetActive(true);
                // 更新ShopCanvas中的仓库UI
                UpdateWarehouseInCanvas(CanvasType.Shop);
            }
        }

        /// <summary>
        /// 更新指定Canvas中的仓库UI
        /// </summary>
        /// <param name="canvasType">Canvas类型</param>
        private void UpdateWarehouseInCanvas(CanvasType canvasType)
        {
            // 查找指定Canvas类型的WarehousePanel
            WarehousePanel[] warehousePanels = FindObjectsOfType<WarehousePanel>();
            foreach (var panel in warehousePanels)
            {
                if (panel.CurrentCanvasType == canvasType)
                {
                    panel.UpdateWarehouseUI();
                    break;
                }
            }
        }

        /// <summary>
        /// 购买按钮点击事件
        /// </summary>
        public void OnBuyButtonClick()
        {
            ShowShopCanvas();
        }
    }
}