using UnityEngine;

namespace Game.Character
{
    /// <summary>
    /// 动画控制器配置指南 - 如何在Unity中设置Animator Controller
    /// 就像给演员准备表演剧本和舞台指导
    /// </summary>
    public class AnimationControllerGuide : MonoBehaviour
    {
        [Header("动画控制器配置说明")]
        [TextArea(10, 20)]
        [SerializeField] private string guideText = 
@"
🎯 动画控制器配置指南

📋 1. 创建Animator Controller
   - 在Project窗口右键 → Create → Animator Controller
   - 命名为'CharacterAnimatorController'

🎭 2. 设置动画参数（Parameters）
   需要创建以下参数：
   - MoveX (Float) - 水平移动
   - MoveY (Float) - 垂直移动
   - IsShooting (Bool) - 是否射击（控制状态机进入/退出）
   - IsReloading (Bool) - 是否换弹
   - IsSprinting (Bool) - 是否冲刺
   - IsHurt (Bool) - 是否受伤
   - Weapon (Int) - 武器类型 (0=手枪, 1=步枪, 2=狙击枪) - 控制武器动画切换

🏃 3. 设置状态机（State Machine）
   
   🔹 核心设计：使用有限状态机（FSM）代码驱动
   - 由CharacterStateMachine类管理状态切换
   - 每个状态对应一个C#类（IdleState, RunningState等）
   - 状态转换逻辑在代码中实现
   
   🔹 状态机结构：
   - Entry → Any State → 主要状态
   
   � 射击子状态机（Sub-State Machine）
   - 名称: ShootingStateMachine
   - 进入条件: IsShooting = true
   - 退出条件: IsShooting = false
   - 内部结构：
     * 根据Weapon参数选择武器动画
     * 0 → 手枪射击动画
     * 1 → 步枪射击动画
     * 2 → 狙击枪射击动画
   
   🔹 主要状态及转换：
   
   📍 空闲状态 (Idle)
   - 动画: 空闲动画
   - 进入条件: 无输入且不在其他状态
   - 退出条件: 移动输入 → 奔跑状态
             射击输入 → 射击状态
             冲刺输入 → 冲刺状态
             受伤输入 → 受伤状态
   
   📍 奔跑状态 (Running)
   - 动画: 移动混合树
   - 进入条件: 移动输入
   - 退出条件: 无移动输入 → 空闲状态
             射击输入 → 射击状态
             换弹输入 → 换弹状态
             冲刺输入 → 冲刺状态
             受伤输入 → 受伤状态
   
   📍 射击状态 (Shooting)
   - 动画: 子状态机根据Weapon参数播放对应武器射击动画
   - 进入条件: 射击输入
   - 退出条件: 射击动画完成 → 回到空闲/奔跑状态
             换弹输入 → 换弹状态
             受伤输入 → 受伤状态
   
   📍 换弹状态 (Reloading)
   - 动画: 换弹动画
   - 进入条件: 换弹输入
   - 退出条件: 换弹动画完成 → 回到空闲/奔跑状态
             受伤输入 → 受伤状态
   
   📍 冲刺状态 (Sprinting)
   - 动画: 冲刺动画
   - 进入条件: 冲刺输入且有移动
   - 退出条件: 无移动输入 → 空闲状态
             停止冲刺 → 奔跑状态
             受伤输入 → 受伤状态
   
   📍 受伤状态 (Hurt)
   - 动画: 受伤动画
   - 进入条件: 受伤输入
   - 退出条件: 受伤动画完成 → 回到空闲/奔跑状态

🎨 4. 混合树配置
   
   🔹 移动混合树（2D Freeform Cartesian）
   - 参数: MoveX, MoveY
   - 动画剪辑:
     * (0,0): 空闲动画
     * (0,1): 向前走
     * (0,-1): 向后走
     * (1,0): 向右走
     * (-1,0): 向左走
     * 对角线: 相应方向的行走动画
   
   🔹 武器动画配置（子状态机替代混合树）
   - 不再使用1D武器混合树
   - 使用子状态机根据Weapon参数切换武器动画
   - 每个武器类型对应一个单独的动画状态

🔧 5. 动画层设置
   
   🔹 Base Layer (权重1.0)
   - 包含所有状态和子状态机
   - 管理完整角色动画
   - 无需额外的Upper Body Layer（代码驱动状态切换）

💡 6. 使用说明
   
   1. 将Animator Controller拖拽到角色的Animator组件
   2. 确保角色有Animator和必要的Collider组件
   3. 在PlayerCharacterController脚本中设置Animator引用
   4. 状态切换由有限状态机代码自动处理
   5. 测试各个状态切换是否流畅

⚠️ 注意事项
   - 确保动画剪辑的循环设置正确
   - 射击、换弹、受伤动画不要循环
   - 设置合理的动画过渡时间（建议0.1-0.2秒）
   - 射击子状态机中，使用Has Exit Time确保动画完整播放
   - 状态转换条件与代码中的有限状态机逻辑保持一致
   - 武器动画长度应与代码中的SHOOT_DURATION等常量匹配

🔧 7. 代码与动画配合
   - CharacterStateMachine类管理状态切换
   - 每个状态类（如ShootingState）控制动画参数
   - 进入状态时设置IsShooting=true，退出时设置IsShooting=false
   - 状态类中的Update方法控制状态持续时间
   - 动画事件可用于通知代码动画完成

📋 8. 状态类对应关系
   - IdleState → 空闲状态
   - RunningState → 奔跑状态
   - ShootingState → 射击状态（控制子状态机）
   - ReloadingState → 换弹状态
   - SprintingState → 冲刺状态
   - HurtState → 受伤状态
";

        [Header("示例动画剪辑命名")]
        [SerializeField] private string[] animationClipNames = new string[]
        {
            "Idle",
            "Walk_Forward", "Walk_Backward", "Walk_Left", "Walk_Right",
            "Run_Forward", "Run_Backward", "Run_Left", "Run_Right",
            "Shoot_Pistol", "Shoot_Rifle", "Shoot_Sniper",
            "Reload_Pistol", "Reload_Rifle", "Reload_Sniper",
            "Sprint",
            "Hurt"
        };

        /// <summary>
        /// 获取配置指南文本
        /// </summary>
        public string GetGuideText()
        {
            return guideText;
        }

        /// <summary>
        /// 获取动画剪辑名称列表
        /// </summary>
        public string[] GetAnimationClipNames()
        {
            return animationClipNames;
        }

        private void OnGUI()
        {
            // 在游戏运行时显示配置指南
            if (Application.isPlaying)
            {
                GUILayout.BeginArea(new Rect(320, 10, 400, 600));
                GUILayout.Label("动画控制器配置指南", GUILayout.Height(30));
                GUILayout.TextArea(guideText, GUILayout.ExpandHeight(true));
                GUILayout.EndArea();
            }
        }
    }
}