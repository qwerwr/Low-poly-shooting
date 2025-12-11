using System.Collections.Generic;

namespace Game.PoolObject
{
    /// <summary>
    /// 通用对象池
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public class ObjectPool<T>
    {
        // 对象创建委托
        public delegate T CreateObjectDelegate();
        // 对象重置委托
        public delegate void ResetObjectDelegate(T obj);

        // 对象队列
        private Queue<T> m_ObjectQueue = new Queue<T>();
        // 对象创建委托
        private CreateObjectDelegate m_CreateObject;
        // 对象重置委托
        private ResetObjectDelegate m_ResetObject;
        // 最大对象数
        private int m_MaxSize = -1;

        /// <summary>
        /// 当前对象池大小
        /// </summary>
        public int PoolSize => m_ObjectQueue.Count;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="createObject">对象创建委托</param>
        /// <param name="resetObject">对象重置委托</param>
        /// <param name="initialSize">初始大小</param>
        /// <param name="maxSize">最大大小</param>
        public ObjectPool(CreateObjectDelegate createObject, ResetObjectDelegate resetObject = null, int initialSize = 10, int maxSize = -1)
        {
            m_CreateObject = createObject;
            m_ResetObject = resetObject;
            m_MaxSize = maxSize;

            // 预加载对象
            Preload(initialSize);
        }

        /// <summary>
        /// 预加载对象
        /// </summary>
        /// <param name="count">预加载数量</param>
        public void Preload(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T obj = m_CreateObject();
                m_ObjectQueue.Enqueue(obj);
            }
        }

        /// <summary>
        /// 从对象池获取对象
        /// </summary>
        /// <returns>对象</returns>
        public T Get()
        {
            T obj;
            if (m_ObjectQueue.Count > 0)
            {
                obj = m_ObjectQueue.Dequeue();
            }
            else
            {
                obj = m_CreateObject();
            }

            // 重置对象
            m_ResetObject?.Invoke(obj);

            return obj;
        }

        /// <summary>
        /// 将对象返回对象池
        /// </summary>
        /// <param name="obj">要返回的对象</param>
        public void Return(T obj)
        {
            if (obj == null)
                return;

            // 如果对象池已满，不回收
            if (m_MaxSize > 0 && m_ObjectQueue.Count >= m_MaxSize)
            {
                // 如果是引用类型，考虑销毁
                if (obj is UnityEngine.Object unityObj)
                {
                    UnityEngine.Object.Destroy(unityObj);
                }
                return;
            }

            m_ObjectQueue.Enqueue(obj);
        }

        /// <summary>
        /// 清空对象池
        /// </summary>
        public void Clear()
        {
            foreach (T obj in m_ObjectQueue)
            {
                // 如果是引用类型，销毁对象
                if (obj is UnityEngine.Object unityObj)
                {
                    UnityEngine.Object.Destroy(unityObj);
                }
            }
            m_ObjectQueue.Clear();
        }
    }
}