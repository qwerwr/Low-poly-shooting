using QFramework;
using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// 游戏数据模型
    /// </summary>
    public class GameDataModel : AbstractModel
    {
        // 物品数据字典
        private Dictionary<string, ItemData> m_Items = new Dictionary<string, ItemData>();
        
        // 角色数据字典
        private Dictionary<string, CharacterData> m_Characters = new Dictionary<string, CharacterData>();
        
        // 地图数据字典
        private Dictionary<int, MapData> m_Maps = new Dictionary<int, MapData>();
        
        // 商店数据
        private ShopData m_Shop;
        
        /// <summary>
        /// 物品数据
        /// </summary>
        public Dictionary<string, ItemData> Items
        {
            get { return m_Items; }
            set { m_Items = value; }
        }
        
        /// <summary>
        /// 角色数据
        /// </summary>
        public Dictionary<string, CharacterData> Characters
        {
            get { return m_Characters; }
            set { m_Characters = value; }
        }
        
        /// <summary>
        /// 地图数据
        /// </summary>
        public Dictionary<int, MapData> Maps
        {
            get { return m_Maps; }
            set { m_Maps = value; }
        }
        
        /// <summary>
        /// 商店数据
        /// </summary>
        public ShopData Shop
        {
            get { return m_Shop; }
            set { m_Shop = value; }
        }
        
        /// <summary>
        /// 初始化模型
        /// </summary>
        protected override void OnInit()
        {
            // 从数据加载器加载数据
            LoadDataFromLoader();
        }
        
        /// <summary>
        /// 从数据加载器加载数据
        /// </summary>
        private void LoadDataFromLoader()
        {
            IDataLoader dataLoader = this.GetUtility<IDataLoader>();
            if (dataLoader != null)
            {
                m_Items = dataLoader.LoadItems();
                m_Characters = dataLoader.LoadCharacters();
                m_Maps = dataLoader.LoadMaps();
                m_Shop = dataLoader.LoadShop();
            }
        }
        
        /// <summary>
        /// 刷新数据
        /// </summary>
        public void RefreshData()
        {
            LoadDataFromLoader();
        }
    }
}