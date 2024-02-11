using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public enum GameState { StartScreen, ButtonsTutorial, MechanicTutorial, Playing, PlayerKilled }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public delegate void GameEvent();
    public static event GameEvent OnMatchStart;
    public static event GameEvent OnMatchEnd;

    [Header("Players")]
    [SerializeField] PlayerController player1;
    [SerializeField] PlayerController player2;

    [Header("Levels")]
    [SerializeField] LevelInfo[] levels;
    int levelIdx;

    [Header("UI")]
    [SerializeField] GameObject startAndLogoParent;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject logo;
    [SerializeField] float startAndLogoMoveOutDuration;
    [SerializeField] Ease startAndLogoMoveOutEase;

    [SerializeField] RectTransform tutorial;
    [SerializeField] float tutorialMoveInDelay;
    [SerializeField] float tutorialMoveInDuration;
    [SerializeField] Ease tutorialMoveInEase;
    [SerializeField] float tutorialMoveOutDuration;
    [SerializeField] Ease tutorialMoveOutEase;

    [SerializeField] float playerKillScoreShowUpDelay;

    [Header("Tutorial")]
    [SerializeField] CanvasGroup[] mechanicTutorialsUIs;
    [SerializeField] float tutorialUIFadeDuration;
    int tutorialUIIdx;

    public GameState state { get; private set; }

    private void Awake()
    {
        Instance = this;
        state = GameState.StartScreen;

        levelIdx = 0;
    }

    public void OnStartButtonClick()
    {
        state = GameState.ButtonsTutorial;

        startButton.transform.DOMoveX(-startButton.GetComponent<RectTransform>().anchoredPosition.x, startAndLogoMoveOutDuration).SetDelay(0.07f).SetEase(startAndLogoMoveOutEase);
        logo.transform.DOMoveX(-logo.GetComponent<RectTransform>().anchoredPosition.x, startAndLogoMoveOutDuration).SetEase(startAndLogoMoveOutEase).OnComplete(() =>
        {
            startAndLogoParent.SetActive(false);

            tutorial.transform.localPosition = new Vector3(1920f, 0f);
            tutorial.gameObject.SetActive(true);
            tutorial.transform.DOMoveX(0f, tutorialMoveInDuration).SetEase(tutorialMoveInEase).SetDelay(tutorialMoveInDelay);
        });
    }

    public void OnPlayButtonClick()
    {
        tutorial.DOAnchorPosX(-1920f, tutorialMoveOutDuration).SetEase(tutorialMoveOutEase).SetDelay(tutorialMoveInDelay + 0.07f).OnComplete(() => { tutorial.gameObject.SetActive(false); Invoke("InitializeLevel", 1f); });
    }

    public void InitializeLevel()
    {
        if (levelIdx >= levels.Length) { EndScreenUI.Instance.Initialize(); return; }

        LevelInfo currentLevel = levels[levelIdx];

        if (currentLevel.playerOneDummy != null)
        {
            state = GameState.MechanicTutorial;
            ShowMechanicTutorial();
        }
        else
        {
            state = GameState.Playing;
            //HideMechanicTutorial();
        }

        player1.transform.position = levels[levelIdx].playerOneStartPos;
        player1.gameObject.SetActive(true);
        player2.transform.position = levels[levelIdx].playerTwoStartPos;
        player2.gameObject.SetActive(true);
        levels[levelIdx].gameObject.SetActive(true);

        OnMatchStart();
    }

    public void ShowMechanicTutorial()
    {
        mechanicTutorialsUIs[tutorialUIIdx].gameObject.SetActive(true);
        mechanicTutorialsUIs[tutorialUIIdx].alpha = 0f;
        mechanicTutorialsUIs[tutorialUIIdx].DOFade(1f, tutorialUIFadeDuration);
    }

    public void HideMechanicTutorial()
    {
        mechanicTutorialsUIs[tutorialUIIdx].DOFade(0f, tutorialUIFadeDuration).OnComplete(() => mechanicTutorialsUIs[tutorialUIIdx - 1].gameObject.SetActive(false));
        tutorialUIIdx++;
    }

    public void OnTutorialDummyKilled(TutorialDummy dummyKilled) => StartCoroutine(OnTutorialDummyKilledRoutine(dummyKilled));

    IEnumerator OnTutorialDummyKilledRoutine(TutorialDummy dummyKilled)
    {
        LevelInfo currentLevel = levels[levelIdx];

        if (dummyKilled == currentLevel.playerOneDummy) currentLevel.playerOneDummy = null;
        else currentLevel.playerTwoDummy = null;

        if (levels[levelIdx].playerOneDummy != null || levels[levelIdx].playerTwoDummy != null) yield break;

        yield return new WaitForSeconds(1f);

        OnMatchEnd();
        HideMechanicTutorial();

        yield return new WaitForSeconds(Mechanic.onMatchEndScaleDownDuration);

        levels[levelIdx].gameObject.SetActive(false);
        levelIdx++;

        InitializeLevel();
    }

    public void OnPlayerKilled(PlayerNumber playerKilled) => StartCoroutine(OnPlayerKilledRoutine(playerKilled));

    IEnumerator OnPlayerKilledRoutine(PlayerNumber playerKilled)
    {
        state = GameState.PlayerKilled;

        yield return new WaitForSeconds(playerKillScoreShowUpDelay);

        ScoreManager.Instance.OnPlayerKilled(playerKilled);

        yield return new WaitUntil(() => !DOTween.IsTweening("ScoreAnimation"));

        OnMatchEnd();

        yield return new WaitForSeconds(Mechanic.onMatchEndScaleDownDuration);

        levels[levelIdx].gameObject.SetActive(false);
        levelIdx++;

        InitializeLevel();
    }

    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
