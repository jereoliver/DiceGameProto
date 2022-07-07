using JetBrains.Annotations;
using Zenject;

namespace DiceGameProto
{
    public interface IGameFlowController
    {
        void StartGame();
        void GameOver();
    }
    [UsedImplicitly]
    public class GameFlowController : IGameFlowController
    {
        [Inject] private IScoreboardController scoreboardController; // todo implement later to have multiple scoreboards for each player, now it's just single player
        [Inject] private IDiceController diceController;
        private int turnIndex;
        
        // todo käytä reactivePropertyja ja subscribaa kaikkien ScoreBoardControllereiden ThisTurnEnded ja
        // kun kaikki on kutsuttu niin alota uusi vuoro
        
        public void StartGame()
        {
            // this currently just starts a new turn, implement later to start game and different method to handle turns
            diceController.Roll();
            var scorePossibilities = diceController.GetScorePossibilities();
            turnIndex++;
            scoreboardController.StartTurn(scorePossibilities, turnIndex % 2 != 0); // hack to change turn every other turn
        }

        public void GameOver()
        {
            throw new System.NotImplementedException();
        }
    }
}