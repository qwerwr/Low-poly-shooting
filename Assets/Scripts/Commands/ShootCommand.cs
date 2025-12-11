using QFramework;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 射击命令
    /// </summary>
    public class ShootCommand : AbstractCommand
    {
        public GameObject Shooter { get; set; }
        public WeaponType WeaponType { get; set; }
        public Vector3 ShootDirection { get; set; }
        public Vector3 ShootPosition { get; set; }  // 保存ShootPoint的位置副本
        public Quaternion ShootRotation { get; set; }  // 保存ShootPoint的旋转副本

        protected override void OnExecute()
        {
            var ammoSystem = this.GetSystem<AmmoSystem>();
            ammoSystem.HandleShootCommand(this);
        }
    }
}