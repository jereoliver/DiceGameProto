using System;
using Extensions;
using GameFlow.Signals;
using JetBrains.Annotations;
using Scoreboard.AI;
using UniRx;
using Zenject;

namespace Scoreboard
{
    public interface IScoreboardController
    {
        IReadOnlyReactiveProperty<bool> IsActiveTurn { get; } // This helps ScoreboardPresenters to visualize turns
        IReadOnlyReactiveProperty<bool> ThisTurnEnded { get; } // This is listened in GameFlowController to know when all players have ended their turn
        void AddCross(SlotColor color);
        void AddError();
        void StartTurn(bool activeTurn);
        void EndTurn();
        AISlotsModel CurrentSlotsState { get; } // This is currently only used for AIScoreboardController.
    }

    [UsedImplicitly]
    public class ScoreboardController : IScoreboardController
    {
        private int amountOfRedCrosses;
        private int amountOfYellowCrosses;
        private int amountOfGreenCrosses;
        private int amountOfBlueCrosses;
        private int amountOfErrors;
        
        private readonly SignalBus signalBus;
        [Inject(Id = "Player")] private IScoreboardModel scoreboard;
        private IReactiveProperty<bool> IsActiveTurn { get; }
        private IReactiveProperty<bool> ThisTurnEnded { get; }
        IReadOnlyReactiveProperty<bool> IScoreboardController.IsActiveTurn => IsActiveTurn;
        IReadOnlyReactiveProperty<bool> IScoreboardController.ThisTurnEnded => ThisTurnEnded;
        public AISlotsModel CurrentSlotsState { get; }

        public ScoreboardController(SignalBus signalBus)
        {
            this.signalBus = signalBus;
            CurrentSlotsState = new AISlotsModel();
            IsActiveTurn = new ReactiveProperty<bool>();
            ThisTurnEnded = new ReactiveProperty<bool>();
        }


        public void AddCross(SlotColor color)
        {
            switch (color)
            {
                case SlotColor.Red:
                    amountOfRedCrosses++;
                    scoreboard.SetPoints(ScoreType.Red, amountOfRedCrosses.ConvertAmountOfCrossesToPoints());
                    break;
                case SlotColor.Yellow:
                    amountOfYellowCrosses++;
                    scoreboard.SetPoints(ScoreType.Yellow, amountOfYellowCrosses.ConvertAmountOfCrossesToPoints());
                    break;
                case SlotColor.Green:
                    amountOfGreenCrosses++;
                    scoreboard.SetPoints(ScoreType.Green, amountOfGreenCrosses.ConvertAmountOfCrossesToPoints());
                    break;
                case SlotColor.Blue:
                    amountOfBlueCrosses++;
                    scoreboard.SetPoints(ScoreType.Blue, amountOfBlueCrosses.ConvertAmountOfCrossesToPoints());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }

            UpdateTotalPoints();
        }

        public void AddError()
        {
            amountOfErrors++;
            scoreboard.SetPoints(ScoreType.Error, amountOfErrors * 5);
            UpdateTotalPoints();
            if (amountOfErrors >= 4)
            {
                signalBus.Fire(new GameOverSignal());
            }
        }

        public void StartTurn(bool activeTurn)
        {
            ThisTurnEnded.Value = false;
            IsActiveTurn.Value = activeTurn;
        }

        public void EndTurn()
        {
            ThisTurnEnded.Value = true;
        }

        private void UpdateTotalPoints()
        {
            var totalPoints = scoreboard.RedPoints.Value + scoreboard.YellowPoints.Value +
                              scoreboard.GreenPoints.Value + scoreboard.BluePoints.Value -
                              scoreboard.ErrorPoints.Value;
            scoreboard.SetPoints(ScoreType.Total, totalPoints);
        }
    }
}