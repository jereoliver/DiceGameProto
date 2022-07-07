using Dice;
using JetBrains.Annotations;
using GameFlow;
using Scoreboard;
using Zenject;


[UsedImplicitly]
public class TestInstaller : Installer<TestInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<TestController>().AsSingle();
        Container.BindInterfacesTo<DiceController>().AsSingle();
        Container.Bind<IScoreboardController>().WithId("Player").To<ScoreboardController>().AsSingle();
        Container.Bind<IScoreboardController>().WithId("AI").To<AIScoreboardController>().AsSingle();
        Container.BindInterfacesTo<ScoreboardModel>().AsSingle();
        Container.BindInterfacesTo<GameFlowController>().AsSingle();
    }
}
