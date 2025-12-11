using UnityEngine;
using UnityEngine.UI;
using QFramework;
using TMPro;

namespace Game
{
    public class PlayerHUD : MonoBehaviour, IController
    {
        [Header("血量UI")]
        public Slider HealthSlider;
        public TextMeshProUGUI HealthText;

        [Header("弹药UI")]
        public Image AmmoIcon;
        public TextMeshProUGUI CurrentAmmoText;
        public TextMeshProUGUI TotalAmmoText;
    

        private Health m_PlayerHealth;
        private AmmoSystem m_AmmoSystem;
        private PlayerController m_PlayerController;

        private void Awake()
        {
            // 获取玩家对象
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                m_PlayerHealth = player.GetComponent<Health>();
                m_PlayerController = player.GetComponent<PlayerController>();
            }

            // 获取AmmoSystem实例
            m_AmmoSystem = this.GetSystem<AmmoSystem>();
        }

        private void OnEnable()
        {
            // 监听血量变化事件
            this.RegisterEvent<HealthChangedEvent>(OnHealthChanged).UnRegisterWhenGameObjectDestroyed(gameObject);

            // 监听弹药变化事件
            this.RegisterEvent<AmmoChangedEvent>(OnAmmoChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Start()
        {
            // 初始化UI
            if (m_PlayerHealth != null)
            {
                UpdateHealthUI(m_PlayerHealth.currentHealth, m_PlayerHealth.maxHealth);
            }

            // 初始化弹药UI
            UpdateAmmoUI();
        }

        private void OnHealthChanged(HealthChangedEvent e)
        {
            // 只处理玩家的血量变化
            if (e.Target.CompareTag("Player"))
            {
                UpdateHealthUI(e.CurrentHealth, e.MaxHealth);
            }
        }

        private void OnAmmoChanged(AmmoChangedEvent e)
        {
            UpdateAmmoUI();
        }

        private void UpdateHealthUI(int currentHealth, int maxHealth)
        {
            if (HealthSlider != null)
            {
                HealthSlider.maxValue = maxHealth;
                HealthSlider.value = currentHealth;
            }

            if (HealthText != null)
            {
                HealthText.text = $"{currentHealth}/{maxHealth}";
            }
        }

        private void UpdateAmmoUI()
        {
            if (m_AmmoSystem == null || m_PlayerController == null)
                return;

            // 获取当前武器类型
            WeaponType currentWeaponType = m_PlayerController.CurrentWeapon;

            // 获取当前武器对应的弹药类型
            AmmoType ammoType = GetAmmoTypeFromWeapon(currentWeaponType);

            // 获取当前弹夹数量和总弹药数量
            (int currentClip, int totalAmmo) = m_AmmoSystem.GetAmmoInfo(ammoType);

            // 更新弹药UI
            if (CurrentAmmoText != null)
            {
                CurrentAmmoText.text = $"{currentClip}";
            }

            if (TotalAmmoText != null)
            {
                TotalAmmoText.text = $"/{totalAmmo}";
            }
        }

        private AmmoType GetAmmoTypeFromWeapon(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.Pistol:
                    return AmmoType.PistolAmmo;
                case WeaponType.Rifle:
                    return AmmoType.RifleAmmo;
                case WeaponType.Sniper:
                    return AmmoType.SniperAmmo;
                default:
                    return AmmoType.PistolAmmo;
            }
        }

        // 实现IController接口
        public IArchitecture GetArchitecture()
        {
            return Architecture<GameArchitecture>.Interface;
        }
    }
}