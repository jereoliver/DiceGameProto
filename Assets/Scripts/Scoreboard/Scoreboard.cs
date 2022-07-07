using System;
using JetBrains.Annotations;
using UniRx;

namespace DiceGameProto
{
    public interface IScoreboard
    {
        IReadOnlyReactiveProperty<int> RedPoints { get; }
        IReadOnlyReactiveProperty<int> YellowPoints { get; }
        IReadOnlyReactiveProperty<int> GreenPoints { get; }
        IReadOnlyReactiveProperty<int> BluePoints { get; }
        IReadOnlyReactiveProperty<int> ErrorPoints { get; }
        IReadOnlyReactiveProperty<int> TotalPoints { get; }
        void SetRedPoints(ScoreType scoreType, int amount);
    }

    [UsedImplicitly]
    public class Scoreboard : IScoreboard
    {

        private IReactiveProperty<int> RedPoints { get; }
        private IReactiveProperty<int> YellowPoints { get; }
        private IReactiveProperty<int> GreenPoints { get; }
        private IReactiveProperty<int> BluePoints { get; }
        private IReactiveProperty<int> ErrorPoints { get; }
        private IReactiveProperty<int> TotalPoints { get; }
        
        public void SetRedPoints(ScoreType scoreType, int amount)
        {
            switch (scoreType)
            {
                case ScoreType.Red:
                    RedPoints.Value = amount;
                    break;
                case ScoreType.Yellow:
                    YellowPoints.Value = amount;
                    break;
                case ScoreType.Green:
                    GreenPoints.Value = amount;
                    break;
                case ScoreType.Blue:
                    BluePoints.Value = amount;
                    break;
                case ScoreType.Error:
                    ErrorPoints.Value = amount;
                    break;
                case ScoreType.Total:
                    TotalPoints.Value = amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scoreType), scoreType, null);
            }
        }

        public Scoreboard()
        {
            RedPoints = new ReactiveProperty<int>();
            YellowPoints = new ReactiveProperty<int>();
            GreenPoints = new ReactiveProperty<int>();
            BluePoints = new ReactiveProperty<int>();
            TotalPoints = new ReactiveProperty<int>();
            ErrorPoints = new ReactiveProperty<int>();
        }
        IReadOnlyReactiveProperty<int> IScoreboard.RedPoints => RedPoints;
        IReadOnlyReactiveProperty<int> IScoreboard.YellowPoints => YellowPoints;
        IReadOnlyReactiveProperty<int> IScoreboard.GreenPoints => GreenPoints;
        IReadOnlyReactiveProperty<int> IScoreboard.BluePoints => BluePoints;
        IReadOnlyReactiveProperty<int> IScoreboard.ErrorPoints => ErrorPoints;
        IReadOnlyReactiveProperty<int> IScoreboard.TotalPoints => TotalPoints;
    }

    public enum ScoreType
    {
        Red,
        Yellow,
        Green,
        Blue,
        Error,
        Total
    }
}