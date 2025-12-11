using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// 人物槽位事件处理器
    /// </summary>
    public class CharacterSlotHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// 槽位索引
        /// </summary>
        public int SlotIndex;

        /// <summary>
        /// 人物选择控制器
        /// </summary>
        private CharacterSelectController m_CharacterSelectController;

        private void Awake()
        {
            // 使用Awake代替Start，更早获取控制器
            m_CharacterSelectController = FindObjectOfType<CharacterSelectController>();
        }

        /// <summary>
        /// 鼠标进入事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData == null)
                return;

            if (m_CharacterSelectController != null)
            {
                // 调用人物选择控制器的方法，处理鼠标悬浮
                m_CharacterSelectController.OnCharacterSlotHover(SlotIndex);
            }
        }

        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        /// <param name="eventData">事件数据</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData == null)
                return;

            if (m_CharacterSelectController != null)
            {
                // 调用人物选择控制器的方法，处理鼠标离开
                m_CharacterSelectController.OnCharacterSlotExit(SlotIndex);
            }
        }
    }
}