using UnityEngine;
using QFramework;

namespace Game
{
    /// <summary>
    /// 移动输入事件
    /// </summary>
    public class MoveInputEvent
    {
        public Vector2 Direction;

        public MoveInputEvent(Vector2 direction)
        {
            Direction = direction;
        }
    }

    /// <summary>
    /// 冲刺输入事件
    /// </summary>
    public class SprintInputEvent
    {
        public bool IsSprinting;

        public SprintInputEvent(bool isSprinting)
        {
            IsSprinting = isSprinting;
        }
    }

    /// <summary>
    /// 射击输入事件
    /// </summary>
    public class ShootInputEvent
    { }
    /// <summary>
    /// 射击方向事件
    /// </summary>
    public class ShootDirectionEvent
    {
        public Vector3 Direction;

        public ShootDirectionEvent(Vector3 direction)
        {
            Direction = direction;
        }
    }
    /// <summary>
    /// 换弹输入事件
    /// </summary>
    public class ReloadInputEvent
    { }

    /// <summary>
    /// 交互提示事件
    /// </summary>
    public class InteractPromptEvent
    {
        public bool ShowPrompt;
        public string PromptText;

        public InteractPromptEvent(bool showPrompt, string promptText = "")
        {
            ShowPrompt = showPrompt;
            PromptText = promptText;
        }
    }

    /// <summary>
    /// 开启物品箱事件
    /// </summary>
    public class OpenItemBoxEvent
    { }

    /// <summary>
    /// 切换背包事件
    /// </summary>
    public class ToggleInventoryEvent
    {
        public bool IncludeItemBox; // 是否包含物品箱面板

        public ToggleInventoryEvent(bool includeItemBox = false)
        {
            IncludeItemBox = includeItemBox;
        }
    }
}