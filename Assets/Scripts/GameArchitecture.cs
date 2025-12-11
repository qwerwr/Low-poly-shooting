using QFramework;
using System.Data;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
namespace Game
{
    /// <summary>
    /// 游戏架构 - 整个游戏系统的核心
    /// 架构首脑只负责注册和协调，不处理具体业务逻辑
    /// </summary>
    public class GameArchitecture : Architecture<GameArchitecture>
    {
        /// <summary>
        /// 数据加载器类型枚举
        /// </summary>
        public enum DataLoaderType
        {
            XML,        // XML数据加载器
            AssetBundle // AssetBundle数据加载器
        }

        /// <summary>
        /// 当前使用的数据加载器类型
        /// </summary>
        public static DataLoaderType CurrentDataLoaderType = DataLoaderType.AssetBundle;

        /// <summary>
        /// 初始化游戏架构
        /// </summary>
        protected override void Init()
        {
            // 注册模型
            RegisterModel<GameDataModel>(new GameDataModel());
            RegisterModel<InventoryModel>(new InventoryModel());
            RegisterModel<CharacterModel>(new CharacterModel());
            RegisterModel<ItemBoxModel>(new ItemBoxModel());
            RegisterModel<WarehouseModel>(new WarehouseModel());
            RegisterModel<EconomyModel>(new EconomyModel());
            // 注册工具 - 根据配置选择数据加载器
            IDataLoader dataLoader;
            switch (CurrentDataLoaderType)
            {
                case DataLoaderType.AssetBundle:
                    dataLoader = new AssetBundleDataLoader();
                    Debug.Log("使用AssetBundle数据加载器");
                    break;
                case DataLoaderType.XML:
                default:
                    dataLoader = new XMLDataLoader();
                    Debug.Log("使用XML数据加载器");
                    break;
            }
            RegisterUtility<IDataLoader>(dataLoader);

            // 注册系统
            RegisterSystem<CharacterInputHandler>(new CharacterInputHandler());

            RegisterSystem<AmmoSystem>(new AmmoSystem());
            RegisterSystem<HealthSystem>(new HealthSystem());
            RegisterSystem<EconomySystem>(new EconomySystem());
            RegisterSystem<ShopSystem>(new ShopSystem());
           
        }
    }
}