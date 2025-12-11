using UnityEngine;

namespace Game
{
    /// <summary>
    /// 奔跑状态 - 角色正常移动的状态
    /// 就像演员在舞台上优雅地走动
    /// </summary>
    public class RunningState : CharacterState
    {
        public RunningState(int id, PlayerController owner) : base(id, owner)
        { }

        public override void OnEnter()
        {
            // 设置移动参数（由外部输入控制）
            owner.Animator.SetBool(AnimationParameters.IsReloading, false);
            owner.Animator.SetBool(AnimationParameters.IsSprinting, false);

         //   Debug.Log("进入奔跑状态");
        }

        public override void OnStay()
        {
            // 使用来自PlayerCharacterController的输入方向
            Vector2 inputDirection = owner.InputDirection;

            // 计算移动方向
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * inputDirection.y + cameraRight * inputDirection.x;
            float currentSpeed = owner.WalkSpeed;

            // 应用移动
            if (moveDirection.magnitude > 0.1f && currentSpeed > 0f)
            {
                // 旋转角色朝向移动方向
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotation, owner.RotationSpeed * Time.deltaTime);

                // 移动角色
                Vector3 movement = moveDirection.normalized * currentSpeed * Time.deltaTime;

                if (owner.UnityCharacterController != null)
                {
                    owner.UnityCharacterController.Move(movement);
                }
                else if (owner.Rigidbody != null)
                {
                    owner.Rigidbody.MovePosition(owner.transform.position + movement);
                }
                else
                {
                    owner.transform.position += movement;
                }
            }

            // 更新动画参数
            owner.Animator.SetFloat(AnimationParameters.MoveX, inputDirection.x);
            owner.Animator.SetFloat(AnimationParameters.MoveY, inputDirection.y);
        }

        public override void HandleInput(CharacterInput input, PlayerController controller)
        {
            switch (input)
            {
                case CharacterInput.Move when controller.InputDirection == Vector2.zero:
                    controller.StateMachine.ChangeState(new IdleState(0, controller));
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
                case CharacterInput.Reload:
                    controller.StateMachine.ChangeState(new ReloadingState(4, controller));
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