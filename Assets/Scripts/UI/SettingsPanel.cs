using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using QFramework;

namespace Game.UI
{
    /// <summary>
    /// 设置界面面板
    /// 处理设置界面的UI交互逻辑
    /// </summary>
    public class SettingsPanel : MonoBehaviour
    {
        /// <summary>
        /// 音量调节滑块
        /// </summary>
        [Header("UI组件")]
        [SerializeField] private Slider volumeSlider;

        /// <summary>
        /// 返回主菜单按钮
        /// </summary>
        [SerializeField] private Button btnBackMenu;

        /// <summary>
        /// 保存游戏按钮
        /// </summary>
        [SerializeField] private Button btnSaveGame;

        /// <summary>
        /// 保存成功提示文本
        /// </summary>
        [SerializeField] private Text txtSaveSuccess;

        /// <summary>
        /// 游戏数据模型
        /// </summary>
        private GameDataModel gameDataModel;

        /// <summary>
        /// 背包模型
        /// </summary>
        private InventoryModel inventoryModel;

        /// <summary>
        /// 人物模型
        /// </summary>
        private CharacterModel characterModel;

        /// <summary>
        /// 仓库模型
        /// </summary>
        private WarehouseModel warehouseModel;

        /// <summary>
        /// 经济模型，管理金币数据
        /// </summary>
        private EconomyModel economyModel;

        private void Start()
        {
            // 获取数据模型
            gameDataModel = GameArchitecture.Interface.GetModel<GameDataModel>();
            inventoryModel = GameArchitecture.Interface.GetModel<InventoryModel>();
            characterModel = GameArchitecture.Interface.GetModel<CharacterModel>();
            warehouseModel = GameArchitecture.Interface.GetModel<WarehouseModel>();
            economyModel = GameArchitecture.Interface.GetModel<EconomyModel>();

            // 初始化UI组件
            InitializeUI();

            // 添加事件监听
            AddEventListeners();
        }

        /// <summary>
        /// 初始化UI组件
        /// </summary>
        private void InitializeUI()
        {
            // 设置音量滑块初始值
            if (volumeSlider != null && AudioManager.Instance != null)
            {
                volumeSlider.value = AudioManager.Instance.GetVolume();
            }

            // 确保保存成功提示文本初始时隐藏
            if (txtSaveSuccess != null)
            {
                txtSaveSuccess.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 添加事件监听器
        /// </summary>
        private void AddEventListeners()
        {
            // 音量滑块事件
            if (volumeSlider != null)
            {
                volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            }

            // 返回主菜单按钮事件
            if (btnBackMenu != null)
            {
                btnBackMenu.onClick.AddListener(OnBackMenuClick);
            }

            // 保存游戏按钮事件
            if (btnSaveGame != null)
            {
                btnSaveGame.onClick.AddListener(OnSaveGameClick);
            }
        }

        /// <summary>
        /// 音量变化事件处理
        /// </summary>
        /// <param name="value">新的音量值</param>
        private void OnVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetVolume(value);
            }
        }

        /// <summary>
        /// 返回主菜单按钮点击事件
        /// </summary>
        private void OnBackMenuClick()
        {
            Debug.Log("返回主菜单按钮点击");
            // 回到Start场景
            SceneManager.LoadScene("Start");
        }
        public void OnBtnBack()
        {
            gameObject.SetActive(false);
        }
        public void OnBtnSettingOpen()
        {
            gameObject.SetActive(true);
        }
        /// <summary>
        /// 保存游戏按钮点击事件
        /// </summary>
        private void OnSaveGameClick()
        {
            Debug.Log("保存游戏按钮点击");
            // 保存游戏数据
            SaveGame();
            // 显示保存成功提示
            ShowSaveSuccessMessage();
        }

        /// <summary>
        /// 保存游戏数据
        /// </summary>
        private void SaveGame()
        {
            // 保存人物数据
            SaveCharacterData();
            // 保存背包物品数据
            SaveInventoryData();
            // 保存仓库物品数据
            SaveWarehouseData();
            // 保存金币数据
            SaveEconomyData();

            Debug.Log("游戏数据保存完成");
        }

        /// <summary>
        /// 保存人物数据
        /// </summary>
        private void SaveCharacterData()
        {
            if (characterModel != null)
            {
                // 保存人物装备
                PlayerPrefs.SetString("Character_Helmet", characterModel.Helmet?.ItemId ?? "");
                PlayerPrefs.SetString("Character_Armor", characterModel.Armor?.ItemId ?? "");
                PlayerPrefs.SetString("Character_Weapon", characterModel.Weapon?.ItemId ?? "");

                Debug.Log("人物数据保存完成");
            }
        }

        /// <summary>
        /// 保存背包物品数据
        /// </summary>
        private void SaveInventoryData()
        {
            if (inventoryModel != null)
            {
                // 保存背包物品数量
                PlayerPrefs.SetInt("Inventory_Count", inventoryModel.Items.Count);

                // 保存每个物品的详细信息
                for (int i = 0; i < inventoryModel.Items.Count; i++)
                {
                    InventoryItemData itemData = inventoryModel.Items[i];
                    PlayerPrefs.SetString($"Inventory_Item_{i}_Id", itemData.ItemId);
                    PlayerPrefs.SetInt($"Inventory_Item_{i}_Quantity", itemData.Quantity);
                    PlayerPrefs.SetInt($"Inventory_Item_{i}_SlotIndex", itemData.SlotIndex);
                }

                Debug.Log("背包物品数据保存完成");
            }
        }

        /// <summary>
        /// 保存仓库物品数据
        /// </summary>
        private void SaveWarehouseData()
        {
            if (warehouseModel != null)
            {
                // 保存仓库物品数量
                PlayerPrefs.SetInt("Warehouse_Count", warehouseModel.Items.Count);

                // 保存每个物品的详细信息
                for (int i = 0; i < warehouseModel.Items.Count; i++)
                {
                    InventoryItemData itemData = warehouseModel.Items[i];
                    PlayerPrefs.SetString($"Warehouse_Item_{i}_Id", itemData.ItemId);
                    PlayerPrefs.SetInt($"Warehouse_Item_{i}_Quantity", itemData.Quantity);
                    PlayerPrefs.SetInt($"Warehouse_Item_{i}_SlotIndex", itemData.SlotIndex);
                }

                Debug.Log("仓库物品数据保存完成");
            }
        }

        /// <summary>
        /// 保存经济数据（金币）
        /// </summary>
        private void SaveEconomyData()
        {
            if (economyModel != null)
            {
                // 保存金币数量
                PlayerPrefs.SetInt("Economy_Coin", economyModel.Coin);
                Debug.Log($"金币数据保存完成：{economyModel.Coin}");
            }
        }

        /// <summary>
        /// 显示保存成功提示
        /// 文本显示1秒后自动消失
        /// </summary>
        private void ShowSaveSuccessMessage()
        {
            Debug.Log("游戏保存成功！");

            if (txtSaveSuccess != null)
            {
                // 设置提示文本
                txtSaveSuccess.text = "保存成功！";
                // 显示提示文本
                txtSaveSuccess.gameObject.SetActive(true);
                // 启动协程，1秒后隐藏提示文本
                StartCoroutine(HideSaveSuccessMessage());
            }
        }

        /// <summary>
        /// 隐藏保存成功提示
        /// 1秒后自动隐藏
        /// </summary>
        /// <returns></returns>
        private System.Collections.IEnumerator HideSaveSuccessMessage()
        {
            // 等待1秒
            yield return new WaitForSeconds(1f);
            // 隐藏提示文本
            if (txtSaveSuccess != null)
            {
                txtSaveSuccess.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            // 移除事件监听，避免内存泄漏
            if (volumeSlider != null)
            {
                volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
            }

            if (btnBackMenu != null)
            {
                btnBackMenu.onClick.RemoveListener(OnBackMenuClick);
            }

            if (btnSaveGame != null)
            {
                btnSaveGame.onClick.RemoveListener(OnSaveGameClick);
            }
        }
    }
}
