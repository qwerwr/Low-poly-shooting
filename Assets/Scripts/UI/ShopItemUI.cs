using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.UI
{
    /// <summary>
    /// 商店物品UI组件
    /// </summary>
    public class ShopItemUI : MonoBehaviour
    {
        /// <summary>
        /// 物品图片
        /// </summary>
        public Image ItemImage;
        
        /// <summary>
        /// 物品名称
        /// </summary>
        public TextMeshProUGUI ItemName;
        
        /// <summary>
        /// 物品价格
        /// </summary>
        public TextMeshProUGUI ItemPrice;
        
        /// <summary>
        /// 购买按钮
        /// </summary>
        public Button BuyButton;
        
        /// <summary>
        /// 物品数据
        /// </summary>
        private ItemData m_ItemData;
        
        /// <summary>
        /// 购买回调
        /// </summary>
        private System.Action<string, int> m_BuyCallback;
        
        /// <summary>
        /// 设置物品数据
        /// </summary>
        /// <param name="itemData">物品数据</param>
        /// <param name="buyCallback">购买回调</param>
        public void Setup(ItemData itemData, System.Action<string, int> buyCallback)
        {
            m_ItemData = itemData;
            m_BuyCallback = buyCallback;
            
            // 设置物品图片
            if (ItemImage != null && itemData.Icon != null)
            {
                ItemImage.sprite = itemData.Icon;
                ItemImage.gameObject.SetActive(true);
            }
            
            // 设置物品名称
            if (ItemName != null)
            {
                ItemName.text = itemData.Name;
            }
            
            // 设置物品价格
            if (ItemPrice != null)
            {
                ItemPrice.text = itemData.Value.ToString();
            }
            
            // 设置购买按钮事件
            if (BuyButton != null)
            {
                BuyButton.onClick.RemoveAllListeners();
                BuyButton.onClick.AddListener(() => OnBuyButtonClick());
            }
        }
        
        /// <summary>
        /// 购买按钮点击事件
        /// </summary>
        private void OnBuyButtonClick()
        {
            m_BuyCallback?.Invoke(m_ItemData.Id, 1);
        }
    }
}