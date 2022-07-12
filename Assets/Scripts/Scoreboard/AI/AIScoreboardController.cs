using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DiceGame;
using GameFlow.Signals;
using JetBrains.Annotations;
using ScorePossibilities;
using UniRx;
using UnityEngine;
using Zenject;

namespace Scoreboard.AI
{
    [UsedImplicitly]
    public class AIScoreboardController : IScoreboardController
    {
        private IReactiveProperty<bool> IsActiveTurn { get; }
        private IReactiveProperty<bool> ThisTurnEnded { get; }

        public AISlotsModel CurrentSlotsState { get; private set; }

        [Inject(Id = "AI")] private IScoreboardModel scoreboard;
        private readonly SignalBus signalBus;
        private readonly IScorePossibilitiesController scorePossibilitiesController;

        private int amountOfRedCrosses;
        private int amountOfYellowCrosses;
        private int amountOfGreenCrosses;
        private int amountOfBlueCrosses;
        private int amountOfErrors;

        private Dictionary<SlotColor, int> lastCrosses;

        IReadOnlyReactiveProperty<bool> IScoreboardController.IsActiveTurn => IsActiveTurn;
        IReadOnlyReactiveProperty<bool> IScoreboardController.ThisTurnEnded => ThisTurnEnded;

        public AIScoreboardController(SignalBus signalBus, IScorePossibilitiesController scorePossibilitiesController)
        {
            this.signalBus = signalBus;
            this.scorePossibilitiesController = scorePossibilitiesController;
            IsActiveTurn = new ReactiveProperty<bool>();
            ThisTurnEnded = new ReactiveProperty<bool>();
            CurrentSlotsState = new AISlotsModel();
            lastCrosses = new Dictionary<SlotColor, int>
            {
                {SlotColor.Red, -1},
                {SlotColor.Yellow, -1},
                {SlotColor.Green, -1},
                {SlotColor.Blue, -1}
            };
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
            StartTurnAsync(activeTurn).Forget();
        }

        private async UniTask StartTurnAsync(bool activeTurn)
        {
            ThisTurnEnded.Value = false;
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            // todo implement playing logic here
            if (activeTurn)
            {
                EvaluateActionForOwnTurn();
            }
            else
            {
                EvaluateActionForOthersTurn();
            }
        }

        private void EvaluateActionForOwnTurn()
        {
            // do something on own turn
            var scorePossibilities = scorePossibilitiesController.CurrentScorePossibilities;
            // combine those with AICrossesModel
            // calculate what should do
            // choices are: white, white+color, color, error
            EndTurn(); // now just always end own turn right away
        }

        private void EvaluateActionForOthersTurn()
        {
            var possibleSlotsToCross = GetSlotsToCrossWithCommonSum(1);

            if (possibleSlotsToCross.Count == 1)
            {
                CrossASlot(possibleSlotsToCross[0]);
            }
            else if (possibleSlotsToCross.Count > 1)
            {
                CrossASlot(possibleSlotsToCross[0]); // todo check best option
            }

            EndTurn();
        }

        private List<AISlotColorNumberPair> GetSlotsToCrossWithCommonSum(int possibleGaps)
        {
            var whiteDiceSum = scorePossibilitiesController.CurrentWhiteDiceSum;
            var possibleSlotsToCrossWithCommonSum = new List<AISlotColorNumberPair>();

            foreach (var slot in CurrentSlotsState.GetAllSlots().Where(t =>
                         t.CurrentSlotState == SlotState.UnavailableByScore && t.Number == whiteDiceSum))
            {
                var newPossibleSlot = new AISlotColorNumberPair(slot.SlotColor, slot.Number);
                // add to list only if one slot to left is crossed or one after that 

                var indexOfSlot = CurrentSlotsState.GetIndexWithColorNumberPair(newPossibleSlot);
                var maximumGapIsOneSlot = indexOfSlot > lastCrosses[slot.SlotColor] &&
                                          indexOfSlot <= lastCrosses[slot.SlotColor] + possibleGaps + 1;
                if (maximumGapIsOneSlot)
                {
                    possibleSlotsToCrossWithCommonSum.Add(newPossibleSlot);
                }
            }

            return possibleSlotsToCrossWithCommonSum;
        }

        public void EndTurn()
        {
            Debug.Log("AI turn ended");
            ThisTurnEnded.Value = true;
        }

        private void CrossASlot(AISlotColorNumberPair colorNumberPair)
        {
            // todo add automated lock slot crossing when crossing last slot
            UpdateCurrentSlotState();

            // call AddCross to increment score in AIScoreboardModel
            AddCross(colorNumberPair.SlotColor);
            LockPreviousSlots(colorNumberPair);

            void UpdateCurrentSlotState()
            {
                switch (colorNumberPair.SlotColor)
                {
                    case SlotColor.Red:
                    {
                        foreach (var slot in CurrentSlotsState.RedSlots.Where(t => t.Number == colorNumberPair.Number))
                        {
                            slot.CurrentSlotState = SlotState.Crossed;
                        }

                        break;
                    }
                    case SlotColor.Yellow:
                    {
                        foreach (var slot in CurrentSlotsState.YellowSlots.Where(
                                     t => t.Number == colorNumberPair.Number))
                        {
                            slot.CurrentSlotState = SlotState.Crossed;
                        }

                        break;
                    }
                    case SlotColor.Green:
                    {
                        foreach (var slot in
                                 CurrentSlotsState.GreenSlots.Where(t => t.Number == colorNumberPair.Number))
                        {
                            slot.CurrentSlotState = SlotState.Crossed;
                        }

                        break;
                    }
                    case SlotColor.Blue:
                    {
                        foreach (var slot in CurrentSlotsState.BlueSlots.Where(t => t.Number == colorNumberPair.Number))
                        {
                            slot.CurrentSlotState = SlotState.Crossed;
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(colorNumberPair.Number), colorNumberPair.SlotColor,
                            null);
                }
            }
        }

        private void LockPreviousSlots(AISlotColorNumberPair colorNumberPair)
        {
            lastCrosses[colorNumberPair.SlotColor] = CurrentSlotsState.GetIndexWithColorNumberPair(colorNumberPair);
        }

        private void FireLockRowSignal(SlotColor slotColor)
        {
            signalBus.Fire(new LockRowSignal(slotColor));
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