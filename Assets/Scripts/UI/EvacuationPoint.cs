using UnityEngine;
using UnityEngine.SceneManagement;
using QFramework;
using System.Collections.Generic;
using TMPro;

namespace Game
{
    /// <summary>
    /// 撤离点脚本
    /// 进入该空物体后触发撤离成功界面
    /// 显示背包物品总价值并跳转到Personal场景
    /// </summary>
    public class EvacuationPoint : MonoBehaviour
    {
        /// <summary>
        /// 撤离成功界面预制体
        /// </summary>
        [Header("撤离设置")]
        [SerializeField] private GameObject evacuationSuccessUI;

        /// <summary>
        /// 总价值文本组件
        /// 通过拖拽方式添加
        /// </summary>
        [SerializeField] private TextMeshProUGUI totalValueText;

        /// <summary>
        /// 玩家标签
        /// </summary>
        [SerializeField] private string playerTag = "Player";

        /// <summary>
        /// 撤离成功后延迟跳转到Personal场景的时间（秒）
        /// </summary>
        [SerializeField] private float delayBeforeSceneLoad = 3f;

        /// <summary>
        /// 是否已经触发过撤离
        /// </summary>
        private bool hasEvacuated = false;

        /// <summary>
        /// 背包模型
        /// </summary>
        private InventoryModel inventoryModel;

        /// <summary>
        /// 人物模型
        /// </summary>
        private CharacterModel characterModel;

        private void Start()
        {
            // 获取数据模型
            inventoryModel = GameArchitecture.Interface.GetModel<InventoryModel>();
            characterModel = GameArchitecture.Interface.GetModel<CharacterModel>();

            // 确保撤离成功界面初始隐藏
            if (evacuationSuccessUI != null)
            {
                evacuationSuccessUI.SetActive(false);
            }
        }

        /// <summary>
        /// 计算所有物品的总价值
        /// 包括背包和人物面板的物品
        /// </summary>
        /// <returns>总价值</returns>
        private int CalculateTotalItemValue()
        {
            int totalValue = 0;

            // 计算背包物品价值
            if (inventoryModel != null && inventoryModel.Items != null)
            {
                foreach (var itemData in inventoryModel.Items)
                {
                    if (itemData != null && itemData.ItemRef != null)
                    {
                        totalValue += itemData.ItemRef.Value * itemData.Quantity;
                    }
                }
            }

            // 计算人物面板装备价值
            if (characterModel != null)
            {
                // 头盔价值
                if (characterModel.Helmet != null && characterModel.Helmet.ItemRef != null)
                {
                    totalValue += characterModel.Helmet.ItemRef.Value;
                }

                // 甲胄价值
                if (characterModel.Armor != null && characterModel.Armor.ItemRef != null)
                {
                    totalValue += characterModel.Armor.ItemRef.Value;
                }

                // 武器价值
                if (characterModel.Weapon != null && characterModel.Weapon.ItemRef != null)
                {
                    totalValue += characterModel.Weapon.ItemRef.Value;
                }

                // 人物物品栏价值
                if (characterModel.Items != null)
                {
                    foreach (var itemData in characterModel.Items)
                    {
                        if (itemData != null && itemData.ItemRef != null)
                        {
                            totalValue += itemData.ItemRef.Value * itemData.Quantity;
                        }
                    }
                }
            }

            Debug.Log($"[EvacuationPoint] 总物品价值计算完成：{totalValue}");
            return totalValue;
        }

        /// <summary>
        /// 触发撤离成功
        /// </summary>
        private void TriggerEvacuationSuccess()
        {
            if (hasEvacuated) return;
            hasEvacuated = true;

            Debug.Log("[EvacuationPoint] 触发撤离成功");

            // 计算总价值
            int totalValue = CalculateTotalItemValue();

            // 显示撤离成功界面
            if (evacuationSuccessUI != null)
            {
                evacuationSuccessUI.SetActive(true);

                // 更新撤离成功界面的总价值显示
                UpdateTotalValueUI(totalValue);
            }

            // 延迟跳转到Personal场景
            Invoke(nameof(LoadPersonalScene), delayBeforeSceneLoad);
        }

        /// <summary>
        /// 更新撤离成功界面的总价值显示
        /// 使用直接拖拽的totalValueText字段
        /// </summary>
        /// <param name="totalValue">总价值</param>
        private void UpdateTotalValueUI(int totalValue)
        {
            if (totalValueText != null)
            {
                totalValueText.text = $"背包物品总价值：{totalValue}";
            }
            else
            {
                Debug.LogWarning("[EvacuationPoint] 未设置TotalValueText组件，请在Inspector中拖拽文本组件到该字段");
            }
        }

        /// <summary>
        /// 加载Personal场景
        /// </summary>
        private void LoadPersonalScene()
        {
            Debug.Log("[EvacuationPoint] 正在跳转到Personal场景");
            SceneManager.LoadScene("Personal");
        }

        /// <summary>
        /// 当玩家进入撤离点时触发
        /// </summary>
        /// <param name="other">碰撞体</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                Debug.Log($"[EvacuationPoint] 玩家 {other.gameObject.name} 进入撤离点");
                TriggerEvacuationSuccess();
            }
        }
    }
}
