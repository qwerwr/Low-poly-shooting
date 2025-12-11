using Game;
using Game.PoolObject;
using QFramework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 1. 在Inspector中设置预制体
    [Header("预制体设置")]
    public GameObject bulletPrefab;  // 子弹预制体

    [Header("敌人预制体")]
    public GameObject enemyPrefab1;  // 第一种敌人预制体
    public GameObject enemyPrefab2;  // 第二种敌人预制体

    [Header("敌人生成位置")]
    public Transform[] spawnPoints = new Transform[10];  // 10个生成位置

    // 对象池引用
    private BulletPool bulletPool;
    private MonoObjectPool enemyPool1;  // 第一种敌人对象池
    private MonoObjectPool enemyPool2;  // 第二种敌人对象池

    void Start()
    {
        // 2. 初始化对象池
        InitializePools();

        // 3. 链接到AmmoSystem
        LinkToAmmoSystem();

        // 4. 测试对象池（可选）
        // TestPool();

        // 5. 生成敌人
        SpawnEnemies();
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    private void InitializePools()
    {
        // 初始化子弹对象池
        if (bulletPrefab != null)
        {
            bulletPool = new BulletPool(
                bulletPrefab,  // 预制体
                20,           // 初始大小
                100           // 最大大小
            );
            // Debug.Log("子弹对象池初始化完成，初始大小:20, 最大大小:100");
        }
        else
        {
            Debug.LogError("子弹预制体未设置！");
        }

        // 初始化第一种敌人对象池
        if (enemyPrefab1 != null)
        {
            enemyPool1 = new MonoObjectPool(
                enemyPrefab1,  // 预制体
                5,           // 初始大小
                25            // 最大大小
            );
            Debug.Log("第一种敌人生成池初始化完成，初始大小:5, 最大大小:25");
        }

        // 初始化第二种敌人对象池
        if (enemyPrefab2 != null)
        {
            enemyPool2 = new MonoObjectPool(
                enemyPrefab2,  // 预制体
                5,           // 初始大小
                25            // 最大大小
            );
            Debug.Log("第二种敌人生成池初始化完成，初始大小:5, 最大大小:25");
        }
    }

    /// <summary>
    /// 链接子弹对象池到AmmoSystem
    /// </summary>
    private void LinkToAmmoSystem()
    {
        if (bulletPool != null)
        {
            var ammoSystem = Architecture<GameArchitecture>.Interface.GetSystem<AmmoSystem>();
            if (ammoSystem != null)
            {
                ammoSystem.bulletPool = bulletPool;
                Debug.Log("子弹对象池已链接到AmmoSystem");
            }
            else
            {
                Debug.LogError("AmmoSystem未找到！");
            }
        }
    }

    /// <summary>
    /// 测试对象池
    /// </summary>
    private void TestPool()
    {
        // 测试获取和回收子弹
        if (bulletPool != null)
        {
            // 获取5个子弹
            for (int i = 0; i < 5; i++)
            {
                Vector3 testPosition = new Vector3(i * 2, 0, 0);
                GameObject bullet = bulletPool.Get(testPosition, Quaternion.identity);
                // Debug.Log($"获取子弹 {i + 1}, 当前对象池大小:{bulletPool.PoolSize}");

                // 2秒后回收子弹
                StartCoroutine(ReturnBulletAfterDelay(bullet, 2f));
            }
        }
    }

    /// <summary>
    /// 延迟回收子弹
    /// </summary>
    private System.Collections.IEnumerator ReturnBulletAfterDelay(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        bulletPool.ReturnBullet(bullet);
        //  Debug.Log($"子弹已回收, 当前对象池大小:{bulletPool.PoolSize}");
    }

    /// <summary>
    /// 在10个位置随机生成敌人，每个地点只生成一个敌人
    /// </summary>
    private void SpawnEnemies()
    {
        // 检查生成位置数量
        if (spawnPoints.Length < 10)
        {
            Debug.LogError("生成位置数量不足10个！");
            return;
        }

        // 创建一个位置索引列表，用于随机选择位置
        List<int> availablePositions = new List<int>();
        for (int i = 0; i < 10; i++)
        {
            availablePositions.Add(i);
        }

        // 随机生成敌人数量（1-10个）
        int enemyCount = Random.Range(1, 11);

        // 生成敌人
        for (int i = 0; i < enemyCount; i++)
        {
            // 如果没有可用位置，退出循环
            if (availablePositions.Count == 0)
            {
                break;
            }

            // 随机选择一个可用位置
            int randomIndex = Random.Range(0, availablePositions.Count);
            int positionIndex = availablePositions[randomIndex];

            // 从可用位置列表中移除该位置，确保每个地点只生成一个敌人
            availablePositions.RemoveAt(randomIndex);

            // 随机选择敌人预制体
            int enemyType = Random.Range(0, 2);
            GameObject enemyObj = null;

            if (enemyType == 0 && enemyPool1 != null)
            {
                enemyObj = enemyPool1.Get(spawnPoints[positionIndex].position, Quaternion.identity);
            }
            else if (enemyType == 1 && enemyPool2 != null)
            {
                enemyObj = enemyPool2.Get(spawnPoints[positionIndex].position, Quaternion.identity);
            }

            if (enemyObj != null)
            {
                // 获取Enemy组件
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    // 随机配置武器
                    enemy.currentWeapon = GetRandomWeapon();
                    Debug.Log($"生成敌人：{enemyType}, 武器：{enemy.currentWeapon}, 位置索引：{positionIndex}, 位置：{spawnPoints[positionIndex].position}");
                }
            }
        }
    }

    /// <summary>
    /// 随机生成武器类型
    /// </summary>
    private Game.WeaponType GetRandomWeapon()
    {
        // 随机生成0-2的整数，对应Pistol, Rifle, Sniper
        int weaponIndex = Random.Range(0, 3);
        return (Game.WeaponType)weaponIndex;
    }
}