using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;
using System.Collections;

public enum PlayerType
{
    PLAYER,
    DEALER
}

public class BlackjackManager : MonoBehaviour
{
    public List<Sprite> cardSprites; 
    public List<Sprite> cardBackSprites; 
    public SpriteRenderer cardRendererPrefab; 
    public SpriteRenderer cardBackRendererPrefab; 
    public Transform centralSpawnPoint; 
    public Transform playerCardStartPos; 
    public Transform dealerCardStartPos; 
    public TextMeshPro playerScoreText;
    public TextMeshPro dealerScoreText;
    public Vector3 playerCardOffset;
    public Vector3 dealerCardOffset;
    public Ease easeCardPlace;
    public float cardPlaceDuration;

    private int playerScore = 0;
    private int dealerScore = 0;
    private int playerCardCount = 0;
    private int dealerCardCount = 0;
    private int playerLastCardValue = 0;
    private int dealerLastCardValue = 0;

    private List<int> deck = new List<int>();
    private System.Random random = new System.Random();
    private int playerCardSortingOrder, dealerCardSortingOrder;
    private int dealerHiddenCardValue;
    private SpriteRenderer hideCardSpriteRenderer;
    private int hideCardIndex;
    void Start()
    {
        InitVariables();
        InitializeDeck();
    }
    private void OnEnable()
    {
        UiManager.Instance.actionHit += PlayerHit;
        UiManager.Instance.actionStand += PlayerStand;
        UiManager.Instance.actionStartGame += StartGame;
        UiManager.Instance.actionSplit += SplitCard;
    }
    private void OnDisable()
    {
        UiManager.Instance.actionHit -= PlayerHit;
        UiManager.Instance.actionStand -= PlayerStand;
        UiManager.Instance.actionStartGame -= StartGame;
        UiManager.Instance.actionSplit -= SplitCard;
    }
    void InitVariables()
    {
        playerCardSortingOrder = 1;
        dealerCardSortingOrder = 1;
    }
    void InitializeDeck()
    {
        for (int i = 0; i < cardSprites.Count; i++)
        {
            deck.Add(i);
        }
    }
    private void StartGame()
    {
        StartCoroutine(DealInitialCards());
    }
    IEnumerator DealInitialCards()
    {
        bool isDone = false;
        yield return new WaitForSeconds(0.5f);
        /*
         Here Spawing 4 cards for Initial stage
         1st card for player, 2nd for Dealer, 3rd for Player and 4th for Dealer and that is Hide card.
         */
        for(int i=1; i<=4; i++)
        {
            if(i%2 == 0)
            {
                if(i==4)
                {
                    isDone = false;
                    DealerHideCardSpawn(() => { isDone = true; }); // This last card for Dealer
                    yield return new WaitUntil(() => isDone);
                }
                else
                {
                    isDone = false;
                    DealerCardSpawn(() => { isDone = true; });
                    yield return new WaitUntil(() => isDone);
                }
            }
            else
            {
                if(i == 3)
                {
                    isDone = false;
                    PlayerCardSpawn(() => { isDone = true; },true);
                    yield return new WaitUntil(() => isDone);
                }
                else
                {
                    isDone = false;
                    PlayerCardSpawn(() => { isDone = true; });
                    yield return new WaitUntil(() => isDone);
                }

            }
        }
        UpdateScoreText();
        UiManager.Instance.actionCompleteCardDistribution?.Invoke();
    }
    private void PlayerCardSpawn(Action actionCardPlaced, bool is2ndCard = false)
    {
        int currentCardValue = DrawCard(playerCardStartPos, ref playerCardCount, playerCardOffset, false, playerCardSortingOrder++, actionCardPlaced, is2ndCard);
        playerLastCardValue = currentCardValue;
        playerScore += AceCardValueCalculate(currentCardValue, PlayerType.PLAYER);
    }   
    private void DealerCardSpawn(Action actionCardPlaced)
    {
        int currentCardValue = DrawCard(dealerCardStartPos, ref dealerCardCount, dealerCardOffset, false, dealerCardSortingOrder++, actionCardPlaced);
        dealerScore += AceCardValueCalculate(currentCardValue, PlayerType.DEALER);
    }  
    private void DealerHideCardSpawn(Action actionCardPlaced)
    {
        int currentCardValue = DrawCard(dealerCardStartPos, ref dealerCardCount, dealerCardOffset, true, dealerCardSortingOrder++, actionCardPlaced);
        dealerHiddenCardValue = AceCardValueCalculate(currentCardValue, PlayerType.DEALER);
    }
    private int AceCardValueCalculate(int value,PlayerType playerType)
    {
        if(value == 11)
        {
            if(PlayerType.PLAYER == playerType) //Here Player logic
            {
                if (playerScore + 11 <= 15)
                {
                    return value;
                }
                else
                {
                    return 1;
                }
            }
            else // Here Dealer logic
            {
                if (dealerScore + 11 <= 15)
                {
                    return value;
                }
                else
                {
                    return 1;
                }
            }
        }
        else
        {
            return value;
        }
    }
    void RotateCard(GameObject goCard, Vector3 endValue, float duration, Sprite spriteCardFace, bool isBackFace = false)
    {
        if(isBackFace)
        {
            goCard.transform.DORotate(endValue, duration)
                .OnComplete(() => {
                    SpriteRenderer spriteRender = goCard.GetComponent<SpriteRenderer>();
                    if (spriteRender != null) spriteRender.sprite = spriteCardFace;
                    goCard.transform.eulerAngles = Vector3.zero;
                    goCard.transform.localScale = new Vector3(1,1,1);
                });
            return;
        }
        
        goCard.transform.DORotate(endValue, duration)
            .OnComplete(() => {
                SpriteRenderer spriteRender = goCard.GetComponent<SpriteRenderer>();
                if (spriteRender != null) spriteRender.sprite = spriteCardFace;
            });
    }
    int DrawCard(Transform targetPos, ref int cardCount, Vector3 offset, bool isHideCard,int sortingOrder, Action actionCardPlaceDone, bool isPlayer2ndcard = false)
    {
        int randomIndex = 0;
        int cardValue = 0;
        if (isPlayer2ndcard)
        {
            if(playerLastCardValue != 0)
            {
                while(true)
                {
                    randomIndex = random.Next(deck.Count);
                    cardValue = GetCardValue(deck[randomIndex]);
                    if (cardValue != playerLastCardValue) break;
                }

            }
        }
        else
        {
            randomIndex = random.Next(deck.Count);
            cardValue = GetCardValue(deck[randomIndex]);
        }

        SpriteRenderer cardRenderer;
        if (isHideCard)
        {
            // Create and initially position the card at the central spawn point
            cardRenderer = Instantiate(cardBackRendererPrefab, centralSpawnPoint.position, Quaternion.identity);
            cardRenderer.sprite = cardBackSprites[0];
            hideCardSpriteRenderer = cardRenderer;
            hideCardIndex = deck[randomIndex];
        }
        else
        {
            // Create and initially position the card at the central spawn point
            cardRenderer = Instantiate(cardRendererPrefab, centralSpawnPoint.position, Quaternion.identity);
            cardRenderer.sprite = cardSprites[deck[randomIndex]];
        }
        cardRenderer.sortingOrder = sortingOrder;

        // Calculate the target position and rotation
        Vector3 targetCardPos = targetPos.position + offset * cardCount;
        Quaternion targetCardRot = Quaternion.Euler(0, 15f * cardCount, 0); // Rotate on Y axis

        // Animate the card from the Spawn point to the target position
        cardRenderer.transform.DOMove(targetCardPos, cardPlaceDuration).SetEase(easeCardPlace).OnComplete(()=> { actionCardPlaceDone?.Invoke(); });

        cardCount++;
        deck.RemoveAt(randomIndex);
        return cardValue;
    }

    int GetCardValue(int cardIndex)
    {
        string cardName = cardSprites[cardIndex].name;
        string cardValue = cardName.Split('_')[1];
        if(cardValue == "A")
        {
            return 11;
        }
        else if(cardValue == "K" || cardValue == "Q" || cardValue == "J")
        {
            return 10;
        }
        else
        {
            return int.Parse(cardValue);
        }
    }

    public void PlayerHit()
    {
        if (playerCardCount < 10) // Assuming a maximum of 10 cards
        {
            int cardValue = DrawCard(playerCardStartPos, ref playerCardCount, playerCardOffset,false, playerCardSortingOrder++, ()=> { });
            playerScore += cardValue;
            UpdateScoreText();

            if (playerScore > 21)
            {
                Sequence sequence = DOTween.Sequence();
                sequence.AppendInterval(2f);
                sequence.AppendCallback(() => {
                    string message = "Player Busts!";
                    UiManager.Instance.actionFinalResult(WinnerState.DEALER, message);
                    UiManager.Instance.actionEndGame?.Invoke();
                });
                sequence.Play();
            }
        }
    }

    private void PlayerStand()
    {
        StartCoroutine(DealerTurn());
    }

    private void SplitCard()
    {

    }
    IEnumerator DealerTurn()
    {
        RotateCard(hideCardSpriteRenderer.gameObject, new Vector3(0, 180, 0), 0.5f, cardSprites[hideCardIndex], true);
        dealerScore += dealerHiddenCardValue;
        yield return new WaitForSeconds(1f);
        dealerScore += DrawCard(dealerCardStartPos, ref dealerCardCount, dealerCardOffset,false, dealerCardSortingOrder++,() => { });

        while (dealerScore < 17 && dealerCardCount < 10)
        {
            yield return new WaitForSeconds(1f);
            int cardValue = DrawCard(dealerCardStartPos, ref dealerCardCount, dealerCardOffset,false, dealerCardSortingOrder++, () => { });
            dealerScore += cardValue;
        }

        yield return new WaitForSeconds(2f);
        UpdateScoreText();
        CheckWinner();
    }

    void UpdateScoreText()
    {
        playerScoreText.text = "Player Score: " + playerScore;
        dealerScoreText.text = "Dealer Score: " + dealerScore;
    }

    void CheckWinner()
    {
        if (playerScore > dealerScore && playerScore <= 21 || dealerScore > 21)
        {
            string message = "Player Wins!";
            UiManager.Instance.actionFinalResult(WinnerState.PLAYER, message);
        }
        else if (dealerScore > playerScore && dealerScore <= 21 || playerScore > 21)
        {
            string message = "Dealer Wins!";
            UiManager.Instance.actionFinalResult(WinnerState.DEALER, message);
        }
        else
        {
            string message = "It's a Tie!";
            UiManager.Instance.actionFinalResult(WinnerState.TIE, message);
        }

        UiManager.Instance.actionEndGame?.Invoke();
    }
}
