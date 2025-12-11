using QFramework;

namespace Game
{
    /// <summary>
    /// 经济系统
    /// 处理物品购买和售卖逻辑
    /// </summary>
    public class EconomySystem : AbstractSystem
    {
        /// <summary>
        /// 获取经济模型
        /// </summary>
        private EconomyModel m_EconomyModel => this.GetModel<EconomyModel>();

        /// <summary>
        /// 初始化经济系统
        /// </summary>
        protected override void OnInit()
        {
            // 经济系统初始化逻辑
        }

        /// <summary>
        /// 获取金币数量
        /// </summary>
        /// <returns>金币数量</returns>
        public int GetCoin() => m_EconomyModel.Coin;

        /// <summary>
        /// 增加金币
        /// </summary>
        /// <param name="amount">增加的数量</param>
        public void AddCoin(int amount) => m_EconomyModel.Coin += amount;

        /// <summary>
        /// 减少金币
        /// </summary>
        /// <param name="amount">减少的数量</param>
        /// <returns>是否减少成功</returns>
        public bool RemoveCoin(int amount)
        {
            if (m_EconomyModel.Coin >= amount)
            {
                m_EconomyModel.Coin -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 购买物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="quantity">数量</param>
        /// <returns>是否购买成功</returns>
        public bool PurchaseItem(string itemId, int quantity = 1)
        {
            // 获取物品价格
            int price = GetItemPrice(itemId);
            int totalPrice = price * quantity;

            // 检查金币是否足够
            if (RemoveCoin(totalPrice))
            {
                // 调用商店系统添加物品到仓库
                this.GetSystem<ShopSystem>().AddItemToWarehouse(itemId, quantity);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 售卖物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="quantity">数量</param>
        /// <returns>是否售卖成功</returns>
        public bool SellItem(string itemId, int quantity = 1)
        {
            // 获取物品售卖价格（购买价格的80%）
            int sellPrice = (int)(GetItemPrice(itemId) * 0.8f);
            int totalSellPrice = sellPrice * quantity;

            // 调用商店系统从仓库移除物品
            if (this.GetSystem<ShopSystem>().RemoveItemFromWarehouse(itemId, quantity))
            {
                // 增加金币
                AddCoin(totalSellPrice);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取物品价格
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品价格</returns>
        private int GetItemPrice(string itemId)
        {
            // 从GameDataModel获取物品数据，返回价格
            GameDataModel gameDataModel = this.GetModel<GameDataModel>();
            if (gameDataModel.Items.ContainsKey(itemId))
            {
                return gameDataModel.Items[itemId].Value;
            }
            return 0;
        }
    }
}