using JetBrains.Annotations;
using DiceGameProto;
using Zenject;


[UsedImplicitly]
public class TestInstaller : Installer<TestInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<TestController>().AsSingle();
        Container.BindInterfacesTo<DiceController>().AsSingle();
        Container.BindInterfacesTo<ScoreboardController>().AsSingle(); // todo later bind as non singleton so can create instance of interface for every player, look for zenject documentation for that
        Container.BindInterfacesTo<Scoreboard>().AsSingle();
        Container.BindInterfacesTo<GameFlowController>().AsSingle();
    }
}
