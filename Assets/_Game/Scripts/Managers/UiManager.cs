using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public enum WinnerState
{
    PLAYER,
    DEALER,
    TIE
}
public class UiManager : MonoBehaviour
{
    public static UiManager Instance;
    #region System_Actions
    public Action<WinnerState, string> actionFinalResult;
    public Action actionHit;
    public Action actionStand;
    public Action actionCompleteCardDistribution;
    public Action actionEndGame;
    public Action actionStartGame;
    #endregion
    #region SerializeField
    [SerializeField] private List<Sprite> spriteAvatars;
    [Header("HUD Panels GamesObjects")]
    [SerializeField] private GameObject goSetBetPanel;
    [SerializeField] private GameObject goActionButtonsGroup;
    [Header("Player Profiles Data")]
    [SerializeField] private Image imageAvatarPlayer;
    [SerializeField] private TextMeshProUGUI textTotalMoneyPlayer;
    [Header("Dealer Profiles Data")]
    [SerializeField] private Image imageAvatarDealer;
    [SerializeField] private TextMeshProUGUI textTotalMoneyDealer;
    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI textFinalResult;
    [Header("UI Generic Buttons")]
    [SerializeField] private Button buttonSetBet;
    [SerializeField] private Button buttonHit;
    [SerializeField] private Button buttonStand;
    [SerializeField] private Button buttonDouble;
    [SerializeField] private Button buttonSplit;
    [SerializeField] private Button buttonHome;
    [SerializeField] private Button buttonReplay;
    [Header("Bet Panel Properties")]
    [SerializeField] private Button[] buttonsBetAmount;
    [SerializeField] private TextMeshProUGUI textTotalBetAmount;
    [SerializeField] private TextMeshProUGUI textFlyEffect;
    [SerializeField] private Button buttonDeal;
    [SerializeField] private Button buttonRefuse;
    #endregion
    #region Private_Fields
    private System.Random random = new System.Random();
    private int totalBetAmount;
    private int playerCurrentMoney;
    private int dealerCurrentMoney;
    private string[] strBetAmounts = new string[] {"1","5","10","20" };
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GameDelegate.Instance.Initialize();
        SoundManager.Instance.Play("bet");
        InitVariables();
    }
    private void OnEnable()
    {
        actionEndGame += EndGame;
        actionFinalResult += FinalResult;
        actionCompleteCardDistribution += CardDistributionComplete;
    }
    private void OnDisable()
    {
        actionEndGame -= EndGame;
        actionFinalResult -= FinalResult;
        actionCompleteCardDistribution -= CardDistributionComplete;
    }
    #region Initialize_Methods
    private void InitVariables()
    {
        totalBetAmount = 0;
        InitButtonsListener();
        playerCurrentMoney = GameDelegate.Instance.TotalPlayerMoney;
        dealerCurrentMoney = GameDelegate.Instance.InitialMoney * 2;
        imageAvatarPlayer.sprite = spriteAvatars[GameDelegate.Instance.PlayerSelectedAvatarIndex];
        textTotalMoneyPlayer.text = $"${playerCurrentMoney}";

        imageAvatarDealer.sprite = spriteAvatars[random.Next(spriteAvatars.Count)];
        textTotalMoneyDealer.text = $"${dealerCurrentMoney}";
    }
    private void InitButtonsListener()
    {
        buttonSetBet.onClick.AddListener(OnClickSetBetButton);
        buttonReplay.onClick.AddListener(OnClickReplayButton);
        buttonHome.onClick.AddListener(OnClickHomeButton);
        buttonHit.onClick.AddListener(OnClickHitButton);
        buttonStand.onClick.AddListener(OnClickStandButton);
        buttonDouble.onClick.AddListener(OnClickDoubleButton);

        for (int i=0; i<buttonsBetAmount.Length; i++)
        {
            int tempIndex = i;
            buttonsBetAmount[i].onClick.AddListener(() => { 
                OnClickSetBetAmountButton(buttonsBetAmount[tempIndex].transform.position, int.Parse(strBetAmounts[tempIndex]));
            });
        }

        buttonDeal.onClick.AddListener(OnClickDealButton);
        buttonRefuse.onClick.AddListener(OnClickRefuseButton);
    }
    #endregion
    #region OnClick_Buttons_Actions
    private void OnClickSetBetButton()
    {
        SoundManager.Instance.Stop("bet");
        SetGameObjectState(goSetBetPanel, true);
    }
    private void OnClickHitButton()
    {
        SoundManager.Instance.Play("click");
        actionHit?.Invoke();
    }
    private void OnClickStandButton()
    {
        SoundManager.Instance.Play("click");
        actionStand?.Invoke();
    }
    private void OnClickHomeButton()
    {
        SoundManager.Instance.Play("click");
        SceneManager.LoadScene(0);
    }
    private void OnClickDealButton()
    {
        SoundManager.Instance.Play("click");
        SetGameObjectState(goSetBetPanel, false);
        textFlyEffect.text = textTotalBetAmount.text;
        textTotalBetAmount.text = "";
        textFlyEffect.transform.DOMoveY(textFlyEffect.transform.position.y + 475f, 0.5f).SetEase(Ease.InSine)
            .OnComplete(() => { actionStartGame?.Invoke(); });
    }
    private void OnClickRefuseButton()
    {
        SoundManager.Instance.Play("click");
        textTotalBetAmount.text = "$0";
        totalBetAmount = 0;
        playerCurrentMoney = GameDelegate.Instance.TotalPlayerMoney;
        textTotalMoneyPlayer.text = "$" + playerCurrentMoney.ToString();
        textTotalMoneyPlayer.transform.localScale = new Vector3(1f, 1f, 1f);
        textTotalMoneyPlayer.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f).SetLoops(2, LoopType.Yoyo);
        SetGameObjectState(buttonDeal.gameObject, false);
        SetGameObjectState(buttonRefuse.gameObject, false);

    }
    private void OnClickDoubleButton()
    {
        SoundManager.Instance.Play("click");
        if (playerCurrentMoney - (totalBetAmount * 2) <= 0)
        {
            textFinalResult.text = "Not Enough Money!";
            return;
        }
        buttonDouble.interactable = false;
        playerCurrentMoney -= totalBetAmount;
        textTotalMoneyPlayer.text = $"${playerCurrentMoney}";
        textTotalMoneyPlayer.transform.localScale = new Vector3(1f, 1f, 1f);
        textTotalMoneyPlayer.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f).SetLoops(2, LoopType.Yoyo);
        textTotalMoneyPlayer.text = $"${playerCurrentMoney}";
        totalBetAmount *= 2;
        textFlyEffect.text = "$" + totalBetAmount.ToString();

    }
    private void OnClickSetBetAmountButton(Vector3 buttonPos, int amount)
    {
        SoundManager.Instance.Play("pick");
        if (playerCurrentMoney - amount <= 0) return;
        SetGameObjectState(buttonDeal.gameObject, true);
        SetGameObjectState(buttonRefuse.gameObject, true);
        playerCurrentMoney -= amount;
        totalBetAmount += amount;
        textTotalMoneyPlayer.transform.localScale = new Vector3(1f, 1f, 1f);
        textTotalMoneyPlayer.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f).SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => {
                textTotalMoneyPlayer.text = $"${playerCurrentMoney}";
                textTotalBetAmount.text = $"${totalBetAmount}";
            });
        textFlyEffect.transform.position = buttonPos;
        textFlyEffect.text = $"${amount}";
        textFlyEffect.transform.DOMove(textTotalBetAmount.transform.position, 0.7f).SetEase(Ease.InSine)
            .OnComplete(() => { textFlyEffect.text = ""; });
    }
    private void OnClickReplayButton()
    {
        SoundManager.Instance.Play("click");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    #region EventAction_Methods
    private void CardDistributionComplete()
    {
        SetGameObjectState(goActionButtonsGroup, true);
    }
    private void FinalResult(WinnerState winnerState, string strFinalMessage)
    {
        if (winnerState == WinnerState.PLAYER)
        {
            playerCurrentMoney += totalBetAmount * 2;
            textTotalMoneyPlayer.text = "$" + playerCurrentMoney.ToString();
            GameDelegate.Instance.TotalPlayerMoney = playerCurrentMoney;
            textTotalMoneyPlayer.transform.localScale = new Vector3(1f, 1f, 1f);
            textTotalMoneyPlayer.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f).SetLoops(2, LoopType.Yoyo);

            dealerCurrentMoney -= totalBetAmount;
            textTotalMoneyDealer.text = "$" + dealerCurrentMoney.ToString();
            textTotalMoneyDealer.transform.localScale = new Vector3(1f, 1f, 1f);
            textTotalMoneyDealer.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f).SetLoops(2, LoopType.Yoyo);

            textFinalResult.text = strFinalMessage;
        }
        else if (winnerState == WinnerState.DEALER)
        {
            GameDelegate.Instance.TotalPlayerMoney = playerCurrentMoney;

            dealerCurrentMoney += totalBetAmount;
            textTotalMoneyDealer.text = "$" + dealerCurrentMoney.ToString();
            textTotalMoneyDealer.transform.localScale = new Vector3(1f, 1f, 1f);
            textTotalMoneyDealer.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f).SetLoops(2, LoopType.Yoyo);

            textFinalResult.text = strFinalMessage;
        }
        else
        {
            textFinalResult.text = strFinalMessage;
        }
    }
    private void EndGame()
    {
        buttonHit.interactable = false;
        buttonStand.interactable = false;
        buttonDouble.interactable = false;
    }
    #endregion
    private void SetGameObjectState(GameObject go, bool state)
    {
        go.SetActive(state);
    }
}
