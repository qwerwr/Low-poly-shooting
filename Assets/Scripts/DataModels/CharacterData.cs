using UnityEngine;

namespace Game
{
    /// <summary>
    /// 角色数据类
    /// </summary>
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "GameData/Character")]
    public class CharacterData : ScriptableObject
    {
        /// <summary>
        /// 角色唯一标识符
        /// </summary>
        public string Id;
        
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name;
        
        /// <summary>
        /// 是否为玩家角色
        /// </summary>
        public bool IsPlayer;
        
        /// <summary>
        /// 角色类型
        /// </summary>
        public CharacterType Type;
        
        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description;
        
        /// <summary>
        /// 角色头像路径
        /// </summary>
        public string Avatar;
    }
}