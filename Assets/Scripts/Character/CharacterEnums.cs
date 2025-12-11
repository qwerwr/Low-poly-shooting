using System;

namespace Game
{
    /// <summary>
    /// 角色状态枚举 - 定义角色可能的所有状态
    /// </summary>
    public enum CharacterStateType
    {
        Idle,           // 空闲状态
        Running,        // 奔跑状态
        Shooting,       // 射击状态
        Reloading,      // 换弹状态
        Sprinting,      // 冲刺状态
        Hurt            // 受伤状态
    }

    /// <summary>
    /// 武器类型枚举 - 对应不同的射击动画
    /// </summary>
    public enum WeaponType
    {
        Pistol,         // 手枪
        Rifle,          // 步枪
        Sniper          // 狙击枪
    }

    /// <summary>
    /// 动画参数名称 - 统一管理所有动画参数
    /// </summary>
    public static class AnimationParameters
    {
        // 移动控制参数
        public const string MoveX = "MoveX";
        public const string MoveY = "MoveY";
        
        // 状态参数
        public const string IsShooting = "IsShooting";
        public const string IsReloading = "IsReloading";
        public const string IsSprinting = "IsSprinting";
        public const string IsHurt = "IsHurt";
        
        // 武器类型参数
        public const string WeaponType = "Weapon"; // 统一使用Weapon参数名称
        
        
    }

    /// <summary>
    /// 角色输入枚举 - 定义玩家输入类型
    /// </summary>
    public enum CharacterInput
    {
        Move,           // 移动输入
        Shoot,          // 射击输入
        Reload,         // 换弹输入
        Sprint,         // 冲刺输入
        Hurt            // 受伤输入
    }
}