using System;
using System.Collections.Generic;
using System.Linq;

namespace Scoreboard.AI
{
    // this class is used to hold data of scoreboard runtime status of AIPlayer
    // and to help AIScoreboardController to make game decisions
    public class AISlotsModel
    {
        public List<AISlot> RedSlots;
        public List<AISlot> YellowSlots;
        public List<AISlot> GreenSlots;
        public List<AISlot> BlueSlots;

        public List<AISlot> GetAllSlots()
        {
            var slots = RedSlots.ToList();
            slots.AddRange(YellowSlots);
            slots.AddRange(GreenSlots);
            slots.AddRange(BlueSlots);
            return slots;
        }

        public int GetIndexWithColorNumberPair(AISlotColorNumberPair colorNumberPair)
        {
            return colorNumberPair.SlotColor switch
            {
                SlotColor.Red => RedSlots.FindIndex(t => t.Number == colorNumberPair.Number),
                SlotColor.Yellow => YellowSlots.FindIndex(t => t.Number == colorNumberPair.Number),
                SlotColor.Green => GreenSlots.FindIndex(t => t.Number == colorNumberPair.Number),
                SlotColor.Blue => BlueSlots.FindIndex(t => t.Number == colorNumberPair.Number),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public AISlotsModel()
        {
            RedSlots = new List<AISlot>();
            YellowSlots = new List<AISlot>();
            GreenSlots = new List<AISlot>();
            BlueSlots = new List<AISlot>();

            for (var i = 2; i < 13; i++)
            {
                if (i == 12)
                {
                    var newRedSlot = new AISlot(SlotColor.Red, i, isLastSlot: true, ascendingNumbers: true,
                        SlotState.UnavailableYetByRules);
                    RedSlots.Add(newRedSlot);
                    var newYellowSlot = new AISlot(SlotColor.Yellow, i, isLastSlot: true, ascendingNumbers: true,
                        SlotState.UnavailableYetByRules);
                    YellowSlots.Add(newYellowSlot);
                }
                else
                {
                    var newRedSlot = new AISlot(SlotColor.Red, i, isLastSlot: false, ascendingNumbers: true,
                        SlotState.UnavailableByScore);
                    RedSlots.Add(newRedSlot);
                    var newYellowSlot = new AISlot(SlotColor.Yellow, i, isLastSlot: false, ascendingNumbers: true,
                        SlotState.UnavailableByScore);
                    YellowSlots.Add(newYellowSlot);
                }
            }

            for (var i = 12; i > 1; i--)
            {
                if (i == 2)
                {
                    var newGreenSlot = new AISlot(SlotColor.Green, i, isLastSlot: true, ascendingNumbers: false,
                        SlotState.UnavailableYetByRules);
                    GreenSlots.Add(newGreenSlot);
                    var newBlueSlot = new AISlot(SlotColor.Blue, i, isLastSlot: true, ascendingNumbers: false,
                        SlotState.UnavailableYetByRules);
                    BlueSlots.Add(newBlueSlot);
                }
                else
                {
                    var newGreenSlot = new AISlot(SlotColor.Green, i, isLastSlot: false, ascendingNumbers: false,
                        SlotState.UnavailableByScore);
                    GreenSlots.Add(newGreenSlot);
                    var newBlueSlot = new AISlot(SlotColor.Blue, i, isLastSlot: false, ascendingNumbers: false,
                        SlotState.UnavailableByScore);
                    BlueSlots.Add(newBlueSlot);
                }
            }
        }
    }
}