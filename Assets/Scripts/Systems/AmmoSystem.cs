using UnityEngine;
using System.Collections.Generic;
using QFramework;

namespace Game
{
    /// <summary>
    /// 弹药信息结构
    /// </summary>
    [System.Serializable]
    public struct AmmoInfo
    {
        public AmmoType type;
        public int level;
        public int count;
        public float[] damageByLevel;
    }

    /// <summary>
    /// 弹药系统 - 处理弹药库存管理和射击命令
    /// </summary>
    public class AmmoSystem : AbstractSystem
    {
        // 子弹对象池
        public Game.PoolObject.BulletPool bulletPool;

        // 引用其他模型
        private InventoryModel m_InventoryModel;
        private CharacterModel m_CharacterModel;

        // 当前使用的弹药等级
        private Dictionary<AmmoType, int> m_CurrentAmmoLevels = new Dictionary<AmmoType, int>();

        // 弹夹容量配置
        private Dictionary<WeaponType, int> m_ClipCapacities = new Dictionary<WeaponType, int> {
            { WeaponType.Pistol, 10 },  // 手枪弹夹容量10发
            { WeaponType.Rifle, 30 },   // 步枪弹夹容量30发
            { WeaponType.Sniper, 3 }    // 狙击枪弹夹容量3发
        };

        // 当前弹夹数量
        private Dictionary<AmmoType, int> m_CurrentClipAmmo = new Dictionary<AmmoType, int>();

        /// <summary>
        /// 初始化系统
        /// </summary>
        protected override void OnInit()
        {
            Debug.Log("[AmmoSystem] 初始化弹药系统");

            // 获取模型引用
            m_InventoryModel = this.GetModel<InventoryModel>();
            m_CharacterModel = this.GetModel<CharacterModel>();

            Debug.Log("[AmmoSystem] 模型引用初始化完成");

            // 初始化当前弹药等级
            InitializeCurrentAmmoLevels();

            Debug.Log("[AmmoSystem] 弹药系统初始化完成");
        }

        /// <summary>
        /// 初始化当前弹药等级
        /// </summary>
        private void InitializeCurrentAmmoLevels()
        {
            Debug.Log("[AmmoSystem] 初始化当前弹药等级");

            // 为每种弹药类型初始化默认等级
            m_CurrentAmmoLevels[AmmoType.PistolAmmo] = 1;
            m_CurrentAmmoLevels[AmmoType.RifleAmmo] = 1;
            m_CurrentAmmoLevels[AmmoType.SniperAmmo] = 1;

            // 为每种弹药类型初始化当前弹夹数量为0
            m_CurrentClipAmmo[AmmoType.PistolAmmo] = 0;
            m_CurrentClipAmmo[AmmoType.RifleAmmo] = 0;
            m_CurrentClipAmmo[AmmoType.SniperAmmo] = 0;

            Debug.Log("[AmmoSystem] 当前弹药等级初始化完成：");
            foreach (var kvp in m_CurrentAmmoLevels)
            {
                Debug.Log($"[AmmoSystem]   {kvp.Key}: {kvp.Value}级");
            }
        }

        /// <summary>
        /// 从背包和人物面板中查找特定类型和等级的弹药
        /// </summary>
        /// <param name="ammoType">弹药类型</param>
        /// <param name="level">弹药等级</param>
        /// <returns>找到的弹药数据，null表示未找到</returns>
        private InventoryItemData FindAmmo(AmmoType ammoType, int level)
        {
            Debug.Log($"[AmmoSystem] 查找弹药：类型={ammoType}，等级={level}");

            List<InventoryItemData> allItems = new List<InventoryItemData>();
            int totalItems = 0;

            // 从背包获取所有物品
            if (m_InventoryModel != null)
            {
                allItems.AddRange(m_InventoryModel.Items);
                totalItems += m_InventoryModel.Items.Count;
            }

            // 从人物面板获取所有物品
            if (m_CharacterModel != null)
            {
                if (m_CharacterModel.Items != null)
                {
                    allItems.AddRange(m_CharacterModel.Items);
                    totalItems += m_CharacterModel.Items.Count;
                }
            }

            Debug.Log($"[AmmoSystem] 共检查 {totalItems} 个物品");

            // 查找对应类型和等级的弹药
            foreach (var itemData in allItems)
            {
                if (itemData?.ItemRef is AmmoData ammoData)
                {
                    if (ammoData.AmmoType == ammoType && ammoData.Level == level)
                    {
                        Debug.Log($"[AmmoSystem] 找到弹药：{ammoData.Name}（{ammoData.AmmoType}，等级{ammoData.Level}），数量：{itemData.Quantity}");
                        return itemData;
                    }
                }
            }

            Debug.Log($"[AmmoSystem] 未找到 {ammoType} 类型 {level} 级弹药");
            return null;
        }

        /// <summary>
        /// 查找可用的最低等级弹药
        /// </summary>
        /// <param name="ammoType">弹药类型</param>
        /// <returns>找到的弹药数据和等级，null表示未找到</returns>
        private (InventoryItemData, int) FindLowestLevelAmmo(AmmoType ammoType)
        {
            Debug.Log($"[AmmoSystem] 查找可用的最低等级弹药：类型={ammoType}");

            // 从1级开始查找可用弹药
            for (int level = 1; level <= 3; level++)
            {
                var ammo = FindAmmo(ammoType, level);
                if (ammo != null && ammo.Quantity > 0)
                {
                    Debug.Log($"[AmmoSystem] 找到最低等级弹药：等级={level}，数量={ammo.Quantity}");
                    return (ammo, level);
                }
            }

            Debug.Log($"[AmmoSystem] 未找到可用的 {ammoType} 类型弹药");
            return (null, -1);
        }

        /// <summary>
        /// 处理射击命令
        /// </summary>
        public void HandleShootCommand(ShootCommand cmd)
        {
            Debug.Log($"[AmmoSystem] 收到射击命令，武器类型：{cmd.WeaponType}");

            // 获取武器对应的弹药类型
            AmmoType ammoType = GetAmmoTypeFromWeapon(cmd.WeaponType);
            Debug.Log($"[AmmoSystem] 武器对应的弹药类型：{ammoType}");

            // 检查当前弹夹是否有子弹
            if (!HasAmmoInClip(cmd.WeaponType, ammoType))
            {
                // 弹夹为空，尝试换弹
                Debug.LogWarning("[AmmoSystem] 弹夹为空，尝试换弹");
                if (!Reload(cmd.WeaponType, ammoType))
                {
                    // 换弹失败，弹药不足
                    Debug.LogWarning("[AmmoSystem] 换弹失败，弹药不足");
                    return;
                }
            }

            // 获取当前使用的弹药等级
            int currentLevel = m_CurrentAmmoLevels.TryGetValue(ammoType, out int level) ? level : 1;
            Debug.Log($"[AmmoSystem] 当前使用的弹药等级：{currentLevel}");

            // 消耗弹夹中的弹药
            ConsumeAmmoFromClip(ammoType);

            // 查找对应等级的弹药
            var ammoData = FindAmmo(ammoType, currentLevel);

            // 如果当前等级没有弹药，尝试查找最低等级的可用弹药
            if (ammoData == null || ammoData.Quantity <= 0)
            {
                Debug.LogWarning($"[AmmoSystem] 当前等级弹药不足，尝试查找最低等级弹药");
                var (lowestAmmo, lowestLevel) = FindLowestLevelAmmo(ammoType);
                if (lowestAmmo != null && lowestAmmo.Quantity > 0)
                {
                    Debug.Log($"[AmmoSystem] 切换到最低等级弹药：等级{lowestLevel}");
                    ammoData = lowestAmmo;
                    currentLevel = lowestLevel;
                    m_CurrentAmmoLevels[ammoType] = currentLevel;
                }
                else
                {
                    Debug.LogWarning("[AmmoSystem] 弹药不足，无法射击");
                    return;
                }
            }

            // 消耗弹药
            Debug.Log($"[AmmoSystem] 准备消耗弹药：类型={ammoType}，等级={currentLevel}，数量=1");
            if (ConsumeAmmo(ammoType, currentLevel))
            {
                Debug.Log("[AmmoSystem] 弹药消耗成功");

                // 计算伤害
                float damage = 0;
                if (ammoData.ItemRef is AmmoData ammoInfo)
                {
                    damage = ammoInfo.Damage;
                    Debug.Log($"[AmmoSystem] 弹药伤害：{damage}");
                }

                // 从对象池获取子弹
                if (bulletPool != null)
                {
                    // 使用保存的位置和旋转，而不是ShootPoint引用
                    GameObject bulletObj = bulletPool.Get(cmd.ShootPosition, Quaternion.LookRotation(cmd.ShootDirection));
                    if (bulletObj != null)
                    {
                        Bullet bullet = bulletObj.GetComponent<Bullet>();
                        if (bullet != null)
                        {
                            bullet.Initialize(cmd.Shooter, cmd.ShootDirection, damage, currentLevel);
                            Debug.Log("[AmmoSystem] 子弹初始化完成");
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("[AmmoSystem] 弹药消耗失败");
            }
        }

        /// <summary>
        /// 消耗弹药
        /// </summary>
        private bool ConsumeAmmo(AmmoType ammoType, int level, int amount = 1)
        {
            Debug.Log($"[AmmoSystem] 执行弹药消耗：类型={ammoType}，等级={level}，数量={amount}");

            // 查找要消耗的弹药
            InventoryItemData ammoToConsume = null;
            bool isFromCharacter = false;

            // 从背包查找
            if (m_InventoryModel != null)
            {
                Debug.Log("[AmmoSystem] 从背包查找弹药");
                foreach (var itemData in m_InventoryModel.Items)
                {
                    if (itemData?.ItemRef is AmmoData ammoData)
                    {
                        if (ammoData.AmmoType == ammoType && ammoData.Level == level)
                        {
                            ammoToConsume = itemData;
                            Debug.Log("[AmmoSystem] 从背包找到弹药");
                            break;
                        }
                    }
                }
            }

            // 如果背包没有，从人物面板查找
            if (ammoToConsume == null && m_CharacterModel != null && m_CharacterModel.Items != null)
            {
                Debug.Log("[AmmoSystem] 从人物面板查找弹药");
                foreach (var itemData in m_CharacterModel.Items)
                {
                    if (itemData?.ItemRef is AmmoData ammoData)
                    {
                        if (ammoData.AmmoType == ammoType && ammoData.Level == level)
                        {
                            ammoToConsume = itemData;
                            isFromCharacter = true;
                            Debug.Log("[AmmoSystem] 从人物面板找到弹药");
                            break;
                        }
                    }
                }
            }

            // 如果找到弹药
            if (ammoToConsume != null)
            {
                Debug.Log($"[AmmoSystem] 找到弹药：数量={ammoToConsume.Quantity}");

                // 检查数量是否足够
                if (ammoToConsume.Quantity >= amount)
                {
                    // 减少数量
                    ammoToConsume.Quantity -= amount;
                    Debug.Log($"[AmmoSystem] 弹药数量减少：{ammoToConsume.Quantity + amount} -> {ammoToConsume.Quantity}");

                    // 如果数量为0，移除该物品
                    if (ammoToConsume.Quantity <= 0)
                    {
                        Debug.Log("[AmmoSystem] 弹药数量为0，准备移除物品");
                        if (isFromCharacter && m_CharacterModel != null && m_CharacterModel.Items != null)
                        {
                            m_CharacterModel.Items.Remove(ammoToConsume);
                            Debug.Log("[AmmoSystem] 从人物面板移除弹药");
                        }
                        else if (m_InventoryModel != null)
                        {
                            m_InventoryModel.Items.Remove(ammoToConsume);
                            Debug.Log("[AmmoSystem] 从背包移除弹药");
                        }
                    }

                    // 触发弹药变化事件
                    this.SendEvent(new AmmoChangedEvent
                    {
                        AmmoType = ammoType,
                        Count = GetAmmoCount(ammoType),
                        Level = level
                    });

                    Debug.Log("[AmmoSystem] 弹药消耗成功");
                    return true;
                }
                else
                {
                    Debug.LogWarning($"[AmmoSystem] 弹药数量不足：需要{amount}，现有{ammoToConsume.Quantity}");
                }
            }
            else
            {
                Debug.LogWarning("[AmmoSystem] 未找到可消耗的弹药");
            }

            return false;
        }

        /// <summary>
        /// 处理消耗弹药命令
        /// </summary>
        public void HandleConsumeAmmoCommand(ConsumeAmmoCommand cmd)
        {
            Debug.Log($"[AmmoSystem] 收到消耗弹药命令：类型={cmd.AmmoType}，数量={cmd.Amount}");

            // 获取当前弹药等级
            int currentLevel = m_CurrentAmmoLevels.TryGetValue(cmd.AmmoType, out int level) ? level : 1;
            Debug.Log($"[AmmoSystem] 当前弹药等级：{currentLevel}");

            ConsumeAmmo(cmd.AmmoType, currentLevel, cmd.Amount);
        }

        /// <summary>
        /// 处理升级弹药命令
        /// </summary>
        public void HandleUpgradeAmmoCommand(UpgradeAmmoCommand cmd)
        {
            Debug.Log($"[AmmoSystem] 收到升级弹药命令：类型={cmd.AmmoType}");

            // 获取当前等级
            int currentLevel = m_CurrentAmmoLevels.TryGetValue(cmd.AmmoType, out int level) ? level : 1;
            Debug.Log($"[AmmoSystem] 当前等级：{currentLevel}");

            // 尝试升级到下一级
            int newLevel = currentLevel + 1;
            Debug.Log($"[AmmoSystem] 尝试升级到等级：{newLevel}");

            // 检查是否有对应等级的弹药
            if (FindAmmo(cmd.AmmoType, newLevel) != null)
            {
                // 更新当前等级
                m_CurrentAmmoLevels[cmd.AmmoType] = newLevel;
                Debug.Log($"[AmmoSystem] 弹药等级升级成功：{currentLevel} -> {newLevel}");

                // 触发弹药变化事件
                this.SendEvent(new AmmoChangedEvent
                {
                    AmmoType = cmd.AmmoType,
                    Count = GetAmmoCount(cmd.AmmoType),
                    Level = newLevel
                });
            }
            else
            {
                Debug.LogWarning($"[AmmoSystem] 没有等级{newLevel}的弹药，无法升级");
            }
        }

        /// <summary>
        /// 切换弹药等级
        /// </summary>
        /// <param name="ammoType">弹药类型</param>
        /// <param name="direction">切换方向，1为升级，-1为降级</param>
        /// <returns>是否切换成功</returns>
        public bool SwitchAmmoLevel(AmmoType ammoType, int direction)
        {
            Debug.Log($"[AmmoSystem] 收到弹药等级切换请求：类型={ammoType}，方向={direction}");

            // 获取当前等级
            int currentLevel = m_CurrentAmmoLevels.TryGetValue(ammoType, out int level) ? level : 1;
            Debug.Log($"[AmmoSystem] 当前弹药等级：{currentLevel}");

            // 计算新等级
            int newLevel = currentLevel + direction;
            Debug.Log($"[AmmoSystem] 计算新等级：{newLevel}");

            // 确保等级在有效范围内
            newLevel = Mathf.Clamp(newLevel, 1, 3);
            Debug.Log($"[AmmoSystem] 修正后新等级：{newLevel}");

            // 如果等级没有变化，直接返回
            if (newLevel == currentLevel)
            {
                Debug.Log("[AmmoSystem] 新等级与当前等级相同，无需切换");
                return false;
            }

            // 检查是否有对应等级的弹药
            Debug.Log($"[AmmoSystem] 检查是否有等级{newLevel}的弹药");
            if (FindAmmo(ammoType, newLevel) != null)
            {
                // 更新当前等级
                m_CurrentAmmoLevels[ammoType] = newLevel;
                Debug.Log($"[AmmoSystem] 弹药等级切换成功：{currentLevel} -> {newLevel}");

                // 触发弹药变化事件
                this.SendEvent(new AmmoChangedEvent
                {
                    AmmoType = ammoType,
                    Count = GetAmmoCount(ammoType),
                    Level = newLevel
                });

                return true;
            }
            else
            {
                Debug.LogWarning($"[AmmoSystem] 没有等级{newLevel}的弹药，无法切换");
                return false;
            }
        }

        /// <summary>
        /// 处理添加弹药命令
        /// </summary>
        public void HandleAddAmmoCommand(AddAmmoCommand cmd)
        {
            Debug.Log($"[AmmoSystem] 收到添加弹药命令：类型={cmd.AmmoType}，数量={cmd.Amount}");
            // 这里需要根据具体需求实现添加弹药的逻辑
            // 例如，将弹药添加到背包中
        }

        /// <summary>
        /// 获取武器对应的弹药类型
        /// </summary>
        public AmmoType GetAmmoTypeFromWeapon(WeaponType weaponType)
        {
            Debug.Log($"[AmmoSystem] 获取武器对应的弹药类型：{weaponType}");

            switch (weaponType)
            {
                case WeaponType.Pistol:
                    return AmmoType.PistolAmmo;
                case WeaponType.Rifle:
                    return AmmoType.RifleAmmo;
                case WeaponType.Sniper:
                    return AmmoType.SniperAmmo;
                default:
                    Debug.LogWarning($"[AmmoSystem] 未知武器类型：{weaponType}，默认返回手枪弹药");
                    return AmmoType.PistolAmmo;
            }
        }

        /// <summary>
        /// 获取弹药等级
        /// </summary>
        public int GetAmmoLevel(AmmoType ammoType)
        {
            int level = m_CurrentAmmoLevels.TryGetValue(ammoType, out int currentLevel) ? currentLevel : 1;
            Debug.Log($"[AmmoSystem] 获取弹药等级：{ammoType} -> {level}");
            return level;
        }

        /// <summary>
        /// 获取弹药数量
        /// </summary>
        public int GetAmmoCount(AmmoType ammoType)
        {
            Debug.Log($"[AmmoSystem] 获取弹药数量：{ammoType}");

            int totalCount = 0;
            List<InventoryItemData> allItems = new List<InventoryItemData>();

            // 从背包获取所有物品
            if (m_InventoryModel != null)
            {
                allItems.AddRange(m_InventoryModel.Items);
            }

            // 从人物面板获取所有物品
            if (m_CharacterModel != null)
            {
                if (m_CharacterModel.Items != null)
                {
                    allItems.AddRange(m_CharacterModel.Items);
                }
            }

            // 计算对应类型弹药的总数量
            foreach (var itemData in allItems)
            {
                if (itemData?.ItemRef is AmmoData ammoData)
                {
                    if (ammoData.AmmoType == ammoType)
                    {
                        totalCount += itemData.Quantity;
                    }
                }
            }

            Debug.Log($"[AmmoSystem] 弹药数量：{ammoType} -> {totalCount}");
            return totalCount;
        }

        /// <summary>
        /// 检查当前弹夹是否有子弹
        /// </summary>
        /// <param name="weaponType">武器类型</param>
        /// <param name="ammoType">弹药类型</param>
        /// <returns>是否有子弹</returns>
        public bool HasAmmoInClip(WeaponType weaponType, AmmoType ammoType)
        {
            // 获取当前弹夹数量
            int currentClipAmmo = m_CurrentClipAmmo.TryGetValue(ammoType, out int ammo) ? ammo : 0;
            Debug.Log($"[AmmoSystem] 检查弹夹子弹：{weaponType} -> {ammoType}，当前弹夹：{currentClipAmmo}");
            return currentClipAmmo > 0;
        }

        /// <summary>
        /// 执行换弹
        /// </summary>
        /// <param name="weaponType">武器类型</param>
        /// <param name="ammoType">弹药类型</param>
        /// <returns>换弹是否成功</returns>
        public bool Reload(WeaponType weaponType, AmmoType ammoType)
        {
            Debug.Log($"[AmmoSystem] 执行换弹：{weaponType} -> {ammoType}");

            // 获取弹夹容量
            int clipCapacity = m_ClipCapacities.TryGetValue(weaponType, out int capacity) ? capacity : 10;

            // 获取当前总弹药数量
            int totalAmmo = GetAmmoCount(ammoType);

            // 计算需要装弹的数量
            int ammoNeeded = clipCapacity;
            int ammoToReload = Mathf.Min(totalAmmo, ammoNeeded);

            if (ammoToReload <= 0)
            {
                Debug.LogWarning($"[AmmoSystem] 换弹失败，没有可用弹药");
                return false;
            }

            // 更新当前弹夹数量
            m_CurrentClipAmmo[ammoType] = ammoToReload;
            Debug.Log($"[AmmoSystem] 换弹成功，当前弹夹：{ammoToReload}/{clipCapacity}");

            // 触发弹药变化事件
            this.SendEvent(new AmmoChangedEvent
            {
                AmmoType = ammoType,
                Count = GetAmmoCount(ammoType),
                Level = m_CurrentAmmoLevels.TryGetValue(ammoType, out int level) ? level : 1
            });

            return true;
        }

        /// <summary>
        /// 从弹夹中消耗子弹
        /// </summary>
        /// <param name="ammoType">弹药类型</param>
        private void ConsumeAmmoFromClip(AmmoType ammoType)
        {
            if (m_CurrentClipAmmo.ContainsKey(ammoType))
            {
                m_CurrentClipAmmo[ammoType]--;
                Debug.Log($"[AmmoSystem] 从弹夹消耗子弹，当前弹夹：{m_CurrentClipAmmo[ammoType]}");

                // 触发弹药变化事件
                this.SendEvent(new AmmoChangedEvent
                {
                    AmmoType = ammoType,
                    Count = GetAmmoCount(ammoType),
                    Level = m_CurrentAmmoLevels.TryGetValue(ammoType, out int level) ? level : 1
                });
            }
        }

        /// <summary>
        /// 获取弹药信息（当前弹夹数量和总弹药数量）
        /// </summary>
        /// <param name="ammoType">弹药类型</param>
        /// <returns>当前弹夹数量和总弹药数量</returns>
        public (int currentClip, int totalAmmo) GetAmmoInfo(AmmoType ammoType)
        {
            int currentClip = m_CurrentClipAmmo.TryGetValue(ammoType, out int clipAmmo) ? clipAmmo : 0;
            int totalAmmo = GetAmmoCount(ammoType);
            return (currentClip, totalAmmo);
        }
    }

    /// <summary>
    /// 射击事件
    /// </summary>
    public class ShootEvent
    {
        public GameObject Shooter { get; set; }
        public WeaponType WeaponType { get; set; }
        public float Damage { get; set; }
    }

    /// <summary>
    /// 弹药变化事件
    /// </summary>
    public class AmmoChangedEvent
    {
        public AmmoType AmmoType { get; set; }
        public int Count { get; set; }
        public int Level { get; set; }
    }
}