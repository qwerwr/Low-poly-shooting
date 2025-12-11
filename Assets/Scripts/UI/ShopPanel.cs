using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using QFramework;

namespace Game.UI
{
    /// <summary>
    /// 商店面板组件
    /// </summary>
    public class ShopPanel : MonoBehaviour, ICanRegisterEvent
    {
        /// <summary>
        /// 实现ICanRegisterEvent接口
        /// </summary>
        /// <returns>游戏架构实例</returns>
        public IArchitecture GetArchitecture()
        {
            return GameArchitecture.Interface;
        }

        /// <summary>
        /// 商店物品容器
        /// </summary>
        public Transform ShopItemsContainer;

        /// <summary>
        /// 商店物品预制体
        /// </summary>
        public GameObject ShopItemPrefab;

        /// <summary>
        /// 返回按钮
        /// </summary>
        public Button BackButton;

        /// <summary>
        /// 经济系统
        /// </summary>
        private EconomySystem m_EconomySystem;

        /// <summary>
        /// 商店系统
        /// </summary>
        private ShopSystem m_ShopSystem;

        /// <summary>
        /// 游戏数据模型
        /// </summary>
        private GameDataModel m_GameDataModel;

        private void Start()
        {
            // 获取系统和模型
            m_EconomySystem = GameArchitecture.Interface.GetSystem<EconomySystem>();
            m_ShopSystem = GameArchitecture.Interface.GetSystem<ShopSystem>();
            m_GameDataModel = GameArchitecture.Interface.GetModel<GameDataModel>();

            // 初始化UI
            InitializeUI();
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitializeUI()
        {
            // 创建商店物品
            CreateShopItems();

            // 设置返回按钮事件
            if (BackButton != null)
            {
                BackButton.onClick.AddListener(() => {
                    // 调用InventoryManager的方法切换Canvas
                    InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
                    if (inventoryManager != null)
                    {
                        inventoryManager.ShowWarehouseCanvas();
                    }
                });
            }
        }

        /// <summary>
        /// 创建商店物品
        /// </summary>
        private void CreateShopItems()
        {
            // 清空现有物品
            foreach (Transform child in ShopItemsContainer)
            {
                Destroy(child.gameObject);
            }

            // 创建商店物品
            foreach (var itemPair in m_GameDataModel.Items)
            {
                ItemData itemData = itemPair.Value;

                // 实例化商店物品预制体
                GameObject shopItemObj = Instantiate(ShopItemPrefab, ShopItemsContainer);

                // 设置物品数据
                ShopItemUI shopItemUI = shopItemObj.GetComponent<ShopItemUI>();
                if (shopItemUI != null)
                {
                    shopItemUI.Setup(itemData, OnPurchaseItem);
                }
            }
        }

        /// <summary>
        /// 购买物品回调
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="quantity">数量</param>
        private void OnPurchaseItem(string itemId, int quantity)
        {
            // 检查仓库是否已满
            if (m_ShopSystem.IsWarehouseFull())
            {
                Debug.Log("仓库已满");
                return;
            }

            // 调用经济系统购买物品
            bool success = m_EconomySystem.PurchaseItem(itemId, quantity);
            if (!success)
            {
                Debug.Log("金币不足");
            }
            else
            {
                // 更新ShopCanvas中的仓库UI
                UpdateWarehouseInShopCanvas();
            }
        }

        /// <summary>
        /// 更新ShopCanvas中的仓库UI
        /// </summary>
        private void UpdateWarehouseInShopCanvas()
        {
            // 查找ShopCanvas中的WarehousePanel
            WarehousePanel[] warehousePanels = FindObjectsOfType<WarehousePanel>();
            foreach (var panel in warehousePanels)
            {
                if (panel.CurrentCanvasType == CanvasType.Shop)
                {
                    panel.UpdateWarehouseUI();
                    break;
                }
            }
        }
    }
}