using UnityEngine;

namespace Game
{
    /// <summary>
    /// 武器数据类
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "GameData/Weapon")]
    public class WeaponData : ItemData
    {
        /// <summary>
        /// 武器类型
        /// </summary>
        public WeaponType WeaponType;
        
        /// <summary>
        /// 弹药类型
        /// </summary>
        public AmmoType AmmoType;
        
        /// <summary>
        /// 基础伤害
        /// </summary>
        public int BaseDamage;
        
        /// <summary>
        /// 最大弹药等级
        /// </summary>
        public int MaxAmmoLevel;
    }
}