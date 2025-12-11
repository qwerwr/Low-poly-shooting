using UnityEngine;

namespace Game.PoolObject
{
    /// <summary>
    /// 子弹对象池
    /// </summary>
    public class BulletPool : MonoObjectPool
    {
        // 子弹回收组件
        private class BulletRecycler : MonoBehaviour
        {
            // 所属对象池
            public BulletPool pool;
            // 子弹组件
            private Bullet bullet;

            private void Awake()
            {
                bullet = GetComponent<Bullet>();
            }

            private void OnEnable()
            {
                // 注册子弹回收事件
                if (bullet != null)
                {
                    // 监听子弹销毁事件
                    // 注意：这里需要确保Bullet类有相应的事件或方法
                }
            }

            private void OnDisable()
            {
                // 取消注册事件
                if (bullet != null)
                {
                    // 取消监听
                }
            }

            private void OnDestroy()
            {
                // 如果是被销毁而不是被回收，重新创建一个
                if (gameObject.activeSelf && pool != null)
                {
                    pool.Preload(1);
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="prefab">子弹预制体</param>
        /// <param name="initialSize">初始大小</param>
        /// <param name="maxSize">最大大小</param>
        public BulletPool(GameObject prefab, int initialSize = 10, int maxSize = -1)
            : base(prefab, initialSize, maxSize)
        {
            // 确保子弹预制体有Bullet组件
            if (prefab != null && prefab.GetComponent<Bullet>() == null)
            {
                Debug.LogError($"子弹预制体 {prefab.name} 缺少 Bullet 组件！");
            }
        }

        /// <summary>
        /// 从对象池获取子弹
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="rotation">旋转</param>
        /// <returns>子弹对象</returns>
        public new GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject bulletObj = base.Get(position, rotation);
            
            // 确保子弹对象有BulletRecycler组件
            if (bulletObj.GetComponent<BulletRecycler>() == null)
            {
                var recycler = bulletObj.AddComponent<BulletRecycler>();
                recycler.pool = this;
            }
            
            return bulletObj;
        }

        /// <summary>
        /// 回收子弹
        /// </summary>
        /// <param name="bulletObj">子弹对象</param>
        public void ReturnBullet(GameObject bulletObj)
        {
            if (bulletObj == null)
                return;
            
            // 重置子弹状态
            ResetBullet(bulletObj);
            
            // 返回对象池
            Return(bulletObj);
        }

        /// <summary>
        /// 重置子弹状态
        /// </summary>
        /// <param name="bulletObj">子弹对象</param>
        private void ResetBullet(GameObject bulletObj)
        {
            // 重置位置和旋转
            bulletObj.transform.position = Vector3.zero;
            bulletObj.transform.rotation = Quaternion.identity;
            
            // 重置刚体
            Rigidbody rb = bulletObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            
            // 重置子弹组件
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            if (bullet != null)
            {
                // 重置子弹的内部状态
                // 注意：这里需要确保Bullet类有相应的重置方法
            }
            
            // 禁用子弹
            bulletObj.SetActive(false);
        }
    }
}