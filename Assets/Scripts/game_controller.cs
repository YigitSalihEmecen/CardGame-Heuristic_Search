using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class game_controller : MonoBehaviour
{
    public List<GameObject> deck;
    public List<GameObject> discard_pile;
    public List<GameObject> player_hand;
    public List<GameObject> AI_hand;

    public Transform player_hand_position;
    public Transform AI_hand_position;
    public Transform discard_position;

    public Transform middleOfScreen;

    public GameObject cardToDiscard;
    public GameObject middleCard;
    public GameObject restartButton; // Add reference to the restart button

    public float hand_spacing = 0.6f; // Spacing between cards
    public float vertical_spacing = 1.0f; // Vertical spacing for 6 card game mode

    private bool isPlayerTurn = true; // Track whose turn it is
    private bool isGameOver = false; // Flag to indicate if the game is over

    public TextMeshProUGUI endGameText;

    void Start()
    {
        int numberOfCards = GameMode.NumberOfCards;
        DealHands(numberOfCards); // Use the number of cards from the game mode
        StartPlayerTurn(); // Start with the player's turn

        Debug.Log("Number of Cards: " + numberOfCards);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DealHands(int numberOfCards)
    {
        // Shuffle the deck
        for (int i = 0; i < deck.Count; i++)
        {
            GameObject temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        // Clear previous hands
        player_hand.Clear();
        AI_hand.Clear();

        // Deal cards to player and AI
        for (int i = 0; i < numberOfCards; i++)
        {
            DrawCard();
            DrawCardForAI();
        }
    }

    void ArrangeHand(List<GameObject> hand, Transform handPosition, int numberOfCards)
    {
        if (numberOfCards == 3)
        {
            float startX = handPosition.position.x - ((hand.Count - 1) * hand_spacing) / 2; // Calculate the starting X position

            for (int i = 0; i < hand.Count; i++)
            {
                Vector3 targetPosition = new Vector3(
                    startX + (i * hand_spacing), // Calculate the X position for each card
                    handPosition.position.y, // Keep the Y position the same
                    handPosition.position.z // Keep the Z position the same
                );
                hand[i].transform.DOMove(targetPosition, 0.5f); // Animate the movement to the target position
                hand[i].transform.DORotate(Vector3.zero, 0.5f); // Ensure the card rotation remains unchanged
            }
        }
        else if (numberOfCards == 6)
        {
            float startX = handPosition.position.x - hand_spacing; // Calculate the starting X position for 3 cards per row
            float startY = handPosition.position.y + vertical_spacing / 2; // Starting Y position

            for (int i = 0; i < hand.Count; i++)
            {
                int row = i / 3; // Determine the row (0 or 1)
                int col = i % 3; // Determine the column (0, 1, or 2)

                Vector3 targetPosition = new Vector3(
                    startX + (col * hand_spacing), // Calculate the X position for each card
                    startY - (row * vertical_spacing), // Calculate the Y position for each row
                    handPosition.position.z // Keep the Z position the same
                );
                hand[i].transform.DOMove(targetPosition, 0.5f); // Animate the movement to the target position
                hand[i].transform.DORotate(Vector3.zero, 0.5f); // Ensure the card rotation remains unchanged
            }
        }
    }

    public void DiscardCard(GameObject card, List<GameObject> hand)
    {
        if (!isPlayerTurn || isGameOver) return; // Prevent player from discarding during AI's turn or if the game is over

        hand.Remove(card);
        discard_pile.Add(card);

        card.transform.DOScale(Vector3.one * 1.2f, 0.2f) // Scale up the card
            .OnComplete(() =>
            {
                card.transform.DOMove(middleOfScreen.position, 0.5f) // Move to the middle of the screen
                    .OnComplete(() =>
                    {
                        card.transform.DOMove(discard_position.position, 0.5f) // Move to the discard position
                            .OnComplete(() =>
                            {
                                card.transform.DOScale(Vector3.one, 0.2f) // Scale back to original size
                                    .OnComplete(() =>
                                    {
                                        DrawCard(); // Draw a new card
                                        ArrangeHand(player_hand, player_hand_position, GameMode.NumberOfCards); // Rearrange the player hand
                                        EndPlayerTurn(); // End the player's turn
                                    });
                            });
                    });
            });
    }

    public void DrawCard()
    {
        if (deck.Count > 0 && !isGameOver)
        {
            GameObject card = Instantiate(deck[0], middleOfScreen.position, Quaternion.identity);
            player_hand.Add(card);
            deck.RemoveAt(0);
            card.GetComponent<Card>().Initialize(this, player_hand); // Initialize the card with the game controller and hand
            ArrangeHand(player_hand, player_hand_position, GameMode.NumberOfCards); // Rearrange the player hand
            if (player_hand.Count == GameMode.NumberOfCards)
            {
                EndGameCheck();
            }
        }
        else
        {
            Debug.Log("Deck is empty, cannot draw a card");
        }
    }

    void StartPlayerTurn()
    {
        if (isGameOver) return; // Prevent starting the player's turn if the game is over

        isPlayerTurn = true;
        Debug.Log("Player's turn");
    }

    void EndPlayerTurn()
    {
        if (isGameOver) return; // Prevent ending the player's turn if the game is over

        isPlayerTurn = false;
        StartAITurn();
    }

    void StartAITurn()
    {
        if (isGameOver) return; // Prevent starting the AI's turn if the game is over

        Debug.Log("AI's turn");
        // Implement AI logic here
        AIDiscardCard();
    }

    void AIDiscardCard()
    {
        if (isGameOver) return; // Prevent AI from discarding if the game is over

        // Implement AI logic to discard a card
        GameObject cardToDiscard = GetAICardToDiscard();

        if (cardToDiscard != null)
        {
            AI_hand.Remove(cardToDiscard);
            discard_pile.Add(cardToDiscard);

            cardToDiscard.transform.DOScale(Vector3.one * 1.2f, 0.2f) // Scale up the card
                .OnComplete(() =>
                {
                    cardToDiscard.transform.DOMove(middleOfScreen.position, 0.5f) // Move to the middle of the screen
                        .OnComplete(() =>
                        {
                            cardToDiscard.transform.DOMove(discard_position.position, 0.5f) // Move to the discard position
                                .OnComplete(() =>
                                {
                                    cardToDiscard.transform.DOScale(Vector3.one, 0.2f) // Scale back to original size
                                        .OnComplete(() =>
                                        {
                                            DrawCardForAI(); // Draw a new card for AI
                                            ArrangeHand(AI_hand, AI_hand_position, GameMode.NumberOfCards); // Rearrange the AI hand
                                            EndAITurn(); // End the AI's turn
                                        });
                                });
                        });
                });
        }
        else
        {
            Debug.Log("AI has no card to discard");
            EndAITurn(); // End AI's turn if no card to discard
        }
    }

    GameObject GetAICardToDiscard()
    {
        cardToDiscard = null; // Default to the first card in hand

        List<string> suits = new List<string> { "hearts", "diamonds", "clubs", "spades" };

        foreach (string suit in suits)
        {
            List<GameObject> cardsInSuit = new List<GameObject>();

            foreach (GameObject card in AI_hand)
            {
                if (card.GetComponent<Card>().card_suit == suit)
                {
                    cardsInSuit.Add(card);
                }
            }

            if (cardsInSuit.Count == 1)
            {
                cardToDiscard = cardsInSuit[0];
                break;
            }
        }

        // If no single card found, discard the first card
        if (cardToDiscard == null && AI_hand.Count > 0)
        {
            cardToDiscard = AI_hand[0];
        }

        return cardToDiscard;
    }

    void DrawCardForAI()
    {
        if (deck.Count > 0 && !isGameOver)
        {
            GameObject card = Instantiate(deck[0], middleOfScreen.position, Quaternion.identity);
            AI_hand.Add(card);
            deck.RemoveAt(0);
            ArrangeHand(AI_hand, AI_hand_position, GameMode.NumberOfCards); // Rearrange the AI hand
            if (AI_hand.Count == GameMode.NumberOfCards)
            {
                EndGameCheck();
            }
        }
        else
        {
            Debug.Log("Deck is empty, AI cannot draw a card");
        }
    }

    void EndAITurn()
    {
        if (isGameOver) return; // Prevent ending the AI's turn if the game is over

        Debug.Log("Ending AI's turn");
        StartPlayerTurn();
    }

    void EndGameCheck()
    {
        if (deck.Count == 0)
        {
            EndGameText(CalculatPoints(player_hand, AI_hand));
        }

        if (AreAllCardsSameSuit(player_hand))
        {
            EndGameText(true);
        }
        else if (AreAllCardsSameSuit(AI_hand))
        {
            EndGameText(false);
        }
    }

    bool AreAllCardsSameSuit(List<GameObject> hand)
    {
        if (hand.Count == 0) return false;

        string suit = hand[0].GetComponent<Card>().card_suit;

        foreach (GameObject card in hand)
        {
            if (card.GetComponent<Card>().card_suit != suit)
            {
                return false;
            }
        }

        return true;
    }

    bool CalculatPoints(List<GameObject> player_hand, List<GameObject> AI_hand)
    {
        int playerPoints = 0;
        int aiPoints = 0;

        foreach (GameObject card in player_hand)
        {
            playerPoints += card.GetComponent<Card>().card_value;
        }

        foreach (GameObject card in AI_hand)
        {
            aiPoints += card.GetComponent<Card>().card_value;
        }

        return playerPoints < aiPoints;
    }

    void EndGameText(bool playerWon)
    {
        if (playerWon)
        {
            endGameText.text = "Player wins!";
        }
        else
        {
            endGameText.text = "AI wins!";
        }

        HandleEndGameState();
    }

    void HandleEndGameState()
    {
        // Slide the middleCard to the right, outside of the main camera
        middleCard.transform.DOMoveX(middleCard.transform.position.x + 10, 1.0f);

        // Slide the restartButton and endGameText from the left to the main camera
        restartButton.transform.DOMoveX(middleCard.transform.position.x, 1.0f);
        endGameText.transform.DOMoveX(middleCard.transform.position.x, 1.0f);

        // Set the game over flag to true
        isGameOver = true;
    }
}
