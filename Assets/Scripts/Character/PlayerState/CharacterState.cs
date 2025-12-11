using UnityEngine;

namespace Game
{
    /// <summary>
    /// 角色状态基类 - 所有状态类的父类
    /// 就像演员表演的基本规则
    /// </summary>
    public abstract class CharacterState : StateTemplate<PlayerController>, ICharacterState
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        protected CharacterState(int id, PlayerController owner) : base(id, owner)
        {}

        /// <summary>
        /// 进入状态时的处理 - 实现ICharacterState接口
        /// </summary>
        public void Enter(PlayerController controller)
        {
            OnEnter();
        }

        /// <summary>
        /// 状态更新处理 - 实现ICharacterState接口
        /// </summary>
        public void Update(PlayerController controller)
        {
            OnStay();
        }

        /// <summary>
        /// 退出状态时的处理 - 实现ICharacterState接口
        /// </summary>
        public void Exit(PlayerController controller)
        {
            OnExit();
        }

        /// <summary>
        /// 处理输入 - 实现ICharacterState接口
        /// 使用模板方法模式，子类可以重写特定输入的处理
        /// </summary>
        public virtual void HandleInput(CharacterInput input, PlayerController controller)
        {
            // 处理通用输入类型
            switch (input)
            {
                case CharacterInput.Hurt:
                    // 所有状态都应该响应受伤输入
                    controller.StateMachine.ChangeState(new HurtState(5, controller));
                    break;
                // 其他通用输入可以在这里添加
            }
        }
    }
}