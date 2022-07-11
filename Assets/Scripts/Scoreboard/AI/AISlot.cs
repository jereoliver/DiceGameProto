namespace Scoreboard.AI
{
    public class AISlot
    {
        public SlotColor SlotColor;
        public int Number;
        public bool IsLastSlot;
        public bool AscendingNumbers;
        public SlotState CurrentSlotState;

        public AISlot(SlotColor slotColor, int number, bool isLastSlot, bool ascendingNumbers, SlotState currentSlotState)
        {
            SlotColor = slotColor;
            Number = number;
            IsLastSlot = isLastSlot;
            AscendingNumbers = ascendingNumbers;
            CurrentSlotState = currentSlotState;
        }
    }
}