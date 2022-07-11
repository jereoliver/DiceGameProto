using System;
using GameFlow.Signals;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Zenject;

namespace Scoreboard
{
    public interface IScoreboardController
    {
        IReadOnlyReactiveProperty<bool> IsActiveTurn { get; } // this helps ScoreboardPresenter to visualize state
        IReadOnlyReactiveProperty<bool> ThisTurnEnded { get; } // listen to this from GameFlowController
        void AddCross(SlotColor color);
        void AddError();
        void StartTurn(bool activeTurn);
        void EndTurn();
        void LockRow(SlotColor slotColor); // todo delete this after AI is implemented
    }

    [UsedImplicitly]
    public class ScoreboardController : IScoreboardController
    {
        private IReactiveProperty<bool> IsActiveTurn { get; }
        private IReactiveProperty<bool> ThisTurnEnded { get; }
        

        private int amountOfRedCrosses;
        private int amountOfYellowCrosses;
        private int amountOfGreenCrosses;
        private int amountOfBlueCrosses;
        private int amountOfErrors;

        [Inject(Id = "Player")] private IScoreboardModel scoreboard; // todo inject correct one with Id or create
        private readonly SignalBus signalBus;


        public ScoreboardController(SignalBus signalBus)
        { 
            this.signalBus = signalBus;
            IsActiveTurn = new ReactiveProperty<bool>();
            ThisTurnEnded = new ReactiveProperty<bool>();
        }
        
        IReadOnlyReactiveProperty<bool> IScoreboardController.IsActiveTurn => IsActiveTurn;
        IReadOnlyReactiveProperty<bool> IScoreboardController.ThisTurnEnded => ThisTurnEnded;

        public void AddCross(SlotColor color)
        {
            switch (color)
            {
                case SlotColor.Red:
                    amountOfRedCrosses++;
                    scoreboard.SetPoints(ScoreType.Red, ConvertAmountOfCrossesToPoints(amountOfRedCrosses));
                    break;
                case SlotColor.Yellow:
                    amountOfYellowCrosses++;
                    scoreboard.SetPoints(ScoreType.Yellow, ConvertAmountOfCrossesToPoints(amountOfYellowCrosses));
                    break;
                case SlotColor.Green:
                    amountOfGreenCrosses++;
                    scoreboard.SetPoints(ScoreType.Green, ConvertAmountOfCrossesToPoints(amountOfGreenCrosses));
                    break;
                case SlotColor.Blue:
                    amountOfBlueCrosses++;
                    scoreboard.SetPoints(ScoreType.Blue, ConvertAmountOfCrossesToPoints(amountOfBlueCrosses));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }

            UpdateTotalPoints();
        }

        public void AddError()
        {
            amountOfErrors++;
            scoreboard.SetPoints(ScoreType.Error, amountOfErrors * 5); // todo get errorSingularScore from config
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
            Debug.Log("turn ended");
            ThisTurnEnded.Value = true;
        }

        public void LockRow(SlotColor slotColor)
        {
            signalBus.Fire(new LockRowSignal(slotColor)); // todo delete this after AI is implemented
        }

        private void UpdateTotalPoints()
        {
            var totalPoints = scoreboard.RedPoints.Value + scoreboard.YellowPoints.Value +
                              scoreboard.GreenPoints.Value + scoreboard.BluePoints.Value -
                              scoreboard.ErrorPoints.Value;
            scoreboard.SetPoints(ScoreType.Total, totalPoints);
        }

        private static int ConvertAmountOfCrossesToPoints(int amount)
        {
            var returnValue = 0;
            switch (amount)
            {
                case 0:
                    returnValue = 0;
                    break;
                case 1:
                    returnValue = 1;
                    break;
                case 2:
                    returnValue = 3;
                    break;
                case 3:
                    returnValue = 6;
                    break;
                case 4:
                    returnValue = 10;
                    break;
                case 5:
                    returnValue = 15;
                    break;
                case 6:
                    returnValue = 21;
                    break;
                case 7:
                    returnValue = 28;
                    break;
                case 8:
                    returnValue = 36;
                    break;
                case 9:
                    returnValue = 45;
                    break;
                case 10:
                    returnValue = 55;
                    break;
                case 11:
                    returnValue = 66;
                    break;
                case 12:
                    returnValue = 78;
                    break;
                default:
                    return 0;
            }

            return returnValue;
        }
    }
}