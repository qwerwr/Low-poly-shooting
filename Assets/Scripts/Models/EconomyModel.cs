using QFramework;

namespace Game
{
    /// <summary>
    /// 金币变化事件
    /// </summary>
    public class CoinChangedEvent 
    {
        public int NewCoin { get; set; }
    }
    
    /// <summary>
    /// 经济模型
    /// 用于管理玩家金币
    /// </summary>
    public class EconomyModel : AbstractModel
    {
        /// <summary>
        /// 金币数量
        /// </summary>
        private int m_Coin = 1000; // 初始金币
        
        /// <summary>
        /// 金币属性
        /// </summary>
        public int Coin
        {
            get => m_Coin;
            set
            {
                m_Coin = value;
                // 触发金币变化事件
                this.SendEvent<CoinChangedEvent>(new CoinChangedEvent { NewCoin = m_Coin });
            }
        }
        
        /// <summary>
        /// 初始化模型
        /// </summary>
        protected override void OnInit()
        {
            // 初始化金币
            m_Coin = 1000;
        }
    }
}