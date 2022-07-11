using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameFlow
{
    public class GameFlowPresenter : MonoBehaviour
    {
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button gameOverButton;
        [SerializeField] private GameObject gameOverView;
        [SerializeField] private Button RestartButtonOfGameOverView;

        [Inject] private IGameFlowController gameFlowController;
        
        private void Awake()
        {
            startGameButton.onClick.AddListener(gameFlowController.StartGame);
            startGameButton.onClick.AddListener(() => ToggleStartGameButton(false));
            gameOverButton.onClick.AddListener(gameFlowController.Restart);
            RestartButtonOfGameOverView.onClick.AddListener(gameFlowController.Restart);
            gameFlowController.GameIsOver.Subscribe(ShowGameOverView).AddTo(gameObject);
        }

        private void OnDestroy()
        {
            startGameButton.onClick.RemoveListener(gameFlowController.StartGame);
            startGameButton.onClick.RemoveListener(() => ToggleStartGameButton(false));
            gameOverButton.onClick.RemoveListener(gameFlowController.Restart);
            RestartButtonOfGameOverView.onClick.RemoveListener(gameFlowController.Restart);
        }

        private void ToggleStartGameButton(bool isInteractable)
        {
            startGameButton.interactable = isInteractable;
        }

        private void ShowGameOverView(bool isGameOver)
        {
            if (isGameOver)
            {
                gameOverView.SetActive(true);
            }
        }
    }
}