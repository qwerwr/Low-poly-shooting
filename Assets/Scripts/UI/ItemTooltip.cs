using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.UI
{
    /// <summary>
    /// 物品悬浮信息组件
    /// </summary>
    public class ItemTooltip : MonoBehaviour
    {
        /// <summary>
        /// 单个文本组件，用于显示所有物品信息
        /// </summary>
        public TextMeshProUGUI ItemInfoText;

        /// <summary>
        /// 悬浮信息面板
        /// </summary>
        private CanvasGroup m_CanvasGroup;

        private void Start()
        {
            // 获取或添加CanvasGroup组件
            m_CanvasGroup = GetComponent<CanvasGroup>();
            if (m_CanvasGroup == null)
            {
                m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            HideItemInfo();
        }

        /// <summary>
        /// 显示物品信息
        /// </summary>
        /// <param name="item">背包物品组件</param>
        /// <param name="position">显示位置</param>
        public void ShowItemInfo(InventoryItem item, Vector3 position)
        {
            if (item == null || item.ItemData == null)
                return;

            ShowItemInfo(item.ItemData, position);
        }

        /// <summary>
        /// 当前显示的物品ID，用于避免频繁更新
        /// </summary>
        private string m_CurrentItemId;

        /// <summary>
        /// 显示物品信息
        /// </summary>
        /// <param name="itemData">物品数据</param>
        /// <param name="position">显示位置</param>
        public void ShowItemInfo(ItemData itemData, Vector3 position)
        {
            if (itemData == null)
                return;

            // 避免频繁更新同一物品的信息，减少闪烁
            if (m_CurrentItemId == itemData.Id)
                return;

            m_CurrentItemId = itemData.Id;

            // 设置位置，显示在鼠标左上角
            // 调整偏移量，确保信息框显示在鼠标左上角
            transform.position = position + new Vector3(143, -159, 0);

            // 生成完整的物品信息文本
            string itemInfo = GenerateItemInfoText(itemData);
            ItemInfoText.text = itemInfo;

            // 显示面板，确保m_CanvasGroup不为null
            if (m_CanvasGroup != null)
            {
                m_CanvasGroup.alpha = 1;
                m_CanvasGroup.blocksRaycasts = false; // 不阻挡鼠标事件，避免闪烁
            }
        }

        /// <summary>
        /// 生成完整的物品信息文本
        /// </summary>
        /// <param name="itemData">物品数据</param>
        /// <returns>完整的物品信息文本</returns>
        private string GenerateItemInfoText(ItemData itemData)
        {
            string info = string.Empty;

            // 基础信息
            info += $"<b>{itemData.Name}</b>\n";
            info += $"类型: {itemData.Type}\n";
            info += $"描述: {itemData.Description}\n";
            info += $"价值: {itemData.Value}\n";

            // 根据物品类型添加特定属性
            switch (itemData.Type)
            {
                case ItemType.Weapon:
                    WeaponData weaponData = itemData as WeaponData;
                    if (weaponData != null)
                    {
                        info += "\n<color=blue><b>武器属性</b></color>\n";
                        info += $"武器类型: {weaponData.WeaponType}\n";
                        info += $"弹药类型: {weaponData.AmmoType}\n";
                        info += $"基础伤害: {weaponData.BaseDamage}\n";
                        info += $"最大弹药等级: {weaponData.MaxAmmoLevel}\n";
                    }
                    break;
                case ItemType.Ammo:
                    AmmoData ammoData = itemData as AmmoData;
                    if (ammoData != null)
                    {
                        info += "\n<color=green><b>弹药属性</b></color>\n";
                        info += $"弹药类型: {ammoData.AmmoType}\n";
                        info += $"弹药等级: {ammoData.Level}\n";
                        info += $"弹药伤害: {ammoData.Damage}\n";
                    }
                    break;
                case ItemType.Armor:
                    ArmorData armorData = itemData as ArmorData;
                    if (armorData != null)
                    {
                        info += "\n<color=red><b>护甲属性</b></color>\n";
                        info += $"护甲槽位: {armorData.Slot}\n";
                        info += $"护甲等级: {armorData.Level}\n";
                        info += $"伤害减免: {armorData.DamageReduction}\n";
                    }
                    break;
            }

            return info;
        }

        /// <summary>
        /// 隐藏物品信息
        /// </summary>
        public void HideItemInfo()
        {
            // 重置当前显示的物品ID
            m_CurrentItemId = null;

            // 隐藏面板，确保m_CanvasGroup不为null
            if (m_CanvasGroup != null)
            {
                m_CanvasGroup.alpha = 0;
                m_CanvasGroup.blocksRaycasts = false;
            }
        }
    }
}