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
    #region Static_Members
    public static UiManager Instance;
    public Action<WinnerState, string> actionFinalResult;
    public Action actionHit;
    public Action actionStand;
    public Action actionSplit;
    public Action actionCompleteCardDistribution;
    public Action actionEndGame;
    public Action actionStartGame;
    #endregion
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
    [SerializeField] private Button buttonReplay;
    [Header("Bet Panel Properties")]
    [SerializeField] private Button[] buttonsBetAmount;
    [SerializeField] private TextMeshProUGUI textTotalBetAmount;
    [SerializeField] private TextMeshProUGUI textFlyEffect;
    [SerializeField] private Button buttonDeal;
    [SerializeField] private Button buttonRefuse;
     
    private System.Random random = new System.Random();

    private int totalBetAmount;
    private int playerCurrentMoney;
    private int dealerCurrentMoney;
    private string[] strBetAmounts = new string[] {"1","5","10","20" };

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
    void InitVariables()
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
        buttonSetBet.onClick.AddListener(() => { SetGameObjectState(goSetBetPanel, true); });
        buttonReplay.onClick.AddListener(OnClickReplayButton);
        buttonHit.onClick.AddListener(delegate () { actionHit?.Invoke(); });
        buttonStand.onClick.AddListener(delegate () { actionStand?.Invoke(); });
        buttonDouble.onClick.AddListener(OnClickDoubleButton);
        buttonSplit.onClick.AddListener(() => { actionSplit?.Invoke(); });

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
    private void CardDistributionComplete()
    {
        SetGameObjectState(goActionButtonsGroup, true);
    }
    void FinalResult(WinnerState winnerState, string strFinalMessage)
    {
        if(winnerState == WinnerState.PLAYER)
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
        else if(winnerState == WinnerState.DEALER)
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
    private void SetGameObjectState(GameObject go, bool state)
    {
        go.SetActive(state);
    }

    #region OnClick_Buttons_Actions
    private void OnClickDealButton()
    {
        SetGameObjectState(goSetBetPanel, false);
        textFlyEffect.text = textTotalBetAmount.text;
        textTotalBetAmount.text = "";
        textFlyEffect.transform.DOMoveY(textFlyEffect.transform.position.y + 475f, 0.5f).SetEase(Ease.InSine)
            .OnComplete(()=> { actionStartGame?.Invoke(); });
    }  
    private void OnClickRefuseButton()
    {
        textTotalBetAmount.text = "$0";
        totalBetAmount = 0;
        playerCurrentMoney = GameDelegate.Instance.TotalPlayerMoney;
        textTotalMoneyPlayer.text = "$"+playerCurrentMoney.ToString();
        textTotalMoneyPlayer.transform.localScale = new Vector3(1f, 1f, 1f);
        textTotalMoneyPlayer.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f).SetLoops(2, LoopType.Yoyo);
        SetGameObjectState(buttonDeal.gameObject, false);
        SetGameObjectState(buttonRefuse.gameObject, false);

    }
    private void OnClickDoubleButton()
    {
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
        textFlyEffect.text = "$"+totalBetAmount.ToString();

    }
    private void OnClickSetBetAmountButton(Vector3 buttonPos, int amount)
    {
        if (playerCurrentMoney - amount <= 0) return;
        SetGameObjectState(buttonDeal.gameObject, true);
        SetGameObjectState(buttonRefuse.gameObject, true);
        playerCurrentMoney -= amount;
        totalBetAmount += amount;
        textTotalMoneyPlayer.transform.localScale = new Vector3(1f,1f,1f);
        textTotalMoneyPlayer.transform.DOScale(new Vector3(0.8f,0.8f,0.8f), 0.2f).SetLoops(2,LoopType.Yoyo)
            .OnComplete(()=> {
                textTotalMoneyPlayer.text = $"${playerCurrentMoney}";
                textTotalBetAmount.text = $"${totalBetAmount}";
            });
        textFlyEffect.transform.position = buttonPos;
        textFlyEffect.text = $"${amount}";
        textFlyEffect.transform.DOMove(textTotalBetAmount.transform.position, 0.7f).SetEase(Ease.InSine)
            .OnComplete(()=> { textFlyEffect.text = ""; });
    }
    private void OnClickReplayButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion
}
