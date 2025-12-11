using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    /// <summary>
    /// 第三人称视角摄像机控制器
    /// </summary>
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

        private void Start()
        {   // 自动查找玩家目标
            if (Target == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    Target = player.transform;
                }
            }

            // 初始化鼠标锁定状态
            SetCursorLock(LockCursor);

            // 初始化旋转角度
            //Vector3 angles = transform.eulerAngles;
            //RotationX = angles.y;
            //RotationY = angles.x;
        }

        private void Update()
        {
            HandleMouseRotation();
            HandleCursorLock();
        }

        private void LateUpdate()
        {
            FollowTarget();
        }

        /// <summary>
        /// 处理鼠标旋转
        /// </summary>
        private void HandleMouseRotation()
        {
            // 只有在鼠标锁定且UI未激活时才处理鼠标旋转
            if (Cursor.lockState == CursorLockMode.Locked && !IsUIActive && Target != null)
            {
                // 获取鼠标输入
                float mouseX = Input.GetAxis("Mouse X") * SensitivityX * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * SensitivityY * Time.deltaTime;

                // 更新旋转角度
                RotationX += mouseX;
                RotationY -= mouseY;

                // 限制Y轴旋转角度
                RotationY = Mathf.Clamp(RotationY, MinRotationY, MaxRotationY);

                // 计算摄像机旋转
                Quaternion cameraRotation = Quaternion.Euler(RotationY, RotationX, 0.0f);

                // 计算摄像机目标位置
                // 从目标位置向后偏移Distance，向上偏移Height
                m_TargetPosition = Target.position - cameraRotation * Vector3.forward * Distance + Vector3.up * Height;

                // 立即设置位置，避免闪烁
                transform.position = m_TargetPosition;

                // 始终看向目标中心
                transform.LookAt(Target.position);
            }
        }

        /// <summary>
        /// 跟随目标
        /// </summary>
        private void FollowTarget()
        {
            if (Target == null) return;

            // 无论是否锁定，都保持跟随
            // 计算摄像机旋转
            Quaternion cameraRotation = Quaternion.Euler(RotationY, RotationX, 0.0f);

            // 计算摄像机目标位置
            m_TargetPosition = Target.position - cameraRotation * Vector3.forward * Distance + Vector3.up * Height;

            // 当鼠标未锁定或UI激活时，平滑跟随
            if (Cursor.lockState != CursorLockMode.Locked || IsUIActive)
            {
                // 平滑移动摄像机
                transform.position = Vector3.SmoothDamp(transform.position, m_TargetPosition, ref m_SmoothVelocity, SmoothTime);
            }
            else
            {
                // 锁定状态下立即跟随，避免晃动
                transform.position = m_TargetPosition;
            }

            // 始终看向目标中心
            transform.LookAt(Target.position);
        }

        /// <summary>
        /// 处理鼠标锁定
        /// </summary>
        private void HandleCursorLock()
        {
            // 按ESC键解锁/锁定鼠标
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleCursorLock();
            }

            // 点击UI时解锁鼠标
            if (EventSystem.current.IsPointerOverGameObject() && Cursor.lockState == CursorLockMode.Locked)
            {
                SetCursorLock(false);
            }

            // 点击场景时锁定鼠标
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && Cursor.lockState != CursorLockMode.Locked)
            {
                SetCursorLock(true);
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