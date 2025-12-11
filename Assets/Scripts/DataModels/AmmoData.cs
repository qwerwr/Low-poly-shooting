using UnityEngine;

namespace Game
{
    /// <summary>
    /// 弹药数据类
    /// </summary>
    [CreateAssetMenu(fileName = "NewAmmo", menuName = "GameData/Ammo")]
    public class AmmoData : ItemData
    {
        /// <summary>
        /// 弹药类型
        /// </summary>
        public AmmoType AmmoType;
        
        /// <summary>
        /// 弹药等级
        /// </summary>
        public int Level;
        
        /// <summary>
        /// 弹药伤害
        /// </summary>
        public int Damage;
    }
}