using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using QFramework;

namespace Game.UI
{
    /// <summary>
    /// Canvas类型枚举
    /// </summary>
    public enum CanvasType
    {
        Warehouse, // 仓库Canvas
        Shop       // 商店Canvas
    }

    /// <summary>
    /// 仓库面板组件
    /// 用于显示仓库中的物品和处理交互
    /// </summary>
    public class WarehousePanel : MonoBehaviour
    {
        /// <summary>
        /// 当前Canvas类型（通过Inspector手动设置）
        /// </summary>
        public CanvasType CurrentCanvasType = CanvasType.Warehouse;

        /// <summary>
        /// 物品槽容器
        /// </summary>
        public Transform ItemSlotsContainer;

        /// <summary>
        /// 物品槽预制体
        /// </summary>
        public GameObject ItemSlotPrefab;

        /// <summary>
        /// 物品预制体
        /// </summary>
        public GameObject ItemPrefab;

        /// <summary>
        /// 每行物品槽数量
        /// </summary>
        public int SlotsPerRow = 5;

        /// <summary>
        /// 物品槽间距
        /// </summary>
        public float SlotSpacing = 10f;

        /// <summary>
        /// 物品槽列表
        /// </summary>
        private List<InventorySlot> m_ItemSlots = new List<InventorySlot>();

        /// <summary>
        /// 物品列表
        /// </summary>
        private List<InventoryItem> m_Items = new List<InventoryItem>();

        /// <summary>
        /// 背包管理器
        /// </summary>
        private InventoryManager m_InventoryManager;

        /// <summary>
        /// 仓库模型
        /// </summary>
        private WarehouseModel m_WarehouseModel;

        /// <summary>
        /// 经济系统
        /// </summary>
        private EconomySystem m_EconomySystem;

        /// <summary>
        /// 是否已初始化
        /// </summary>
        private bool m_IsInitialized = false;

        /// <summary>
        /// Start方法
        /// 自动初始化仓库面板
        /// </summary>
        private void Start()
        {
            // 如果尚未初始化，尝试自动初始化
            if (!m_IsInitialized)
            {
                // 查找InventoryManager实例
                InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
                if (inventoryManager != null)
                {
                    Initialize(inventoryManager);
                }
                else
                {
                    Debug.LogWarning("[WarehousePanel] 找不到InventoryManager实例，无法自动初始化");
                }
            }
        }

        /// <summary>
        /// 初始化仓库面板
        /// </summary>
        /// <param name="inventoryManager">背包管理器</param>
        public void Initialize(InventoryManager inventoryManager)
        {
            // 如果已经初始化，直接返回
            if (m_IsInitialized)
            {
                return;
            }

            m_InventoryManager = inventoryManager;
            m_WarehouseModel = GameArchitecture.Interface.GetModel<WarehouseModel>();
            m_EconomySystem = GameArchitecture.Interface.GetSystem<EconomySystem>();

            // 创建物品槽
            CreateItemSlots();

            // 更新UI
            UpdateWarehouseUI();

            // 标记为已初始化
            m_IsInitialized = true;
        }

        /// <summary>
        /// 创建物品槽
        /// </summary>
        private void CreateItemSlots()
        {
            // 清空现有槽位
            foreach (Transform child in ItemSlotsContainer)
            {
                Destroy(child.gameObject);
            }

            m_ItemSlots.Clear();

            // 创建物品槽
            for (int i = 0; i < m_WarehouseModel.MaxCapacity; i++)
            {
                // 计算位置
                int row = i / SlotsPerRow;
                int col = i % SlotsPerRow;

                // 创建槽位
                GameObject slotObj = Instantiate(ItemSlotPrefab, ItemSlotsContainer);
                slotObj.transform.localPosition = new Vector3(col * (slotObj.GetComponent<RectTransform>().sizeDelta.x + SlotSpacing),
                                                           -row * (slotObj.GetComponent<RectTransform>().sizeDelta.y + SlotSpacing),
                                                           0);

                // 设置槽位索引
                InventorySlot slot = slotObj.GetComponent<InventorySlot>();
                slot.SlotIndex = i;

                // 添加到槽位列表
                m_ItemSlots.Add(slot);
            }
        }

        /// <summary>
        /// 更新仓库UI
        /// </summary>
        public void UpdateWarehouseUI()
        {
            // 检查m_WarehouseModel是否为null
            if (m_WarehouseModel == null)
            {
                Debug.LogWarning("[WarehousePanel] m_WarehouseModel为null，无法更新仓库UI");
                return;
            }

            // 清空现有物品
            ClearItems();

            // 获取仓库物品数据
            List<InventoryItemData> items = m_WarehouseModel.Items;

            // 创建物品
            for (int i = 0; i < items.Count && i < m_ItemSlots.Count; i++)
            {
                InventoryItemData itemData = items[i];
                InventorySlot slot = m_ItemSlots[i];

                // 清空槽位
                slot.ClearSlot();

                // 创建物品
                GameObject itemObj = Instantiate(ItemPrefab, slot.transform);
                itemObj.transform.localPosition = Vector3.zero;

                // 设置物品数据
                InventoryItem item = itemObj.GetComponent<InventoryItem>();
                item.ItemId = itemData.ItemId;
                item.Quantity = itemData.Quantity;
                item.CurrentSlot = slot;
                item.ItemData = itemData.ItemRef;

                // 更新物品显示
                item.UpdateItemDisplay();

                // 设置物品图片
                if (item.ItemImage != null && item.ItemData != null && item.ItemData.Icon != null)
                {
                    item.ItemImage.sprite = item.ItemData.Icon;
                    item.ItemImage.gameObject.SetActive(true);
                }

                // 设置槽位物品
                slot.SetItem(item);

                // 添加到物品列表
                m_Items.Add(item);
            }

            // 根据Canvas类型设置特定功能
            SetupCanvasSpecificFeatures();
        }

        /// <summary>
        /// 根据Canvas类型设置特定功能
        /// </summary>
        private void SetupCanvasSpecificFeatures()
        {
            if (CurrentCanvasType == CanvasType.Shop)
            {
                // 在ShopCanvas中，添加售卖功能
                EnableSellFunctionality();
            }
            else
            {
                // 在WarehouseCanvas中，移除售卖功能
                DisableSellFunctionality();
            }
        }

        /// <summary>
        /// 启用售卖功能
        /// </summary>
        private void EnableSellFunctionality()
        {
            // 为所有物品添加点击售卖事件
            foreach (var item in m_Items)
            {
                // 添加点击事件监听器
                AddSellEventListener(item);
            }
        }

        /// <summary>
        /// 禁用售卖功能
        /// </summary>
        private void DisableSellFunctionality()
        {
            // 移除所有物品的点击售卖事件
            foreach (var item in m_Items)
            {
                // 移除点击事件监听器
                RemoveSellEventListener(item);
            }
        }

        /// <summary>
        /// 添加售卖事件监听器
        /// </summary>
        /// <param name="item">物品</param>
        private void AddSellEventListener(InventoryItem item)
        {
            // 为物品添加Button组件（如果没有的话）
            Button itemButton = item.GetComponent<Button>();
            if (itemButton == null)
            {
                itemButton = item.gameObject.AddComponent<Button>();
            }

            // 移除现有监听器，避免重复添加
            itemButton.onClick.RemoveAllListeners();

            // 添加售卖事件监听器
            itemButton.onClick.AddListener(() => OnSellItem(item));
        }

        /// <summary>
        /// 移除售卖事件监听器
        /// </summary>
        /// <param name="item">物品</param>
        private void RemoveSellEventListener(InventoryItem item)
        {
            // 移除物品的点击事件
            Button itemButton = item.GetComponent<Button>();
            if (itemButton != null)
            {
                itemButton.onClick.RemoveAllListeners();
            }
        }

        /// <summary>
        /// 售卖物品
        /// </summary>
        /// <param name="item">物品</param>
        private void OnSellItem(InventoryItem item)
        {
            // 调用经济系统售卖物品
            bool success = m_EconomySystem.SellItem(item.ItemId, item.Quantity);
            if (success)
            {
                // 更新仓库UI
                UpdateWarehouseUI();
            }
        }

        /// <summary>
        /// 清空所有物品
        /// </summary>
        private void ClearItems()
        {
            // 清空所有槽位
            foreach (InventorySlot slot in m_ItemSlots)
            {
                slot.ClearSlot();
                ClearSlotItems(slot.transform);
            }

            // 销毁物品
            foreach (InventoryItem item in m_Items)
            {
                if (item != null && item.gameObject != null)
                {
                    Destroy(item.gameObject);
                }
            }

            // 清空物品列表
            m_Items.Clear();
        }

        /// <summary>
        /// 清空槽位中的物品
        /// </summary>
        private void ClearSlotItems(Transform slotTransform)
        {
            // 遍历并销毁所有子物体
            for (int i = slotTransform.childCount - 1; i >= 0; i--)
            {
                Transform child = slotTransform.GetChild(i);
                if (child.GetComponent<InventoryItem>() != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        /// <summary>
        /// 显示仓库面板
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            UpdateWarehouseUI();
        }

        /// <summary>
        /// 隐藏仓库面板
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 切换仓库面板显示
        /// </summary>
        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            if (gameObject.activeSelf)
            {
                UpdateWarehouseUI();
            }
        }

        /// <summary>
        /// 更新仓库数据模型
        /// 从UI同步数据到WarehouseModel
        /// </summary>
        public void UpdateWarehouseModel()
        {
            // 清空现有物品
            m_WarehouseModel.ClearItems();

            // 遍历所有物品槽
            foreach (InventorySlot slot in m_ItemSlots)
            {
                if (slot.CurrentItem != null)
                {
                    // 创建物品数据
                    InventoryItem item = slot.CurrentItem;
                    InventoryItemData itemData = new InventoryItemData
                    {
                        ItemId = item.ItemId,
                        Quantity = item.Quantity,
                        ItemRef = item.ItemData
                    };

                    // 添加到仓库模型
                    m_WarehouseModel.Items.Add(itemData);
                }
            }
        }

        /// <summary>
        /// 获取所有物品槽
        /// </summary>
        /// <returns>物品槽列表</returns>
        public List<InventorySlot> GetSlots()
        {
            return m_ItemSlots;
        }
    }
}