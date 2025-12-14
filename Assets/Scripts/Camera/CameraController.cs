using UnityEngine;   使用UnityEngine;
using UnityEngine.EventSystems;使用UnityEngine.EventSystems;

namespace Game   名称空间游戏
{
    /// <summary>   > / / / <总结
    /// 第三人称视角摄像机控制器
    /// </summary>   > / / / < /总结
    public class CameraController : MonoBehaviour
    {
        [Header("跟随目标")]
        public Transform Target; // 跟随的玩家目标

        [Header("摄像机设置")]
        public float Distance = 5.0f; // 摄像机与目标的距离
        public float Height = 2.0f; // 摄像机相对于目标的高度
        public float SensitivityX = 100.0f; // 鼠标X轴灵敏度
        public float SensitivityY = 100.0f; // 鼠标Y轴灵敏度
        public float RotationX = 0.0f; // X轴旋转角度
        public float RotationY = 0.0f; // Y轴旋转角度
        public float MinRotationY = -60.0f; // Y轴最小旋转角度
        public float MaxRotationY = 60.0f; // Y轴最大旋转角度

        [Header("平滑设置")]
        public float SmoothTime = 0.3f; // 平滑跟随时间

        [Header("鼠标设置")]
        public bool LockCursor = true; // 是否锁定鼠标
        public bool IsUIActive = false; // UI是否激活

        private Vector3 m_SmoothVelocity; // 平滑速度
        private Vector3 m_TargetPosition; // 目标位置

        private void Start()   私有void Start（）private void   无效 Start（）私有void Start（）
        {   // 自动查找玩家目标
            if (Target == null)   if   如果 （Target == null）if (Target == null   零) if   如果 （Target == null）
            {
                GameObject player = GameObject.FindWithTag("Player");GameObject player = GameObject. findwithtag（“玩家”）；GameObject player = GameObject. findwithtag（“玩家”）；GameObject player = GameObject。findwithtag（“玩家”）；
                if (player != null)   If(玩家！= null   零)
                {
                    Target = player.transform;Target = player.transform；Target = player.transform；Target = player.transform；
                }
            }

            // 初始化鼠标锁定状态
            SetCursorLock(LockCursor);

          
        }

        private   私人 void   无效 Update()   私有的void Update（）
        {
            HandleMouseRotation();
            HandleCursorLock();
        }

        私有的void LateUpdate（）private void LateUpdate()
        {
            FollowTarget();
        }

        /// <summary>   > / / / <总结
        /// 处理鼠标旋转
        /// </summary>   > / / / < /总结
        私有的void handlemousertation （）private void   无效 HandleMouseRotation()
        {
            // 只有在鼠标锁定且UI未激活时才处理鼠标旋转
            如果游标。lockState == CursorLockMode。锁定&& ！视觉主动&&目标！= null)if (Cursor.lockState == CursorLockMode.Locked && !IsUIActive && Target != null)
            {
                // 获取鼠标输入
                float mouseX = Input.GetAxis(浮动mouseX =输入。GetAxis("Mouse X") * SensitivityX * Time.deltaTime；"Mouse X") * SensitivityX * Time.deltaTime;
                float mouseY = Input.GetAxis(浮动鼠标=输入。GetAxis("Mouse Y") *灵敏度* Time.deltaTime；"Mouse Y") * SensitivityY * Time.deltaTime;

                // 更新旋转角度
                RotationX += mouseX;   RotationX = mouseX；
                RotationY -= mouseY;   rotate -=鼠标；

                // 限制Y轴旋转角度
                RotationY = Mathf.Clamp(RotationY, MinRotationY, MaxRotationY);rotate = Mathf。夹具（旋转，MinRotationY, MaxRotationY）；

                // 计算摄像机旋转
                Quaternion cameraRotation = Quaternion.Euler(RotationY, RotationX, 四元数相机旋转=四元数。欧拉（rotate, RotationX, 0.0f）；0.0f);

                // 计算摄像机目标位置
                // 从目标位置向后偏移Distance，向上偏移Height
                m_TargetPosition = Target.position - cameraRotation * Vector3.forward * Distance + Vector3.up * Height;m_TargetPosition =目标。□position: camerarrotation * Vector3。3.向前*距离矢量up *高度；

                // 立即设置位置，避免闪烁
                transform.position = m_TargetPosition;

                // 始终看向目标中心
                transform.LookAt(Target.position);
            }
        }

           > / / / <总结/// <summary>   > / / / <总结
        /// 跟随目标
           > / / / < /总结/// </summary>   > / / / < /总结
        private void   无效 FollowTarget()
        {
            if   如果   如果   如果   如果 (Target == null   零   零) return；if   如果 (Target == null   零   零) return   返回   返回;

            // 无论是否锁定，都保持跟随
            // 计算摄像机旋转
            Quaternion cameraRotation = Quaternion.Euler(RotationY, RotationX, 四元数相机旋转=四元数。欧拉（rotate, RotationX, 0.0f   0.0度）；0.0f);

            // 计算摄像机目标位置
            m_TargetPosition = Target.position - cameraRotation * Vector3.forward * Distance + Vector3.up * Height;m_TargetPosition =目标。□position: camerarrotation * Vector3。3.向前*距离矢量up *高度；

            // 当鼠标未锁定或UI激活时，平滑跟随
            if (Cursor.lockState != CursorLockMode.Locked || IsUIActive)
            {
                // 平滑移动摄像机
                transform.position = Vector3.SmoothDamp(transform.position, m_TargetPosition, ref m_SmoothVelocity, SmoothTime);变换。position = Vector3.SmoothDamp（变换后的效果）ref m_TargetPosition, m_SmoothVelocity, SmoothTime)；
            }
               其他else
            {
                // 锁定状态下立即跟随，避免晃动
                transform.position = m_TargetPosition;
            }

            // 始终看向目标中心
            transform.LookAt(Target.position);
        }

           > / / / <总结/// <summary>
        /// 处理鼠标锁定
           > / / / < /总结/// </summary>
        私有void HandleCursorLock（）private void HandleCursorLock()
        {
            // 按ESC键解锁/锁定鼠标
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleCursorLock();
            }

            // 点击UI时解锁鼠标
            如果(EventSystem.current。IsPointerOverGameObject() &&光标。lockState == CursorLockMode。锁)if   如果 (EventSystem.current.IsPointerOverGameObject() && Cursor.lockState == CursorLockMode.Locked)
            {
                SetCursorLock(false);
            }

            // 点击场景时锁定鼠标
            if   如果 (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && Cursor.lockState != CursorLockMode.Locked)
            {
                SetCursorLock(true   真正的   真正的);
            }
        }

        /// <summary>
        /// 设置鼠标锁定状态
        /// </summary>
        /// <param name="lockState">是否锁定</param>
        public void SetCursorLock(bool lockState)
        {
            Cursor.lockState = lockState ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !lockState;
            LockCursor = lockState;
        }

        /// <summary>
        /// 切换鼠标锁定状态
        /// </summary>
        public void ToggleCursorLock()
        {
            SetCursorLock(!LockCursor);
        }

        /// <summary>
        /// 设置UI激活状态
        /// </summary>
        /// <param name="active">是否激活</param>
        public void SetUIActive(bool active)
        {
            IsUIActive = active;
            if (active)
            {
                SetCursorLock(false);
            }
            else
            {
                SetCursorLock(true);
            }
        }

        /// <summary>
        /// 获取摄像机的前向方向
        /// 用于射击方向
        /// </summary>
        public Vector3 GetCameraForward()
        {
            Vector3 forward = transform.forward;
            forward.y = 0;
            return forward.normalized;
        }
    }
}
