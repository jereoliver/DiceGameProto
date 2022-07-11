using Dice;
using JetBrains.Annotations;
using GameFlow;
using GameFlow.Signals;
using Scoreboard;
using Scoreboard.AI;
using ScorePossibilities;
using Zenject;


[UsedImplicitly]
public class TestInstaller : Installer<TestInstaller>
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.BindInterfacesTo<TestController>().AsSingle();
        Container.BindInterfacesTo<DiceController>().AsSingle();
        Container.Bind<IScoreboardController>().WithId("Player").To<ScoreboardController>().AsSingle();
        Container.Bind<IScoreboardController>().WithId("AI").To<AIScoreboardController>().AsSingle();
        Container.Bind<IAIScoreboardController>().To<AIScoreboardController>().AsSingle();
        Container.Bind<IScoreboardModel>().WithId("Player").To<ScoreboardModel>().AsSingle();
        Container.Bind<IScoreboardModel>().WithId("AI").To<AIScoreboardModel>().AsSingle();
        Container.BindInterfacesTo<GameFlowController>().AsSingle();
        Container.BindInterfacesTo<ScorePossibilitiesController>().AsSingle();

        DeclareAndBindSignals();
    }

    private void DeclareAndBindSignals()
    {
        Container.DeclareSignal<GameOverSignal>();
        Container.DeclareSignal<LockRowSignal>();
    }
}