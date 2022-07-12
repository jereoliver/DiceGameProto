using Dice;
using GameFlow;
using GameFlow.Signals;
using JetBrains.Annotations;
using Scoreboard;
using Scoreboard.AI;
using ScorePossibilities;
using Zenject;

namespace Installers
{
    [UsedImplicitly]
    public class DiceGameInstaller : Installer<DiceGameInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.BindInterfacesTo<DiceController>().AsSingle();
            Container.Bind<IScoreboardController>().WithId("Player").To<ScoreboardController>().AsSingle();
            Container.Bind<IScoreboardController>().WithId("AI").To<AIScoreboardController>().AsSingle();
            Container.Bind<IScoreboardModel>().WithId("Player").To<ScoreboardModel>().AsSingle();
            Container.Bind<IScoreboardModel>().WithId("AI").To<AIScoreboardModel>().AsSingle();
            Container.BindInterfacesTo<GameFlowController>().AsSingle();
            Container.BindInterfacesTo<ScorePossibilitiesController>().AsSingle();

            DeclareSignals();
        }

        private void DeclareSignals()
        {
            Container.DeclareSignal<GameOverSignal>();
            Container.DeclareSignal<LockRowSignal>();
        }
    }
}