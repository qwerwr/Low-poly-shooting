using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Game.UI
{
    /// <summary>
    /// 背包槽位组件
    /// </summary>
    public class InventorySlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// 槽位索引
        /// </summary>
        public int SlotIndex;

        /// <summary>
        /// 当前槽位中的物品
        /// </summary>
        public InventoryItem CurrentItem { get; private set; }

        /// <summary>
        /// 槽位背景
        /// </summary>
        public Image SlotBackground;

        /// <summary>
        /// 高亮颜色
        /// </summary>
        public Color HighlightColor = new Color(1, 1, 1, 0.5f);

        /// <summary>
        /// 正常颜色
        /// </summary>
        public Color NormalColor = new Color(1, 1, 1, 1);

        /// <summary>
        /// 允许放置的物品类型
        /// </summary>
        public ItemType AllowedItemType = ItemType.Misc; // Misc表示允许所有类型

        /// <summary>
        /// 背包管理器
        /// </summary>
        private InventoryManager m_InventoryManager;

        private void Start()
        {
            // 获取背包管理器
            m_InventoryManager = FindObjectOfType<InventoryManager>();
        }

        /// <summary>
        /// 设置槽位物品
        /// </summary>
        /// <param name="item">物品组件</param>
        public void SetItem(InventoryItem item)
        {
            CurrentItem = item;
        }

        /// <summary>
        /// 清空槽位
        /// </summary>
        public void ClearSlot()
        {
            CurrentItem = null;
        }

        /// <summary>
        /// 高亮槽位
        /// </summary>
        public void HighlightSlot()
        {
            if (SlotBackground != null)
            {
                SlotBackground.color = HighlightColor;
            }
        }

        /// <summary>
        /// 恢复槽位正常状态
        /// </summary>
        public void NormalSlot()
        {
            if (SlotBackground != null)
            {
                SlotBackground.color = NormalColor;
            }
        }

        /// <summary>
        /// 检查物品是否可以放入该槽位
        /// </summary>
        public bool CanAcceptItem(InventoryItem item)
        {
            // 日志：检查物品是否可以放入槽位
           // Debug.Log($"[InventorySlot] CanAcceptItem - 物品类型: {item.ItemData.Type}, 槽位允许类型: {AllowedItemType}");

            // 如果允许所有类型，直接返回true
            if (AllowedItemType == ItemType.Misc)
            {
              //  Debug.Log($"[InventorySlot] CanAcceptItem - 槽位允许所有类型，返回true");
                return true;
            }

            // 检查物品类型是否匹配
            // 特殊处理：枪械类型物品可以放入Weapon槽位
            if (AllowedItemType == ItemType.Weapon && item.ItemData.Type == ItemType.Weapon)
            {
              //  Debug.Log($"[InventorySlot] CanAcceptItem - 武器类型匹配，返回true");
                return true;
            }

            // 检查物品类型是否匹配
            bool result = item.ItemData.Type == AllowedItemType;
          //  Debug.Log($"[InventorySlot] CanAcceptItem - 类型匹配结果: {result}");
            return result;
        }

        /// <summary>
        /// 处理物品拖拽到槽位
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnDrop(PointerEventData eventData)
        {
            // 日志：物品被拖拽到槽位
           // Debug.Log($"[InventorySlot] OnDrop - 物品被拖拽到槽位，槽位名称: {gameObject.name}");

            // 获取拖拽的物品
            InventoryItem draggedItem = eventData.pointerDrag?.GetComponent<InventoryItem>();
            if (draggedItem != null)
            {
              // Debug.Log($"[InventorySlot] OnDrop - 拖拽的物品: {draggedItem.ItemId}, 当前槽位: {draggedItem.CurrentSlot.gameObject.name}");

                // 检查物品是否可以放入该槽位
                if (CanAcceptItem(draggedItem))
                {
                   //Debug.Log($"[InventorySlot] OnDrop - 物品可以放入该槽位，调用HandleItemDrop");

                    // 处理拖拽逻辑
                    if (m_InventoryManager != null)
                    {
                        m_InventoryManager.HandleItemDrop(draggedItem, this);
                    }
                    else
                    {
                       Debug.LogError($"[InventorySlot] OnDrop - 找不到InventoryManager");
                    }
                }
                else
                {
                    Debug.Log($"[InventorySlot] OnDrop - 物品不可以放入该槽位");
                }
            }
            else
            {
                Debug.Log($"[InventorySlot] OnDrop - 没有拖拽的物品");
            }
        }

        /// <summary>
        /// 处理鼠标进入
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            // 确保鼠标事件系统正确初始化
            if (eventData == null)
                return;

            // 显示物品悬浮信息
            if (CurrentItem != null && m_InventoryManager != null && CurrentItem.ItemData != null)
            {
                m_InventoryManager.ShowItemTooltip(CurrentItem, eventData.position);
            }
        }

        /// <summary>
        /// 处理鼠标离开
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            // 隐藏物品悬浮信息
            if (m_InventoryManager != null)
            {
                m_InventoryManager.HideItemTooltip();
            }
        }
    }
}