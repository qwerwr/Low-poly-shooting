using UnityEngine;

namespace Game
{
    /// <summary>
    /// 冲刺状态 - 角色快速移动的状态
    /// 就像演员在表演紧急奔跑
    /// </summary>
    public class SprintingState : CharacterState
    {
        public SprintingState(int id, PlayerController owner) : base(id, owner)
        {}

        public override void OnEnter()
        {
            // 设置冲刺状态
            owner.Animator.SetBool(AnimationParameters.IsSprinting, true);
         
        }

        public override void OnStay()
        {
            // 处理移动输入
            float horizontal = UnityEngine.Input.GetAxis("Horizontal");
            float vertical = UnityEngine.Input.GetAxis("Vertical");
            Vector2 inputDirection = new Vector2(horizontal, vertical);
            
            // 计算移动方向
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = cameraForward * inputDirection.y + cameraRight * inputDirection.x;
            float currentSpeed = owner.SprintSpeed;

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

        public override void OnExit()
        {
            owner.Animator.SetBool(AnimationParameters.IsSprinting, false);
        }

        public override void HandleInput(CharacterInput input, PlayerController controller)
        {
            switch (input)
            {
                case CharacterInput.Sprint when controller.InputDirection == Vector2.zero:
                    controller.StateMachine.ChangeState(new IdleState(0, controller));
                    break;
                case CharacterInput.Sprint when controller.InputDirection != Vector2.zero:
                    controller.StateMachine.ChangeState(new RunningState(1, controller));
                    break;
                default:
                    // 调用基类处理通用输入
                    base.HandleInput(input, controller);
                    break;
            }
        }
    }
}