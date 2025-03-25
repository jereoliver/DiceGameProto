using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Dice
{
    public class DicePresenter : MonoBehaviour
    {
        private Image diceImage;
        [SerializeField] private List<Sprite> diceSprites;
        [SerializeField] private List<Color> diceColors;
        [SerializeField] private DiceType diceType;

        [Inject] private readonly IDiceController diceController;

        private void Awake()
        {
            diceImage = GetComponent<Image>();
            SubscribeToReactivePropertyValue();
            SetDiceTypeVisualization();
        }

        private void SubscribeToReactivePropertyValue()
        {
            switch (diceType)
            {
                case DiceType.White1:
                    diceController.CurrentValueForFirstWhiteDice.Subscribe(SetDiceImage).AddTo(gameObject);
                    break;
                case DiceType.White2:
                    diceController.CurrentValueForSecondWhiteDice.Subscribe(SetDiceImage).AddTo(gameObject);
                    break;
                case DiceType.Red:
                    diceController.CurrentValueForRedDice.Subscribe(SetDiceImage).AddTo(gameObject);
                    break;
                case DiceType.Yellow:
                    diceController.CurrentValueForYellowDice.Subscribe(SetDiceImage).AddTo(gameObject);
                    break;
                case DiceType.Green:
                    diceController.CurrentValueForGreenDice.Subscribe(SetDiceImage).AddTo(gameObject);
                    break;
                case DiceType.Blue:
                    diceController.CurrentValueForBlueDice.Subscribe(SetDiceImage).AddTo(gameObject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                diceController.Roll();
            }
        }

        private void SetDiceImage(int value)
        {
            if (value > 0)
            {
                diceImage.sprite = diceSprites[value - 1];
            }
        }

        private void SetDiceTypeVisualization()
        {
            diceImage.color = diceType switch
            {
                DiceType.White1 => diceColors[0],
                DiceType.White2 => diceColors[0],
                DiceType.Red => diceColors[1],
                DiceType.Yellow => diceColors[2],
                DiceType.Green => diceColors[3],
                DiceType.Blue => diceColors[4],
                _ => diceImage.color
            };
        }
    }

    public enum DiceType
    {
        White1 = 0,
        White2 = 1,
        Red = 2,
        Yellow = 3,
        Green = 4,
        Blue = 5
    }
}