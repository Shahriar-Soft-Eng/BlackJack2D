using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlackjackManager : MonoBehaviour
{
    public List<Sprite> cardSprites; // Assign card sprites in the inspector
    public SpriteRenderer cardRendererPrefab; // Prefab for card rendering
    public Transform centralSpawnPoint; // Central point where all cards come from
    public Transform playerCardStartPos; // Starting position for player cards
    public Transform dealerCardStartPos; // Starting position for dealer cards
    public TextMesh playerScoreText; // Use TextMesh for in-world text
    public TextMesh dealerScoreText;
    public TextMesh resultText;
    public Button hitButton;
    public Button standButton;

    private int playerScore = 0;
    private int dealerScore = 0;
    private Vector3 playerCardOffset = new Vector3(1.2f, -2f, 0f); // Offset for each new player card
    private Vector3 dealerCardOffset = new Vector3(1.2f, -2f, 0f); // Offset for each new dealer card
    private int playerCardCount = 0;
    private int dealerCardCount = 0;

    private List<int> deck = new List<int>();
    private System.Random random = new System.Random();

    void Start()
    {
        InitializeDeck();
        DealInitialCards();
        hitButton.onClick.AddListener(PlayerHit);
        standButton.onClick.AddListener(PlayerStand);
    }

    void InitializeDeck()
    {
        for (int i = 0; i < cardSprites.Count; i++)
        {
            deck.Add(i);
        }
    }

    void DealInitialCards()
    {
        playerScore = DrawCard(playerCardStartPos, ref playerCardCount, playerCardOffset, true);
        dealerScore = DrawCard(dealerCardStartPos, ref dealerCardCount, dealerCardOffset, true);
        playerScore += DrawCard(playerCardStartPos, ref playerCardCount, playerCardOffset, true);

        // Hide the dealer's second card (facing down)
        dealerCardCount++; // Increment to reserve space for the hidden card

        UpdateScoreText();
    }

    int DrawCard(Transform targetPos, ref int cardCount, Vector3 offset, bool animate)
    {
        int randomIndex = random.Next(deck.Count);
        int cardValue = GetCardValue(deck[randomIndex]);
        deck.RemoveAt(randomIndex);

        // Create and initially position the card at the central spawn point
        SpriteRenderer cardRenderer = Instantiate(cardRendererPrefab, centralSpawnPoint.position, Quaternion.identity);
        cardRenderer.sprite = cardSprites[randomIndex];

        // Calculate the target position and rotation
        Vector3 targetCardPos = targetPos.position + offset * cardCount;
        Quaternion targetCardRot = Quaternion.Euler(0, 15f * cardCount, 0); // Rotate on Y axis

        if (animate)
        {
            // Animate the card from the central point to the target position
            cardRenderer.transform.DOMove(targetCardPos, 0.5f).SetEase(Ease.OutBack);
            cardRenderer.transform.DORotateQuaternion(targetCardRot, 0.5f).SetEase(Ease.OutBack);
        }
        else
        {
            // Immediately place the card without animation
            cardRenderer.transform.position = targetCardPos;
            cardRenderer.transform.rotation = targetCardRot;
        }

        cardCount++; // Increment the card count for the next card's position

        return cardValue;
    }

    int GetCardValue(int cardIndex)
    {
        int value = (cardIndex % 13) + 1;
        return value > 10 ? 10 : value;
    }

    public void PlayerHit()
    {
        if (playerCardCount < 10) // Assuming a maximum of 10 cards
        {
            int cardValue = DrawCard(playerCardStartPos, ref playerCardCount, playerCardOffset, true);
            playerScore += cardValue;
            UpdateScoreText();

            if (playerScore > 21)
            {
                resultText.text = "Player Busts!";
                EndGame();
            }
        }
    }

    public void PlayerStand()
    {
        // Reveal the dealer's hidden card
        dealerCardCount--; // Move back to the hidden card position
        dealerScore += DrawCard(dealerCardStartPos, ref dealerCardCount, dealerCardOffset, false);

        while (dealerScore < 17 && dealerCardCount < 10) // Assuming a maximum of 10 cards
        {
            int cardValue = DrawCard(dealerCardStartPos, ref dealerCardCount, dealerCardOffset, false);
            dealerScore += cardValue;
        }

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
            resultText.text = "Player Wins!";
        }
        else if (dealerScore > playerScore && dealerScore <= 21 || playerScore > 21)
        {
            resultText.text = "Dealer Wins!";
        }
        else
        {
            resultText.text = "It's a Tie!";
        }

        EndGame();
    }

    void EndGame()
    {
        hitButton.interactable = false;
        standButton.interactable = false;
    }
}
