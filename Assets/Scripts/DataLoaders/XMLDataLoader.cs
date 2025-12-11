using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// XML数据加载器实现类
    /// </summary>
    public class XMLDataLoader : IDataLoader
    {
        private const string GAME_DATA_PATH = "StreamingAssets/数据配置xml.xml";

        private XDocument mGameData;

        public XMLDataLoader()
        {
            LoadXMLData();
        }

        /// <summary>
        /// 加载XML数据文件
        /// </summary>
        private void LoadXMLData()
        {
            try
            {
                // 获取正确的StreamingAssets路径
                string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "数据配置xml.xml");

                if (System.IO.File.Exists(filePath))
                {
                    string xmlContent = System.IO.File.ReadAllText(filePath);
                    mGameData = XDocument.Parse(xmlContent);
                    Debug.Log("XML数据加载成功");
                }
                else
                {
                    Debug.LogError($"无法加载XML文件: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"XML数据加载失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 加载物品数据
        /// </summary>
        public Dictionary<string, ItemData> LoadItems()
        {
            var items = new Dictionary<string, ItemData>();

            if (mGameData == null) return items;

            try
            {
                var itemElements = mGameData.Root.Element("Items")?.Elements("Item");
                if (itemElements != null)
                {
                    foreach (var itemElement in itemElements)
                    {
                        var itemData = ParseItemData(itemElement);
                        if (itemData != null)
                        {
                            items[itemData.Id] = itemData;
                        }
                    }
                }
                Debug.Log($"成功加载 {items.Count} 个物品");
            }
            catch (Exception ex)
            {
                Debug.LogError($"物品数据加载失败: {ex.Message}");
            }

            return items;
        }

        /// <summary>
        /// 解析物品数据
        /// </summary>
        private ItemData ParseItemData(XElement itemElement)
        {
            try
            {
                string id = itemElement.Attribute("Id")?.Value ?? "";
                string name = itemElement.Attribute("Name")?.Value ?? "";
                string typeStr = itemElement.Attribute("Type")?.Value ?? "";
                int value = int.Parse(itemElement.Attribute("Value")?.Value ?? "0");
                bool canStack = bool.Parse(itemElement.Attribute("CanStack")?.Value ?? "false");
                string description = itemElement.Element("Description")?.Value ?? "";

                // 根据物品类型创建对应的数据对象
                if (Enum.TryParse<ItemType>(typeStr, out var itemType))
                {
                    ItemData itemData = null;

                    switch (itemType)
                    {
                        case ItemType.Weapon:
                            itemData = ParseWeaponData(itemElement, id, name, value, canStack, description);
                            break;
                        case ItemType.Ammo:
                            itemData = ParseAmmoData(itemElement, id, name, value, canStack, description);
                            break;
                        case ItemType.Armor:
                            itemData = ParseArmorData(itemElement, id, name, value, canStack, description);
                            break;
                        case ItemType.Misc:
                            itemData = new ItemData();
                            itemData.Id = id;
                            itemData.Name = name;
                            itemData.Type = itemType;
                            itemData.Value = value;
                            itemData.CanStack = canStack;
                            itemData.Description = description;
                            break;
                    }

                    return itemData;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"解析物品数据失败: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// 解析武器数据
        /// </summary>
        private WeaponData ParseWeaponData(XElement itemElement, string id, string name, int value, bool canStack, string description)
        {
            var weaponElement = itemElement.Element("Weapon");
            if (weaponElement == null) return null;

            string weaponTypeStr = weaponElement.Attribute("WeaponType")?.Value ?? "";
            string ammoTypeStr = weaponElement.Attribute("AmmoType")?.Value ?? "";
            int baseDamage = int.Parse(weaponElement.Attribute("BaseDamage")?.Value ?? "0");
            int maxAmmoLevel = int.Parse(weaponElement.Attribute("MaxAmmoLevel")?.Value ?? "0");

            if (Enum.TryParse<WeaponType>(weaponTypeStr, out var weaponType) &&
                Enum.TryParse<AmmoType>(ammoTypeStr, out var ammoType))
            {
                WeaponData weaponData = new WeaponData();
                weaponData.Id = id;
                weaponData.Name = name;
                weaponData.Type = ItemType.Weapon;
                weaponData.Value = value;
                weaponData.CanStack = canStack;
                weaponData.Description = description;
                weaponData.WeaponType = weaponType;
                weaponData.AmmoType = ammoType;
                weaponData.BaseDamage = baseDamage;
                weaponData.MaxAmmoLevel = maxAmmoLevel;

                return weaponData;
            }

            return null;
        }

        /// <summary>
        /// 解析弹药数据
        /// </summary>
        private AmmoData ParseAmmoData(XElement itemElement, string id, string name, int value, bool canStack, string description)
        {
            var ammoElement = itemElement.Element("Ammo");
            if (ammoElement == null) return null;

            string ammoTypeStr = ammoElement.Attribute("AmmoType")?.Value ?? "";
            int level = int.Parse(ammoElement.Attribute("Level")?.Value ?? "1");
            int damage = int.Parse(ammoElement.Attribute("Damage")?.Value ?? "0");

            if (Enum.TryParse<AmmoType>(ammoTypeStr, out var ammoType))
            {
                AmmoData ammoData = new AmmoData();
                ammoData.Id = id;
                ammoData.Name = name;
                ammoData.Type = ItemType.Ammo;
                ammoData.Value = value;
                ammoData.CanStack = canStack;
                ammoData.Description = description;
                ammoData.AmmoType = ammoType;
                ammoData.Level = level;
                ammoData.Damage = damage;

                return ammoData;
            }

            return null;
        }

        /// <summary>
        /// 解析防具数据
        /// </summary>
        private ArmorData ParseArmorData(XElement itemElement, string id, string name, int value, bool canStack, string description)
        {
            var armorElement = itemElement.Element("Armor");
            if (armorElement == null) return null;

            string slot = armorElement.Attribute("Slot")?.Value ?? "";
            int level = int.Parse(armorElement.Attribute("Level")?.Value ?? "1");
            int damageReduction = int.Parse(armorElement.Attribute("DamageReduction")?.Value ?? "0");

            ArmorData armorData = new ArmorData();
            armorData.Id = id;
            armorData.Name = name;
            armorData.Type = ItemType.Armor;
            armorData.Value = value;
            armorData.CanStack = canStack;
            armorData.Description = description;
            armorData.Slot = slot;
            armorData.Level = level;
            armorData.DamageReduction = damageReduction;

            return armorData;
        }

        /// <summary>
        /// 加载角色数据
        /// </summary>
        public Dictionary<string, CharacterData> LoadCharacters()
        {
            var characters = new Dictionary<string, CharacterData>();

            if (mGameData == null) return characters;

            try
            {
                var charElements = mGameData.Root.Element("Characters")?.Elements("Character");
                if (charElements != null)
                {
                    foreach (var charElement in charElements)
                    {
                        var charData = ParseCharacterData(charElement);
                        if (charData != null)
                        {
                            characters[charData.Id] = charData;
                        }
                    }
                }
                Debug.Log($"成功加载 {characters.Count} 个角色");
            }
            catch (Exception ex)
            {
                Debug.LogError($"角色数据加载失败: {ex.Message}");
            }

            return characters;
        }

        /// <summary>
        /// 解析角色数据
        /// </summary>
        private CharacterData ParseCharacterData(XElement charElement)
        {
            try
            {
                string id = charElement.Attribute("Id")?.Value ?? "";
                string name = charElement.Attribute("Name")?.Value ?? "";
                bool isPlayer = bool.Parse(charElement.Attribute("IsPlayer")?.Value ?? "false");
                string description = charElement.Element("Description")?.Value ?? "";
                string avatar = charElement.Element("Avatar")?.Value ?? "";

                CharacterData charData = new CharacterData();
                charData.Id = id;
                charData.Name = name;
                charData.IsPlayer = isPlayer;
                charData.Type = isPlayer ? CharacterType.Player : CharacterType.Enemy;
                charData.Description = description;
                charData.Avatar = avatar;

                return charData;
            }
            catch (Exception ex)
            {
                Debug.LogError($"解析角色数据失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 加载地图数据
        /// </summary>
        public Dictionary<int, MapData> LoadMaps()
        {
            var maps = new Dictionary<int, MapData>();

            if (mGameData == null) return maps;

            try
            {
                var mapElements = mGameData.Root.Element("Maps")?.Elements("Map");
                if (mapElements != null)
                {
                    foreach (var mapElement in mapElements)
                    {
                        var mapData = ParseMapData(mapElement);
                        if (mapData != null)
                        {
                            // 使用MapId转换为int作为字典键
                            if (int.TryParse(mapData.MapId, out int mapId))
                            {
                                maps[mapId] = mapData;
                            }
                        }
                    }
                }
                Debug.Log($"成功加载 {maps.Count} 个地图");
            }
            catch (Exception ex)
            {
                Debug.LogError($"地图数据加载失败: {ex.Message}");
            }

            return maps;
        }

        /// <summary>
        /// 解析地图数据
        /// </summary>
        private MapData ParseMapData(XElement mapElement)
        {
            try
            {
                string mapId = mapElement.Attribute("Id")?.Value ?? "0";
                string name = mapElement.Attribute("Name")?.Value ?? "";
                string description = mapElement.Element("Description")?.Value ?? "";

                MapData mapData = new MapData();
                mapData.MapId = mapId;
                mapData.MapName = name;
                mapData.Description = description;
                mapData.SceneName = $"Map{mapId}";

                return mapData;
            }
            catch (Exception ex)
            {
                Debug.LogError($"解析地图数据失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 加载商店数据
        /// </summary>
        public ShopData LoadShop()
        {
            ShopData shopData = new ShopData();
            shopData.ShopId = "main_shop";
            shopData.ShopName = "主商店";
            shopData.InitialGold = 1000;

            if (mGameData == null) return shopData;

            try
            {
                var shopElements = mGameData.Root.Element("Shop")?.Elements("ShopItem");
                if (shopElements != null)
                {
                    foreach (var shopElement in shopElements)
                    {
                        var shopItem = ParseShopItemData(shopElement);
                        if (shopItem != null)
                        {
                            shopData.AddItem(shopItem);
                        }
                    }
                }
                Debug.Log($"成功加载 {shopData.Items.Count} 个商店物品");
            }
            catch (Exception ex)
            {
                Debug.LogError($"商店数据加载失败: {ex.Message}");
            }

            return shopData;
        }

        /// <summary>
        /// 解析商店物品数据
        /// </summary>
        private ShopItemData ParseShopItemData(XElement shopElement)
        {
            try
            {
                string itemId = shopElement.Attribute("ItemId")?.Value ?? "";
                int price = int.Parse(shopElement.Attribute("Price")?.Value ?? "0");

                ShopItemData shopItem = new ShopItemData();
                shopItem.ItemId = itemId;
                shopItem.Price = price;

                return shopItem;
            }
            catch (Exception ex)
            {
                Debug.LogError($"解析商店物品数据失败: {ex.Message}");
                return null;
            }
        }
    }
}