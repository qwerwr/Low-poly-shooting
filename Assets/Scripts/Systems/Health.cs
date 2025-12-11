using UnityEngine;
using System;

namespace Game
{
    /// <summary>
    /// 生命值系统 - 通用的受伤扣血类，可用于玩家和敌人
    /// </summary>
    public class Health : MonoBehaviour
    {
        [Header("生命值设置")]
        public int maxHealth = 100;
        public int currentHealth;
        public float invulnerableDuration = 0.5f;
        public bool isInvulnerable;
        public bool canDie = true;

        [Header("护甲和头盔设置")]
        public int armorValue = 0;          // 护甲值，百分比减免伤害
        public int helmetValue = 0;         // 头盔值，百分比减免伤害
        public int armorDurability = 100;   // 护甲耐久度
        public int helmetDurability = 100;  // 头盔耐久度

       
        public event Action<int, int> OnHealthChanged;
        public event Action OnTakeDamage;
        public event Action OnDie;
        public event Action<int, int> OnArmorChanged;    // 护甲变化事件
        public event Action<int, int> OnHelmetChanged;   // 头盔变化事件

        private float invulnerableTimer;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        private void Update()
        {
            // 处理无敌时间
            if (isInvulnerable)
            {
                invulnerableTimer -= Time.deltaTime;
                if (invulnerableTimer <= 0)
                {
                    isInvulnerable = false;
                }
            }
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (isInvulnerable || currentHealth <= 0)
                return;

            // 计算护甲减免
            int armorReduction = Mathf.RoundToInt(damage * (armorValue / 100f));
            int remainingDamageAfterArmor = Mathf.Max(1, damage - armorReduction);

            // 计算头盔减免
            int helmetReduction = Mathf.RoundToInt(remainingDamageAfterArmor * (helmetValue / 100f));
            int finalDamage = Mathf.Max(1, remainingDamageAfterArmor - helmetReduction);

            // 消耗护甲耐久度
            if (armorValue > 0)
            {
                armorDurability = Mathf.Max(0, armorDurability - armorReduction);
                OnArmorChanged?.Invoke(armorDurability, 100);
            }

            // 消耗头盔耐久度
            if (helmetValue > 0)
            {
                helmetDurability = Mathf.Max(0, helmetDurability - helmetReduction);
                OnHelmetChanged?.Invoke(helmetDurability, 100);
            }

            // 扣除生命值
            currentHealth = Mathf.Clamp(currentHealth - finalDamage, 0, maxHealth);

            // 触发事件
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            OnTakeDamage?.Invoke();

            // 进入无敌状态
            if (invulnerableDuration > 0)
            {
                isInvulnerable = true;
                invulnerableTimer = invulnerableDuration;
            }

            // 检查死亡
            if (currentHealth <= 0 && canDie)
            {
                Die();
            }
        }

        /// <summary>
        /// 恢复生命值
        /// </summary>
        public void Heal(int amount)
        {
            if (currentHealth <= 0)
                return;

            // 恢复生命值
            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

            // 触发事件
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        /// <summary>
        /// 死亡处理
        /// </summary>
        public virtual void Die()
        {
            // 触发死亡事件
            OnDie?.Invoke();

            // 检查是否是敌人
            Enemy enemy = GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Die();
            }
            // 检查是否是玩家
            PlayerController player = GetComponent<PlayerController>();
            if (player != null)
            {
                // 玩家死亡逻辑
                Debug.Log($"玩家 {gameObject.name} 已死亡");
            }
            
            // 默认死亡逻辑
            Debug.Log($"{gameObject.name} 已死亡");
        }
    }
}