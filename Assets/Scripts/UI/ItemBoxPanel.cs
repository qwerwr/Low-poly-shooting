using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// 物品箱面板
    /// 显示物品箱物品和刷新逻辑
    /// </summary>
    public class ItemBoxPanel : MonoBehaviour
    {
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
        public int SlotsPerRow = 4;

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
        /// 物品箱模型
        /// </summary>
        private ItemBoxModel m_ItemBoxModel;

        /// <summary>
        /// 初始化物品箱面板
        /// </summary>
        /// <param name="inventoryManager">背包管理器</param>
        public void Initialize(InventoryManager inventoryManager)
        {
            m_InventoryManager = inventoryManager;
            m_ItemBoxModel = GameArchitecture.Interface.GetModel<ItemBoxModel>();

            // 创建物品槽
            CreateItemSlots();

            // 更新UI
            UpdateItemBoxUI();
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
            for (int i = 0; i < m_ItemBoxModel.MaxCapacity; i++)
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
        /// 更新物品箱UI
        /// </summary>
        public void UpdateItemBoxUI()
        {
            // 清空所有物品
            ClearItems();

            // 获取物品箱物品数据
            List<InventoryItemData> inventoryItems = m_ItemBoxModel.Items;

            // 创建物品
            for (int i = 0; i < inventoryItems.Count && i < m_ItemSlots.Count; i++)
            {
                InventoryItemData itemData = inventoryItems[i];
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
        }

        /// <summary>
        /// 刷新物品箱
        /// </summary>
        public void RefreshItemBox()
        {
            // 调用物品箱模型的刷新方法
            m_ItemBoxModel.RefreshItems();

            // 更新UI
            UpdateItemBoxUI();
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
        /// 显示物品箱面板
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            UpdateItemBoxUI();
        }

        /// <summary>
        /// 隐藏物品箱面板
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 切换物品箱面板显示
        /// </summary>
        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            if (gameObject.activeSelf)
            {
                UpdateItemBoxUI();
            }
        }

        /// <summary>
        /// 获取物品箱模型
        /// </summary>
        public ItemBoxModel GetItemBoxModel()
        {
            return m_ItemBoxModel;
        }

        /// <summary>
        /// 更新物品箱数据模型
        /// 从UI同步数据到ItemBoxModel
        /// </summary>
        public void UpdateItemBoxModel()
        {
            // 清空现有物品
            m_ItemBoxModel.Items.Clear();

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
                        SlotIndex = slot.SlotIndex,
                        ItemRef = item.ItemData
                    };

                    // 添加到物品箱模型
                    m_ItemBoxModel.Items.Add(itemData);
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