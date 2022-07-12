using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ScorePossibilities;
using UniRx;

namespace Dice
{
    public interface IDiceController
    {
        IReadOnlyReactiveProperty<int> CurrentValueForWhite1 { get; }
        IReadOnlyReactiveProperty<int> CurrentValueForWhite2 { get; }
        IReadOnlyReactiveProperty<int> CurrentValueForRed { get; }
        IReadOnlyReactiveProperty<int> CurrentValueForYellow { get; }
        IReadOnlyReactiveProperty<int> CurrentValueForGreen { get; }
        IReadOnlyReactiveProperty<int> CurrentValueForBlue { get; }

        /// <summary>
        /// Roll the dice to get new values for them
        /// </summary>
        void Roll();
        /// <summary>
        /// Get current possibilities to score in scoreboard according to dices current values
        /// </summary>
        ScorePossibilitiesModel GetScorePossibilities();
    }

    [UsedImplicitly]
    public class DiceController : IDiceController
    {
        private IReactiveProperty<int> valueForWhite1 { get; }
        private IReactiveProperty<int> valueForWhite2 { get; }
        private IReactiveProperty<int> valueForRed { get; }
        private IReactiveProperty<int> valueForYellow { get; }
        private IReactiveProperty<int> valueForGreen { get; }
        private IReactiveProperty<int> valueForBlue { get; }

        public DiceController()
        {
            valueForWhite1 = new ReactiveProperty<int>();
            valueForWhite2 = new ReactiveProperty<int>();
            valueForRed = new ReactiveProperty<int>();
            valueForYellow = new ReactiveProperty<int>();
            valueForGreen = new ReactiveProperty<int>();
            valueForBlue = new ReactiveProperty<int>();
        }


        IReadOnlyReactiveProperty<int> IDiceController.CurrentValueForWhite1 => valueForWhite1;
        IReadOnlyReactiveProperty<int> IDiceController.CurrentValueForWhite2 => valueForWhite2;
        IReadOnlyReactiveProperty<int> IDiceController.CurrentValueForRed => valueForRed;
        IReadOnlyReactiveProperty<int> IDiceController.CurrentValueForYellow => valueForYellow;
        IReadOnlyReactiveProperty<int> IDiceController.CurrentValueForGreen => valueForGreen;
        IReadOnlyReactiveProperty<int> IDiceController.CurrentValueForBlue => valueForBlue;

        public void Roll()
        {
            var rnd = new Random();
            valueForWhite1.Value = rnd.Next(1, 7);
            valueForWhite2.Value = rnd.Next(1, 7);
            valueForRed.Value = rnd.Next(1, 7);
            valueForYellow.Value = rnd.Next(1, 7);
            valueForGreen.Value = rnd.Next(1, 7);
            valueForBlue.Value = rnd.Next(1, 7);
        }

        private int GetWhiteDiceSum()
        {
            var whiteDiceSum = valueForWhite1.Value + valueForWhite2.Value;
            return whiteDiceSum;
        }

        private List<int> GetRedDiceSums()
        {
            var redDiceSums = new List<int>
            {
                valueForWhite1.Value + valueForRed.Value,
                valueForWhite2.Value + valueForRed.Value
            };
            return redDiceSums;
        }

        private List<int> GetYellowDiceSums()
        {
            var yellowDiceSums = new List<int>
            {
                valueForWhite1.Value + valueForYellow.Value,
                valueForWhite2.Value + valueForYellow.Value
            };
            return yellowDiceSums;
        }

        private List<int> GetGreenDiceSums()
        {
            var greenDiceSums = new List<int>
            {
                valueForWhite1.Value + valueForGreen.Value,
                valueForWhite2.Value + valueForGreen.Value
            };
            return greenDiceSums;
        }

        private List<int> GetBlueDiceSums()
        {
            var blueDiceSums = new List<int>
            {
                valueForWhite1.Value + valueForBlue.Value,
                valueForWhite2.Value + valueForBlue.Value
            };
            return blueDiceSums;
        }

        public ScorePossibilitiesModel GetScorePossibilities()
        {
            var scorePossibilities = new ScorePossibilitiesModel
            {
                WhiteDiceSum = GetWhiteDiceSum(),
                RedDiceSums = GetRedDiceSums(),
                YellowDiceSums = GetYellowDiceSums(),
                GreenDiceSums = GetGreenDiceSums(),
                BlueDiceSums = GetBlueDiceSums()
            };
            return scorePossibilities;
        }
    }
}