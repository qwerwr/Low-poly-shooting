using System;

namespace Game
{
    /// <summary>
    /// 物品类型枚举
    /// </summary>
    public enum ItemType
    {
        Weapon,     // 武器
        Ammo,       // 弹药
        Helmet,     // 头盔
        Armor,      // 甲胄
        Misc        // 杂物
    }

    /// <summary>
    /// 武器类型枚举
    /// </summary>
    //public enum WeaponType
    //{
    //    Pistol,     // 手枪
    //    Rifle,      // 步枪
    //    Sniper      // 狙击枪
    //}

    /// <summary>
    /// 弹药类型枚举
    /// </summary>
    public enum AmmoType
    {
        PistolAmmo,  // 手枪弹药
        RifleAmmo,   // 步枪弹药
        SniperAmmo   // 狙击枪弹药
    }

    /// <summary>
    /// 弹药等级枚举
    /// </summary>
    public enum AmmoLevel
    {
        Level1,     // 1级
        Level2,     // 2级
        Level3      // 3级
    }

    /// <summary>
    /// 角色类型枚举
    /// </summary>
    public enum CharacterType
    {
        Player,     // 玩家角色
        Enemy       // 敌人角色
    }

    /// <summary>
    /// 装备等级枚举
    /// </summary>
    public enum EquipmentLevel
    {
        Level1 = 1, // 1级装备
        Level2 = 2  // 2级装备
    }

    /// <summary>
    /// 游戏状态枚举
    /// </summary>
    public enum GameState
    {
        Loading,    // 加载中
        Menu,       // 菜单界面
        Playing,    // 游戏中
        Paused,     // 暂停
        GameOver    // 游戏结束
    }

    /// <summary>
    /// 商店操作结果枚举
    /// </summary>
    public enum ShopResult
    {
        Success,            // 成功
        InsufficientFunds,  // 资金不足
        InventoryFull,      // 背包已满
        ItemNotFound,       // 物品不存在
        InvalidOperation    // 无效操作
    }
}