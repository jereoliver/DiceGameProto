using Dice;
using JetBrains.Annotations;
using Scoreboard;
using ScorePossibilities;
using UniRx;
using UnityEngine;
using Zenject;

namespace GameFlow
{
    public interface IGameFlowController
    {
        void StartGame();
        void GameOver();
    }

    [UsedImplicitly]
    public class GameFlowController : IGameFlowController, IInitializable
    {
        [Inject(Id = "Player")] private IScoreboardController playerScoreboardController;
        [Inject(Id = "AI")] private IScoreboardController aiScoreboardController;
        [Inject] private IDiceController diceController;

        [Inject] private IScorePossibilitiesController scorePossibilitiesController;

        private bool playersTurn;
        private bool isGameOn;
        private int playersEndedTurn;

        // public GameFlowController()
        // {
        //     playerScoreboardController.ThisTurnEnded.Subscribe(HandleTurnChanged);
        // }
        public void Initialize()
        {
            playerScoreboardController.ThisTurnEnded.Subscribe(HandleTurnChanged);
            aiScoreboardController.ThisTurnEnded.Subscribe(HandleTurnChanged);
            Debug.Log("GameFlowController initialized and ThisTurnEnded subscribed");
        }
        public void StartGame()
        {
            if (isGameOn)
            {
                return;
            }
            
            StartNewTurn();
            isGameOn = true;
        }

        private void StartNewTurn()
        {
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

        public void GameOver()
        {
            throw new System.NotImplementedException();
        }
        
    }
}