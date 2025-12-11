using UnityEngine;

namespace Game.PoolObject
{
    /// <summary>
    /// MonoBehaviour 对象池
    /// </summary>
    public class MonoObjectPool : ObjectPool<GameObject>
    {
        // 预制体
        private GameObject m_Prefab;
        // 父对象
        private Transform m_Parent;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <param name="initialSize">初始大小</param>
        /// <param name="maxSize">最大大小</param>
        public MonoObjectPool(GameObject prefab, int initialSize = 10, int maxSize = -1)
            : base(
                () => CreateObject(prefab),
                ResetObject,
                initialSize,
                maxSize)
        {
            m_Prefab = prefab;
            // 创建父对象
            m_Parent = new GameObject($"{prefab.name}Pool").transform;
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <returns>创建的对象</returns>
        private static GameObject CreateObject(GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            return obj;
        }

        /// <summary>
        /// 重置对象
        /// </summary>
        /// <param name="obj">要重置的对象</param>
        private static void ResetObject(GameObject obj)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        /// <summary>
        /// 从对象池获取对象并设置位置和旋转
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <returns>对象</returns>
        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject obj = Get();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }
    }
}