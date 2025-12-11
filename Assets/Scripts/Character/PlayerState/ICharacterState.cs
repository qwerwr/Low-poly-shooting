using UnityEngine;

namespace Game
{
    /// <summary>
    /// 角色状态接口 - 所有状态类的基类
    /// 就像演员的表演指导，每个状态都有自己的表演规则
    /// </summary>
    public interface ICharacterState
    {
        /// <summary>
        /// 进入状态时的初始化操作
        /// </summary>
        /// <param name="controller">角色控制器</param>
        void Enter(PlayerController controller);
        
        /// <summary>
        /// 状态更新逻辑
        /// </summary>
        /// <param name="controller">角色控制器</param>
        void Update(PlayerController controller);
        
        /// <summary>
        /// 退出状态时的清理操作
        /// </summary>
        /// <param name="controller">角色控制器</param>
        void Exit(PlayerController controller);
        
        /// <summary>
        /// 处理输入事件
        /// </summary>
        /// <param name="input">输入类型</param>
        /// <param name="controller">角色控制器</param>
        void HandleInput(CharacterInput input, PlayerController controller);
    }
}