using Dice;
using JetBrains.Annotations;
using Scoreboard;
using ScorePossibilities;
using Zenject;

namespace GameFlow
{
    public interface IGameFlowController
    {
        void StartGame();
        void GameOver();
    }

    [UsedImplicitly]
    public class GameFlowController : IGameFlowController
    {
        [Inject(Id = "Player")] private IScoreboardController playerScoreboardController;
        [Inject(Id = "AI")] private IScoreboardController aiScoreboardController;
        [Inject] private IDiceController diceController;

        [Inject] private IScorePossibilitiesController scorePossibilitiesController;

        private bool playersTurn;


        public void StartGame()
        {
            // this currently just starts a new turn, implement later to start game and different method to handle turns
            diceController.Roll();
            var scorePossibilities = diceController.GetScorePossibilities();
            scorePossibilitiesController.SetCurrentScorePossibilities(scorePossibilities);
            playersTurn = !playersTurn;
            playerScoreboardController.StartTurn(playersTurn);
            aiScoreboardController.StartTurn(!playersTurn);
        }

        public void GameOver()
        {
            throw new System.NotImplementedException();
        }
    }
}