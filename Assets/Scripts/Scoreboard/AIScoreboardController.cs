using Dice;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace Scoreboard
{
    [UsedImplicitly]
    public class AIScoreboardController : IScoreboardController
    {
        private IReactiveProperty<bool> IsActiveTurn { get; }
        private IReactiveProperty<bool> ThisTurnEnded { get; }
        public ScorePossibilities CurrentScorePossibilities { get; }
        public int CurrentWhiteDiceSum { get; }

        private IScoreboardModel scoreboard;


        IReadOnlyReactiveProperty<bool> IScoreboardController.IsActiveTurn => IsActiveTurn;
        IReadOnlyReactiveProperty<bool> IScoreboardController.ThisTurnEnded => ThisTurnEnded;

        public AIScoreboardController()
        {
            IsActiveTurn = new ReactiveProperty<bool>();
            ThisTurnEnded = new ReactiveProperty<bool>();
            CurrentWhiteDiceSum = 0;
            CurrentScorePossibilities = new ScorePossibilities();
            scoreboard = new ScoreboardModel();
        }

        public void AddCross(SlotColor color)
        {
            
        }


        public void AddError()
        {
            
        }

        public void StartTurn(ScorePossibilities scorePossibilities, bool activeTurn)
        {
            EndTurn(); // now just always end own turn right away
        }

        public void EndTurn()
        {
            Debug.Log("AI turn ended");
            ThisTurnEnded.Value = true;
        }
    }
}