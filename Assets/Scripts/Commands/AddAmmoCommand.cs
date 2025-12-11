using QFramework;

namespace Game
{
    /// <summary>
    /// 添加弹药命令
    /// </summary>
    public class AddAmmoCommand : AbstractCommand
    {
        public AmmoType AmmoType { get; set; }
        public int Amount { get; set; } = 10;

        protected override void OnExecute() 
        {
            var ammoSystem = this.GetSystem<AmmoSystem>();
            ammoSystem.HandleAddAmmoCommand(this);
        }
    }
}