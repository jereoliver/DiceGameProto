using Zenject;

namespace Installers
{
    public class DiceGameMonoInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            DiceGameInstaller.Install(Container);
        }
    }
}