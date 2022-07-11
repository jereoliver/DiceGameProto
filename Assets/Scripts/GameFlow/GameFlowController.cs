using System;
using Dice;
using GameFlow.Signals;
using JetBrains.Annotations;
using Scoreboard;
using ScorePossibilities;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace GameFlow
{
    public interface IGameFlowController
    {
        void StartGame();
        void Restart();
        IReadOnlyReactiveProperty<bool> GameIsOver { get; }
    }

    [UsedImplicitly]
    public class GameFlowController : IGameFlowController, IInitializable, IDisposable
    {
        [Inject(Id = "Player")] private IScoreboardController playerScoreboardController;
        [Inject(Id = "AI")] private IScoreboardController aiScoreboardController;
        private readonly IDiceController diceController;
        private readonly IScorePossibilitiesController scorePossibilitiesController;
        private readonly SignalBus signalBus;

        private bool playersTurn;
        private bool isGameOn;
        private int playersEndedTurn;
        private int rowsLocked;
        private ReactiveProperty<bool> GameIsOver { get; }

        public GameFlowController(SignalBus signalBus, IDiceController diceController,
            IScorePossibilitiesController scorePossibilitiesController)
        {
            this.signalBus = signalBus;
            this.diceController = diceController;
            this.scorePossibilitiesController = scorePossibilitiesController;
            GameIsOver = new ReactiveProperty<bool>();
        }

        IReadOnlyReactiveProperty<bool> IGameFlowController.GameIsOver => GameIsOver;

        public void Initialize()
        {
            playerScoreboardController.ThisTurnEnded.Subscribe(HandleTurnChanged);
            aiScoreboardController.ThisTurnEnded.Subscribe(HandleTurnChanged);
            Debug.Log("GameFlowController initialized and ThisTurnEnded subscribed");
            signalBus.Subscribe<GameOverSignal>(QueueGameOverToNextTurn);
            signalBus.Subscribe<LockRowSignal>(LockRow);
        }

        public void Dispose()
        {
            signalBus.TryUnsubscribe<GameOverSignal>(QueueGameOverToNextTurn);
            signalBus.TryUnsubscribe<LockRowSignal>(LockRow);
        }

        public void StartGame()
        {
            if (isGameOn)
            {
                Debug.LogWarning("Tried to start a game which is already on");
                return;
            }

            isGameOn = true;
            StartNewTurn();
        }


        private void QueueGameOverToNextTurn()
        {
            isGameOn = false;
            Debug.Log("Game over NEXT ROUND");
        }

        public void Restart()
        {
            SceneManager.LoadScene(0); // todo this could be prettier
        }

        private void LockRow(LockRowSignal lockRowSignal)
        {
            Debug.Log(lockRowSignal.ColorToLock + " row locked now");
            rowsLocked++;
            if (rowsLocked >= 2)
            {
                // game over
                QueueGameOverToNextTurn();
            }
        }

        private void StartNewTurn()
        {
            // check first if row should be locked now.
            if (!isGameOn)
            {
                Debug.Log("NOW game over");
                GameIsOver.Value = true;
            }

            playersEndedTurn = 0;
            diceController.Roll();
            var scorePossibilities = diceController.GetScorePossibilities();
            scorePossibilitiesController.SetCurrentScorePossibilities(scorePossibilities);
            playersTurn = !playersTurn;
            playerScoreboardController.StartTurn(playersTurn);
            aiScoreboardController.StartTurn(!playersTurn);
        }

        private void HandleTurnChanged(bool turnEnded)
        {
            if (!turnEnded)
            {
                return;
            }

            playersEndedTurn++;
            Debug.Log("one turn ended now and amount of playersEnded turn is: " + playersEndedTurn);
            if (playersEndedTurn >= 2)
            {
                StartNewTurn();
            }
        }
    }
}