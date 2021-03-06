namespace Scoreboard.AI
{
    public class AISlot
    {
        public readonly SlotColor SlotColor;
        public readonly int Number;
        public readonly bool IsLastSlot;
        public readonly bool AscendingNumbers;
        public SlotState CurrentSlotState;

        public AISlot(SlotColor slotColor, int number, bool isLastSlot, bool ascendingNumbers,
            SlotState currentSlotState)
        {
            SlotColor = slotColor;
            Number = number;
            IsLastSlot = isLastSlot;
            AscendingNumbers = ascendingNumbers;
            CurrentSlotState = currentSlotState;
        }
    }

    public class AISlotColorNumberPair
    {
        public SlotColor SlotColor;
        public int Number;

        public AISlotColorNumberPair(SlotColor slotColor, int number)
        {
            SlotColor = slotColor;
            Number = number;
        }
    }
}