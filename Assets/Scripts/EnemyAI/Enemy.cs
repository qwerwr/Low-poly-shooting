using UnityEngine;
using UnityEngine.AI;
using QFramework;
using System.Collections;

namespace Game
{
    /// <summary>
    /// 敌人AI主类
    /// </summary>
    public class Enemy : MonoBehaviour, IController
    {
        [Header("基本设置")]
        public float patrolRange = 5f;
        public float shootRange = 7f;
        public float chaseRange = 10f;
        public float loseRange = 15f;
        public float fovAngle = 120f;
        public float rotationSpeed = 5f;
        public float hurtDuration = 1f;
        public WeaponType currentWeapon = WeaponType.Pistol;

        [Header("组件引用")]
        public Animator anim;
        public Transform player;
        public Transform attackPoint;
        public NavMeshAgent navMeshAgent;
        public Health health;
        public WeaponModelManager weaponModelManager;

        // 内部字段
        private StateMachine stateMachine;
        private Vector3 patrolTarget;
        private float shootCooldown;
        private float patrolTimer;
        private float hurtTimer;

        // 状态枚举
        private enum EnemyStateType { Patrol = 0, Chase = 1, Shoot = 2, Hurt = 3, Die = 4 }

        private void Awake()
        {
            // 获取组件
            if (anim == null)
                anim = GetComponentInChildren<Animator>();
            if (player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    player = playerObj.transform;
                }
            }
            if (attackPoint == null)
                attackPoint = transform.Find("AttackPoint");
            if (navMeshAgent == null)
                navMeshAgent = GetComponent<NavMeshAgent>();
            if (health == null)
                health = GetComponent<Health>();

            // 初始化武器模型管理器
            if (weaponModelManager == null)
            {
                weaponModelManager = GetComponent<WeaponModelManager>();
                if (weaponModelManager == null)
                {
                    weaponModelManager = gameObject.AddComponent<WeaponModelManager>();
                }
            }
            weaponModelManager.Initialize();

            // 初始化状态机
            stateMachine = new StateMachine(new PatrolState(0, this));
            stateMachine.AddState(new ChaseState(1, this));
            stateMachine.AddState(new ShootState(2, this));
            stateMachine.AddState(new HurtState(3, this));
            stateMachine.AddState(new DieState(4, this));

            // 生成初始巡逻点
            GeneratePatrolTarget();
        }

        private void OnEnable()
        {
            // 监听伤害事件
            this.RegisterEvent<DamageEvent>(OnDamageEvent);
            // 监听死亡事件
            this.RegisterEvent<DieEvent>(OnDieEvent);
        }

        private void OnDisable()
        {
            // 取消监听伤害事件
            this.UnRegisterEvent<DamageEvent>(OnDamageEvent);
            // 取消监听死亡事件
            this.UnRegisterEvent<DieEvent>(OnDieEvent);
        }

        /// <summary>
        /// 处理伤害事件
        /// </summary>
        private void OnDamageEvent(DamageEvent e)
        {
            // 检查事件目标是否是当前敌人
            if (e.Target == gameObject)
            {
                // 进入受伤状态
                stateMachine.TranslateState((int)EnemyStateType.Hurt);
            }
        }

        /// <summary>
        /// 处理死亡事件
        /// </summary>
        private void OnDieEvent(DieEvent e)
        {
            // 检查事件目标是否是当前敌人
            if (e.Target == gameObject)
            {
                // 执行死亡逻辑
                Die();
            }
        }

        private void Update()
        {
            // 更新状态机
            stateMachine.Update();

            // 评估状态转换
            EvaluateTransitions();
        }

        /// <summary>
        /// 评估状态转换
        /// </summary>
        private void EvaluateTransitions()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            bool isPlayerInFOV = IsPlayerInFOV();

            switch (stateMachine.currentState.ID)
            {
                case (int)EnemyStateType.Patrol:
                    if (distanceToPlayer <= loseRange && isPlayerInFOV)
                    {
                        if (distanceToPlayer <= shootRange)
                            stateMachine.TranslateState((int)EnemyStateType.Shoot);
                        else
                            stateMachine.TranslateState((int)EnemyStateType.Chase);
                    }
                    break;

                case (int)EnemyStateType.Chase:
                    if (distanceToPlayer > loseRange)
                        stateMachine.TranslateState((int)EnemyStateType.Patrol);
                    else if (distanceToPlayer <= shootRange && isPlayerInFOV)
                        stateMachine.TranslateState((int)EnemyStateType.Shoot);
                    break;

                case (int)EnemyStateType.Shoot:
                    if (distanceToPlayer > chaseRange)
                        stateMachine.TranslateState((int)EnemyStateType.Chase);
                    else if (distanceToPlayer > loseRange)
                        stateMachine.TranslateState((int)EnemyStateType.Patrol);
                    break;

                case (int)EnemyStateType.Hurt:
                    // 受伤状态自动转换，不需要在此处理
                    break;
            }
        }

        /// <summary>
        /// 检测玩家是否在视野内
        /// </summary>
        private bool IsPlayerInFOV()
        {
            Vector3 dirToPlayer = player.position - transform.position;
            float angle = Vector3.Angle(transform.forward, dirToPlayer);
            return angle <= fovAngle * 0.5f;
        }

        /// <summary>
        /// 生成巡逻目标点
        /// </summary>
        private void GeneratePatrolTarget()
        {
            Vector3 randomOffset = Random.insideUnitSphere * patrolRange;
            randomOffset.y = 0;
            patrolTarget = transform.position + randomOffset;

            // 确保巡逻点在NavMesh上
            NavMesh.SamplePosition(patrolTarget, out NavMeshHit hit, patrolRange, NavMesh.AllAreas);
            patrolTarget = hit.position;
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (health != null)
            {
                health.TakeDamage(damage);
                // 进入受伤状态
                stateMachine.TranslateState((int)EnemyStateType.Hurt);
            }
        }

        /// <summary>
        /// 死亡处理
        /// </summary>
        public void Die()
        {
            // 进入死亡状态
            stateMachine.TranslateState((int)EnemyStateType.Die);
        }

        /// <summary>
        /// 巡逻状态
        /// </summary>
        private class PatrolState : StateTemplate<Enemy>
        {
            private enum PatrolSubState { Moving, Idling }
            private PatrolSubState subState;
            private float idleDuration = 2f;
            private float moveDuration = 3f;
            private float stateTimer;

            public PatrolState(int id, Enemy owner) : base(id, owner) { }

            public override void OnEnter()
            {
                base.OnEnter();

                // 进入巡逻状态时重置所有动画参数
                owner.anim.SetInteger(AnimationParameters.WeaponType, 0); // 重置武器类型

                // 初始状态：移动
                subState = PatrolSubState.Moving;
                stateTimer = 0;

                // 开始移动，切入Run动画
                owner.anim.SetBool("Run", true);
                owner.GeneratePatrolTarget();
                owner.navMeshAgent.SetDestination(owner.patrolTarget);
                owner.navMeshAgent.isStopped = false;
            }

            public override void OnStay()
            {
                base.OnStay();

                stateTimer += Time.deltaTime;

                switch (subState)
                {
                    case PatrolSubState.Moving:
                        // 确保移动时Run动画持续播放
                        owner.anim.SetBool("Run", true);

                        // 检查是否到达巡逻点或移动时间结束
                        if (!owner.navMeshAgent.pathPending && owner.navMeshAgent.remainingDistance <= owner.navMeshAgent.stoppingDistance || stateTimer >= moveDuration)
                        {
                            // 切换到Idle状态
                            subState = PatrolSubState.Idling;
                            stateTimer = 0;

                            // 停止移动，切入Idle动画
                            owner.anim.SetBool("Run", false);
                            owner.navMeshAgent.isStopped = true;
                        }
                        break;

                    case PatrolSubState.Idling:
                        // 确保Idle时Run动画关闭
                        owner.anim.SetBool("Run", false);

                        // 检查Idle时间是否结束
                        if (stateTimer >= idleDuration)
                        {
                            // 切换到移动状态
                            subState = PatrolSubState.Moving;
                            stateTimer = 0;

                            // 开始移动，切入Run动画
                            owner.anim.SetBool("Run", true);
                            owner.GeneratePatrolTarget();
                            owner.navMeshAgent.SetDestination(owner.patrolTarget);
                            owner.navMeshAgent.isStopped = false;
                        }
                        break;
                }
            }

            public override void OnExit()
            {
                base.OnExit();
                owner.anim.SetBool("Run", false);
                owner.navMeshAgent.isStopped = true;
            }
        }

        /// <summary>
        /// 追击状态
        /// </summary>
        private class ChaseState : StateTemplate<Enemy>
        {
            public ChaseState(int id, Enemy owner) : base(id, owner) { }

            public override void OnEnter()
            {
                base.OnEnter();

                // 进入追击状态时重置所有动画参数
                owner.anim.SetInteger(AnimationParameters.WeaponType, 0); // 重置武器类型
                owner.anim.SetBool("Run", true); // 播放奔跑动画

                // 开始移动追击玩家
                owner.navMeshAgent.SetDestination(owner.player.position);
                owner.navMeshAgent.isStopped = false;
            }

            public override void OnStay()
            {
                base.OnStay();

                // 确保Run动画持续播放
                owner.anim.SetBool("Run", true);

                // 持续追击玩家
                owner.navMeshAgent.SetDestination(owner.player.position);
            }

            public override void OnExit()
            {
                base.OnExit();
                owner.anim.SetBool("Run", false);
                owner.navMeshAgent.isStopped = true;
            }
        }

        /// <summary>
        /// 射击状态
        /// </summary>
        private class ShootState : StateTemplate<Enemy>
        {
            public ShootState(int id, Enemy owner) : base(id, owner) { }

            private float shootTimer;
            private float weaponCooldown;

            public override void OnEnter()
            {
                base.OnEnter();
                shootTimer = 0;

                // 进入射击状态时停止移动，设置武器类型
                owner.navMeshAgent.isStopped = true;
                owner.anim.SetBool("Run", false);

                // 切换武器模型
                if (owner.weaponModelManager != null)
                {
                    owner.weaponModelManager.SwitchWeaponModel(owner.currentWeapon);
                }

                // 设置武器类型
                //int weaponInt = (int)owner.currentWeapon + 1;
                //owner.anim.SetInteger(AnimationParameters.WeaponType, weaponInt);

                // 初始化射击冷却时间
                weaponCooldown = Random.Range(3f, 6f);
            }

            public override void OnStay()
            {
                base.OnStay();

                // 朝向玩家
                Vector3 dirToPlayer = owner.player.position - owner.transform.position;
                dirToPlayer.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(dirToPlayer);
                owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotation, owner.rotationSpeed * Time.deltaTime);

                // 射击冷却 - 每3-6秒射击一次
                shootTimer += Time.deltaTime;

                if (shootTimer >= weaponCooldown)
                {
                    // 保存attackPoint的位置和旋转副本，而不是引用
                    Vector3 shootPosition = owner.attackPoint.position;
                    shootPosition.y = 0.53f;  // 固定y轴高度为0.53
                    Quaternion shootRotation = owner.attackPoint.rotation;

                    // 使用命令模式执行射击
                    owner.SendCommand(new ShootCommand
                    {
                        Shooter = owner.gameObject,
                        WeaponType = owner.currentWeapon,
                        ShootDirection = dirToPlayer.normalized,
                        ShootPosition = shootPosition,
                        ShootRotation = shootRotation
                    });

                    // 重新设置武器类型，触发对应武器的射击动画
                    int weaponInt = (int)owner.currentWeapon + 1;
                    owner.anim.SetInteger(AnimationParameters.WeaponType, weaponInt);

                    // 延迟0.5秒重置武器类型，让射击动画播放完成
                    // 使用Action和协程替代字符串Invoke
                    owner.ExecuteAfterDelay(0.5f, owner.ResetWeaponType);

                    // 重置射击冷却时间
                    shootTimer = 0;
                    weaponCooldown = Random.Range(3f, 6f);
                }
            }

            public override void OnExit()
            {
                base.OnExit();
                // 直接重置武器类型，避免与OnStay中的延迟调用冲突
                owner.anim.SetInteger(AnimationParameters.WeaponType, 0);
                // 离开射击状态时，保持当前武器模型激活，但重置动画参数
            }
        }

        /// <summary>
        /// 延迟执行Action的辅助方法
        /// </summary>
        /// <param name="delay">延迟时间（秒）</param>
        /// <param name="action">要执行的Action</param>
        public void ExecuteAfterDelay(float delay, System.Action action)
        {
            StartCoroutine(ExecuteAfterDelayCoroutine(delay, action));
        }

        /// <summary>
        /// 延迟执行Action的协程
        /// </summary>
        private IEnumerator ExecuteAfterDelayCoroutine(float delay, System.Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        /// <summary>
        /// 重置武器类型，用于延迟调用
        /// </summary>
        protected void ResetWeaponType()
        {
            anim.SetInteger(AnimationParameters.WeaponType, 0);
        }

        /// <summary>
        /// 受伤状态
        /// </summary>
        private class HurtState : StateTemplate<Enemy>
        {
            public HurtState(int id, Enemy owner) : base(id, owner) { }

            public override void OnEnter()
            {
                base.OnEnter();
                owner.hurtTimer = 0;

                // 播放受伤动画
                owner.anim.SetTrigger("Hurt");

                // 停止移动
                owner.navMeshAgent.isStopped = true;
            }

            public override void OnStay()
            {
                base.OnStay();

                // 受伤持续时间
                owner.hurtTimer += Time.deltaTime;
                if (owner.hurtTimer >= owner.hurtDuration)
                {
                    // 受伤结束，返回之前的状态或进入巡逻
                    owner.stateMachine.TranslateState((int)EnemyStateType.Patrol);
                }
            }

            public override void OnExit()
            {
                base.OnExit();
                // 受伤状态结束，恢复移动
                owner.navMeshAgent.isStopped = false;
            }
        }

        /// <summary>
        /// 死亡状态
        /// </summary>
        private class DieState : StateTemplate<Enemy>
        {
            public DieState(int id, Enemy owner) : base(id, owner) { }

            public override void OnEnter()
            {
                base.OnEnter();

                // 播放死亡动画
                owner.anim.SetTrigger("Die");

                // 禁用AI
                owner.navMeshAgent.isStopped = true;

                // 这里可以添加死亡效果、经验值奖励等
                Debug.Log($"{owner.gameObject.name} 已死亡");
            }

            public override void OnStay()
            {
                base.OnStay();
                // 死亡状态不需要更新
            }

            public override void OnExit()
            {
                base.OnExit();
                // 死亡状态不能退出
            }
        }

        /// <summary>
        /// 获取游戏架构实例 - 实现IController接口
        /// </summary>
        /// <returns></returns>
        public IArchitecture GetArchitecture()
        {
            return Architecture<GameArchitecture>.Interface;
        }
    }
}