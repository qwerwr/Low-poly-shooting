using Game.Core;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// 人物选择控制器
    /// 用于处理人物选择和地图选择功能
    /// </summary>
    public class CharacterSelectController : MonoBehaviour
    {
        /// <summary>
        /// 人物图片
        /// </summary>
        public Image CharacterImage;

        /// <summary>
        /// 人物信息文本
        /// </summary>
        public TextMeshProUGUI CharacterInfoText;

        /// <summary>
        /// 人物槽位数组
        /// </summary>
        public Button[] CharacterSlots;

        /// <summary>
        /// 关卡按钮数组
        /// </summary>
        public Button[] LevelButtons;

        /// <summary>
        /// 进入游戏按钮
        /// </summary>
        public Button EnterGameButton;

        /// <summary>
        /// 场景加载器
        /// </summary>
        public SceneLoader SceneLoader;

        /// <summary>
        /// 游戏数据模型
        /// </summary>
        private GameDataModel m_GameDataModel;

        /// <summary>
        /// 当前选择的人物
        /// </summary>
        private CharacterData m_SelectedCharacter;

        /// <summary>
        /// 当前选择的关卡
        /// </summary>
        private int m_SelectedLevel = 1;

        private void Start()
        {
            // 获取游戏数据模型
            m_GameDataModel = GameArchitecture.Interface.GetModel<GameDataModel>();

            // 初始化UI
            InitializeUI();

            // 注册事件
            RegisterEvents();
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitializeUI()
        {
            //// 隐藏加载面板
            //if (LoadingPanel != null)
            //{
            //    LoadingPanel.SetActive(false);
            //}

            // 设置默认选中的人物（第一个人物）
            if (m_GameDataModel != null && m_GameDataModel.Characters.Count > 0)
            {
                // 获取第一个人物
                foreach (var character in m_GameDataModel.Characters.Values)
                {
                    m_SelectedCharacter = character;
                    break;
                }

                // 更新人物信息
                UpdateCharacterInfo(m_SelectedCharacter);
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            // 注册人物槽位事件
            if (CharacterSlots != null)
            {
                for (int i = 0; i < CharacterSlots.Length; i++)
                {
                    if (CharacterSlots[i] == null)
                    {
                        continue;
                    }

                    int index = i;
                    CharacterSlots[i].onClick.AddListener(() => OnCharacterSlotClick(index));
                }
            }

            // 注册关卡按钮事件
            if (LevelButtons != null)
            {
                for (int i = 0; i < LevelButtons.Length; i++)
                {
                    int level = i + 1;
                    LevelButtons[i].onClick.AddListener(() => OnLevelButtonClick(level));
                }
            }

            // 注册进入游戏按钮事件
            if (EnterGameButton != null)
            {
                EnterGameButton.onClick.AddListener(OnEnterGameButtonClick);
            }
        }

        /// <summary>
        /// 人物槽位鼠标悬浮事件
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        public void OnCharacterSlotHover(int slotIndex)
        {
            // 获取对应索引的人物数据
            CharacterData characterData = GetCharacterByIndex(slotIndex);
            if (characterData != null)
            {
                // 更新人物信息
                UpdateCharacterInfo(characterData);
            }
        }

        /// <summary>
        /// 人物槽位鼠标离开事件
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        public void OnCharacterSlotExit(int slotIndex)
        {
            // 鼠标离开时恢复显示当前选中的人物信息
            if (m_SelectedCharacter != null)
            {
                UpdateCharacterInfo(m_SelectedCharacter);
            }
        }

        /// <summary>
        /// 人物槽位点击事件
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        private void OnCharacterSlotClick(int slotIndex)
        {
            // 获取对应索引的人物数据
            CharacterData characterData = GetCharacterByIndex(slotIndex);
            if (characterData != null)
            {
                // 设置当前选择的人物
                m_SelectedCharacter = characterData;
                // 更新人物信息
                UpdateCharacterInfo(characterData);
                // 保存选择的人物
                SaveSelectedCharacter();
                // 更新选中状态
                UpdateCharacterSlotSelection(slotIndex);
            }
        }

        /// <summary>
        /// 关卡按钮点击事件
        /// </summary>
        /// <param name="level">关卡号</param>
        private void OnLevelButtonClick(int level)
        {
            // 设置当前选择的关卡
            m_SelectedLevel = level;
            // 保存选择的关卡
            SaveSelectedLevel();
            // 更新选中状态
            UpdateLevelButtonSelection(level - 1);
        }

        /// <summary>
        /// 进入游戏按钮点击事件
        /// </summary>
        private void OnEnterGameButtonClick()
        {
            // 使用SceneLoader加载场景
            if (SceneLoader != null)
            {
                SceneLoader.LoadSceneAsync(2); // 假设Game场景的索引是2
            }
        }

        /// <summary>
        /// 更新人物信息
        /// </summary>
        /// <param name="characterData">人物数据</param>
        private void UpdateCharacterInfo(CharacterData characterData)
        {
            if (characterData == null)
                return;

            // 更新人物图片
            if (CharacterImage != null)
            {
                // 尝试加载人物图片
                Sprite characterSprite = Resources.Load<Sprite>(characterData.Avatar);

                if (characterSprite != null)
                {
                    CharacterImage.sprite = characterSprite;
                    CharacterImage.gameObject.SetActive(true);
                }
            }

            // 更新人物信息文本
            if (CharacterInfoText != null)
            {
                CharacterInfoText.text = $"名叫{characterData.Name}，是一名{characterData.Description}";
            }
        }

        /// <summary>
        /// 根据索引获取人物数据
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>人物数据</returns>
        private CharacterData GetCharacterByIndex(int index)
        {
            if (m_GameDataModel == null)
            {
                return null;
            }

            if (m_GameDataModel.Characters.Count == 0)
            {
                return null;
            }

            // 将字典转换为列表，确保顺序一致
            var characterList = new List<CharacterData>(m_GameDataModel.Characters.Values);

            if (index >= 0 && index < characterList.Count)
            {
                return characterList[index];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 更新人物槽位选中状态
        /// </summary>
        /// <param name="selectedIndex">选中的索引</param>
        private void UpdateCharacterSlotSelection(int selectedIndex)
        {
            if (CharacterSlots == null)
                return;

            for (int i = 0; i < CharacterSlots.Length; i++)
            {
                // 这里可以根据需要添加选中状态的视觉反馈
                // 例如：改变按钮的颜色、添加选中边框等
                if (i == selectedIndex)
                {
                    // 选中状态
                    CharacterSlots[i].interactable = false;
                }
                else
                {
                    // 未选中状态
                    CharacterSlots[i].interactable = true;
                }
            }
        }

        /// <summary>
        /// 更新关卡按钮选中状态
        /// </summary>
        /// <param name="selectedIndex">选中的索引</param>
        private void UpdateLevelButtonSelection(int selectedIndex)
        {
            if (LevelButtons == null)
                return;

            for (int i = 0; i < LevelButtons.Length; i++)
            {
                // 这里可以根据需要添加选中状态的视觉反馈
                // 例如：改变按钮的颜色、添加选中边框等
                if (i == selectedIndex)
                {
                    // 选中状态
                    LevelButtons[i].interactable = false;
                }
                else
                {
                    // 未选中状态
                    LevelButtons[i].interactable = true;
                }
            }
        }

        /// <summary>
        /// 保存选择的人物
        /// </summary>
        private void SaveSelectedCharacter()
        {
            if (m_SelectedCharacter != null)
            {
                PlayerPrefs.SetString("SelectedCharacterId", m_SelectedCharacter.Id);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// 保存选择的关卡
        /// </summary>
        private void SaveSelectedLevel()
        {
            PlayerPrefs.SetInt("SelectedLevel", m_SelectedLevel);
            PlayerPrefs.Save();
        }


    }
}