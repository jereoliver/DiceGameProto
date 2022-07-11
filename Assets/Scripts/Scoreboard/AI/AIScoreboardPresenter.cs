using System;
using System.Collections.Generic;
using System.Linq;
using GameFlow.Signals;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Scoreboard.AI
{
    public class AIScoreboardPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text redPointsText;
        [SerializeField] private TMP_Text yellowPointsText;
        [SerializeField] private TMP_Text greenPointsText;
        [SerializeField] private TMP_Text bluePointsText;
        [SerializeField] private TMP_Text totalPointsText;
        [SerializeField] private TMP_Text errorPointsText;
        [SerializeField] private GameObject slotsParent;
        [SerializeField] private GameObject ownTurnIndicator;
        [SerializeField] private List<ErrorPresenter> errorPresenters;

        private readonly List<AISlotPresenter> slotPresenters = new List<AISlotPresenter>();
        private bool currentlyOwnTurn;

        [Inject(Id = "AI")] private IScoreboardController scoreboardController;
        [Inject] private IAIScoreboardController aiScoreboardController;
        [Inject(Id = "AI")] private IScoreboardModel scoreboard;
        [Inject] private SignalBus signalBus;

        private void Awake()
        {
            AggregateAllSlotPresenters();
            SubscribeReactiveProperties();
        }

        private void AggregateAllSlotPresenters()
        {
            foreach (Transform slots in slotsParent.transform)
            foreach (Transform slot in slots)
            {
                slotPresenters.Add(slot.GetComponent<AISlotPresenter>());
            }
        }

        private void SubscribeReactiveProperties()
        {
            scoreboard.TotalPoints.Subscribe(UpdateTotalPointsText).AddTo(gameObject);
            scoreboard.RedPoints.Subscribe(UpdateRedPointsText).AddTo(gameObject);
            scoreboard.YellowPoints.Subscribe(UpdateYellowPointsText).AddTo(gameObject);
            scoreboard.GreenPoints.Subscribe(UpdateGreenPointsText).AddTo(gameObject);
            scoreboard.BluePoints.Subscribe(UpdateBluePointsText).AddTo(gameObject);
            scoreboard.ErrorPoints.Subscribe(HandleErrorPointsChanged).AddTo(gameObject);
            scoreboardController.IsActiveTurn.Subscribe(HandleTurnStarted).AddTo(gameObject);
            scoreboardController.ThisTurnEnded.Subscribe(HandleTurnEnded).AddTo(gameObject);
            signalBus.Subscribe<LockRowSignal>(LockRow);
        }

        #region UpdatePointsTexts
        
        private void UpdateTotalPointsText(int pointsAmount)
        {
            totalPointsText.text = pointsAmount.ToString();
        }

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
        
        private void HandleErrorPointsChanged(int amount)
        {
            // todo add check for amount and use that to visualize errors
            errorPointsText.text = amount.ToString();
            var amountOfCrosses = amount / 5;
            for (var i = 0; i < amountOfCrosses; i++)
            {
                errorPresenters[i].SetErrorButtonState(ErrorButtonState.Crossed);
                // todo check that math works
            }
        }

        private void HandleTurnStarted(bool isOwnTurn)
        {
            currentlyOwnTurn = isOwnTurn;
            ownTurnIndicator.SetActive(currentlyOwnTurn);
            Debug.Log("it's now AI's new turn and isOwnTurn is: " + isOwnTurn);
            
        }

        private void HandleTurnEnded(bool turnEnded)
        {
            if (!turnEnded)
            {
                return;
            }

            UpdateSlotPresenters();
        }

        private void UpdateSlotPresenters()
        {
            // update slotPresenters to reflect AICrossesModel
            var status = aiScoreboardController.CurrentState;
            foreach (var slot in status.RedSlots.Where(t => t.CurrentSlotState == SlotState.Crossed))
            foreach (var slotPresenter in slotPresenters.Where(t =>
                         t.Number == slot.Number && t.SlotColor == slot.SlotColor)) // todo more checks?
            {
                slotPresenter.SetCrossedState(true);
            }
        }

        private void LockRow(LockRowSignal lockRowSignal)
        {
            // todo some own row lock implementation
            // todo 
        }
    }
}