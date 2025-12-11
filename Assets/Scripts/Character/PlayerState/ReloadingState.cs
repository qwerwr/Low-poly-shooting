using UnityEngine;
using QFramework;

namespace Game
{
    /// <summary>
    /// 换弹状态 - 角色更换弹匣的状态
    /// 就像演员在表演装弹动作
    /// </summary>
    public class ReloadingState : CharacterState
    {
        private float _reloadTimer;
        private const float RELOAD_DURATION = 2.0f; // 换弹动画持续时间
        private AmmoSystem _ammoSystem;

        public ReloadingState(int id, PlayerController owner) : base(id, owner)
        { }

        public override void OnEnter()
        {
            // 设置换弹状态
            owner.Animator.SetBool(AnimationParameters.IsReloading, true);

            _reloadTimer = RELOAD_DURATION;

            // 获取AmmoSystem实例
            _ammoSystem = Architecture<GameArchitecture>.Interface.GetSystem<AmmoSystem>();

            Debug.Log("进入换弹状态");
        }

        public override void OnStay()
        {
            _reloadTimer -= Time.deltaTime;

            // 换弹动画结束后自动回到空闲或奔跑状态
            if (_reloadTimer <= 0f)
            {
                // 执行实际的换弹操作
                if (_ammoSystem != null)
                {
                    // 获取当前武器类型
                    WeaponType currentWeapon = owner.CurrentWeapon;

                    // 获取武器对应的弹药类型
                    AmmoType ammoType = _ammoSystem.GetAmmoTypeFromWeapon(currentWeapon);

                    // 调用AmmoSystem的Reload方法进行换弹
                    _ammoSystem.Reload(currentWeapon, ammoType);
                }

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

        public override void OnExit()
        {
            owner.Animator.SetBool(AnimationParameters.IsReloading, false);
        }

        public override void HandleInput(CharacterInput input, PlayerController controller)
        {
            // 换弹状态下只接受受伤输入，其他输入忽略
            // 调用基类处理通用输入（如受伤）
            base.HandleInput(input, controller);
        }
    }
}