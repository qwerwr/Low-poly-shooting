using QFramework;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 人物模型
    /// 管理人物装备和物品栏
    /// </summary>
    public class CharacterModel : AbstractModel
    {
        /// <summary>
        /// 头盔装备
        /// </summary>
        public InventoryItemData Helmet { get; set; }

        /// <summary>
        /// 甲胄装备
        /// </summary>
        public InventoryItemData Armor { get; set; }

        /// <summary>
        /// 武器变化事件
        /// </summary>
        public event System.Action<InventoryItemData> WeaponChanged;

        /// <summary>
        /// 枪械装备
        /// </summary>
        private InventoryItemData m_Weapon;
        public InventoryItemData Weapon
        {
            get { return m_Weapon; }
            set
            {
                if (m_Weapon != value)
                {
                    string oldWeaponName = m_Weapon?.ItemRef?.Name ?? "空";
                    string newWeaponName = value?.ItemRef?.Name ?? "空";
                    Debug.Log($"[CharacterModel] 武器变化：{oldWeaponName} -> {newWeaponName}");

                    m_Weapon = value;
                    WeaponChanged?.Invoke(value);
                    Debug.Log($"[CharacterModel] 武器变化事件已触发");
                }
            }
        }

        /// <summary>
        /// 人物物品栏（8个格子）
        /// </summary>
        private List<InventoryItemData> m_Items = new List<InventoryItemData>();

        /// <summary>
        /// 人物物品栏
        /// </summary>
        public List<InventoryItemData> Items
        {
            get { return m_Items; }
            set { m_Items = value; }
        }

        /// <summary>
        /// 人物物品栏最大容量
        /// </summary>
        public int MaxCapacity = 8;

        /// <summary>
        /// 初始化模型
        /// </summary>
        protected override void OnInit()
        {
            // 初始化物品栏
            m_Items = new List<InventoryItemData>();
        }

        /// <summary>
        /// 检查物品是否可以装备
        /// </summary>
        public bool CanEquip(InventoryItemData itemData)
        {
            switch (itemData.ItemRef.Type)
            {
                case ItemType.Helmet:
                    return Helmet == null;
                case ItemType.Armor:
                    return Armor == null;
                case ItemType.Weapon:
                    return Weapon == null;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 装备物品
        /// </summary>
        public bool EquipItem(InventoryItemData itemData)
        {
            if (!CanEquip(itemData))
                return false;

            // 根据物品类型装备到对应的槽位
            switch (itemData.ItemRef.Type)
            {
                case ItemType.Helmet:
                    Helmet = itemData;
                    break;
                case ItemType.Armor:
                    Armor = itemData;
                    break;
                case ItemType.Weapon:
                    Weapon = itemData;
                    break;
            }

            return true;
        }

        /// <summary>
        /// 卸下装备
        /// </summary>
        public InventoryItemData UnequipItem(ItemType itemType)
        {
            InventoryItemData itemData = null;

            switch (itemType)
            {
                case ItemType.Helmet:
                    itemData = Helmet;
                    Helmet = null;
                    break;
                case ItemType.Armor:
                    itemData = Armor;
                    Armor = null;
                    break;
                case ItemType.Weapon:
                    itemData = Weapon;
                    Weapon = null;
                    break;
            }

            return itemData;
        }
    }
}