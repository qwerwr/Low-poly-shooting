using UnityEngine;
using QFramework;

namespace Game
{
    /// <summary>
    /// 输入管理器
    /// 负责检测输入并调用CharacterInputHandler系统的方法
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        private CharacterInputHandler _inputHandler;
        private IArchitecture _architecture;

        private void Awake()
        {
            // 获取架构实例
            _architecture = Architecture<GameArchitecture>.Interface;

            // 获取CharacterInputHandler系统
            _inputHandler = _architecture.GetSystem<CharacterInputHandler>();
        }

        private void Update()
        {
            // 处理移动输入
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector2 direction = new Vector2(horizontal, vertical);
            _inputHandler.HandleMoveInput(direction);

            // 处理冲刺输入
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _inputHandler.HandleSprintInput(true);
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _inputHandler.HandleSprintInput(false);
            }

            // 处理射击输入
            if (Input.GetMouseButtonDown(0))
            {
                _inputHandler.HandleShootInput();
            }

            // 处理换弹输入
            if (Input.GetKeyDown(KeyCode.R))
            {
                _inputHandler.HandleReloadInput();
            }

            // 处理交互输入（F键）
            if (Input.GetKeyDown(KeyCode.F))
            {
                _inputHandler.HandleInteractInput();
            }

            // 处理切换背包输入（Tab键）
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _inputHandler.HandleToggleInventoryInput();
            }
        }
    }
}