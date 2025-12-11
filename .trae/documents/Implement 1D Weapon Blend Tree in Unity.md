# 射击状态类修改建议

## 分析现状

### 您的状态机配置
- **Sub-State Machine**：管理射击状态
- **进入条件**：`IsShooting=true`
- **内部逻辑**：
  - `Weapon=0` → 手枪射击动画
  - `Weapon=1` → 步枪射击动画  
  - `Weapon=2` → 狙击枪射击动画
- **退出条件**：`IsShooting=false`

### 射击状态类的作用
代码中的`ShootingState`类（继承自`CharacterState`）主要负责：
1. 处理射击状态的进入逻辑
2. 更新射击状态
3. 处理射击状态的退出逻辑
4. 处理射击相关的输入

## 修改建议

### 1. 核心修改点

#### 1.1 进入状态逻辑
```csharp
public override void Enter(PlayerCharacterController controller)
{
    // 设置射击状态参数
    controller.Animator.SetBool("IsShooting", true);
    
    // 设置当前武器类型参数
    controller.Animator.SetInteger("Weapon", (int)controller.CurrentWeapon);
    
    // 设置武器类型对应的动画参数
    controller.Animator.SetInteger(AnimationParameters.WeaponType, (int)controller.CurrentWeapon);
    
    // 其他初始化逻辑...
}
```

#### 1.2 退出状态逻辑
```csharp
public override void Exit(PlayerCharacterController controller)
{
    // 重置射击状态参数
    controller.Animator.SetBool("IsShooting", false);
    
    // 其他清理逻辑...
}
```

#### 1.3 更新状态逻辑
```csharp
public override void Update(PlayerCharacterController controller)
{
    // 检查是否需要退出射击状态
    if (_shootTimer <= 0f)
    {
        // 退出射击状态，回到空闲或奔跑状态
        if (controller.InputDirection != Vector2.zero)
        {
            controller.StateMachine.ChangeState(new RunningState());
        }
        else
        {
            controller.StateMachine.ChangeState(new IdleState());
        }
    }
}
```

### 2. 不需要修改的部分
- 状态机的结构设计（您已配置合理）
- 子状态机的内部过渡逻辑
- 武器类型的判断逻辑（已由状态机处理）

### 3. 优化建议

#### 3.1 同步武器参数
确保在切换武器时，同时更新Animator的Weapon参数：
```csharp
// 在武器切换方法中
public void SwitchWeapon(WeaponType weaponType)
{
    _currentWeapon = weaponType;
    controller.Animator.SetInteger("Weapon", (int)weaponType);
}
```

#### 3.2 处理射击完成
在射击动画结束时，通过动画事件通知代码：
```csharp
// 在角色脚本中
public void OnShootComplete()
{
    // 射击动画完成后的逻辑
    IsShooting = false;
    animator.SetBool("IsShooting", false);
}
```

#### 3.3 优化状态切换
确保状态切换时参数设置正确：
```csharp
// 进入射击状态时
controller.StateMachine.ChangeState(new ShootingState());
// ShootingState.Enter() 会自动设置 IsShooting=true
```

## 总结

### 需要修改的射击状态类
1. **Enter方法**：设置`IsShooting=true`和`Weapon`参数
2. **Exit方法**：设置`IsShooting=false`
3. **Update方法**：处理射击状态的更新和退出逻辑

### 不需要修改的部分
- 状态机的整体结构
- 子状态机的内部逻辑
- 武器类型的判断逻辑

### 优化建议
- 确保武器参数同步更新
- 添加动画事件处理射击完成
- 优化状态切换逻辑

您的状态机配置已经很合理，只需要确保代码中的射击状态类与这个配置正确配合即可。主要是在进入和退出射击状态时，正确设置IsShooting和Weapon参数，让状态机能够按照您的设计工作。