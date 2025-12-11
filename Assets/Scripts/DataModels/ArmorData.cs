using UnityEngine;

namespace Game
{
    /// <summary>
    /// 防具数据类
    /// </summary>
    [CreateAssetMenu(fileName = "NewArmor", menuName = "GameData/Armor")]
    public class ArmorData : ItemData
    {
        /// <summary>
        /// 装备部位
        /// </summary>
        public string Slot;
        
        /// <summary>
        /// 装备等级
        /// </summary>
        public int Level;
        
        /// <summary>
        /// 伤害减免
        /// </summary>
        public int DamageReduction;
    }
}