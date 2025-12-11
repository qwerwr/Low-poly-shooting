using UnityEngine;
using QFramework;
using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// 角色预制体映射
    /// 用于在Inspector中配置角色ID和预制体的对应关系
    /// </summary>
    [System.Serializable]
    public class CharacterPrefabMapping
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public string characterId;

        /// <summary>
        /// 角色预制体
        /// 通过拖拽方式添加
        /// </summary>
        public GameObject characterPrefab;
    }

    /// <summary>
    /// 地图预制体映射
    /// 用于在Inspector中配置关卡号和预制体的对应关系
    /// </summary>
    [System.Serializable]
    public class MapPrefabMapping
    {
        /// <summary>
        /// 关卡号
        /// </summary>
        public int levelId;

        /// <summary>
        /// 地图预制体
        /// 通过拖拽方式添加
        /// </summary>
        public GameObject mapPrefab;
    }

    /// <summary>
    /// 游戏场景控制器
    /// 用于在Game场景中读取玩家选择的人物和关卡信息
    /// 根据选择实例化对应的角色和地图预制体
    /// </summary>
    public class GameSceneController : MonoBehaviour
    {
        /// <summary>
        /// 角色预制体映射列表
        /// 通过拖拽方式添加
        /// </summary>
        [Header("预制体配置")]
        [SerializeField] private List<CharacterPrefabMapping> characterPrefabMappings = new List<CharacterPrefabMapping>();

        /// <summary>
        /// 地图预制体映射列表
        /// 通过拖拽方式添加
        /// </summary>
        [SerializeField] private List<MapPrefabMapping> mapPrefabMappings = new List<MapPrefabMapping>();

        /// <summary>
        /// 默认角色预制体
        /// 当找不到对应角色时使用
        /// </summary>
        [SerializeField] private GameObject defaultCharacterPrefab;

        /// <summary>
        /// 默认地图预制体
        /// 当找不到对应地图时使用
        /// </summary>
        [SerializeField] private GameObject defaultMapPrefab;

        /// <summary>
        /// 游戏数据模型
        /// </summary>
        private GameDataModel m_GameDataModel;

        /// <summary>
        /// 实例化的角色对象
        /// </summary>
        private GameObject instantiatedCharacter;

        /// <summary>
        /// 实例化的地图对象
        /// </summary>
        private GameObject instantiatedMap;

        private void Start()
        {
            // 获取游戏数据模型
            m_GameDataModel = GameArchitecture.Interface.GetModel<GameDataModel>();

            // 读取玩家选择的人物
            ReadSelectedCharacter();

            // 读取玩家选择的关卡
            ReadSelectedLevel();

            // 加载对应的地图
            LoadSelectedMap();
        }

        /// <summary>
        /// 读取玩家选择的人物
        /// </summary>
        private void ReadSelectedCharacter()
        {
            // 从PlayerPrefs中读取选择的人物ID
            string selectedCharacterId = PlayerPrefs.GetString("SelectedCharacterId", string.Empty);

            if (!string.IsNullOrEmpty(selectedCharacterId))
            {
                // 根据人物ID获取人物数据
                if (m_GameDataModel.Characters.ContainsKey(selectedCharacterId))
                {
                    CharacterData selectedCharacter = m_GameDataModel.Characters[selectedCharacterId];
                    Debug.Log($"选择的人物: {selectedCharacter.Name}");

                    // 在这里可以根据选择的人物初始化玩家角色
                    // 例如：设置玩家角色的模型、属性等
                    InitializePlayerCharacter(selectedCharacter);
                }
                else
                {
                    Debug.LogWarning($"找不到ID为{selectedCharacterId}的人物数据");
                    // 使用默认人物
                    UseDefaultCharacter();
                }
            }
            else
            {
                Debug.LogWarning("未选择人物，使用默认人物");
                // 使用默认人物
                UseDefaultCharacter();
            }
        }

        /// <summary>
        /// 读取玩家选择的关卡
        /// </summary>
        private void ReadSelectedLevel()
        {
            // 从PlayerPrefs中读取选择的关卡
            int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
            Debug.Log($"选择的关卡: {selectedLevel}");

            // 在这里可以根据选择的关卡执行相应的逻辑
            // 例如：设置游戏难度、生成敌人等
        }

        /// <summary>
        /// 加载选择的地图
        /// </summary>
        private void LoadSelectedMap()
        {
            // 从PlayerPrefs中读取选择的关卡
            int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);

            // 根据关卡号获取对应的地图数据
            if (m_GameDataModel.Maps.ContainsKey(selectedLevel))
            {
                MapData selectedMap = m_GameDataModel.Maps[selectedLevel];
                Debug.Log($"加载地图: {selectedMap.MapName}");

                // 根据地图数据加载对应的地图预制体
                LoadMap(selectedMap, selectedLevel);
            }
            else
            {
                Debug.LogWarning($"找不到关卡{selectedLevel}对应的地图数据");
                // 使用默认地图
                LoadDefaultMap();
            }
        }

        /// <summary>
        /// 初始化玩家角色
        /// 根据角色ID实例化对应的预制体
        /// </summary>
        /// <param name="characterData">人物数据</param>
        private void InitializePlayerCharacter(CharacterData characterData)
        {
            Debug.Log($"初始化玩家角色: {characterData.Name}");

            // 根据角色ID查找对应的预制体
            GameObject characterPrefab = FindCharacterPrefab(characterData.Id);

            if (characterPrefab != null)
            {
                // 实例化角色预制体
                instantiatedCharacter = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity);
                instantiatedCharacter.name = characterData.Name;
                Debug.Log($"成功实例化角色预制体: {characterData.Name}");
            }
            else
            {
                Debug.LogWarning($"未找到ID为{characterData.Id}的角色预制体");
            }
        }

        /// <summary>
        /// 根据角色ID查找对应的预制体
        /// </summary>
        /// <param name="characterId">角色ID</param>
        /// <returns>角色预制体，如果找不到则返回默认预制体或null</returns>
        private GameObject FindCharacterPrefab(string characterId)
        {
            // 在映射列表中查找对应角色ID的预制体
            foreach (var mapping in characterPrefabMappings)
            {
                if (mapping.characterId == characterId)
                {
                    return mapping.characterPrefab;
                }
            }

            // 如果找不到对应预制体，使用默认预制体
            Debug.LogWarning($"未找到ID为{characterId}的角色预制体，使用默认预制体");
            return defaultCharacterPrefab;
        }

        /// <summary>
        /// 根据关卡号查找对应的地图预制体
        /// </summary>
        /// <param name="levelId">关卡号</param>
        /// <returns>地图预制体，如果找不到则返回默认预制体或null</returns>
        private GameObject FindMapPrefab(int levelId)
        {
            // 在映射列表中查找对应关卡号的预制体
            foreach (var mapping in mapPrefabMappings)
            {
                if (mapping.levelId == levelId)
                {
                    return mapping.mapPrefab;
                }
            }

            // 如果找不到对应预制体，使用默认预制体
            Debug.LogWarning($"未找到关卡{levelId}的地图预制体，使用默认预制体");
            return defaultMapPrefab;
        }

        /// <summary>
        /// 加载地图
        /// 根据地图数据实例化对应的预制体
        /// </summary>
        /// <param name="mapData">地图数据</param>
        /// <param name="levelId">关卡号</param>
        private void LoadMap(MapData mapData, int levelId)
        {
            Debug.Log($"加载地图: {mapData.MapName}");

            // 根据关卡号查找对应的地图预制体
            GameObject mapPrefab = FindMapPrefab(levelId);

            if (mapPrefab != null)
            {
                // 实例化地图预制体
                instantiatedMap = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity);
                instantiatedMap.name = mapData.MapName;
                Debug.Log($"成功实例化地图预制体: {mapData.MapName}");
            }
            else
            {
                Debug.LogWarning($"未找到关卡{levelId}的地图预制体");
            }
        }

        /// <summary>
        /// 使用默认人物
        /// </summary>
        private void UseDefaultCharacter()
        {
            // 如果没有选择人物或选择的人物不存在，使用第一个人物作为默认人物
            if (m_GameDataModel.Characters.Count > 0)
            {
                foreach (var character in m_GameDataModel.Characters.Values)
                {
                    Debug.Log($"使用默认人物: {character.Name}");
                    InitializePlayerCharacter(character);
                    break;
                }
            }
            // 如果没有可用的角色数据，直接使用默认预制体
            else if (defaultCharacterPrefab != null)
            {
                Debug.Log($"使用默认角色预制体");
                instantiatedCharacter = Instantiate(defaultCharacterPrefab, Vector3.zero, Quaternion.identity);
                instantiatedCharacter.name = "DefaultCharacter";
            }
        }

        /// <summary>
        /// 加载默认地图
        /// </summary>
        private void LoadDefaultMap()
        {
            // 如果没有选择地图或选择的地图不存在，使用第一个地图作为默认地图
            if (m_GameDataModel.Maps.Count > 0)
            {
                foreach (KeyValuePair<int, MapData> mapEntry in m_GameDataModel.Maps)
                {
                    int levelId = mapEntry.Key;
                    MapData mapData = mapEntry.Value;
                    Debug.Log($"使用默认地图: {mapData.MapName}");
                    LoadMap(mapData, levelId);
                    break;
                }
            }
            // 如果没有可用的地图数据，直接使用默认预制体
            else if (defaultMapPrefab != null)
            {
                Debug.Log($"使用默认地图预制体");
                instantiatedMap = Instantiate(defaultMapPrefab, Vector3.zero, Quaternion.identity);
                instantiatedMap.name = "DefaultMap";
            }
        }
    }
}