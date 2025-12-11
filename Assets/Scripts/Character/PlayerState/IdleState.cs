using UnityEngine;

namespace Game
{
    /// <summary>
    /// 空闲状态 - 角色站立不动的状态
    /// 就像演员在等待上场指令
    /// </summary>
    public class IdleState : CharacterState
    {
        public IdleState(int id, PlayerController owner) : base(id, owner)
        { }

        public override void OnEnter()
        {
            // 重置移动参数
            owner.Animator.SetFloat(AnimationParameters.MoveX, 0f);
            owner.Animator.SetFloat(AnimationParameters.MoveY, 0f);

            // 重置状态参数
            owner.Animator.SetBool(AnimationParameters.IsReloading, false);
            owner.Animator.SetBool(AnimationParameters.IsSprinting, false);
            owner.Animator.SetBool(AnimationParameters.IsHurt, false);

            //Debug.Log("进入空闲状态");
        }

        public override void OnStay()
        {
            // 空闲状态下可以检查是否需要切换到其他状态
        }

        public override void HandleInput(CharacterInput input, PlayerController controller)
        {
            switch (input)
            {
                case CharacterInput.Move:
                    // 控制器状态机暂时保留旧的ChangeState方法，后续统一替换
                    controller.StateMachine.ChangeState(new RunningState(1, controller));
                    break;
                case CharacterInput.Shoot:
                    // 根据当前武器类型选择对应的射击状态
                    switch (controller.CurrentWeapon)
                    {
                        case WeaponType.Pistol:
                            controller.StateMachine.ChangeState(new PistolShootingState(3, controller));
                            break;
                        case WeaponType.Rifle:
                            controller.StateMachine.ChangeState(new RifleShootingState(3, controller));
                            break;
                        case WeaponType.Sniper:
                            controller.StateMachine.ChangeState(new SniperShootingState(3, controller));
                            break;
                        default:
                            controller.StateMachine.ChangeState(new PistolShootingState(3, controller));
                            break;
                    }
                    break;
                case CharacterInput.Sprint:
                    controller.StateMachine.ChangeState(new SprintingState(2, controller));
                    break;
                default:
                    // 调用基类处理通用输入
                    base.HandleInput(input, controller);
                    break;
            }
        }
    }
}