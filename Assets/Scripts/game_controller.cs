using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    public float hand_spacing = 0.6f; // Spacing between cards

    private bool isPlayerTurn = true; // Track whose turn it is

    void Start()
    {
        DealHands(3); // Example: deal 3 cards to each hand
        StartPlayerTurn(); // Start with the player's turn
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
            if (deck.Count > 0)
            {
                GameObject card = Instantiate(deck[0], middleOfScreen.position, Quaternion.identity);
                player_hand.Add(card);
                deck.RemoveAt(0);
                card.transform.DOMove(player_hand_position.position, 1.0f).OnComplete(() => ArrangeHand(player_hand, player_hand_position)); // Move to player's hand and arrange
                card.GetComponent<Card>().Initialize(this, player_hand); // Initialize the card with the game controller and hand
            }
            if (deck.Count > 0)
            {
                GameObject card = Instantiate(deck[0], middleOfScreen.position, Quaternion.identity);
                AI_hand.Add(card);
                deck.RemoveAt(0);
                card.transform.DOMove(AI_hand_position.position, 1.0f).OnComplete(() => ArrangeHand(AI_hand, AI_hand_position)); // Move to AI's hand and arrange
            }
        }
    }

    void ArrangeHand(List<GameObject> hand, Transform handPosition)
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

    public void DiscardCard(GameObject card, List<GameObject> hand)
    {
        if (!isPlayerTurn) return; // Prevent player from discarding during AI's turn

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
                                        ArrangeHand(player_hand, player_hand_position); // Rearrange the player hand
                                        EndPlayerTurn(); // End the player's turn
                                        CheckEndGameCondition(); // Check end game condition after discarding a card
                                    });
                                    
                            });
                    });
            });
    }

    public void DrawCard()
    {
        if (deck.Count > 0)
        {
            GameObject card = Instantiate(deck[0], middleOfScreen.position, Quaternion.identity);
            player_hand.Add(card);
            deck.RemoveAt(0);
            card.GetComponent<Card>().Initialize(this, player_hand); // Initialize the card with the game controller and hand
            CheckEndGameCondition(); // Check end game condition after drawing a card
        }
        else
        {
            Debug.Log("Deck is empty, cannot draw a card");
        }
    }

    void StartPlayerTurn()
    {
        isPlayerTurn = true;
        Debug.Log("Player's turn");
    }

    void EndPlayerTurn()
    {
        isPlayerTurn = false;
        StartAITurn();
    }

    void StartAITurn()
    {
        Debug.Log("AI's turn");
        // Implement AI logic here
        AIDiscardCard();
    }

    void AIDiscardCard()
    {
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
                                            ArrangeHand(AI_hand, AI_hand_position); // Rearrange the AI hand
                                            EndAITurn(); // End the AI's turn
                                            CheckEndGameCondition(); // Check end game condition after discarding a card
                                        });
                                });
                        });
                });
        }
    }

    GameObject GetAICardToDiscard()
    {
        cardToDiscard = null; // Default to the first card in hand

        List<string> suits = new List<string>();
        suits = new List<string> { "hearts", "diamonds", "clubs", "spades" };

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
        return cardToDiscard;
    }

    void DrawCardForAI()
    {
        if (deck.Count > 0)
        {
            GameObject card = Instantiate(deck[0], middleOfScreen.position, Quaternion.identity);
            AI_hand.Add(card);
            deck.RemoveAt(0);
            CheckEndGameCondition(); // Check end game condition after drawing a card
        }
        else
        {
            Debug.Log("Deck is empty, AI cannot draw a card");
        }
    }

    void EndAITurn()
    {
        Debug.Log("Ending AI's turn");
        StartPlayerTurn();
    }

    void CheckEndGameCondition()
    {
        if (HasThreeCardsOfSameSuit(player_hand))
        {
            Debug.Log("Player wins!");
            EndGame();
        }
        else if (HasThreeCardsOfSameSuit(AI_hand))
        {
            Debug.Log("AI wins!");
            EndGame();
        }
    }

    bool HasThreeCardsOfSameSuit(List<GameObject> hand)
    {
        Dictionary<string, int> suitCount = new Dictionary<string, int>();

        foreach (GameObject card in hand)
        {
            string suit = card.GetComponent<Card>().card_suit;
            if (suitCount.ContainsKey(suit))
            {
                suitCount[suit]++;
            }
            else
            {
                suitCount[suit] = 1;
            }

            if (suitCount[suit] >= 3)
            {
                return true;
            }
        }

        return false;
    }

    void EndGame()
    {
        // Implement end game logic here, such as stopping the game, showing a message, etc.
        Debug.Log("Game Over");
        // Example: Disable player and AI actions
        isPlayerTurn = false;
        // Additional end game logic can be added here
    }
}
