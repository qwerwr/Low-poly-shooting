using QFramework;

namespace Game
{
    /// <summary>
    /// 消耗弹药命令
    /// </summary>
    public class ConsumeAmmoCommand : AbstractCommand
    {
        public AmmoType AmmoType { get; set; }
        public int Amount { get; set; } = 1;

        protected override void OnExecute() 
        {
            var ammoSystem = this.GetSystem<AmmoSystem>();
            ammoSystem.HandleConsumeAmmoCommand(this);
        }
    }
}