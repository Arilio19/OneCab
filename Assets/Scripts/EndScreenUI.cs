using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class EndScreenUI : MonoBehaviour
{
    public static EndScreenUI Instance;

    [Header("References")]
    [SerializeField] GameObject canvas;

    [SerializeField] TMP_Text playerText;
    [SerializeField] TMP_Text wonText;
    [SerializeField] RectTransform playerOne;
    [SerializeField] RectTransform playerTwo;

    [SerializeField] RectTransform restartButton;

    [Header("Visuals")]
    [SerializeField] Color playerOneColor;
    [SerializeField] Color playerTwoColor;

    [Header("Players Animation")]
    [SerializeField] float playersStartXOffset;
    [SerializeField] float playersMoveInDuration;
    [SerializeField] Ease playersMoveInEase;

    [Header("Crown Animation")]
    [SerializeField] float crownStartYOffset;
    [SerializeField] float crownScaleUpDuration;
    [SerializeField] Ease crownScaleUpEase;
    [SerializeField] float crownMoveDuration;
    [SerializeField] Ease crownMoveEase;

    [Header("Tear Drop Animation")]
    [SerializeField] float tearDropYOffset;
    [SerializeField] float tearDropScaleUpDuration;
    [SerializeField] Ease tearDropScaleUpEase;
    [SerializeField] float tearDropFallDuration;
    [SerializeField] Ease tearDropFallEase;
    [SerializeField] float tearDropRepeatDelay;

    [Header("Restart Button Animation")]
    [SerializeField] float restartButtonShowUpDelay;
    [SerializeField] float restartButtonScaleUpDuration;
    [SerializeField] Ease restartButtonScaleUpEase;

    private void Awake() => Instance = this;

    [ContextMenu("Play")]
    public void Initialize()
    {
        canvas.SetActive(true);

        RectTransform playerWon;
        RectTransform playerLost;

        if (ScoreManager.Instance.GetPlayerOneScore() > ScoreManager.Instance.GetPlayerTwoScore())
        {
            playerText.text = "Player 1";
            playerText.color = playerOneColor;

            playerWon = playerOne;
            playerLost = playerTwo;
        }
        else
        {
            playerText.text = "Player 2";
            playerText.color = playerTwoColor;

            playerWon = playerTwo;
            playerLost = playerOne;
        }

        Transform crown = playerWon.GetChild(1).transform;
        crown.gameObject.SetActive(true);
        float crownEndPos = crown.localPosition.y;

        crown.localPosition = new Vector3(crown.position.x, crownEndPos + crownStartYOffset);
        crown.localScale = Vector3.zero;

        crown.DOScale(1f, crownScaleUpDuration).SetEase(crownScaleUpEase).OnComplete(() =>
        {
            crown.DOLocalMoveY(crownEndPos, crownMoveDuration).SetEase(crownMoveEase);
        });

        StartCoroutine(TearDropAnimationLoop(playerLost));

        restartButton.transform.localScale = Vector3.zero;
        restartButton.transform.DOScale(1f, restartButtonScaleUpDuration).SetEase(restartButtonScaleUpEase).SetDelay(restartButtonShowUpDelay);
    }

    IEnumerator TearDropAnimationLoop(RectTransform playerLost)
    {
        Transform tearDrop = playerLost.GetChild(2);
        tearDrop.gameObject.SetActive(true);
        float tearDropStartY = tearDrop.localPosition.y;

        while (true)
        {
            tearDrop.localScale = Vector3.zero;
            tearDrop.DOScale(1f, tearDropScaleUpDuration).SetEase(tearDropScaleUpEase).SetId("TearDropFall").OnComplete(() =>
            {
                tearDrop.DOLocalMoveY(tearDropStartY - tearDropYOffset, tearDropFallDuration).SetEase(tearDropFallEase).SetId("TearDropFall");
            });

            yield return new WaitUntil(() => !DOTween.IsTweening("TearDropFall"));

            yield return new WaitForSeconds(tearDropRepeatDelay);

            tearDrop.localPosition = new Vector3(tearDrop.localPosition.x, tearDropStartY);
        }
    }

    public void OnRestartButtonClick() => StartCoroutine(OnRestartButtonClickRoutine());

    IEnumerator OnRestartButtonClickRoutine()
    {
        yield return new WaitForSeconds(0.07f);

        GameManager.Instance.RestartGame();
    }
}
