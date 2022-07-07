using System;
using System.Collections.Generic;
using System.Linq;
using Scoreboard;
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
        private bool currentlyOwnTurn = false;

        // todo change this when multiplayer and each scoreboardPresenter needs own ScoreboardController
        [Inject] private readonly IScoreboardController scoreboardController;
        [Inject] private readonly IScoreboardModel scoreboard;

        private void Awake()
        {
            SubscribeReactiveProperties();
            AggregateAllSlotButtons();
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
        }

        private void AggregateAllSlotButtons()
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

            foreach (var button in errorPresenters)
            {
                button.ErrorButton.onClick.AddListener(() => HandleAddingError(button));
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
                LockRow(slotPresenter.SlotColor);
            }
            else
            {
                SetAllPreviousSameColoredSlotsInactive(slotPresenter);
            }

            var isFromWhiteDice = scoreboardController.CurrentWhiteDiceSum == slotPresenter.Number;
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
            InitSlotsForNewTurn();
            var scorePossibilities = scoreboardController.CurrentScorePossibilities;
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
            InitSlotsForNewTurn();
            scoreboardController.EndTurn();
            endTurnButton.interactable = false;
        }

        private void SetAllPreviousSameColoredSlotsInactive(SlotPresenter slotPresenter)
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

        #endregion


        private void UpdateErrorPointsText(int pointsAmount)
        {
            errorPointsText.text = pointsAmount.ToString();
        }

        private void HandleTurnStarted(bool isOwnTurn)
        {
            InitSlotsForNewTurn();
            currentlyOwnTurn = isOwnTurn;
            ownTurnIndicator.SetActive(isOwnTurn);
            Debug.Log("it's now new turn " + DateTime.UtcNow + " and isOwnTurn is: " + isOwnTurn);
        //    Debug.Log($"it's now new turn DateTime.UtcNow and isOwnTurn is: {isOwnTurn}");


            var scorePossibilities = scoreboardController.CurrentScorePossibilities;
            UpdateErrorButtonsState(isOwnTurn);

            if (isOwnTurn)
            {
                OpenSlotsForWhite(scoreboardController.CurrentWhiteDiceSum);
                OpenSlotsForColor(SlotColor.Red, scorePossibilities.RedDiceSums);
                OpenSlotsForColor(SlotColor.Yellow, scorePossibilities.YellowDiceSums);
                OpenSlotsForColor(SlotColor.Green, scorePossibilities.GreenDiceSums);
                OpenSlotsForColor(SlotColor.Blue, scorePossibilities.BlueDiceSums);
            }
            else
            {
                OpenSlotsForWhite(scoreboardController.CurrentWhiteDiceSum);
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
                SetSlotAvailableIfUnavailable(slot);
            }
        }

        private void OpenSlotsForColor(SlotColor slotColor, List<int> diceSums)
        {
            foreach (var slot in slotPresenters.Where(t => t.SlotColor == slotColor && diceSums.Contains(t.Number)))
            {
                SetSlotAvailableIfUnavailable(slot);
            }
        }

        private static void SetSlotAvailableIfUnavailable(SlotPresenter slot)
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

        private void InitSlotsForNewTurn()
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

        private void UpdateTotalPointsText(int pointsAmount)
        {
            totalPointsText.text = pointsAmount.ToString();
        }

        private void LockRow(SlotColor slotColor)
        {
            // todo implement something reactive to GameController to tell every scoreboard that row is locked
            foreach (var slotPresenter in slotPresenters
                         .Where(slotPresenter => slotPresenter.SlotColor == slotColor && !slotPresenter.IsCrossed))
            {
                slotPresenter.SetSlotState(SlotState.Removed);
            }
        }
    }
}