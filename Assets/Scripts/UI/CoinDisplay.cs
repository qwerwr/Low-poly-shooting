using UnityEngine;
using TMPro;
using QFramework;

namespace Game.UI
{
    /// <summary>
    /// 金币显示组件
    /// 用于显示玩家金币数量并监听金币变化事件
    /// </summary>
    public class CoinDisplay : MonoBehaviour, ICanRegisterEvent
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
        /// 金币文本
        /// </summary>
        public TextMeshProUGUI CoinText;
        
        /// <summary>
        /// 经济系统
        /// </summary>
        private EconomySystem m_EconomySystem;
        
        private void Start()
        {
            // 获取经济系统
            m_EconomySystem = GameArchitecture.Interface.GetSystem<EconomySystem>();
            
            // 初始化金币显示
            UpdateCoinDisplay();
            
            // 注册金币变化事件
            RegisterEvents();
        }
        
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents()
        {
            // 监听金币变化事件
            this.RegisterEvent<CoinChangedEvent>(OnCoinChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        
        /// <summary>
        /// 金币变化事件处理
        /// </summary>
        /// <param name="e">金币变化事件</param>
        private void OnCoinChanged(CoinChangedEvent e)
        {
            UpdateCoinDisplay();
        }
        
        /// <summary>
        /// 更新金币显示
        /// </summary>
        public void UpdateCoinDisplay()
        {
            if (CoinText != null && m_EconomySystem != null)
            {
                CoinText.text = m_EconomySystem.GetCoin().ToString();
            }
        }
    }
}