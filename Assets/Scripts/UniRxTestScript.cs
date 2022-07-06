using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class UniRxTestScript : MonoBehaviour
{
    private readonly ReactiveProperty<int> ReactiveNumber = new ReactiveProperty<int>();
    private readonly ReactiveProperty<int> ReactiveNumber2 = new ReactiveProperty<int>();
    private int NumberToUpdate = 0;

    [FormerlySerializedAs("TextField")] [SerializeField] private TMP_Text NumberToUpdateText;
    [SerializeField] private TMP_Text ReactiveNumberText;
    [SerializeField] private TMP_Text ReactiveNumber1Text;


    private void Start()
    {
        ReactiveNumber.Subscribe(UpdateFirstReactiveText).AddTo(gameObject);
        ReactiveNumber2.Subscribe(UpdateSecondReactiveText).AddTo(gameObject);
        ReactiveNumber.Merge(ReactiveNumber2).Subscribe(UpdateText).AddTo(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ReactiveNumber.Value++;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ReactiveNumber2.Value++;
        }
    }

    private void UpdateText(int increment)
    {
        NumberToUpdate += increment;
        NumberToUpdateText.SetText(NumberToUpdate.ToString());
    }

    private void UpdateFirstReactiveText(int value)
    {
        ReactiveNumberText.SetText(value.ToString());
    }
    private void UpdateSecondReactiveText(int value)
    {
        ReactiveNumber1Text.SetText(value.ToString());

    }
}