using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace DiceGameProto
{
    public class ErrorPresenter : MonoBehaviour
    {
        [SerializeField] private Sprite crossSprite;
        [SerializeField] private Sprite overlaySprite;
        [SerializeField] private Color crossColor;
        [SerializeField] private Color overlayColor;
        [SerializeField] private Color invisible;

        private readonly IReactiveProperty<ErrorButtonState> currentState =
            new ReactiveProperty<ErrorButtonState>(ErrorButtonState.NonInteractable);
        private Image image;

        public ErrorButtonState CurrentState => currentState.Value;
        public Button ErrorButton { get; private set; }

        public void SetErrorButtonState(ErrorButtonState newState)
        {
            currentState.Value = newState;
        }

        private void Awake()
        {
            GetComponents();
            currentState.Subscribe(HandleStateChanged).AddTo(gameObject);
        }

        private void GetComponents()
        {
            image = GetComponent<Image>();
            ErrorButton = GetComponent<Button>();
        }

        private void HandleStateChanged(ErrorButtonState newState)
        {
            switch (newState)
            {
                case ErrorButtonState.NonInteractable:
                    image.sprite = overlaySprite;
                    image.color = overlayColor;
                    ErrorButton.interactable = false;
                    break;
                case ErrorButtonState.Interactable:
                    image.sprite = overlaySprite;
                    image.color = invisible;
                    ErrorButton.interactable = true;
                    break;
                case ErrorButtonState.Crossed:
                    image.sprite = crossSprite;
                    image.color = crossColor;
                    ErrorButton.interactable = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }
    }

    public enum ErrorButtonState
    {
        Crossed,
        Interactable,
        NonInteractable
    }
}