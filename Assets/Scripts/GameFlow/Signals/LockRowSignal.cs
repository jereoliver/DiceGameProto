using Scoreboard;

namespace GameFlow.Signals
{
    public class LockRowSignal
    {
        public SlotColor ColorToLock;

        public LockRowSignal(SlotColor colorToLock)
        {
            ColorToLock = colorToLock;
        }
    }
}