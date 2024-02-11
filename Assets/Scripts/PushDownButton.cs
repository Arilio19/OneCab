using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PushDownButton : MonoBehaviour
{
    [SerializeField] float pushDownDuration;
    [SerializeField] Ease pushDownEase;
    [SerializeField] float letDuration;
    [SerializeField] Ease letEase;
    [SerializeField] AudioClip clickSound;

    public void OnButtonClick(GameObject button)
    {
        if (DOTween.IsTweening("ButtonAnimation")) DOTween.Kill("ButtonAnimation", true);

        AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position, 0.8f);

        float initialY = button.transform.localPosition.y;

        button.GetComponent<RectTransform>().DOLocalMoveY(0f, pushDownDuration).SetEase(pushDownEase).SetId("ButtonAnimation").OnComplete(() =>
        {
            button.transform.DOLocalMoveY(initialY, letDuration).SetId("ButtonAnimation").SetEase(letEase);
        });
    }
}
