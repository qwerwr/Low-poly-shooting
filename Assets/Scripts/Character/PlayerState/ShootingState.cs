using QFramework;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 射击状态基类 - 所有武器射击状态的父类
    /// </summary>
    public abstract class ShootingState : CharacterState
    {
        protected float _shootTimer;

        /// <summary>
        /// 射击持续时间（抽象属性，由子类实现）
        /// </summary>
        protected abstract float ShootDuration { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">状态ID</param>
        /// <param name="owner">角色控制器</param>
        protected ShootingState(int id, PlayerController owner) : base(id, owner)
        { }

        /// <summary>
        /// 进入状态时的处理
        /// </summary>
        public override void OnEnter()
        {
            // 设置当前武器类型（0=退出射击动画，1=手枪，2=步枪，3=狙击枪）
            // WeaponType枚举：Pistol(0) → 1, Rifle(1) → 2, Sniper(2) → 3
            int weaponTypeInt = (int)owner.CurrentWeapon + 1;
            owner.Animator.SetInteger(AnimationParameters.WeaponType, weaponTypeInt);
            // 使用命令模式执行射击
            if (owner.ShootPoint != null)
            {
                // 保存ShootPoint的位置和旋转副本，而不是引用
                Vector3 shootPosition = owner.ShootPoint.position;
                shootPosition.y = 0.53f;  // 固定y轴高度为0.53
                Quaternion shootRotation = owner.ShootPoint.rotation;
               // Debug.Log(shootRotation);
                owner.SendCommand(new ShootCommand
                {
                    Shooter = owner.gameObject,
                    WeaponType = owner.CurrentWeapon,
                    ShootDirection = owner.transform.forward,
                    ShootPosition = shootPosition,
                    ShootRotation = shootRotation
                });
            }

            // Debug.Log($"进入{owner.CurrentWeapon}射击状态，设置Weapon参数为{weaponTypeInt}");
        }

        /// <summary>
        /// 状态更新处理
        /// </summary>
        public override void OnStay()
        {
            _shootTimer -= Time.deltaTime;

            // 射击结束后自动回到空闲或奔跑状态
            if (_shootTimer <= 0f)
            {
                if (owner.InputDirection != Vector2.zero)
                {
                    owner.StateMachine.ChangeState(new RunningState(1, owner));
                }
                else
                {
                    owner.StateMachine.ChangeState(new IdleState(0, owner));
                }
            }
        }

        /// <summary>
        /// 退出状态时的处理
        /// </summary>
        public override void OnExit()
        {
            // 退出射击动画，设置Weapon=0
            owner.Animator.SetInteger(AnimationParameters.WeaponType, 0);
        }

        /// <summary>
        /// 处理输入
        /// </summary>
        public override void HandleInput(CharacterInput input, PlayerController controller)
        {
            switch (input)
            {
                case CharacterInput.Reload:
                    controller.StateMachine.ChangeState(new ReloadingState(4, controller));
                    break;
                default:
                    // 调用基类处理通用输入
                    base.HandleInput(input, controller);
                    break;
            }
        }
    }

    /// <summary>
    /// 手枪射击状态
    /// </summary>
    public class PistolShootingState : ShootingState
    {
        /// <summary>
        /// 手枪射击持续时间
        /// </summary>
        protected override float ShootDuration => 0.3f;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PistolShootingState(int id, PlayerController owner) : base(id, owner)
        { }

        /// <summary>
        /// 进入状态时的处理（手枪特有逻辑）
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            // 手枪特有逻辑
           // Debug.Log("手枪射击：快速射击，低伤害");
        }
    }

    /// <summary>
    /// 步枪射击状态
    /// </summary>
    public class RifleShootingState : ShootingState
    {
        /// <summary>
        /// 步枪射击持续时间
        /// </summary>
        protected override float ShootDuration => 0.5f;

        /// <summary>
        /// 构造函数
        /// </summary>
        public RifleShootingState(int id, PlayerController owner) : base(id, owner)
        { }

        /// <summary>
        /// 进入状态时的处理（步枪特有逻辑）
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            // 步枪特有逻辑
            Debug.Log("步枪射击：连射，中等伤害");
        }
    }

    /// <summary>
    /// 狙击枪射击状态
    /// </summary>
    public class SniperShootingState : ShootingState
    {
        /// <summary>
        /// 狙击枪射击持续时间
        /// </summary>
        protected override float ShootDuration => 0.5f;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SniperShootingState(int id, PlayerController owner) : base(id, owner)
        { }

        /// <summary>
        /// 进入状态时的处理（狙击枪特有逻辑）
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
            // 狙击枪特有逻辑
            Debug.Log("狙击枪射击：单次射击，高伤害，长时间瞄准");
        }
    }
}