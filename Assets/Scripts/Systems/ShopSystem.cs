using QFramework;

namespace Game
{
    /// <summary>
    /// 商店系统
    /// 处理物品的添加和移除逻辑
    /// </summary>
    public class ShopSystem : AbstractSystem
    {
        /// <summary>
        /// 获取仓库模型
        /// </summary>
        private WarehouseModel m_WarehouseModel => this.GetModel<WarehouseModel>();

        /// <summary>
        /// 初始化商店系统
        /// </summary>
        protected override void OnInit()
        {
            // 商店系统初始化逻辑
        }

        /// <summary>
        /// 添加物品到仓库
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="quantity">数量</param>
        /// <returns>是否添加成功</returns>
        public bool AddItemToWarehouse(string itemId, int quantity = 1)
        {
            return m_WarehouseModel.AddItem(itemId, quantity);
        }

        /// <summary>
        /// 从仓库移除物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="quantity">数量</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveItemFromWarehouse(string itemId, int quantity = 1)
        {
            return m_WarehouseModel.RemoveItem(itemId, quantity);
        }

        /// <summary>
        /// 检查仓库是否已满
        /// </summary>
        /// <returns>是否已满</returns>
        public bool IsWarehouseFull()
        {
            return m_WarehouseModel.IsFull();
        }
    }
}