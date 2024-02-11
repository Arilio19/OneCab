using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("References")]
    [SerializeField] RectTransform scoreParent;
    [SerializeField] TMP_Text playerOneScoreText;
    [SerializeField] TMP_Text playerTwoScoreText;

    [Header("Score Move")]
    [SerializeField] float scoreParentOffset;
    [SerializeField] float scoreMoveInDuration;
    [SerializeField] Ease scoreMoveInEase;
    [SerializeField] float scoreMoveOutDelay;
    [SerializeField] float scoreMoveOutDuration;
    [SerializeField] Ease scoreMoveOutEase;

    [Header("Number Shake")]
    [SerializeField] float shakeStartDelay;
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeStrength;
    [SerializeField] int vibrato;
    [SerializeField] float randomness;
    [SerializeField] Ease shakeEase;

    [Header("Audio")]
    [SerializeField] AudioClip scoreSound;

    private void Awake() => Instance = this;

    public void OnPlayerKilled(PlayerNumber killedPlayer)
    {
        scoreParent.gameObject.SetActive(true);
        scoreParent.localPosition = new Vector3(-scoreParentOffset, scoreParent.localPosition.y);

        scoreParent.DOLocalMoveX(0f, scoreMoveInDuration)
                   .SetEase(scoreMoveInEase)
                   .SetId("ScoreAnimation")
                   .OnComplete(() =>
                   {
                       var textToChange = killedPlayer == PlayerNumber.One ? playerTwoScoreText : playerOneScoreText;

                       textToChange.rectTransform.DOShakePosition(shakeDuration, shakeStrength, vibrato, randomness)
                                                 .OnPlay(() =>
                                                 {
                                                     textToChange.text = (int.Parse(textToChange.text) + 1).ToString();
                                                     AudioSource.PlayClipAtPoint(scoreSound, Camera.main.transform.position);
                                                 })
                                                 .SetDelay(shakeStartDelay)
                                                 .SetId("ScoreAnimation")
                                                 .SetEase(shakeEase)
                                                 .OnComplete(() =>
                                                 {
                                                     scoreParent.DOLocalMoveX(scoreParentOffset, scoreMoveOutDuration)
                                                     .SetEase(scoreMoveOutEase)
                                                     .SetDelay(scoreMoveOutDelay)
                                                     .SetId("ScoreAnimation")
                                                     .OnComplete(() => scoreParent.gameObject.SetActive(false));
                                                 });
                    });
    }

    [ContextMenu("Test")]
    public void Debug()
    {
        scoreParent.gameObject.SetActive(true);
        scoreParent.localPosition = new Vector3(-scoreParentOffset, scoreParent.localPosition.y);

        scoreParent.DOLocalMoveX(0f, scoreMoveInDuration).SetEase(scoreMoveInEase).OnComplete(() =>
        {
            var textToChange = playerOneScoreText;

            textToChange.rectTransform.DOShakePosition(shakeDuration, shakeStrength, vibrato, randomness).SetDelay(shakeStartDelay).OnPlay(() => textToChange.text = (int.Parse(textToChange.text) + 1).ToString()).SetEase(shakeEase).OnComplete(() =>
            {
                scoreParent.DOLocalMoveX(scoreParentOffset, scoreMoveOutDuration).SetEase(scoreMoveOutEase).SetDelay(scoreMoveOutDelay).OnComplete(() => scoreParent.gameObject.SetActive(false));
            });
        });
    }

    public int GetPlayerOneScore() => int.Parse(playerOneScoreText.text);

    public int GetPlayerTwoScore() => int.Parse(playerTwoScoreText.text);
}
