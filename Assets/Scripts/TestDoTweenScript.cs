using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TestDoTweenScript : MonoBehaviour
{
    [SerializeField] private float startScale;
    [SerializeField] private float endScale;
    [SerializeField] private float duration;
    [SerializeField] private Ease ease;

    private void Start()
    {
        transform.localScale = new Vector3(startScale, startScale, startScale);
        var mySequence = DOTween.Sequence();
        mySequence
            .Append(transform.DOScale(new Vector3(endScale, endScale, endScale), duration / 2))
            .Append(transform.DOScale(new Vector3(startScale, startScale, startScale), duration / 2))
            .SetEase(ease);
        mySequence.Play().SetLoops(-1);
    }
}