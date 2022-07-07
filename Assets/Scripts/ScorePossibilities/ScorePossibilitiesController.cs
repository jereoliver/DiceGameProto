using JetBrains.Annotations;

namespace ScorePossibilities
{
    public interface IScorePossibilitiesController
    {
        ScorePossibilitiesModel CurrentScorePossibilities { get; }
        int CurrentWhiteDiceSum { get; }
        void SetCurrentScorePossibilities(ScorePossibilitiesModel scorePossibilities);
    }

    [UsedImplicitly]
    public class ScorePossibilitiesController : IScorePossibilitiesController
    {
        public ScorePossibilitiesModel CurrentScorePossibilities { get; private set; }
        public int CurrentWhiteDiceSum { get; private set; }
        
        public void SetCurrentScorePossibilities(ScorePossibilitiesModel scorePossibilities)
        {
            CurrentScorePossibilities = scorePossibilities;
            CurrentWhiteDiceSum = scorePossibilities.WhiteDiceSum;
        }
    }
}