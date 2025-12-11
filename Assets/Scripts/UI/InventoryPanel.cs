using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// 背包面板组件
    /// </summary>
    public class InventoryPanel : MonoBehaviour
    {
        /// <summary>
        /// 背包管理器
        /// </summary>
        private InventoryManager m_InventoryManager;

        /// <summary>
        /// 槽位容器
        /// </summary>
        public Transform SlotContainer;

        /// <summary>
        /// 槽位预制体
        /// </summary>
        public GameObject SlotPrefab;

        /// <summary>
        /// 物品预制体
        /// </summary>
        public GameObject ItemPrefab;

        /// <summary>
        /// 每行槽位数
        /// </summary>
        public int SlotsPerRow = 5;

        /// <summary>
        /// 槽位间距
        /// </summary>
        public float SlotSpacing = 10f;

        /// <summary>
        /// 背包容量
        /// </summary>
        public int MaxCapacity = 20;

        /// <summary>
        /// 槽位列表
        /// </summary>
        private List<InventorySlot> m_Slots = new List<InventorySlot>();

        /// <summary>
        /// 物品列表
        /// </summary>
        private List<InventoryItem> m_Items = new List<InventoryItem>();

        /// <summary>
        /// 初始化背包面板
        /// </summary>
        /// <param name="inventoryManager">背包管理器</param>
        public void Initialize(InventoryManager inventoryManager)
        {
            m_InventoryManager = inventoryManager;
            CreateSlots();
            UpdateInventoryUI();
        }

        /// <summary>
        /// 创建槽位
        /// </summary>
        private void CreateSlots()
        {
            for (int i = 0; i < MaxCapacity; i++)
            {
                // 计算位置
                int row = i / SlotsPerRow;
                int col = i % SlotsPerRow;
                Vector2 position = new Vector2(col * (SlotPrefab.GetComponent<RectTransform>().sizeDelta.x + SlotSpacing),
                                             -row * (SlotPrefab.GetComponent<RectTransform>().sizeDelta.y + SlotSpacing));

                // 创建槽位
                GameObject slotObj = Instantiate(SlotPrefab, SlotContainer);
                slotObj.GetComponent<RectTransform>().anchoredPosition = position;
                slotObj.name = $"Slot_{i}";

                // 设置槽位索引
                InventorySlot slot = slotObj.GetComponent<InventorySlot>();
                slot.SlotIndex = i;
                m_Slots.Add(slot);
            }
        }

        /// <summary>
        /// 更新背包UI
        /// </summary>
        public void UpdateInventoryUI()
        {
            // 清空所有物品
            ClearItems();

            // 获取背包物品数据
            List<InventoryItemData> inventoryItems = m_InventoryManager.GetInventoryModel().Items;

            // 创建物品
            for (int i = 0; i < inventoryItems.Count && i < m_Slots.Count; i++)
            {
                InventoryItemData itemData = inventoryItems[i];
                InventorySlot slot = m_Slots[i];

                // 清空槽位
                slot.ClearSlot();

                // 清空槽位中的所有子物体，确保只有一个物品显示
                ClearSlotChildren(slot.transform);

                // 创建物品
                GameObject itemObj = Instantiate(ItemPrefab, slot.transform);
                itemObj.transform.localPosition = Vector3.zero;
                itemObj.transform.localScale = Vector3.one;

                // 设置物品数据
                InventoryItem item = itemObj.GetComponent<InventoryItem>();
                item.ItemId = itemData.ItemId;
                item.Quantity = itemData.Quantity;
                item.CurrentSlot = slot;
                item.ItemData = itemData.ItemRef;

                // 设置物品图片
                if (item.ItemImage != null && item.ItemData != null && item.ItemData.Icon != null)
                {
                    item.ItemImage.sprite = item.ItemData.Icon;
                    item.ItemImage.gameObject.SetActive(true);
                }

                // 更新物品显示
                item.UpdateItemDisplay();

                // 设置槽位物品
                slot.SetItem(item);

                // 添加到物品列表
                m_Items.Add(item);
            }
        }

        /// <summary>
        /// 清空槽位中的所有子物体
        /// </summary>
        /// <param name="slotTransform">槽位变换组件</param>
        private void ClearSlotChildren(Transform slotTransform)
        {
            // 遍历并销毁所有子物体，但保留槽位本身的UI组件
            for (int i = slotTransform.childCount - 1; i >= 0; i--)
            {
                Transform child = slotTransform.GetChild(i);
                // 只销毁动态创建的物品预制体，不销毁槽位本身的UI组件
                // 假设动态创建的物品名称包含"ItemPrefab"
                if (child.name.Contains("ItemPrefab"))
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        /// <summary>
        /// 清空所有物品
        /// </summary>
        private void ClearItems()
        {
            // 清空槽位
            foreach (InventorySlot slot in m_Slots)
            {
                slot.ClearSlot();
            }

            // 销毁临时创建的物品
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
        /// 显示背包
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            UpdateInventoryUI();
        }

        /// <summary>
        /// 隐藏背包
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 切换背包显示
        /// </summary>
        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            if (gameObject.activeSelf)
            {
                UpdateInventoryUI();
            }
        }

        /// <summary>
        /// 获取槽位列表
        /// </summary>
        public List<InventorySlot> GetSlots()
        {
            return m_Slots;
        }

        /// <summary>
        /// 获取物品列表
        /// </summary>
        public List<InventoryItem> GetItems()
        {
            return m_Items;
        }
    }
}