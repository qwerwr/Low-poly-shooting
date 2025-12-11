using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// 人物面板
    /// 显示人物装备和物品栏
    /// </summary>
    public class CharacterPanel : MonoBehaviour
    {
        /// <summary>
        /// 人物图片
        /// </summary>
        public Image CharacterImage;

        /// <summary>
        /// 头盔槽位
        /// </summary>
        public InventorySlot HelmetSlot;

        /// <summary>
        /// 甲胄槽位
        /// </summary>
        public InventorySlot ArmorSlot;

        /// <summary>
        /// 枪械槽位
        /// </summary>
        public InventorySlot WeaponSlot;

        /// <summary>
        /// 物品槽容器
        /// </summary>
        public Transform ItemSlotsContainer;

        /// <summary>
        /// 物品槽预制体
        /// </summary>
        public GameObject ItemSlotPrefab;

        /// <summary>
        /// 物品预制体
        /// </summary>
        public GameObject ItemPrefab;

        /// <summary>
        /// 每行物品槽数量
        /// </summary>
        public int SlotsPerRow = 4;

        /// <summary>
        /// 物品槽间距
        /// </summary>
        public float SlotSpacing = 10f;

        /// <summary>
        /// 物品槽列表
        /// </summary>
        private List<InventorySlot> m_ItemSlots = new List<InventorySlot>();

        /// <summary>
        /// 物品列表
        /// </summary>
        private List<InventoryItem> m_Items = new List<InventoryItem>();

        /// <summary>
        /// 背包管理器
        /// </summary>
        private InventoryManager m_InventoryManager;

        /// <summary>
        /// 人物模型
        /// </summary>
        private CharacterModel m_CharacterModel;

        /// <summary>
        /// 初始化人物面板
        /// </summary>
        /// <param name="inventoryManager">背包管理器</param>
        public void Initialize(InventoryManager inventoryManager)
        {
            m_InventoryManager = inventoryManager;
            m_CharacterModel = GameArchitecture.Interface.GetModel<CharacterModel>();

            // 初始化特殊槽位
            InitializeSpecialSlots();

            // 创建物品槽
            CreateItemSlots();

            // 更新UI
            UpdateCharacterUI();
        }

        /// <summary>
        /// 初始化特殊槽位
        /// 设置允许的物品类型
        /// </summary>
        private void InitializeSpecialSlots()
        {
            // 设置头盔槽位允许的物品类型
            HelmetSlot.AllowedItemType = ItemType.Helmet;

            // 设置甲胄槽位允许的物品类型
            ArmorSlot.AllowedItemType = ItemType.Armor;

            // 设置枪械槽位允许的物品类型
            WeaponSlot.AllowedItemType = ItemType.Weapon;
        }

        /// <summary>
        /// 创建物品槽
        /// </summary>
        private void CreateItemSlots()
        {
            // 清空现有槽位
            foreach (Transform child in ItemSlotsContainer)
            {
                Destroy(child.gameObject);
            }

            m_ItemSlots.Clear();

            // 创建8个物品槽
            for (int i = 0; i < m_CharacterModel.MaxCapacity; i++)
            {
                // 计算位置
                int row = i / SlotsPerRow;
                int col = i % SlotsPerRow;

                // 创建槽位
                GameObject slotObj = Instantiate(ItemSlotPrefab, ItemSlotsContainer);
                slotObj.transform.localPosition = new Vector3(col * (slotObj.GetComponent<RectTransform>().sizeDelta.x + SlotSpacing),
                                                           -row * (slotObj.GetComponent<RectTransform>().sizeDelta.y + SlotSpacing),
                                                           0);

                // 设置槽位索引
                InventorySlot slot = slotObj.GetComponent<InventorySlot>();
                slot.SlotIndex = i;

                // 添加到槽位列表
                m_ItemSlots.Add(slot);
            }
        }

        /// <summary>
        /// 更新人物面板UI
        /// </summary>
        public void UpdateCharacterUI()
        {
            // 清空现有物品
            ClearItems();

            // 更新人物图片
            UpdateCharacterImage();

            // 更新特殊槽位
            UpdateSpecialSlots();

            // 更新物品槽
            UpdateItemSlots();
        }

        /// <summary>
        /// 更新人物图片
        /// </summary>
        private void UpdateCharacterImage()
        {
            if (CharacterImage != null)
            {
                // 设置默认图片为黄毛人物图片
                CharacterImage.gameObject.SetActive(true);

                // 尝试加载并设置默认图片
                Sprite defaultSprite = Resources.Load<Sprite>("Characters/Yellow");
                if (defaultSprite != null)
                {
                    CharacterImage.sprite = defaultSprite;
                }
                else
                {
                    Debug.LogWarning("无法加载默认人物图片，请确保Resources/Characters/Yellow路径下存在对应的Sprite资源");
                }
            }
        }

        /// <summary>
        /// 更新特殊槽位
        /// </summary>
        private void UpdateSpecialSlots()
        {
            // 更新头盔槽位
            UpdateSpecialSlot(HelmetSlot, m_CharacterModel.Helmet);

            // 更新甲胄槽位
            UpdateSpecialSlot(ArmorSlot, m_CharacterModel.Armor);

            // 更新枪械槽位
            UpdateSpecialSlot(WeaponSlot, m_CharacterModel.Weapon);
        }

        /// <summary>
        /// 更新特殊槽位
        /// </summary>
        private void UpdateSpecialSlot(InventorySlot slot, InventoryItemData itemData)
        {
            // 清空槽位
            slot.ClearSlot();

            // 如果有物品数据，创建物品
            if (itemData != null)
            {
                // 创建物品
                GameObject itemObj = Instantiate(ItemPrefab, slot.transform);
                itemObj.transform.localPosition = Vector3.zero;

                // 设置物品数据
                InventoryItem item = itemObj.GetComponent<InventoryItem>();
                item.ItemId = itemData.ItemId;
                item.Quantity = itemData.Quantity;
                item.CurrentSlot = slot;
                item.ItemData = itemData.ItemRef;

                // 更新物品显示
                item.UpdateItemDisplay();

                // 设置物品图片
                if (item.ItemImage != null && item.ItemData != null && item.ItemData.Icon != null)
                {
                    item.ItemImage.sprite = item.ItemData.Icon;
                    item.ItemImage.gameObject.SetActive(true);
                }

                // 设置槽位物品
                slot.SetItem(item);

                // 添加到物品列表
                m_Items.Add(item);
            }
        }

        /// <summary>
        /// 更新物品槽
        /// </summary>
        private void UpdateItemSlots()
        {
            // 获取人物物品栏数据
            List<InventoryItemData> items = m_CharacterModel.Items;

            // 创建物品
            for (int i = 0; i < items.Count && i < m_ItemSlots.Count; i++)
            {
                InventoryItemData itemData = items[i];
                InventorySlot slot = m_ItemSlots[i];

                // 清空槽位
                slot.ClearSlot();

                // 创建物品
                GameObject itemObj = Instantiate(ItemPrefab, slot.transform);
                itemObj.transform.localPosition = Vector3.zero;

                // 设置物品数据
                InventoryItem item = itemObj.GetComponent<InventoryItem>();
                item.ItemId = itemData.ItemId;
                item.Quantity = itemData.Quantity;
                item.CurrentSlot = slot;
                item.ItemData = itemData.ItemRef;

                // 更新物品显示
                item.UpdateItemDisplay();

                // 设置物品图片
                if (item.ItemImage != null && item.ItemData != null && item.ItemData.Icon != null)
                {
                    item.ItemImage.sprite = item.ItemData.Icon;
                    item.ItemImage.gameObject.SetActive(true);
                }

                // 设置槽位物品
                slot.SetItem(item);

                // 添加到物品列表
                m_Items.Add(item);
            }
        }

        /// <summary>
        /// 清空所有物品
        /// </summary>
        private void ClearItems()
        {
            // 清空特殊槽位物品
            ClearSlotItems(HelmetSlot.transform);
            ClearSlotItems(ArmorSlot.transform);
            ClearSlotItems(WeaponSlot.transform);

            // 清空物品槽物品
            foreach (InventorySlot slot in m_ItemSlots)
            {
                slot.ClearSlot();
                ClearSlotItems(slot.transform);
            }

            // 销毁物品
            foreach (InventoryItem item in m_Items)
            {
                if (item != null && item.gameObject != null)
                {
                    Destroy(item.gameObject);
                }
            }

            // 清空物品列表
            m_Items.Clear();
        }

        /// <summary>
        /// 清空槽位中的物品
        /// </summary>
        private void ClearSlotItems(Transform slotTransform)
        {
            // 遍历并销毁所有子物体
            for (int i = slotTransform.childCount - 1; i >= 0; i--)
            {
                Transform child = slotTransform.GetChild(i);
                if (child.GetComponent<InventoryItem>() != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        /// <summary>
        /// 显示人物面板
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            UpdateCharacterUI();
        }

        /// <summary>
        /// 隐藏人物面板
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 切换人物面板显示
        /// </summary>
        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            if (gameObject.activeSelf)
            {
                UpdateCharacterUI();
            }
        }

        /// <summary>
        /// 更新人物数据模型
        /// 从UI同步数据到CharacterModel
        /// </summary>
        public void UpdateCharacterModel()
        {
            // 日志：开始更新人物数据模型
            Debug.Log($"[CharacterPanel] UpdateCharacterModel - 开始更新人物数据模型");

            // 更新特殊槽位数据
            // 更新头盔槽位
            InventoryItemData helmetData = GetInventoryItemDataFromSlot(HelmetSlot);
            m_CharacterModel.Helmet = helmetData;
            Debug.Log($"[CharacterPanel] UpdateCharacterModel - 更新头盔槽位: {(helmetData != null ? helmetData.ItemId : "空")}");

            // 更新甲胄槽位
            InventoryItemData armorData = GetInventoryItemDataFromSlot(ArmorSlot);
            m_CharacterModel.Armor = armorData;
            Debug.Log($"[CharacterPanel] UpdateCharacterModel - 更新甲胄槽位: {(armorData != null ? armorData.ItemId : "空")}");

            // 更新武器槽位
            InventoryItemData weaponData = GetInventoryItemDataFromSlot(WeaponSlot);
            m_CharacterModel.Weapon = weaponData;
            Debug.Log($"[CharacterPanel] UpdateCharacterModel - 更新武器槽位: {(weaponData != null ? weaponData.ItemId : "空")}");

            // 更新普通物品槽数据
            // 清空现有物品
            m_CharacterModel.Items.Clear();
            Debug.Log($"[CharacterPanel] UpdateCharacterModel - 清空普通物品槽数据");

            // 遍历所有普通物品槽
            foreach (InventorySlot slot in m_ItemSlots)
            {
                InventoryItemData itemData = GetInventoryItemDataFromSlot(slot);
                if (itemData != null)
                {
                    // 设置槽位索引
                    itemData.SlotIndex = slot.SlotIndex;
                    m_CharacterModel.Items.Add(itemData);
                    Debug.Log($"[CharacterPanel] UpdateCharacterModel - 添加普通物品: {itemData.ItemId}, 数量: {itemData.Quantity}, 槽位索引: {itemData.SlotIndex}");
                }
            }

            Debug.Log($"[CharacterPanel] UpdateCharacterModel - 人物数据模型更新完成");
        }

        /// <summary>
        /// 从槽位获取物品数据
        /// </summary>
        /// <param name="slot">槽位</param>
        /// <returns>物品数据</returns>
        private InventoryItemData GetInventoryItemDataFromSlot(InventorySlot slot)
        {
            if (slot == null || slot.CurrentItem == null)
            {
                return null;
            }

            InventoryItem item = slot.CurrentItem;
            return new InventoryItemData
            {
                ItemId = item.ItemId,
                Quantity = item.Quantity,
                SlotIndex = slot.SlotIndex,
                ItemRef = item.ItemData
            };
        }

        /// <summary>
        /// 获取所有物品槽
        /// </summary>
        /// <returns>物品槽列表</returns>
        public List<InventorySlot> GetAllSlots()
        {
            List<InventorySlot> allSlots = new List<InventorySlot>();

            // 添加特殊槽位
            allSlots.Add(HelmetSlot);
            allSlots.Add(ArmorSlot);
            allSlots.Add(WeaponSlot);

            // 添加普通物品槽
            allSlots.AddRange(m_ItemSlots);

            return allSlots;
        }
    }
}