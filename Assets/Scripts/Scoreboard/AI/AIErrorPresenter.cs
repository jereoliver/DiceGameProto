using UnityEngine;
using UnityEngine.UI;

namespace Scoreboard.AI
{
    public class AIErrorPresenter : MonoBehaviour
    {
        private Image crossImage;

        private void Awake()
        {
            crossImage = GetComponent<Image>();
        }

        public void SetCrossStatus(bool isCrossed)
        {
            crossImage.enabled = isCrossed;
        }
    }
}