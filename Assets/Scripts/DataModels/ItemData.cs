using UnityEngine;
using System;

namespace Game
{
    /// <summary>
    /// 物品数据基类
    /// </summary>
    [CreateAssetMenu(fileName = "NewItem", menuName = "GameData/Item")]
    public class ItemData : ScriptableObject
    {
        /// <summary>
        /// 物品唯一标识符
        /// </summary>
        public string Id;

        /// <summary>
        /// 物品名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 物品类型
        /// </summary>
        public ItemType Type;

        /// <summary>
        /// 物品价值
        /// </summary>
        public int Value;

        /// <summary>
        /// 是否可堆叠
        /// </summary>
        public bool CanStack;

        /// <summary>
        /// 物品描述
        /// </summary>
        public string Description;

        /// <summary>
        /// 物品图标
        /// </summary>
        public Sprite Icon;
    }
}