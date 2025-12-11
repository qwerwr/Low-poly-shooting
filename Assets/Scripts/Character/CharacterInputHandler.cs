using UnityEngine;
using QFramework;

namespace Game
{
    /// <summary>
    /// 角色输入处理系统
    /// 负责处理输入并发送事件
    /// </summary>
    public class CharacterInputHandler : AbstractSystem
    {
        /// <summary>
        /// 初始化系统
        /// </summary>
        protected override void OnInit()
        {
            // 初始化逻辑
            Debug.Log("CharacterInputHandler系统初始化");
        }

        /// <summary>
        /// 处理移动输入
        /// </summary>
        /// <param name="direction">移动方向</param>
        public void HandleMoveInput(Vector2 direction)
        {
            this.SendEvent(new MoveInputEvent(direction));
        }

        /// <summary>
        /// 处理冲刺输入
        /// </summary>
        /// <param name="isSprinting">是否正在冲刺</param>
        public void HandleSprintInput(bool isSprinting)
        {
            this.SendEvent(new SprintInputEvent(isSprinting));
        }

        /// <summary>
        /// 处理射击输入
        /// </summary>
        public void HandleShootInput()
        {
            this.SendEvent(new ShootInputEvent());
        }
        /// <summary>
        /// 处理射击方向
        /// </summary>
        public void HandleShootDirection(Vector3 direction)
        {
            Debug.Log($"Shoot direction: {direction}");
            // 这里可以处理射击逻辑，例如更新角色的射击方向
            // 或者发送事件给其他系统处理射击
        }
        /// <summary>
        /// 处理换弹输入
        /// </summary>
        public void HandleReloadInput()
        {
            this.SendEvent(new ReloadInputEvent());
        }

        /// <summary>
        /// 处理交互输入
        /// </summary>
        public void HandleInteractInput()
        {
            this.SendEvent(new OpenItemBoxEvent());
        }

        /// <summary>
        /// 处理切换背包输入
        /// </summary>
        /// <param name="includeItemBox">是否包含物品箱面板</param>
        public void HandleToggleInventoryInput(bool includeItemBox = false)
        {
            this.SendEvent(new ToggleInventoryEvent(includeItemBox));
        }
    }
}