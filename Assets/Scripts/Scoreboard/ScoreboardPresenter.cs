using System;
using System.Collections.Generic;
using System.Linq;
using GameFlow.Signals;
using ScorePossibilities;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scoreboard
{
    public class ScoreboardPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text redPointsText;
        [SerializeField] private TMP_Text yellowPointsText;
        [SerializeField] private TMP_Text greenPointsText;
        [SerializeField] private TMP_Text bluePointsText;
        [SerializeField] private TMP_Text totalPointsText;
        [SerializeField] private TMP_Text errorPointsText;
        [SerializeField] private GameObject slotsParent;
        [SerializeField] private GameObject ownTurnIndicator;
        [SerializeField] private Button endTurnButton;
        [SerializeField] private List<ErrorPresenter> errorPresenters;

        private readonly List<SlotPresenter> slotPresenters = new List<SlotPresenter>();
        private bool currentlyOwnTurn;

        [Inject(Id = "Player")] private IScoreboardController scoreboardController;
        [Inject(Id = "Player")] private readonly IScoreboardModel scoreboard;
        [Inject] private readonly IScorePossibilitiesController scorePossibilitiesController;
        [Inject] private SignalBus signalBus;

        private void Awake()
        {
            SubscribeReactiveProperties();
            AggregateAllSlotPresenters();
            SetupButtons();
        }

        private void SubscribeReactiveProperties()
        {
            scoreboard.RedPoints.Subscribe(UpdateRedPointsText).AddTo(gameObject);
            scoreboard.YellowPoints.Subscribe(UpdateYellowPointsText).AddTo(gameObject);
            scoreboard.GreenPoints.Subscribe(UpdateGreenPointsText).AddTo(gameObject);
            scoreboard.BluePoints.Subscribe(UpdateBluePointsText).AddTo(gameObject);
            scoreboard.TotalPoints.Subscribe(UpdateTotalPointsText).AddTo(gameObject);
            scoreboard.ErrorPoints.Subscribe(UpdateErrorPointsText).AddTo(gameObject);
            scoreboardController.IsActiveTurn.Subscribe(HandleTurnStarted).AddTo(gameObject);
            signalBus.Subscribe<LockRowSignal>(LockRow);
        }

        private void AggregateAllSlotPresenters()
        {
            foreach (Transform slots in slotsParent.transform)
            foreach (Transform slot in slots)
            {
                slotPresenters.Add(slot.GetComponent<SlotPresenter>());
            }
        }

        private void SetupButtons()
        {
            foreach (var slot in slotPresenters)
            {
                var button = slot.GetComponent<Button>();
                button.onClick.AddListener(() =>
                    HandleAddingCross(slot));
            }

            foreach (var errorPresenter in errorPresenters)
            {
                errorPresenter.ErrorButton.onClick.AddListener(() => HandleAddingError(errorPresenter));
            }

            endTurnButton.onClick.AddListener(EndTurn);
            endTurnButton.interactable = false;
        }

        private void HandleAddingCross(SlotPresenter slotPresenter)
        {
            scoreboardController.AddCross(slotPresenter.SlotColor);
            if (slotPresenter.IsLastSlot)
            {
                CrossLockSlot(slotPresenter.SlotColor);
                scoreboardController.AddCross(slotPresenter.SlotColor);
                signalBus.Fire(new LockRowSignal(slotPresenter.SlotColor));
            }
            else
            {
                CloseSlotsOnLeftSide(slotPresenter);
            }

            var isFromWhiteDice = scorePossibilitiesController.CurrentWhiteDiceSum == slotPresenter.Number;
            if (isFromWhiteDice && currentlyOwnTurn)
            {
                HandleCrossingWhiteSumOnOwnTurn();
            }
            else
            {
                EndTurn();
            }
        }

        private void HandleCrossingWhiteSumOnOwnTurn()
        {
            endTurnButton.interactable = true;
            UpdateErrorButtonsState(areUncrossedInteractable: false);
            ResetSlotsForNewTurn();
            var scorePossibilities = scorePossibilitiesController.CurrentScorePossibilities;
            OpenSlotsForColor(SlotColor.Red, scorePossibilities.RedDiceSums);
            OpenSlotsForColor(SlotColor.Yellow, scorePossibilities.YellowDiceSums);
            OpenSlotsForColor(SlotColor.Green, scorePossibilities.GreenDiceSums);
            OpenSlotsForColor(SlotColor.Blue, scorePossibilities.BlueDiceSums);
        }

        private void HandleAddingError(ErrorPresenter errorPresenter)
        {
            scoreboardController.AddError();
            errorPresenter.SetErrorButtonState(ErrorButtonState.Crossed);
            EndTurn();
        }

        private void EndTurn()
        {
            UpdateErrorButtonsState(areUncrossedInteractable: false);
            endTurnButton.interactable = false;
            ResetSlotsForNewTurn();
            scoreboardController.EndTurn();
        }

        private void CloseSlotsOnLeftSide(SlotPresenter slotPresenter)
        {
            // set all previous same colored slots inactive
            foreach (var slot in slotPresenters)
            {
                // skip slot if it's not same color, lockSlot or already locked
                if (slot.SlotColor != slotPresenter.SlotColor || slot.IsLockSlot ||
                    slot.IsCrossed)
                {
                    continue;
                }

                if (slotPresenter.AscendingNumbers && slot.Number < slotPresenter.Number ||
                    !slotPresenter.AscendingNumbers && slot.Number > slotPresenter.Number)
                {
                    slot.SetSlotState(SlotState.Removed);
                }
            }
        }

        private void CrossLockSlot(SlotColor slotColor)
        {
            foreach (var slotPresenter in slotPresenters.Where(t => t.SlotColor == slotColor && t.IsLockSlot))
            {
                slotPresenter.SetCrossed();
            }
        }

        #region UpdatePointsTexts

        private void UpdateRedPointsText(int pointsAmount)
        {
            HandleUpdatedPoints(pointsAmount, SlotColor.Red);
        }

        private void UpdateYellowPointsText(int pointsAmount)
        {
            HandleUpdatedPoints(pointsAmount, SlotColor.Yellow);
        }

        private void UpdateGreenPointsText(int pointsAmount)
        {
            HandleUpdatedPoints(pointsAmount, SlotColor.Green);
        }

        private void UpdateBluePointsText(int pointsAmount)
        {
            HandleUpdatedPoints(pointsAmount, SlotColor.Blue);
        }

        private void HandleUpdatedPoints(int pointsAmount, SlotColor slotColor)
        {
            UpdatePointsText(pointsAmount, slotColor);

            if (pointsAmount >= 10)
            {
                SetLastSlotAvailable(slotColor);
            }
        }

        private void UpdatePointsText(int pointsAmount, SlotColor slotColor)
        {
            switch (slotColor)
            {
                case SlotColor.Red:
                    redPointsText.text = pointsAmount.ToString();
                    break;
                case SlotColor.Yellow:
                    yellowPointsText.text = pointsAmount.ToString();
                    break;
                case SlotColor.Green:
                    greenPointsText.text = pointsAmount.ToString();
                    break;
                case SlotColor.Blue:
                    bluePointsText.text = pointsAmount.ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(slotColor), slotColor, null);
            }
        }
        
        private void UpdateTotalPointsText(int pointsAmount)
        {
            totalPointsText.text = pointsAmount.ToString();
        }

        private void UpdateErrorPointsText(int amount)
        {
            errorPointsText.text = amount.ToString();
        }
        #endregion
        
        private void HandleTurnStarted(bool isOwnTurn)
        {
            ResetSlotsForNewTurn();
            currentlyOwnTurn = isOwnTurn;
            ownTurnIndicator.SetActive(currentlyOwnTurn);
            Debug.Log("it's now new turn and isOwnTurn is: " + isOwnTurn);

            UpdateErrorButtonsState(isOwnTurn);

            if (isOwnTurn)
            {
                var scorePossibilities = scorePossibilitiesController.CurrentScorePossibilities;
                OpenSlotsForWhite(scorePossibilitiesController.CurrentWhiteDiceSum);
                OpenSlotsForColor(SlotColor.Red, scorePossibilities.RedDiceSums);
                OpenSlotsForColor(SlotColor.Yellow, scorePossibilities.YellowDiceSums);
                OpenSlotsForColor(SlotColor.Green, scorePossibilities.GreenDiceSums);
                OpenSlotsForColor(SlotColor.Blue, scorePossibilities.BlueDiceSums);
            }
            else
            {
                OpenSlotsForWhite(scorePossibilitiesController.CurrentWhiteDiceSum);
                endTurnButton.interactable = true;
            }
        }

        private void UpdateErrorButtonsState(bool areUncrossedInteractable)
        {
            foreach (var errorPresenter in errorPresenters.Where(t => t.CurrentState != ErrorButtonState.Crossed))
            {
                errorPresenter.SetErrorButtonState(areUncrossedInteractable
                    ? ErrorButtonState.Interactable
                    : ErrorButtonState.NonInteractable);
            }
        }

        private void OpenSlotsForWhite(int whiteDiceSum)
        {
            foreach (var slot in slotPresenters.Where(slot => slot.Number == whiteDiceSum))
            {
                SetSlotAvailableByScore(slot);
            }
        }

        private void OpenSlotsForColor(SlotColor slotColor, List<int> diceSums)
        {
            foreach (var slot in slotPresenters.Where(t => t.SlotColor == slotColor && diceSums.Contains(t.Number)))
            {
                SetSlotAvailableByScore(slot);
            }
        }

        private static void SetSlotAvailableByScore(SlotPresenter slot)
        {
            if (slot.CurrentSlotState == SlotState.UnavailableByScore)
                slot.SetSlotState(SlotState.Available);
        }

        private void SetLastSlotAvailable(SlotColor slotColor)
        {
            var lastSlot = slotPresenters.First(t =>
                t.IsLastSlot &&
                t.SlotColor == slotColor);
            // set last slot available only if it is currently unavailableYetByRules to prevent setting it always and multiple times available
            if (lastSlot.CurrentSlotState == SlotState.UnavailableYetByRules)
            {
                lastSlot.SetSlotState(SlotState.Available);
            }
        }

        private void ResetSlotsForNewTurn()
        {
            if (slotPresenters == null)
            {
                return;
            }

            foreach (var slot in slotPresenters.Where(t => t.CurrentSlotState == SlotState.Available))
            {
                slot.SetSlotState(SlotState.UnavailableByScore);
            }
        }
        

        private void LockRow(LockRowSignal lockRowSignal)
        {
            foreach (var slotPresenter in slotPresenters.Where(slotPresenter =>
                         slotPresenter.SlotColor == lockRowSignal.ColorToLock && !slotPresenter.IsCrossed))
            {
                slotPresenter.SetSlotState(SlotState.Removed);
            }
        }
    }
}