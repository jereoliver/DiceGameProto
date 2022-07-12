using System;
using JetBrains.Annotations;
using UniRx;

namespace Scoreboard
{
    public interface IScoreboardModel
    {
        IReadOnlyReactiveProperty<int> RedPoints { get; }
        IReadOnlyReactiveProperty<int> YellowPoints { get; }
        IReadOnlyReactiveProperty<int> GreenPoints { get; }
        IReadOnlyReactiveProperty<int> BluePoints { get; }
        IReadOnlyReactiveProperty<int> ErrorPoints { get; }
        IReadOnlyReactiveProperty<int> TotalPoints { get; }
        void SetPoints(ScoreType scoreType, int amount);
    }

    [UsedImplicitly]
    public class ScoreboardModel : IScoreboardModel
    {
        private IReactiveProperty<int> RedPoints { get; }
        private IReactiveProperty<int> YellowPoints { get; }
        private IReactiveProperty<int> GreenPoints { get; }
        private IReactiveProperty<int> BluePoints { get; }
        private IReactiveProperty<int> ErrorPoints { get; }
        private IReactiveProperty<int> TotalPoints { get; }

        public void SetPoints(ScoreType scoreType, int amount)
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

        public ScoreboardModel()
        {
            RedPoints = new ReactiveProperty<int>();
            YellowPoints = new ReactiveProperty<int>();
            GreenPoints = new ReactiveProperty<int>();
            BluePoints = new ReactiveProperty<int>();
            TotalPoints = new ReactiveProperty<int>();
            ErrorPoints = new ReactiveProperty<int>();
        }

        IReadOnlyReactiveProperty<int> IScoreboardModel.RedPoints => RedPoints;
        IReadOnlyReactiveProperty<int> IScoreboardModel.YellowPoints => YellowPoints;
        IReadOnlyReactiveProperty<int> IScoreboardModel.GreenPoints => GreenPoints;
        IReadOnlyReactiveProperty<int> IScoreboardModel.BluePoints => BluePoints;
        IReadOnlyReactiveProperty<int> IScoreboardModel.ErrorPoints => ErrorPoints;
        IReadOnlyReactiveProperty<int> IScoreboardModel.TotalPoints => TotalPoints;
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