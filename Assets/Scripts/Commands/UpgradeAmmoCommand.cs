using QFramework;

namespace Game
{
    /// <summary>
    /// 升级弹药命令
    /// </summary>
    public class UpgradeAmmoCommand : AbstractCommand
    {
        public AmmoType AmmoType { get; set; }

        protected override void OnExecute() 
        {
            var ammoSystem = this.GetSystem<AmmoSystem>();
            ammoSystem.HandleUpgradeAmmoCommand(this);
        }
    }
}