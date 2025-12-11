using UnityEngine;
using QFramework;

namespace Game
{
    /// <summary>
    /// 物品箱交互管理器
    /// </summary>
    public class ItemBoxInteractionManager : MonoBehaviour
    {
        [Header("交互设置")]
        public float InteractionDistance = 2f; // 交互距离
        public LayerMask ItemBoxLayer; // 物品箱所在的层

        [Header("UI设置")]
        public GameObject InteractionPrompt; // 交互提示UI
        public GameObject SearchPanel; // 搜索面板
        public GameObject BackpackPanel; // 背包面板
        public GameObject ItemBoxPanel; // 物品箱面板
        public GameObject FigurePanel; // 人物面板

        private bool m_IsNearItemBox = false;
        private bool m_IsInventoryOpen = false;
        private GameObject m_CurrentItemBox = null;

        // 防抖相关变量
        private int m_NearItemBoxFrameCount = 0;
        private int m_FarFromItemBoxFrameCount = 0;
        private const int k_StableFrameThreshold = 3; // 状态稳定所需的连续帧数

        private void Start()
        {
            // 注册事件 - 使用架构实例直接注册
            GameArchitecture.Interface.RegisterEvent<OpenItemBoxEvent>(OnOpenItemBox);
            GameArchitecture.Interface.RegisterEvent<ToggleInventoryEvent>(OnToggleInventory);

            // 初始化UI
            HideAllPanels();
            if (InteractionPrompt != null)
            {
                InteractionPrompt.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            // 注销事件
            GameArchitecture.Interface.UnRegisterEvent<OpenItemBoxEvent>(OnOpenItemBox);
            GameArchitecture.Interface.UnRegisterEvent<ToggleInventoryEvent>(OnToggleInventory);
        }

        private void Update()
        {
            // 检测是否靠近物品箱
            CheckNearItemBox();
        }

        /// <summary>
        /// 检测是否靠近物品箱
        /// </summary>
        private void CheckNearItemBox()
        {
            // 从角色位置发射射线检测物品箱
            RaycastHit hit;
            bool isNear = Physics.Raycast(transform.position, transform.forward, out hit, InteractionDistance, ItemBoxLayer);

            if (isNear)
            {
                // 检测到靠近物品箱
                m_FarFromItemBoxFrameCount = 0;
                m_NearItemBoxFrameCount++;

                // 连续多帧检测到靠近状态，才更新实际状态
                if (m_NearItemBoxFrameCount >= k_StableFrameThreshold && !m_IsNearItemBox)
                {
                    m_IsNearItemBox = true;
                    m_CurrentItemBox = hit.collider.gameObject;
                    ShowInteractionPrompt(true, "按F键开启");
                    m_NearItemBoxFrameCount = 0; // 重置计数器
                }
            }
            else
            {
                // 检测到远离物品箱
                m_NearItemBoxFrameCount = 0;
                m_FarFromItemBoxFrameCount++;

                // 连续多帧检测到远离状态，才更新实际状态
                if (m_FarFromItemBoxFrameCount >= k_StableFrameThreshold && m_IsNearItemBox)
                {
                    m_IsNearItemBox = false;
                    m_CurrentItemBox = null;
                    ShowInteractionPrompt(false);
                    m_FarFromItemBoxFrameCount = 0; // 重置计数器
                }
            }
        }

        /// <summary>
        /// 显示/隐藏交互提示
        /// </summary>
        /// <param name="show">是否显示</param>
        /// <param name="promptText">提示文本</param>
        private void ShowInteractionPrompt(bool show, string promptText = "")
        {
            if (InteractionPrompt != null)
            {
                InteractionPrompt.SetActive(show);
            }
            GameArchitecture.Interface.SendEvent(new InteractPromptEvent(show, promptText));
        }

        /// <summary>
        /// 打开物品箱
        /// </summary>
        private void OnOpenItemBox(OpenItemBoxEvent e)
        {
            if (m_IsNearItemBox && m_CurrentItemBox != null)
            {
                // 打开所有面板
                ShowAllPanels();
                m_IsInventoryOpen = true;
            }
        }

        /// <summary>
        /// 切换背包
        /// </summary>
        private void OnToggleInventory(ToggleInventoryEvent e)
        {
            if (m_IsInventoryOpen)
            {
                // 关闭所有面板
                HideAllPanels();
                m_IsInventoryOpen = false;
            }
            else
            {
                // 打开面板
                if (m_IsNearItemBox)
                {
                    ShowAllPanels();
                }
                else
                {
                    // 只显示背包和人物面板
                    ShowInventoryPanels(false);
                }
                m_IsInventoryOpen = true;
            }
        }

        /// <summary>
        /// 显示所有面板
        /// </summary>
        private void ShowAllPanels()
        {
            if (SearchPanel != null) SearchPanel.SetActive(true);
            if (BackpackPanel != null) BackpackPanel.SetActive(true);
            if (ItemBoxPanel != null) ItemBoxPanel.SetActive(true);
            if (FigurePanel != null) FigurePanel.SetActive(true);
        }

        /// <summary>
        /// 显示背包和人物面板
        /// </summary>
        /// <param name="includeItemBox">是否包含物品箱面板</param>
        private void ShowInventoryPanels(bool includeItemBox)
        {
            if (SearchPanel != null) SearchPanel.SetActive(true);
            if (BackpackPanel != null) BackpackPanel.SetActive(true);
            if (ItemBoxPanel != null) ItemBoxPanel.SetActive(includeItemBox);
            if (FigurePanel != null) FigurePanel.SetActive(true);
        }

        /// <summary>
        /// 隐藏所有面板
        /// </summary>
        private void HideAllPanels()
        {
            if (SearchPanel != null) SearchPanel.SetActive(false);
            if (BackpackPanel != null) BackpackPanel.SetActive(false);
            if (ItemBoxPanel != null) ItemBoxPanel.SetActive(false);
            if (FigurePanel != null) FigurePanel.SetActive(false);
        }
    }
}