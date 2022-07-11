using System;
using Scoreboard;
using UnityEngine;
using Zenject;

namespace DiceGame
{
    public class TestAILockRow : MonoBehaviour
    {
        [Inject(Id = "AI")] private IScoreboardController scoreboardController;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                scoreboardController.LockRow(SlotColor.Blue);
            }
        }
    }
}