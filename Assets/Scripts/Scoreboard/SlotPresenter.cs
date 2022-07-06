using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Scoreboard
{
    public class SlotPresenter : MonoBehaviour
    {
        [Range(2, 13)] [SerializeField] private int number;
        [SerializeField] private SlotColor slotColor;
        [SerializeField] private bool isLockSlot;
        [SerializeField] private bool isLastSlot;
        [SerializeField] private bool ascendingNumbers;
        [SerializeField] private Sprite crossSprite;
        [SerializeField] private Sprite overlaySprite;
        [SerializeField] private Color unavailableByRulesOverlayColor;
        [SerializeField] private Color removedOverlayColor;
        [SerializeField] private Color invisible;
        [SerializeField] private Color visible;


        private Image image;
        private Button button;
        private IReactiveProperty<SlotState> slotState;
        public SlotState CurrentSlotState => slotState.Value;

        public bool IsCrossed { get; private set; }

        public bool IsLockSlot => isLockSlot;
        public bool IsLastSlot => isLastSlot;
        public int Number => number;
        public SlotColor SlotColor => slotColor;
        public bool AscendingNumbers => ascendingNumbers;

        public void SetSlotState(SlotState newState)
        {
            slotState.Value = newState;
        }

        public void SetCrossed()
        {
            IsCrossed = true;
            SetSlotState(SlotState.Crossed);
        }

        private void Awake()
        {
            GetComponents();
            InitSlotsState();
            button.onClick.AddListener(HandleButtonPressed);
        }

        private void GetComponents()
        {
            image = GetComponent<Image>();
            button = GetComponent<Button>();
        }

        private void InitSlotsState()
        {
            if (isLockSlot || isLastSlot)
            {
                slotState = new ReactiveProperty<SlotState>(SlotState.UnavailableYetByRules);
            }
            else
            {
                // todo change later to init to unavailable and set available from game state
                slotState = new ReactiveProperty<SlotState>(SlotState.UnavailableByScore);
            }

            slotState.Subscribe(HandleStateChanged).AddTo(gameObject);
        }

        private void HandleStateChanged(SlotState newState)
        {
            switch (newState)
            {
                case SlotState.Removed:
                    image.sprite = overlaySprite;
                    image.color = removedOverlayColor;
                    button.interactable = false;
                    break;
                case SlotState.Available:
                    image.sprite = crossSprite;
                    image.color = invisible;
                    button.interactable = true;
                    break;
                case SlotState.Crossed:
                    image.sprite = crossSprite;
                    image.color = visible;
                    button.interactable = false;
                    break;
                case SlotState.UnavailableYetByRules:
                    image.sprite = overlaySprite;
                    image.color = unavailableByRulesOverlayColor;
                    button.interactable = false;
                    break;
                case SlotState.UnavailableByScore:
                    image.sprite = overlaySprite;
                    image.color = unavailableByRulesOverlayColor;
                    button.interactable = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        private void HandleButtonPressed()
        {
            if (!IsCrossed)
            {
                SetCrossed();
            }
        }
    }

    public enum SlotColor
    {
        Red,
        Yellow,
        Green,
        Blue
    }

    public enum SlotState
    {
        UnavailableYetByRules,
        UnavailableByScore,
        Available,
        Removed,
        Crossed
    }
}