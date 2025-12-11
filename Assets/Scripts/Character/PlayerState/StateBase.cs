using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 所有状态类的基类
    /// </summary>
    public class StateBase
    {
        // 状态的唯一标识符
        public int ID { get; set; }
        
        // 所属的状态机实例
        public StateMachine machine;
        
        // 构造函数，接收状态ID作为参数
        public StateBase(int id)
        {
            ID = id;
        }
        
        // 进入状态时调用的方法，可被重写
        public virtual void OnEnter() { }
        
        // 处于状态时每帧调用的方法，可被重写
        public virtual void OnStay() { }
        
        // 离开状态时调用的方法，可被重写
        public virtual void OnExit() { }
    }
    
    /// <summary>
    /// 继承自StateBase，是一个泛型类，允许状态关联一个特定类型的所有者对象
    /// </summary>
    public class StateTemplate<T> : StateBase
    {
        // 状态的所有者对象
        public T owner;
        
        // 构造函数，接收状态ID和所有者对象作为参数
        public StateTemplate(int id, T owner) : base(id)
        {
            this.owner = owner;
        }
    }
    
    /// <summary>
    /// 状态机类 - 管理所有状态的切换
    /// </summary>
    public class StateMachine
    {
        // 用于存储状态ID和对应的状态实例
        public Dictionary<int, StateBase> stateDic;
        
        // 当前激活的状态实例
        public StateBase currentState;
        
        // 构造函数，接收初始状态作为参数
        public StateMachine(StateBase startState)
        {
            currentState = startState;
            stateDic = new Dictionary<int, StateBase>();
            // 将初始状态添加到状态字典中
            AddState(startState);
            // 调用初始状态的OnEnter方法，表示进入该状态
            startState.OnEnter();
        }
        
        // 向状态字典中添加新的状态
        public void AddState(StateBase stateBase)
        {
            if (!stateDic.ContainsKey(stateBase.ID))
            {
                stateDic.Add(stateBase.ID, stateBase);
                // 为状态设置所属的状态机
                stateBase.machine = this;
            }
        }
        
        // 进行状态转换
        public void TranslateState(int id)
        {
            if (!stateDic.ContainsKey(id))
                return;
            
            // 调用当前状态的OnExit方法，表示离开该状态
            currentState.OnExit();
            
            // 更新当前状态为目标状态
            currentState = stateDic[id];
            
            // 调用目标状态的OnEnter方法，表示进入该状态
            currentState.OnEnter();
        }
        
        // 每帧更新当前状态
        public void Update()
        {
            currentState?.OnStay();
        }
        
        // 处理输入
        public void HandleInput(CharacterInput input)
        {
            if (currentState is CharacterState characterState)
            {
                characterState.HandleInput(input, characterState.owner);
            }
        }
        
        // 切换状态（旧方法，保持兼容性）
        public void ChangeState(CharacterState newState)
        {
            // 调用当前状态的OnExit方法
            currentState?.OnExit();
            
            // 更新当前状态
            currentState = newState;
            
            // 添加新状态到字典
            AddState(newState);
            
            // 调用新状态的OnEnter方法
            currentState.OnEnter();
        }
    }
}