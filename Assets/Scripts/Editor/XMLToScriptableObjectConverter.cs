using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace Game.Editor
{
    /// <summary>
    /// XML到ScriptableObject转换工具
    /// </summary>
    public class XMLToScriptableObjectConverter
    {
        private const string XML_FILE_NAME = "数据配置xml.xml";
        private const string ASSETS_OUTPUT_PATH = "Assets/Resources/GameData/";

        /// <summary>
        /// 编辑器菜单选项：转换所有XML数据到ScriptableObject
        /// </summary>
        [MenuItem("GameTools/Convert XML to ScriptableObject")]
        public static void ConvertAllXMLData()
        {
            // 创建输出目录
            EnsureOutputDirectoryExists();

            // 读取XML文件
            XDocument xmlDoc = ReadXMLFile();
            if (xmlDoc == null) return;

            // 转换物品数据
            ConvertItems(xmlDoc);

            // 转换角色数据
            ConvertCharacters(xmlDoc);

            // 转换地图数据
            ConvertMaps(xmlDoc);

            // 转换商店数据
            ConvertShop(xmlDoc);

            // 刷新AssetDatabase
            AssetDatabase.Refresh();

            Debug.Log("XML到ScriptableObject转换完成！");
        }

        /// <summary>
        /// 确保输出目录存在
        /// </summary>
        private static void EnsureOutputDirectoryExists()
        {
            Debug.Log($"检查输出目录：{ASSETS_OUTPUT_PATH}");

            // 创建主目录
            if (!Directory.Exists(ASSETS_OUTPUT_PATH))
            {
                Debug.Log($"创建主目录：{ASSETS_OUTPUT_PATH}");
                Directory.CreateDirectory(ASSETS_OUTPUT_PATH);
            }

            // 创建子目录
            string[] subdirectories = { "Items", "Characters", "Maps", "Shop" };
            foreach (string subdir in subdirectories)
            {
                string subdirPath = Path.Combine(ASSETS_OUTPUT_PATH, subdir);
                if (!Directory.Exists(subdirPath))
                {
                    Debug.Log($"创建子目录：{subdirPath}");
                    Directory.CreateDirectory(subdirPath);
                }
            }
        }

        /// <summary>
        /// 读取XML文件
        /// </summary>
        private static XDocument ReadXMLFile()
        {
            // 直接使用Application.streamingAssetsPath获取StreamingAssets文件夹路径
            string fullPath = Path.Combine(Application.streamingAssetsPath, XML_FILE_NAME);
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"XML文件不存在：{fullPath}");
                Debug.LogError($"请确保 {XML_FILE_NAME} 文件存在于 StreamingAssets 文件夹中");
                return null;
            }

            try
            {
                return XDocument.Load(fullPath);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"读取XML文件失败：{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 转换物品数据
        /// </summary>
        private static void ConvertItems(XDocument xmlDoc)
        {
            Debug.Log("开始转换物品数据...");

            var itemsElement = xmlDoc.Root.Element("Items");
            if (itemsElement == null)
            {
                Debug.LogError("XML中没有找到Items元素");
                return;
            }

            var itemElements = itemsElement.Elements("Item");
            Debug.Log($"找到 {itemElements.Count()} 个Item元素");

            int createdCount = 0;

            foreach (var itemElement in itemElements)
            {
                try
                {
                    string id = itemElement.Attribute("Id").Value;
                    string name = itemElement.Attribute("Name").Value;
                    string typeStr = itemElement.Attribute("Type").Value;
                    int value = int.Parse(itemElement.Attribute("Value").Value);
                    bool canStack = bool.Parse(itemElement.Attribute("CanStack").Value);
                    string description = itemElement.Element("Description").Value;

                    Debug.Log($"处理物品：{id} - {name} ({typeStr})");

                    ItemType itemType = (ItemType)System.Enum.Parse(typeof(ItemType), typeStr);

                    // 根据物品类型创建不同的ScriptableObject
                    ItemData itemData = null;

                    switch (itemType)
                    {
                        case ItemType.Weapon:
                            itemData = CreateWeaponData(itemElement, id, name, value, canStack, description);
                            break;
                        case ItemType.Ammo:
                            itemData = CreateAmmoData(itemElement, id, name, value, canStack, description);
                            break;
                        case ItemType.Armor:
                            itemData = CreateArmorData(itemElement, id, name, value, canStack, description);
                            break;
                        case ItemType.Misc:
                            itemData = CreateMiscData(itemElement, id, name, value, canStack, description);
                            break;
                    }

                    if (itemData != null)
                    {
                        // 保存为Asset文件
                        string assetPath = Path.Combine(ASSETS_OUTPUT_PATH, "Items", $"{id}.asset");
                        // 统一路径格式为Unity标准的正斜杠
                        assetPath = assetPath.Replace('\\', '/');
                        Debug.Log($"创建Asset文件：{assetPath}");
                        AssetDatabase.CreateAsset(itemData, assetPath);
                        createdCount++;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"处理物品时出错：{ex.Message}");
                    Debug.LogError($"物品元素：{itemElement.ToString()}");
                }
            }

            Debug.Log($"物品数据转换完成，成功创建 {createdCount} 个ScriptableObject");
        }

        /// <summary>
        /// 创建武器数据
        /// </summary>
        private static WeaponData CreateWeaponData(XElement itemElement, string id, string name, int value, bool canStack, string description)
        {
            var weaponElement = itemElement.Element("Weapon");
            if (weaponElement == null) return null;

            WeaponType weaponType = (WeaponType)System.Enum.Parse(typeof(WeaponType), weaponElement.Attribute("WeaponType").Value);
            AmmoType ammoType = (AmmoType)System.Enum.Parse(typeof(AmmoType), weaponElement.Attribute("AmmoType").Value);
            int baseDamage = int.Parse(weaponElement.Attribute("BaseDamage").Value);
            int maxAmmoLevel = int.Parse(weaponElement.Attribute("MaxAmmoLevel").Value);

            WeaponData weaponData = ScriptableObject.CreateInstance<WeaponData>();
            SetBaseItemData(weaponData, id, name, ItemType.Weapon, value, canStack, description);
            weaponData.WeaponType = weaponType;
            weaponData.AmmoType = ammoType;
            weaponData.BaseDamage = baseDamage;
            weaponData.MaxAmmoLevel = maxAmmoLevel;

            return weaponData;
        }

        /// <summary>
        /// 创建弹药数据
        /// </summary>
        private static AmmoData CreateAmmoData(XElement itemElement, string id, string name, int value, bool canStack, string description)
        {
            var ammoElement = itemElement.Element("Ammo");
            if (ammoElement == null) return null;

            AmmoType ammoType = (AmmoType)System.Enum.Parse(typeof(AmmoType), ammoElement.Attribute("AmmoType").Value);
            int level = int.Parse(ammoElement.Attribute("Level").Value);
            int damage = int.Parse(ammoElement.Attribute("Damage").Value);

            AmmoData ammoData = ScriptableObject.CreateInstance<AmmoData>();
            SetBaseItemData(ammoData, id, name, ItemType.Ammo, value, canStack, description);
            ammoData.AmmoType = ammoType;
            ammoData.Level = level;
            ammoData.Damage = damage;

            return ammoData;
        }

        /// <summary>
        /// 创建防具数据
        /// </summary>
        private static ArmorData CreateArmorData(XElement itemElement, string id, string name, int value, bool canStack, string description)
        {
            var armorElement = itemElement.Element("Armor");
            if (armorElement == null) return null;

            string slot = armorElement.Attribute("Slot").Value;
            int level = int.Parse(armorElement.Attribute("Level").Value);
            int damageReduction = int.Parse(armorElement.Attribute("DamageReduction").Value);

            ArmorData armorData = ScriptableObject.CreateInstance<ArmorData>();
            SetBaseItemData(armorData, id, name, ItemType.Armor, value, canStack, description);
            armorData.Slot = slot;
            armorData.Level = level;
            armorData.DamageReduction = damageReduction;

            return armorData;
        }

        /// <summary>
        /// 创建杂物数据
        /// </summary>
        private static ItemData CreateMiscData(XElement itemElement, string id, string name, int value, bool canStack, string description)
        {
            ItemData miscData = ScriptableObject.CreateInstance<ItemData>();
            SetBaseItemData(miscData, id, name, ItemType.Misc, value, canStack, description);

            return miscData;
        }

        /// <summary>
        /// 设置基础物品数据
        /// </summary>
        private static void SetBaseItemData(ItemData itemData, string id, string name, ItemType type, int value, bool canStack, string description)
        {
            itemData.Id = id;
            itemData.Name = name;
            itemData.Type = type;
            itemData.Value = value;
            itemData.CanStack = canStack;
            itemData.Description = description;
        }

        /// <summary>
        /// 转换角色数据
        /// </summary>
        private static void ConvertCharacters(XDocument xmlDoc)
        {
            var charElements = xmlDoc.Root.Element("Characters").Elements("Character");

            foreach (var charElement in charElements)
            {
                string id = charElement.Attribute("Id").Value;
                string name = charElement.Attribute("Name").Value;
                bool isPlayer = bool.Parse(charElement.Attribute("IsPlayer").Value);
                string description = charElement.Element("Description").Value;
                string avatar = charElement.Element("Avatar").Value;

                CharacterData charData = ScriptableObject.CreateInstance<CharacterData>();
                charData.Id = id;
                charData.Name = name;
                charData.IsPlayer = isPlayer;
                charData.Type = isPlayer ? CharacterType.Player : CharacterType.Enemy;
                charData.Description = description;
                charData.Avatar = avatar;

                // 保存为Asset文件
                string assetPath = Path.Combine(ASSETS_OUTPUT_PATH, "Characters", $"{id}.asset");
                // 统一路径格式为Unity标准的正斜杠
                assetPath = assetPath.Replace('\\', '/');
                AssetDatabase.CreateAsset(charData, assetPath);
            }
        }

        /// <summary>
        /// 转换地图数据
        /// </summary>
        private static void ConvertMaps(XDocument xmlDoc)
        {
            var mapElements = xmlDoc.Root.Element("Maps").Elements("Map");

            foreach (var mapElement in mapElements)
            {
                string mapId = mapElement.Attribute("Id").Value;
                string name = mapElement.Attribute("Name").Value;
                string description = mapElement.Element("Description").Value;

                MapData mapData = ScriptableObject.CreateInstance<MapData>();
                mapData.MapId = mapId;
                mapData.MapName = name;
                mapData.Description = description;
                mapData.SceneName = $"Map{mapId}";

                // 保存为Asset文件
                string assetPath = Path.Combine(ASSETS_OUTPUT_PATH, "Maps", $"map_{mapId}.asset");
                // 统一路径格式为Unity标准的正斜杠
                assetPath = assetPath.Replace('\\', '/');
                AssetDatabase.CreateAsset(mapData, assetPath);
            }
        }

        /// <summary>
        /// 转换商店数据
        /// </summary>
        private static void ConvertShop(XDocument xmlDoc)
        {
            ShopData shopData = ScriptableObject.CreateInstance<ShopData>();
            shopData.ShopId = "main_shop";
            shopData.ShopName = "主商店";
            shopData.InitialGold = 1000;

            var shopElements = xmlDoc.Root.Element("Shop").Elements("ShopItem");

            foreach (var shopElement in shopElements)
            {
                string itemId = shopElement.Attribute("ItemId").Value;
                int price = int.Parse(shopElement.Attribute("Price").Value);

                ShopItemData shopItem = new ShopItemData();
                shopItem.ItemId = itemId;
                shopItem.Price = price;

                shopData.AddItem(shopItem);
            }

            // 保存为Asset文件
            string assetPath = Path.Combine(ASSETS_OUTPUT_PATH, "Shop", "main_shop.asset");
            // 统一路径格式为Unity标准的正斜杠
            assetPath = assetPath.Replace('\\', '/');
            AssetDatabase.CreateAsset(shopData, assetPath);
        }
    }
}