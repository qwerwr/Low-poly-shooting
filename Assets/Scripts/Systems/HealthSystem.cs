using UnityEngine;
using QFramework;

namespace Game
{
    /// <summary>
    /// 生命值系统 - 处理伤害计算和生命值管理
    /// </summary>
    public class HealthSystem : AbstractSystem
    {
        /// <summary>
        /// 初始化系统
        /// </summary>
        protected override void OnInit()
        {
            // 监听子弹命中事件
            this.RegisterEvent<BulletHitEvent>(OnBulletHitEvent);
        }

        /// <summary>
        /// 处理子弹命中事件
        /// </summary>
        private void OnBulletHitEvent(BulletHitEvent e)
        {
            // 直接处理伤害，不使用命令模式
            HandleDamage(e.Target, e.Damage, e.Shooter);
        }
        
        /// <summary>
        /// 处理伤害
        /// </summary>
        private void HandleDamage(GameObject target, float damage, GameObject attacker)
        {
            // 获取目标的Health组件
            Health health = target.GetComponent<Health>();
            if (health == null)
                return;

            // 计算最终伤害
            int finalDamage = Mathf.RoundToInt(damage);

            // 调用Health组件的TakeDamage方法
            health.TakeDamage(finalDamage);

            // 获取剩余生命值
            int remainingHealth = health.currentHealth;

            // 触发伤害事件
            this.SendEvent(new DamageEvent
            {
                Target = target,
                Damage = finalDamage,
                RemainingHealth = remainingHealth
            });

            // 触发生命值变化事件
            this.SendEvent(new HealthChangedEvent
            {
                Target = target,
                CurrentHealth = remainingHealth,
                MaxHealth = health.maxHealth
            });

            // 检查是否死亡
            if (remainingHealth <= 0)
            {
                // 触发死亡事件
                this.SendEvent(new DieEvent
                {
                    Target = target,
                    Killer = attacker
                });
            }
        }


    }

    /// <summary>
    /// 子弹命中事件
    /// </summary>
    public class BulletHitEvent
    {
        public GameObject Shooter { get; set; }
        public GameObject Target { get; set; }
        public float Damage { get; set; }
    }

    /// <summary>
    /// 伤害事件
    /// </summary>
    public class DamageEvent
    {
        public GameObject Target { get; set; }
        public float Damage { get; set; }
        public int RemainingHealth { get; set; }
    }

    /// <summary>
    /// 死亡事件
    /// </summary>
    public class DieEvent
    {
        public GameObject Target { get; set; }
        public GameObject Killer { get; set; }
    }

    /// <summary>
    /// 生命值变化事件
    /// </summary>
    public class HealthChangedEvent
    {
        public GameObject Target { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
    }
}

namespace Game
{
    /// <summary>
    /// 子弹回收事件
    /// </summary>
    public class BulletRecycleEvent
    {
        public GameObject BulletObject { get; set; }
    }
}