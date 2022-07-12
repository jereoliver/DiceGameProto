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
    public class AIScoreboardController : IScoreboardController, IInitializable, IDisposable
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
            AddOneToAmountOfCrosses(color);
            switch (color)
            {
                case SlotColor.Red:
                    scoreboard.SetPoints(ScoreType.Red, amountOfRedCrosses.ConvertAmountOfCrossesToPoints());
                    break;
                case SlotColor.Yellow:
                    scoreboard.SetPoints(ScoreType.Yellow, amountOfYellowCrosses.ConvertAmountOfCrossesToPoints());
                    break;
                case SlotColor.Green:
                    scoreboard.SetPoints(ScoreType.Green, amountOfGreenCrosses.ConvertAmountOfCrossesToPoints());
                    break;
                case SlotColor.Blue:
                    scoreboard.SetPoints(ScoreType.Blue, amountOfBlueCrosses.ConvertAmountOfCrossesToPoints());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }

            UpdateTotalPoints();
        }

        private void AddOneToAmountOfCrosses(SlotColor slotColor)
        {
            if (slotColor == SlotColor.Red)
            {
                amountOfRedCrosses++;
                OpenLastSlotIfEnoughCrosses(slotColor, amountOfRedCrosses);
            }
            else if (slotColor == SlotColor.Yellow)
            {
                amountOfYellowCrosses++;
                OpenLastSlotIfEnoughCrosses(slotColor, amountOfYellowCrosses);
            }
            else if (slotColor == SlotColor.Green)
            {
                amountOfGreenCrosses++;
                OpenLastSlotIfEnoughCrosses(slotColor, amountOfGreenCrosses);
            }
            else if (slotColor == SlotColor.Blue)
            {
                amountOfBlueCrosses++;
                OpenLastSlotIfEnoughCrosses(slotColor, amountOfBlueCrosses);
            }
        }

        private void OpenLastSlotIfEnoughCrosses(SlotColor slotColor, int crossesAmount)
        {
            if (crossesAmount < 4)
            {
                return;
            }

            var slotToOpen = slotColor switch
            {
                SlotColor.Red => CurrentSlotsState.RedSlots.FirstOrDefault(t => t.IsLastSlot),
                SlotColor.Yellow => CurrentSlotsState.YellowSlots.FirstOrDefault(t => t.IsLastSlot),
                SlotColor.Green => CurrentSlotsState.GreenSlots.FirstOrDefault(t => t.IsLastSlot),
                SlotColor.Blue => CurrentSlotsState.BlueSlots.FirstOrDefault(t => t.IsLastSlot),
                _ => throw new ArgumentOutOfRangeException(nameof(slotColor), slotColor, null)
            };

            if (slotToOpen != null)
            {
                slotToOpen.CurrentSlotState = SlotState.UnavailableByScore;
            }
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

            EndTurn();
        }

        public void StartTurn(bool activeTurn)
        {
            StartTurnAsync(activeTurn).Forget();
        }

        private async UniTask StartTurnAsync(bool activeTurn)
        {
            IsActiveTurn.Value = activeTurn;
            ThisTurnEnded.Value = false;
            await UniTask.Delay(TimeSpan.FromSeconds(2)); // mock waiting time for AI player
            if (activeTurn)
            {
                EvaluateFirstActionForOwnTurn();
            }
            else
            {
                EvaluateActionForOthersTurn();
            }
        }

        private void EvaluateFirstActionForOwnTurn()
        {
            bool TryToFindFindBestAction(int possibleGaps)
            {
                var commonSlotsWithoutGap = GetSlotsToCrossWithCommonSum(possibleGaps);
                if (commonSlotsWithoutGap.Count > 0)
                {
                    CrossASlot(commonSlotsWithoutGap[0]);
                    EvaluateSecondActionForOwnTurn();
                    return true;
                }

                // Option 2 for evaluation 1
                var colorSlotsWithoutGap = GetColorSlotsToCross(possibleGaps);

                if (colorSlotsWithoutGap.Count < 0)
                {
                    CrossASlot(colorSlotsWithoutGap[0]);
                    EndTurn();
                    return true;
                }

                return false;
            }

            if (TryToFindFindBestAction(possibleGaps: 1))
            {
                return;
            }

            if (TryToFindFindBestAction(possibleGaps: 2))
            {
                return;
            }

            if (TryToFindFindBestAction(possibleGaps: 3))
            {
                return;
            }

            if (TryToFindFindBestAction(possibleGaps: 4))
            {
                return;
            }

            AddError();
        }

        private void EvaluateSecondActionForOwnTurn()
        {
            // now crosses color sum as well if one founded without gaps, other way just ends turn
            var colorSlotsWithoutGap = GetColorSlotsToCross(0);

            if (colorSlotsWithoutGap.Count < 0)
            {
                CrossASlot(colorSlotsWithoutGap[0]);
            }

            EndTurn();
        }

        private void EvaluateActionForOthersTurn()
        {
            // todo check first if slot without gap
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

        private List<AISlotColorNumberPair> GetColorSlotsToCross(int possibleGaps)
        {
            var scorePossibilities = scorePossibilitiesController.CurrentScorePossibilities;

            var possibleSlotsToCross = new List<AISlotColorNumberPair>();

            AddPossibleSlotsByColor(scorePossibilities.RedDiceSums, CurrentSlotsState.RedSlots);
            AddPossibleSlotsByColor(scorePossibilities.YellowDiceSums, CurrentSlotsState.YellowSlots);
            AddPossibleSlotsByColor(scorePossibilities.GreenDiceSums, CurrentSlotsState.GreenSlots);
            AddPossibleSlotsByColor(scorePossibilities.BlueDiceSums, CurrentSlotsState.BlueSlots);

            return possibleSlotsToCross;

            void AddPossibleSlotsByColor(List<int> colorScorePossibilities, List<AISlot> currentColorSlots)
            {
                foreach (var slot in currentColorSlots.Where(t =>
                             t.CurrentSlotState == SlotState.UnavailableByScore))
                {
                    foreach (var scorePossibility in colorScorePossibilities)
                    {
                        if (slot.Number != scorePossibility)
                        {
                            continue;
                        }

                        var newPossibleSlot = new AISlotColorNumberPair(slot.SlotColor, slot.Number);
                        var indexOfSlot = CurrentSlotsState.GetIndexWithColorNumberPair(newPossibleSlot);
                        var doesNotLeaveTooBigGap = indexOfSlot > lastCrosses[slot.SlotColor] &&
                                                    indexOfSlot <= lastCrosses[slot.SlotColor] + possibleGaps + 1;
                        if (doesNotLeaveTooBigGap)
                        {
                            possibleSlotsToCross.Add(newPossibleSlot);
                        }
                    }
                }
            }
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

            // check if last then cross lock slot and lock the row
            var isLastSlot = CheckIfSlotIsLast();

            if (isLastSlot)
            {
                // todo cross lock slot to get points and visualization
                FireLockRowSignal(colorNumberPair.SlotColor);
            }

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

            bool CheckIfSlotIsLast()
            {
                if (colorNumberPair.SlotColor == SlotColor.Red || colorNumberPair.SlotColor == SlotColor.Yellow)
                {
                    if (colorNumberPair.Number == 12)
                    {
                        return true;
                    }
                }
                else
                {
                    if (colorNumberPair.Number == 2)
                    {
                        return true;
                    }
                }

                return false;
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

        public void Initialize()
        {
            signalBus.Subscribe<LockRowSignal>(LockRow);
        }

        public void Dispose()
        {
            signalBus.TryUnsubscribe<LockRowSignal>(LockRow);
        }

        private void LockRow(LockRowSignal lockRowSignal)
        {
            Debug.Log("row locked for AI as well");
            var color = lockRowSignal.ColorToLock;
            var slotsToLock = color switch
            {
                SlotColor.Red => CurrentSlotsState.RedSlots.Where(t => t.CurrentSlotState != SlotState.Crossed)
                    .ToList(),
                SlotColor.Yellow => CurrentSlotsState.YellowSlots.Where(t => t.CurrentSlotState != SlotState.Crossed)
                    .ToList(),
                SlotColor.Green => CurrentSlotsState.GreenSlots.Where(t => t.CurrentSlotState != SlotState.Crossed)
                    .ToList(),
                SlotColor.Blue => CurrentSlotsState.BlueSlots.Where(t => t.CurrentSlotState != SlotState.Crossed)
                    .ToList(),
                _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
            };

            foreach (var slot in slotsToLock)
            {
                slot.CurrentSlotState = SlotState.Removed;
            }
        }
        
    }
}