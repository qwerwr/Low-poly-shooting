using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Game.UI
{
    /// <summary>
    /// 背包物品组件
    /// </summary>
    public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
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
        /// 当前所在槽位
        /// </summary>
        public InventorySlot CurrentSlot { get; set; }

        /// <summary>
        /// 物品图片
        /// </summary>
        public Image ItemImage;

        /// <summary>
        /// 数量文本
        /// </summary>
        public TextMeshProUGUI QuantityText;

        /// <summary>
        /// 物品数据
        /// </summary>
        public ItemData ItemData;

        /// <summary>
        /// 拖拽时的偏移量
        /// </summary>
        private Vector2 m_DragOffset;

        /// <summary>
        /// 原始父物体
        /// </summary>
        private Transform m_OriginalParent;

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
        /// 更新物品显示
        /// </summary>
        public void UpdateItemDisplay()
        {
            // 更新数量文本
            if (Quantity > 1)
            {
                if (QuantityText != null)
                {
                    QuantityText.gameObject.SetActive(true);
                    QuantityText.text = Quantity.ToString();
                }
            }
            else
            {
                if (QuantityText != null)
                {
                    QuantityText.gameObject.SetActive(false);
                }
            }

            // 确保物品图片正确显示
            if (ItemImage != null && ItemData != null)
            {
                if (ItemData.Icon != null)
                {
                    ItemImage.sprite = ItemData.Icon;
                    ItemImage.gameObject.SetActive(true);
                }
                else
                {
                    ItemImage.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // 确保物品数据存在
            if (ItemData == null)
                return;

            // 保存原始父物体
            m_OriginalParent = transform.parent;
            // 移除这行代码，避免槽位顺序被改变
            // m_OriginalParent.SetAsLastSibling();
            // 计算拖拽偏移量
            RectTransform rectTransform = GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out m_DragOffset);

            // 设置为Canvas的直接子物体，便于拖拽
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();

            // 设置拖拽层，确保CanvasGroup组件存在
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                // 如果没有CanvasGroup组件，添加一个
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
                canvasGroup.blocksRaycasts = false;
            }
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnDrag(PointerEventData eventData)
        {
            // 更新物品位置
            Vector3 position;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out position))
            {
                transform.position = position;
            }
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            // 恢复射线检测，确保CanvasGroup组件存在
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = true;
            }
            else
            {
                // 如果没有CanvasGroup组件，添加一个
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
                canvasGroup.blocksRaycasts = true;
            }

            // 如果没有拖拽到任何槽位，返回原位置
            if (transform.parent == transform.root)
            {
                transform.SetParent(m_OriginalParent);
                transform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// 设置物品数据
        /// </summary>
        /// <param name="itemData">物品数据</param>
        public void SetItemData(ItemData itemData)
        {
            ItemData = itemData;
            ItemId = itemData.Id;

            // 设置物品图片
            ItemImage.sprite = itemData.Icon;
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
            if (ItemData != null && m_InventoryManager != null)
            {
                m_InventoryManager.ShowItemTooltip(this, eventData.position);
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