using QFramework;
using System;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 子弹类 - 处理子弹飞行和碰撞检测
    /// </summary>
    public class Bullet : MonoBehaviour
    {

        [Header("子弹设置")]
        public float bulletSpeed = 100f;
        public float bulletLifetime = 3f;
        public float bulletGravity = 0f;
        public GameObject hitEffect;
        public AudioClip hitSound;

        private GameObject m_Shooter;
        private float m_Damage;
        private int m_Level;
        private float m_CurrentLifetime = 0f;
        private Vector3 m_Direction;

        /// <summary>
        /// 初始化子弹
        /// </summary>
        public void Initialize(GameObject shooter, Vector3 direction, float damage, int level)
        {
            m_Shooter = shooter;
            m_Direction = direction.normalized;
            m_Damage = damage;
            m_Level = level;
            m_CurrentLifetime = 0f;
            gameObject.SetActive(true);
            transform.forward = direction;
        }

        private void Update()
        {
            // 更新子弹位置
            Vector3 velocity = m_Direction * bulletSpeed * Time.deltaTime;
            if (bulletGravity != 0)
            {
                velocity.y += bulletGravity * Time.deltaTime;
            }
            transform.position += velocity;

            // 更新生命周期
            m_CurrentLifetime += Time.deltaTime;
            if (m_CurrentLifetime >= bulletLifetime)
            {
                // 直接销毁子弹，由SimpleBulletPool的BulletRecycler处理回收
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 碰撞检测 - 当子弹碰撞到其他物体时调用
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            // 忽略自己和射手
            if (collision.gameObject == m_Shooter || collision.gameObject.transform.IsChildOf(m_Shooter.transform))
                return;

            // 发送子弹命中事件
            Architecture<GameArchitecture>.Interface.SendEvent(new BulletHitEvent
            {
                Target = collision.gameObject,
                Damage = m_Damage,
                Shooter = m_Shooter
            });

            // 播放命中效果
            if (hitEffect != null)
            {
                GameObject effect = Instantiate(hitEffect, collision.contacts[0].point, Quaternion.identity);
                Destroy(effect, 2f);
            }

            // 播放命中音效
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, collision.contacts[0].point);
            }

            // 直接销毁子弹，由SimpleBulletPool的BulletRecycler处理回收
            Destroy(gameObject);
        }

        /// <summary>
        /// 触发检测 - 当子弹进入其他碰撞体时调用
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            // 1. 检查other是否为null
            if (other == null)
            {
                Debug.LogError("OnTriggerEnter: other is null!");
                return;
            }

            // 2. 检查other.gameObject是否为null
            if (other.gameObject == null)
            {
                Debug.LogError("OnTriggerEnter: other.gameObject is null!");
                return;
            }

            // 3. 忽略自己和射手
            if (other.gameObject == gameObject) // 忽略自己
            {
                return;
            }

            // 4. 忽略射手
            if (m_Shooter != null && (other.gameObject == m_Shooter || other.gameObject.transform.IsChildOf(m_Shooter.transform)))
            {
                return;
            }

            Debug.Log($"子弹触发了：{other.gameObject.name}");

            // 5. 检查架构接口是否为null
            if (Architecture<GameArchitecture>.Interface == null)
            {
                Debug.LogError("OnTriggerEnter: Architecture.Interface is null!");
                // 直接调用Health组件的TakeDamage方法作为备选方案
                Health health = other.gameObject.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(Mathf.RoundToInt(m_Damage));
                }
                // 销毁子弹
                Destroy(gameObject);
                return;
            }

            // 6. 发送子弹命中事件
            try
            {
                Architecture<GameArchitecture>.Interface.SendEvent(new BulletHitEvent
                {
                    Target = other.gameObject,
                    Damage = m_Damage,
                    Shooter = m_Shooter
                });
                Debug.Log("BulletHitEvent发送成功");
            }
            catch (Exception e)
            {
                Debug.LogError($"发送BulletHitEvent失败：{e.Message}");
                // 备选方案：直接调用Health组件
                Health health = other.gameObject.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(Mathf.RoundToInt(m_Damage));
                }
            }

            // 7. 播放命中效果（带null检查）
            if (hitEffect != null)
            {
                try
                {
                    GameObject effect = Instantiate(hitEffect, other.transform.position, Quaternion.identity);
                    Destroy(effect, 2f);
                }
                catch (Exception e)
                {
                    Debug.LogError($"播放命中效果失败：{e.Message}");
                }
            }

            // 8. 播放命中音效（带null检查）
            if (hitSound != null)
            {
                try
                {
                    AudioSource.PlayClipAtPoint(hitSound, other.transform.position);
                }
                catch (Exception e)
                {
                    Debug.LogError($"播放命中音效失败：{e.Message}");
                }
            }

            // 9. 销毁子弹
            Destroy(gameObject);
        }
    }
}