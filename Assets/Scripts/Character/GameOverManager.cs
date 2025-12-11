using UnityEngine;
using UnityEngine.SceneManagement;
using QFramework;

namespace Game
{
    /// <summary>
    /// 游戏结束管理器
    /// 处理玩家死亡时的游戏结束逻辑
    /// </summary>
    public class GameOverManager : MonoBehaviour
    {
        /// <summary>
        /// 游戏结束界面预制体
        /// </summary>
        [Header("游戏结束设置")]
        [SerializeField] private GameObject gameOverUI;
        
        /// <summary>
        /// 玩家标签
        /// </summary>
      //  [SerializeField] private string playerTag = "Player";
        
        /// <summary>
        /// 游戏结束后延迟跳转到Personal场景的时间（秒）
        /// </summary>
        [SerializeField] private float delayBeforeSceneLoad = 3f;
        
        /// <summary>
        /// 是否已经处理过游戏结束
        /// </summary>
        private bool hasProcessedGameOver = false;
        
        /// <summary>
        /// 背包模型
        /// </summary>
        private InventoryModel inventoryModel;
        
        /// <summary>
        /// 人物模型
        /// </summary>
        private CharacterModel characterModel;
        
        private void Start()
        {
            // 获取数据模型
            inventoryModel = GameArchitecture.Interface.GetModel<InventoryModel>();
            characterModel = GameArchitecture.Interface.GetModel<CharacterModel>();
            
            // 确保游戏结束界面初始隐藏
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(false);
            }
        }
    
        
        /// <summary>
        /// 处理玩家死亡
        /// 可从PlayerCharacterController直接调用
        /// </summary>
        public void HandlePlayerDeath()
        {
            if (hasProcessedGameOver) return;
            hasProcessedGameOver = true;
            
            Debug.Log("[GameOverManager] 处理玩家死亡");
            
            // 显示游戏结束界面
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
            }
            
            // 清空背包物品
            ClearAllItems();
            
            // 延迟跳转到Personal场景
            Invoke(nameof(LoadPersonalScene), delayBeforeSceneLoad);
        }
        
        /// <summary>
        /// 清空所有物品
        /// 包括背包和人物面板的物品
        /// </summary>
        private void ClearAllItems()
        {
          //  Debug.Log("[GameOverManager] 清空所有物品");
            
            // 清空背包物品
            if (inventoryModel != null)
            {
                inventoryModel.ClearInventory();
                Debug.Log("[GameOverManager] 背包物品已清空");
            }
            
            // 清空人物面板物品
            if (characterModel != null)
            {
                // 清空装备
                characterModel.Helmet = null;
                characterModel.Armor = null;
                characterModel.Weapon = null;
                
                // 清空人物物品栏
                if (characterModel.Items != null)
                {
                    characterModel.Items.Clear();
                }
                
                //Debug.Log("[GameOverManager] 人物面板物品已清空");
            }
        }
        
        /// <summary>
        /// 加载Personal场景
        /// </summary>
        private void LoadPersonalScene()
        {
            Debug.Log("[GameOverManager] 正在跳转到Personal场景");
            SceneManager.LoadScene("Personal");
        }
    }
}