using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameFlow
{
    public class GameFlowPresenter : MonoBehaviour
    {
        [SerializeField] private Button StartGameButton;
        [SerializeField] private Button GameOverButton;

        [Inject] private IGameFlowController gameFlowController;

        private void Awake()
        {
            StartGameButton.onClick.AddListener(gameFlowController.StartGame);
            GameOverButton.onClick.AddListener(gameFlowController.GameOver);

        }
    }
}