using UnityEngine;

namespace Game
{
    /// <summary>
    /// 受伤状态 - 角色受到伤害的状态
    /// 就像演员在表演受伤场景
    /// </summary>
    public class HurtState : CharacterState
    {
        private float _hurtTimer;
        private const float HURT_DURATION = 1.0f; // 受伤动画持续时间

        public HurtState(int id, PlayerController owner) : base(id, owner)
        {}

        public override void OnEnter()
        {
            // 设置受伤状态
            owner.Animator.SetBool(AnimationParameters.IsHurt, true);
            
            _hurtTimer = HURT_DURATION;
            
            Debug.Log("进入受伤状态");
        }

        public override void OnStay()
        {
            _hurtTimer -= Time.deltaTime;
            
            // 受伤动画结束后自动回到空闲或奔跑状态
            if (_hurtTimer <= 0f)
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

        public override void OnExit()
        {
            owner.Animator.SetBool(AnimationParameters.IsHurt, false);
        }

        public override void HandleInput(CharacterInput input, PlayerController controller)
        {
            // 受伤状态下不接受其他输入，直到受伤动画完成
            // 不调用base.HandleInput，因为受伤状态下不应再次处理受伤输入
        }
    }
}