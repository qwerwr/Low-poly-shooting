using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// SelectCanvas控制器
    /// 用于控制SelectCanvas的显示和隐藏
    /// </summary>
    public class SelectConversion : MonoBehaviour
    {
        /// <summary>
        /// 获取游戏按钮
        /// </summary>
        public Button Btn_GetGame;
        
        /// <summary>
        /// SelectCanvas的CanvasGroup组件
        /// </summary>
        public CanvasGroup SelectCanvasGroup;
        
        private void Start()
        {
            // 确保按钮和CanvasGroup组件已赋值
            if (Btn_GetGame == null)
            {
                Debug.LogError("Btn_GetGame未赋值");
                return;
            }
            
            if (SelectCanvasGroup == null)
            {
                Debug.LogError("SelectCanvasGroup未赋值");
                return;
            }
            
            // 注册按钮点击事件
            Btn_GetGame.onClick.AddListener(OnGetGameButtonClick);
        }
        
        /// <summary>
        /// 获取游戏按钮点击事件
        /// </summary>
        private void OnGetGameButtonClick()
        {
            // 切换SelectCanvas的显示状态
            ToggleSelectCanvas();
        }
        
        /// <summary>
        /// 切换SelectCanvas的显示状态
        /// </summary>
        private void ToggleSelectCanvas()
        {
            if (SelectCanvasGroup.alpha == 0)
            {
                // 显示SelectCanvas
                ShowSelectCanvas();
            }
            else
            {
                // 隐藏SelectCanvas
                HideSelectCanvas();
            }
        }
        
        /// <summary>
        /// 显示SelectCanvas
        /// </summary>
        public void ShowSelectCanvas()
        {
            SelectCanvasGroup.alpha = 1;
            SelectCanvasGroup.interactable = true;
            SelectCanvasGroup.blocksRaycasts = true;
        }
        
        /// <summary>
        /// 隐藏SelectCanvas
        /// </summary>
        public void HideSelectCanvas()
        {
            SelectCanvasGroup.alpha = 0;
            SelectCanvasGroup.interactable = false;
            SelectCanvasGroup.blocksRaycasts = false;
        }
    }
}