using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum WinnerType
{
    PLAYER,
    DEALER,
    TIE
}
public class UiManager : MonoBehaviour
{
    #region Static_Members
    public static UiManager Instance;
    public Action<WinnerType, string> actionFinalResult;
    public Action actionHit;
    public Action actionStand;
    public Action actionEndGame;
    #endregion
    [SerializeField] private List<Sprite> spriteAvatars;
    [Header("UI Panels")]
    [SerializeField] private GameObject goSetBetPanel;
    [Header("Player Profiles Data")]
    [SerializeField] private Image imageAvatarPlayer;
    [SerializeField] private TextMeshProUGUI textTotalCreditPlayer;
    [Header("Dealer Profiles Data")]
    [SerializeField] private Image imageAvatarDealer;
    [SerializeField] private TextMeshProUGUI textTotalCreditDealer;
    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI textFinalResult;
    [Header("UI Generic Buttons")]
    [SerializeField] private Button buttonSetBet;
    [SerializeField] private Button buttonBetPanelCross;
    [SerializeField] private Button buttonHit;
    [SerializeField] private Button buttonStand;
    [SerializeField] private Button buttonReplay;
    [Header("Bet Panel Properties")]
    [SerializeField] private Button[] buttonsBetAmount;
    [SerializeField] private TextMeshProUGUI textTotalBetAmount;
    [SerializeField] private TextMeshProUGUI textFlyEffect;
    [SerializeField] private Button buttonDeal;
    [SerializeField] private Button buttonRefuse;
     
    private System.Random random = new System.Random();

    private int totalBetAmount;
    private string[] strBetAmounts = new string[] {"1","5","10","20" };
    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitVariables();
    }
    private void OnEnable()
    {
        actionEndGame += EndGame;
        actionFinalResult += FinalResult;
    }
    private void OnDisable()
    {
        actionEndGame -= EndGame;
        actionFinalResult -= FinalResult;
    }
    void InitVariables()
    {
        totalBetAmount = 0;
        InitButtonsListener();
        imageAvatarPlayer.sprite = spriteAvatars[GameDelegate.Instance.PlayerSelectedAvatarIndex];
        imageAvatarDealer.sprite = spriteAvatars[random.Next(spriteAvatars.Count)];
        
    }

    private void InitButtonsListener()
    {
/*        buttonSetBet.onClick.AddListener(OnClickBetButton);
        buttonBetPanelCross.onClick.AddListener(OnClickBetCrossButton);
        buttonReplay.onClick.AddListener(OnClickReplayButton);
        buttonHit.onClick.AddListener(delegate () { actionHit?.Invoke(); });
        buttonStand.onClick.AddListener(delegate () { actionStand?.Invoke(); });*/

        for(int i=0; i<buttonsBetAmount.Length; i++)
        {
            buttonsBetAmount[i].onClick.AddListener(() => { 
                OnClickSetBetAmountButton(buttonsBetAmount[i].transform.position, int.Parse(strBetAmounts[i]));
            });
        }
/*        buttonsBetAmount[0].onClick.AddListener(()=> { OnClickSetBetAmountButton(buttonsBetAmount[0].transform.position,1); });
        buttonsBetAmount[1].onClick.AddListener(()=> { OnClickSetBetAmountButton(buttonsBetAmount[1].transform.position, 5); });
        buttonsBetAmount[2].onClick.AddListener(()=> { OnClickSetBetAmountButton(buttonsBetAmount[2].transform.position, 10); });
        buttonsBetAmount[3].onClick.AddListener(()=> { OnClickSetBetAmountButton(buttonsBetAmount[3].transform.position, 20); });*/
        

    }

    void FinalResult(WinnerType winnerType, string strFinalMessage)
    {
        if(winnerType == WinnerType.PLAYER)
        {
            textFinalResult.text = strFinalMessage;
        }
        else if(winnerType == WinnerType.DEALER)
        {
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
    }
    private void SetGameObjectState(GameObject go, bool state)
    {
        go.SetActive(state);
    }

    #region OnClick_Buttons_Actions
    private void OnClickBetButton()
    {
        SetGameObjectState(goSetBetPanel,true);

    }
    private void OnClickBetCrossButton()
    {
        SetGameObjectState(goSetBetPanel, false);
    }   
    private void OnClickDealButton()
    {
        SetGameObjectState(goSetBetPanel, false);
    }  
    private void OnClickRefuseButton()
    {
        SetGameObjectState(goSetBetPanel, false);
    }

    private void OnClickSetBetAmountButton(Vector3 buttonPos, int amount)
    {
        if (totalBetAmount + amount > GameDelegate.Instance.InitialCredit) return;
        totalBetAmount += amount;
        textTotalCreditPlayer.transform.localScale = new Vector3(1f,1f,1f);
        textTotalCreditPlayer.transform.DOScale(new Vector3(0.8f,0.8f,0.8f), 0.2f).SetLoops(2,LoopType.Yoyo)
            .OnComplete(()=> { CalculateTheTotalBetAmount(); });
        textFlyEffect.transform.position = buttonPos;
        textFlyEffect.text = $"${amount}";
        textFlyEffect.transform.DOMove(textTotalBetAmount.transform.position, 0.7f).SetEase(Ease.InSine)
            .OnComplete(()=> { textFlyEffect.text = ""; });
    }
    private void CalculateTheTotalBetAmount()
    {
        int currentPlayerCredit = GameDelegate.Instance.TotalPlayerCredit - totalBetAmount;
        textTotalCreditPlayer.text = $"${currentPlayerCredit}";
        textTotalBetAmount.text = $"${totalBetAmount}";
    }
    private void OnClickReplayButton()
    {

    }
    #endregion
}
