using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ScorePossibilities;
using UniRx;

namespace Dice
{
    public interface IDiceController
    {
        IReadOnlyReactiveProperty<int> CurrentValueForFirstWhiteDice { get; }
        IReadOnlyReactiveProperty<int> CurrentValueForSecondWhiteDice { get; }
        IReadOnlyReactiveProperty<int> CurrentValueForRedDice { get; }
        IReadOnlyReactiveProperty<int> CurrentValueForYellowDice { get; }
        IReadOnlyReactiveProperty<int> CurrentValueForGreenDice { get; }
        IReadOnlyReactiveProperty<int> CurrentValueForBlueDice { get; }

        /// <summary>
        /// Roll the dice to get new values
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
        private IReactiveProperty<int> valueForFirstWhiteDice { get; }
        private IReactiveProperty<int> valueForSecondWhiteDice { get; }
        private IReactiveProperty<int> valueForRedDice { get; }
        private IReactiveProperty<int> valueForYellowDice { get; }
        private IReactiveProperty<int> valueForGreenDice { get; }
        private IReactiveProperty<int> valueForBlueDice { get; }

        public DiceController()
        {
            valueForFirstWhiteDice = new ReactiveProperty<int>();
            valueForSecondWhiteDice = new ReactiveProperty<int>();
            valueForRedDice = new ReactiveProperty<int>();
            valueForYellowDice = new ReactiveProperty<int>();
            valueForGreenDice = new ReactiveProperty<int>();
            valueForBlueDice = new ReactiveProperty<int>();
        }


        IReadOnlyReactiveProperty<int> IDiceController.CurrentValueForFirstWhiteDice => valueForFirstWhiteDice;
        IReadOnlyReactiveProperty<int> IDiceController.CurrentValueForSecondWhiteDice => valueForSecondWhiteDice;
        IReadOnlyReactiveProperty<int> IDiceController.CurrentValueForRedDice => valueForRedDice;
        IReadOnlyReactiveProperty<int> IDiceController.CurrentValueForYellowDice => valueForYellowDice;
        IReadOnlyReactiveProperty<int> IDiceController.CurrentValueForGreenDice => valueForGreenDice;
        IReadOnlyReactiveProperty<int> IDiceController.CurrentValueForBlueDice => valueForBlueDice;

        public void Roll()
        {
            var rnd = new Random();
            valueForFirstWhiteDice.Value = rnd.Next(1, 7);
            valueForSecondWhiteDice.Value = rnd.Next(1, 7);
            valueForRedDice.Value = rnd.Next(1, 7);
            valueForYellowDice.Value = rnd.Next(1, 7);
            valueForGreenDice.Value = rnd.Next(1, 7);
            valueForBlueDice.Value = rnd.Next(1, 7);
        }

        private int GetWhiteDiceSum()
        {
            var whiteDiceSum = valueForFirstWhiteDice.Value + valueForSecondWhiteDice.Value;
            return whiteDiceSum;
        }

        private List<int> GetRedDiceSums()
        {
            var redDiceSums = new List<int>
            {
                valueForFirstWhiteDice.Value + valueForRedDice.Value,
                valueForSecondWhiteDice.Value + valueForRedDice.Value
            };
            return redDiceSums;
        }

        private List<int> GetYellowDiceSums()
        {
            var yellowDiceSums = new List<int>
            {
                valueForFirstWhiteDice.Value + valueForYellowDice.Value,
                valueForSecondWhiteDice.Value + valueForYellowDice.Value
            };
            return yellowDiceSums;
        }

        private List<int> GetGreenDiceSums()
        {
            var greenDiceSums = new List<int>
            {
                valueForFirstWhiteDice.Value + valueForGreenDice.Value,
                valueForSecondWhiteDice.Value + valueForGreenDice.Value
            };
            return greenDiceSums;
        }

        private List<int> GetBlueDiceSums()
        {
            var blueDiceSums = new List<int>
            {
                valueForFirstWhiteDice.Value + valueForBlueDice.Value,
                valueForSecondWhiteDice.Value + valueForBlueDice.Value
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