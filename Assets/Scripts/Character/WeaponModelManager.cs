using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Game
{
    /// <summary>
    /// 武器模型映射类
    /// 用于在Inspector中配置武器类型和模型的对应关系
    /// </summary>
    [System.Serializable]
    public class WeaponModelMapping
    {
        /// <summary>
        /// 武器类型
        /// </summary>
        public WeaponType weaponType;

        /// <summary>
        /// 武器模型Transform
        /// </summary>
        public Transform weaponModel;
    }

    /// <summary>
    /// 武器模型管理器
    /// 负责管理武器模型的激活和隐藏
    /// 可附加到玩家和敌人GameObject上
    /// </summary>
    public class WeaponModelManager : MonoBehaviour
    {
        /// <summary>
        /// 武器模型映射列表
        /// 可在Inspector中手动配置
        /// </summary>
        [Header("武器模型配置")]
        [SerializeField] private List<WeaponModelMapping> weaponModelMappings = new List<WeaponModelMapping>();

        /// <summary>
        /// 武器类型到模型Transform的映射
        /// </summary>
        private Dictionary<WeaponType, Transform> weaponModels = new Dictionary<WeaponType, Transform>();

        /// <summary>
        /// 当前激活的武器类型
        /// </summary>
        private WeaponType currentWeaponType;

        /// <summary>
        /// 初始化武器模型管理器
        /// 使用Inspector中配置的武器模型映射
        /// </summary>
        public void Initialize()
        {
            Debug.Log($"[WeaponModelManager] 初始化武器模型管理器，父对象：{gameObject.name}");

            // 清空现有映射
            weaponModels.Clear();

            // 使用Inspector中配置的武器模型映射
            foreach (WeaponModelMapping mapping in weaponModelMappings)
            {
                if (mapping.weaponModel != null)
                {
                    // 将武器模型添加到映射中
                    weaponModels[mapping.weaponType] = mapping.weaponModel;
                    Debug.Log($"[WeaponModelManager] 配置武器模型：{mapping.weaponType} -> {mapping.weaponModel.name}");

                    // 默认隐藏所有武器模型
                    mapping.weaponModel.gameObject.SetActive(false);
                }
            }

            // 如果有武器模型，默认激活第一个
            if (weaponModels.Count > 0)
            {
                // 获取第一个武器类型
                WeaponType firstWeaponType = (WeaponType)System.Enum.GetValues(typeof(WeaponType)).GetValue(0);
                if (weaponModels.ContainsKey(firstWeaponType))
                {
                    SwitchWeaponModel(firstWeaponType);
                }
            }
        }

        /// <summary>
        /// 切换武器模型
        /// 隐藏当前武器模型，激活目标武器模型
        /// </summary>
        /// <param name="weaponType">目标武器类型</param>
        public void SwitchWeaponModel(WeaponType weaponType)
        {
            Debug.Log($"[WeaponModelManager] 切换武器模型：{currentWeaponType} -> {weaponType}");

            // 如果目标武器类型与当前相同，不需要切换
            if (currentWeaponType == weaponType)
            {
                Debug.Log($"[WeaponModelManager] 武器模型已为 {weaponType}，无需切换");
                return;
            }

            // 隐藏当前武器模型
            if (weaponModels.ContainsKey(currentWeaponType))
            {
                weaponModels[currentWeaponType].gameObject.SetActive(false);
                Debug.Log($"[WeaponModelManager] 隐藏武器模型：{currentWeaponType}");
            }

            // 激活目标武器模型
            if (weaponModels.ContainsKey(weaponType))
            {
                weaponModels[weaponType].gameObject.SetActive(true);
                Debug.Log($"[WeaponModelManager] 激活武器模型：{weaponType}");

                // 更新当前武器类型
                currentWeaponType = weaponType;
            }
            else
            {
                Debug.LogWarning($"[WeaponModelManager] 未找到武器模型：{weaponType}");
            }
        }

        /// <summary>
        /// 获取当前激活的武器类型
        /// </summary>
        public WeaponType CurrentWeaponType => currentWeaponType;

        /// <summary>
        /// 检查是否包含指定武器类型的模型
        /// </summary>
        /// <param name="weaponType">武器类型</param>
        /// <returns>是否包含该武器类型的模型</returns>
        public bool ContainsWeaponType(WeaponType weaponType)
        {
            return weaponModels.ContainsKey(weaponType);
        }

        private void Start()
        {
            // 自动初始化
            Initialize();
        }
    }
}
