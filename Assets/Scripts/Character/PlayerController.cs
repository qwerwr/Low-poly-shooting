using UnityEngine;
using System.Collections;
using QFramework;

namespace Game
{
    /// <summary>
    /// 角色控制器 - 集成状态机和动画控制的核心类
    /// 就像电影的导演，协调所有演员的表演
    /// </summary>
    public class PlayerController : MonoBehaviour, IController
    {
        [Header("组件引用")]
        [SerializeField] private Animator _animator;
        [SerializeField] protected Rigidbody _rigidbody;
        [SerializeField] protected UnityEngine.CharacterController _unityCharacterController;
        [SerializeField] protected Transform _shootPoint;
        [SerializeField] private WeaponModelManager _weaponModelManager;
        [SerializeField] private GameOverManager _gameOverManager;

        [Header("移动设置")]
        [SerializeField] private float _walkSpeed = 3f;
        [SerializeField] private float _sprintSpeed = 6f;
        [SerializeField] private float _rotationSpeed = 10f;

        [Header("状态设置")]
        [SerializeField] private WeaponType _currentWeapon = WeaponType.Pistol;

        // 公共属性
        public Animator Animator => _animator;
        public StateMachine StateMachine { get; private set; }
        public Vector2 InputDirection { get; set; }
        public WeaponType CurrentWeapon => _currentWeapon;

        // 组件引用属性
        public Rigidbody Rigidbody => _rigidbody;
        public UnityEngine.CharacterController UnityCharacterController => _unityCharacterController;
        public Transform ShootPoint => _shootPoint;

        // 移动参数属性
        public float WalkSpeed => _walkSpeed;
        public float SprintSpeed => _sprintSpeed;
        public float RotationSpeed => _rotationSpeed;

        // 移动相关
        private Vector3 _moveDirection;
        private float _currentSpeed;
        private bool _isSprinting;

        // 状态类型枚举
        private enum CharacterStateType { Idle = 0, Running = 1, Sprinting = 2, Shooting = 3, Reloading = 4, Hurt = 5 }
        private CameraController _cameraController;
        private CharacterModel m_CharacterModel;
        private AmmoSystem m_AmmoSystem;

        private void Awake()
        {
            // 获取组件引用
            if (_animator == null)
                _animator = GetComponent<Animator>();
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();
            if (_unityCharacterController == null)
                _unityCharacterController = GetComponent<UnityEngine.CharacterController>();
        }

        private void Start()
        {
            // 获取CharacterModel实例
            m_CharacterModel = this.GetModel<CharacterModel>();

            // 获取AmmoSystem实例
            m_AmmoSystem = this.GetSystem<AmmoSystem>();

            // 注册武器变化事件
            if (m_CharacterModel != null)
            {
                m_CharacterModel.WeaponChanged += OnWeaponChanged;
            }

            // 初始化状态机
            StateMachine = new StateMachine(new IdleState((int)CharacterStateType.Idle, this));
            StateMachine.AddState(new RunningState((int)CharacterStateType.Running, this));
            StateMachine.AddState(new SprintingState((int)CharacterStateType.Sprinting, this));
            // 移除ShootingState的直接添加，因为它现在是抽象基类，由子类动态创建
            StateMachine.AddState(new ReloadingState((int)CharacterStateType.Reloading, this));
            StateMachine.AddState(new HurtState((int)CharacterStateType.Hurt, this));

            // 初始化武器模型管理器
            if (_weaponModelManager == null)
            {
                _weaponModelManager = GetComponent<WeaponModelManager>();
                if (_weaponModelManager == null)
                {
                    _weaponModelManager = gameObject.AddComponent<WeaponModelManager>();
                }
            }
            _weaponModelManager.Initialize();

            // 设置初始武器类型
            _animator.SetInteger(AnimationParameters.WeaponType, (int)_currentWeapon);
            _cameraController = FindObjectOfType<CameraController>();

            // 如果CharacterModel中已有武器，立即应用
            if (m_CharacterModel != null && m_CharacterModel.Weapon != null)
            {
                OnWeaponChanged(m_CharacterModel.Weapon);
            }
        }

        /// <summary>
        /// 处理武器变化事件
        /// </summary>
        private void OnWeaponChanged(InventoryItemData weaponData)
        {
            Debug.Log("[PlayerCharacterController] 收到武器变化事件");

            if (weaponData != null && weaponData.ItemRef is WeaponData weaponRef)
            {
                Debug.Log($"[PlayerCharacterController] 准备切换到武器：{weaponRef.WeaponType}（{weaponRef.Name}），使用弹药类型：{weaponRef.AmmoType}");
                // 根据WeaponData切换武器
                SwitchWeapon(weaponRef.WeaponType);
                Debug.Log($"[PlayerCharacterController] 成功切换到武器：{weaponRef.WeaponType}");
            }
            else
            {
                // 没有武器装备，切换到默认武器或隐藏武器
                Debug.Log("[PlayerCharacterController] 玩家卸下了武器，准备切换到默认武器");
                SwitchWeapon(WeaponType.Pistol); // 切换到默认武器（手枪）
            }
        }

        private void OnEnable()
        {
            // 注册事件监听器
            this.RegisterEvent<MoveInputEvent>(OnMoveInput).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<SprintInputEvent>(OnSprintInput).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<ShootInputEvent>(OnShootInput).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<ReloadInputEvent>(OnReloadInput).UnRegisterWhenGameObjectDestroyed(gameObject);

            // 监听弹药变化事件
            // this.RegisterEvent<AmmoChangedEvent>(OnAmmoChangedEvent).UnRegisterWhenGameObjectDestroyed(gameObject);
            // 监听伤害事件
            this.RegisterEvent<DamageEvent>(OnDamageEvent).UnRegisterWhenGameObjectDestroyed(gameObject);
            // 监听死亡事件
            this.RegisterEvent<DieEvent>(OnDieEvent).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        /// <summary>
        /// 处理弹药变化事件
        /// </summary>
        //private void OnAmmoChangedEvent(AmmoChangedEvent e)
        //{
        //    // 更新UI显示
        //    Debug.Log($"弹药变化：{e.AmmoType}，数量：{e.Count}，等级：{e.Level}");
        //}

        /// <summary>
        /// 处理伤害事件
        /// </summary>
        private void OnDamageEvent(DamageEvent e)
        {
            // 检查事件目标是否是当前玩家
            if (e.Target == gameObject)
            {
                // 播放受伤动画
                Debug.Log($"玩家受到伤害：{e.Damage}，剩余生命值：{e.RemainingHealth}");
                // 进入受伤状态
                StateMachine.HandleInput(CharacterInput.Hurt);
            }
        }

        /// <summary>
        /// 处理死亡事件
        /// </summary>
        private void OnDieEvent(DieEvent e)
        {
            // 检查事件目标是否是当前玩家
            if (e.Target == gameObject)
            {
                // 处理死亡逻辑
                Debug.Log("玩家死亡");

                // 调用游戏结束管理器
                if (_gameOverManager != null)
                {
                    _gameOverManager.HandlePlayerDeath();
                }
            }
        }

        private void Update()
        {
            // 更新状态机
            StateMachine.Update();
            // 获取射击方向
            if (_cameraController != null)
            {
                Vector3 shootDirection = _cameraController.GetCameraForward();
                // 使用射击方向进行攻击等操作
            }

            // 处理鼠标滚轮切换弹药等级
            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if (scrollWheel != 0)
            {
                HandleAmmoLevelSwitch(scrollWheel > 0 ? 1 : -1);
            }
        }

        /// <summary>
        /// 处理弹药等级切换
        /// </summary>
        /// <param name="direction">切换方向，1为升级，-1为降级</param>
        private void HandleAmmoLevelSwitch(int direction)
        {
            Debug.Log($"[PlayerCharacterController] 收到鼠标滚轮事件，切换方向：{(direction > 0 ? '上' : '下')}");

            if (m_AmmoSystem != null)
            {
                // 获取当前武器对应的弹药类型
                AmmoType ammoType = GetAmmoTypeFromWeapon(_currentWeapon);
                string directionStr = direction > 0 ? "升级" : "降级";

                Debug.Log($"[PlayerCharacterController] 准备切换弹药等级：{ammoType} {directionStr}");

                // 调用AmmoSystem的SwitchAmmoLevel方法
                bool success = m_AmmoSystem.SwitchAmmoLevel(ammoType, direction);

                if (success)
                {
                    Debug.Log($"[PlayerCharacterController] 弹药等级切换成功");
                }
                else
                {
                    Debug.LogWarning($"[PlayerCharacterController] 弹药等级切换失败，可能没有对应等级的弹药");
                }
            }
            else
            {
                Debug.LogError("[PlayerCharacterController] AmmoSystem引用为空，无法切换弹药等级");
            }
        }

        /// <summary>
        /// 获取武器对应的弹药类型
        /// </summary>
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

        /// <summary>
        /// 处理移动输入事件
        /// </summary>
        /// <param name="e">移动输入事件</param>
        private void OnMoveInput(MoveInputEvent e)
        {
            InputDirection = e.Direction;
        }

        /// <summary>
        /// 处理冲刺输入事件
        /// </summary>
        /// <param name="e">冲刺输入事件</param>
        private void OnSprintInput(SprintInputEvent e)
        {
            _isSprinting = e.IsSprinting;
            // 通知当前状态
            if (StateMachine.currentState is CharacterState characterState)
            {
                characterState.HandleInput(CharacterInput.Sprint, this);
            }
        }

        /// <summary>
        /// 处理射击输入事件
        /// </summary>
        /// <param name="e">射击输入事件</param>
        private void OnShootInput(ShootInputEvent e)
        {
            if (StateMachine.currentState is CharacterState characterState)
            {
                characterState.HandleInput(CharacterInput.Shoot, this);
            }
        }

        /// <summary>
        /// 处理换弹输入事件
        /// </summary>
        /// <param name="e">换弹输入事件</param>
        private void OnReloadInput(ReloadInputEvent e)
        {
            if (StateMachine.currentState is CharacterState characterState)
            {
                characterState.HandleInput(CharacterInput.Reload, this);
            }
        }

        /// <summary>
        /// 切换武器
        /// </summary>
        /// <param name="weaponType">武器类型</param>
        public void SwitchWeapon(WeaponType weaponType)
        {
            Debug.Log($"[PlayerCharacterController] 执行武器切换：{_currentWeapon} -> {weaponType}");

            _currentWeapon = weaponType;
            // 武器切换时，如果不在射击状态，应该保持Weapon参数为0
            // 只有在射击状态下才设置对应武器类型
            // _animator.SetInteger(AnimationParameters.WeaponType, (int)_currentWeapon);

            // 切换武器模型
            if (_weaponModelManager != null)
            {
                _weaponModelManager.SwitchWeaponModel(weaponType);
            }

            Debug.Log($"[PlayerCharacterController] 武器切换完成，当前武器：{weaponType}");
        }



        /// <summary>
        /// 获取游戏架构实例 - 实现IController接口
        /// 使用Architecture<T>.Interface避免编译顺序依赖
        /// </summary>
        /// <returns></returns>
        public IArchitecture GetArchitecture()
        {
            return Architecture<GameArchitecture>.Interface;
        }


    }
}