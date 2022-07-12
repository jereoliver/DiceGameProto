using UnityEngine;
using UnityEngine.UI;

namespace Scoreboard.AI
{
    public class AISlotPresenter : MonoBehaviour
    {
        [Range(2, 13)] [SerializeField] private int number;
        [SerializeField] private SlotColor slotColor;


        private Image image;
        public int Number => number;
        public SlotColor SlotColor => slotColor;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        public void SetCrossedState(bool isCrossed)
        {
            image.enabled = isCrossed;
        }
    }
}