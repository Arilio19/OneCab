using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Mechanic : MonoBehaviour
{
    public static float onMatchStartScaleUpDuration  = 0.3f;
    public static Ease onMatchStartScaleUpEase = Ease.OutExpo;
    public static float onMatchEndScaleDownDuration = 0.3f;
    public static Ease onMatchEndScaleDownEase;

    public virtual void OnEnable()
    {
        GameManager.OnMatchStart += OnMatchStart;
        GameManager.OnMatchEnd += OnMatchEnd;
    }

    Vector3 originalScale;
    public virtual void Awake()
    {
        originalScale = transform.localScale;
    }

    public virtual void OnMatchStart()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(originalScale, onMatchStartScaleUpDuration).SetEase(onMatchStartScaleUpEase);
    }

    public virtual void OnMatchEnd()
    {
        transform.DOScale(Vector3.zero, onMatchEndScaleDownDuration).SetEase(onMatchEndScaleDownEase);
    }

    public virtual void OnDisable()
    {
        GameManager.OnMatchStart -= OnMatchStart;
        GameManager.OnMatchEnd -= OnMatchEnd;
    }
}
