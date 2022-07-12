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
        IReadOnlyReactiveProperty<bool> IGameFlowController.GameIsOver => GameIsOver;

        public GameFlowController(SignalBus signalBus, IDiceController diceController,
            IScorePossibilitiesController scorePossibilitiesController)
        {
            this.signalBus = signalBus;
            this.diceController = diceController;
            this.scorePossibilitiesController = scorePossibilitiesController;
            GameIsOver = new ReactiveProperty<bool>();
        }

        public void Initialize()
        {
            playerScoreboardController.ThisTurnEnded.Subscribe(HandleTurnChanged);
            aiScoreboardController.ThisTurnEnded.Subscribe(HandleTurnChanged);
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

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void LockRow(LockRowSignal lockRowSignal)
        {
            rowsLocked++;
            if (rowsLocked >= 2)
            {
                QueueGameOverToNextTurn();
            }
        }
        
        private void QueueGameOverToNextTurn()
        {
            isGameOn = false;
        }

        private void StartNewTurn()
        {
            if (!isGameOn)
            {
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
            if (playersEndedTurn >= 2)
            {
                StartNewTurn();
            }
        }
    }
}