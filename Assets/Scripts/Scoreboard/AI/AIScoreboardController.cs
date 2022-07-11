using GameFlow.Signals;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Zenject;

namespace Scoreboard.AI
{
    public interface IAIScoreboardController
    {
        AICrossesModel CurrentState { get; }
    }
    [UsedImplicitly]
    public class AIScoreboardController : IScoreboardController, IAIScoreboardController
    {
        private IReactiveProperty<bool> IsActiveTurn { get; }
        private IReactiveProperty<bool> ThisTurnEnded { get; }

        public AICrossesModel CurrentState { get; private set; }

        [Inject(Id = "AI")] private IScoreboardModel scoreboard;
        private readonly SignalBus signalBus;


        IReadOnlyReactiveProperty<bool> IScoreboardController.IsActiveTurn => IsActiveTurn;
        IReadOnlyReactiveProperty<bool> IScoreboardController.ThisTurnEnded => ThisTurnEnded;

        public AIScoreboardController(SignalBus signalBus)
        {
            this.signalBus = signalBus;
            IsActiveTurn = new ReactiveProperty<bool>();
            ThisTurnEnded = new ReactiveProperty<bool>();
            CurrentState = new AICrossesModel();
        }

        public void AddCross(SlotColor color)
        {
            CurrentState.BlueSlots[3].CurrentSlotState = SlotState.Crossed;
            EndTurn();
        }


        public void AddError()
        {
            // if (amountOfErrors >= 4)
            // {
            //     signalBus.Fire(new GameOverSignal());
            // }
        }

        public void StartTurn(bool activeTurn)
        {
            ThisTurnEnded.Value = false;
            // todo implement playing logic here

            EndTurn(); // now just always end own turn right away
        }

        public void EndTurn()
        {
            Debug.Log("AI turn ended");
            ThisTurnEnded.Value = true;
        }

        public void LockRow(SlotColor slotColor)
        {
            signalBus.Fire(new LockRowSignal(slotColor));
        }
    }
}