using Scoreboard;

namespace GameFlow.Signals
{
    public class LockRowSignal
    {
        public readonly SlotColor ColorToLock;

        public LockRowSignal(SlotColor colorToLock)
        {
            ColorToLock = colorToLock;
        }
    }
}