using System;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace DiceGameProto
{
    public interface IScoreboardController
    {
        // IReadOnlyReactiveProperty<int> RedPoints { get; }
        // IReadOnlyReactiveProperty<int> YellowPoints { get; }
        // IReadOnlyReactiveProperty<int> GreenPoints { get; }
        // IReadOnlyReactiveProperty<int> BluePoints { get; }
        // IReadOnlyReactiveProperty<int> ErrorPoints { get; }
        // IReadOnlyReactiveProperty<int> TotalPoints { get; }
        IReadOnlyReactiveProperty<bool> IsActiveTurn { get; }
        IReadOnlyReactiveProperty<bool> ThisTurnEnded { get; }
        ScorePossibilities CurrentScorePossibilities { get; }
        int CurrentWhiteDiceSum { get; }
        int AmountOfErrors { get; }

        void AddCross(SlotColor color);
        void AddError();
        void StartTurn(ScorePossibilities scorePossibilities, bool activeTurn);
        void EndTurn();
    }

    [UsedImplicitly]
    public class ScoreboardController : IScoreboardController
    {
        // private IReactiveProperty<int> RedPoints { get; }
        // private IReactiveProperty<int> YellowPoints { get; }
        // private IReactiveProperty<int> GreenPoints { get; }
        // private IReactiveProperty<int> BluePoints { get; }
        // private IReactiveProperty<int> ErrorPoints { get; }
        // private IReactiveProperty<int> TotalPoints { get; }
        private IReactiveProperty<bool> IsActiveTurn { get; }
        private IReactiveProperty<bool> ThisTurnEnded { get; }

        public ScorePossibilities CurrentScorePossibilities { get; private set; }
        public int CurrentWhiteDiceSum { get; private set; }

        private int amountOfRedCrosses;
        private int amountOfYellowCrosses;
        private int amountOfGreenCrosses;
        private int amountOfBlueCrosses;
        public int AmountOfErrors { get; private set; }

        private IScoreboard scoreboard; // todo inject correct one with Id


        public ScoreboardController(IScoreboard scoreboard)
        {
            this.scoreboard = scoreboard;
            // RedPoints = new ReactiveProperty<int>();
            // YellowPoints = new ReactiveProperty<int>();
            // GreenPoints = new ReactiveProperty<int>();
            // BluePoints = new ReactiveProperty<int>();
            // TotalPoints = new ReactiveProperty<int>();
            // ErrorPoints = new ReactiveProperty<int>();
            IsActiveTurn = new ReactiveProperty<bool>();
            ThisTurnEnded = new ReactiveProperty<bool>();
            CurrentWhiteDiceSum = 0;
            CurrentScorePossibilities = new ScorePossibilities();
        }

        // IReadOnlyReactiveProperty<int> IScoreboardController.RedPoints => RedPoints;
        // IReadOnlyReactiveProperty<int> IScoreboardController.YellowPoints => YellowPoints;
        // IReadOnlyReactiveProperty<int> IScoreboardController.GreenPoints => GreenPoints;
        // IReadOnlyReactiveProperty<int> IScoreboardController.BluePoints => BluePoints;
        // IReadOnlyReactiveProperty<int> IScoreboardController.ErrorPoints => ErrorPoints;
        // IReadOnlyReactiveProperty<int> IScoreboardController.TotalPoints => TotalPoints;
        IReadOnlyReactiveProperty<bool> IScoreboardController.IsActiveTurn => IsActiveTurn;
        IReadOnlyReactiveProperty<bool> IScoreboardController.ThisTurnEnded => ThisTurnEnded;

        public void AddCross(SlotColor color)
        {
            switch (color)
            {
                case SlotColor.Red:
                    amountOfRedCrosses++;
                    scoreboard.SetRedPoints(ScoreType.Red, ConvertAmountOfCrossesToPoints(amountOfRedCrosses));
                    break;
                case SlotColor.Yellow:
                    amountOfYellowCrosses++;
                    scoreboard.SetRedPoints(ScoreType.Yellow, ConvertAmountOfCrossesToPoints(amountOfYellowCrosses));
                    break;
                case SlotColor.Green:
                    amountOfGreenCrosses++;
                    scoreboard.SetRedPoints(ScoreType.Green, ConvertAmountOfCrossesToPoints(amountOfGreenCrosses));
                    break;
                case SlotColor.Blue:
                    amountOfBlueCrosses++;
                    scoreboard.SetRedPoints(ScoreType.Blue, ConvertAmountOfCrossesToPoints(amountOfBlueCrosses));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }

            UpdateTotalPoints();
        }

        public void AddError()
        {
            AmountOfErrors++;
            scoreboard.SetRedPoints(ScoreType.Error, AmountOfErrors * 5); // todo get errorSingularScore from config
            UpdateTotalPoints();
            if (AmountOfErrors >= 4)
            {
                Debug.Log("game over, implement handling later");
            }
        }

        public void StartTurn(ScorePossibilities scorePossibilities, bool activeTurn)
        {
            if (activeTurn)
            {
                CurrentScorePossibilities = scorePossibilities;
                CurrentWhiteDiceSum = scorePossibilities.WhiteDiceSum;
            }
            else
            {
                CurrentScorePossibilities = new ScorePossibilities();
                CurrentWhiteDiceSum = scorePossibilities.WhiteDiceSum;
            }
            ThisTurnEnded.Value = false;
            IsActiveTurn.Value = activeTurn;
        }

        public void EndTurn()
        {
            Debug.Log("turn ended");
            ThisTurnEnded.Value = true;
        }

        private void UpdateTotalPoints()
        {
            var totalPoints = scoreboard.RedPoints.Value + scoreboard.YellowPoints.Value +
                              scoreboard.GreenPoints.Value + scoreboard.BluePoints.Value -
                              scoreboard.ErrorPoints.Value;
            scoreboard.SetRedPoints(ScoreType.Total, totalPoints);
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