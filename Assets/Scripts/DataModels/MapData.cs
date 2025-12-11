using UnityEngine;

namespace Game
{
    /// <summary>
    /// 地图数据类
    /// </summary>
    [CreateAssetMenu(fileName = "NewMap", menuName = "GameData/Map")]
    public class MapData : ScriptableObject
    {
        /// <summary>
        /// 地图唯一标识符
        /// </summary>
        public string MapId;
        
        /// <summary>
        /// 地图名称
        /// </summary>
        public string MapName;
        
        /// <summary>
        /// 地图描述
        /// </summary>
        public string Description;
        
        /// <summary>
        /// 场景名称
        /// </summary>
        public string SceneName;
    }
}