// filepath: /Users/yigitsalihemecen/CardGame_Heuristic/Assets/Scripts/game_controller.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class game_controller : MonoBehaviour
{
    // === Card Collections ===
    public List<GameObject> deck;
    public List<GameObject> discard_pile;
    public List<GameObject> player_hand;
    public List<GameObject> AI_hand;

    // === Positions and References ===
    public Transform player_hand_position;
    public Transform AI_hand_position;
    public Transform discard_position;
    public Transform middleOfScreen;
    public GameObject cardToDiscard;
    public GameObject middleCard;
    public GameObject restartButton;

    // === UI Elements ===
    public TextMeshProUGUI endGameText;
    public TextMeshProUGUI deckCountText;

    // === Game Settings ===
    public float hand_spacing = 0.6f;     // Spacing between cards
    public float vertical_spacing = 1.0f; // Vertical spacing for 6 card game mode

    // === Game State ===
    private bool isPlayerTurn = true;       // Track whose turn it is
    private bool isGameOver = false;        // Flag to indicate if the game is over
    private bool isDiscardInProgress = false; // Flag to track if a discard is in progress

    // === Unity Lifecycle Methods ===
    void Start()
    {
        Application.targetFrameRate = 120; // Set the target frame rate to 120

        int numberOfCards = GameMode.NumberOfCards;
        DealHands(numberOfCards); // Use the number of cards from the game mode
        StartPlayerTurn(); // Start with the player's turn

        Debug.Log("Number of Cards: " + numberOfCards);
    }

    // Update is called once per frame
    void Update()
    {
        deckCountText.text = deck.Count.ToString(); // Update the deck count text
    }

    // === Game Setup Methods ===
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

    // === Card Layout Methods ===
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

    // === Card Action Methods ===
    public void DiscardCard(GameObject card, List<GameObject> hand)
    {
        // Check if already discarding or not player's turn or game is over
        if (isDiscardInProgress || !isPlayerTurn || isGameOver) return;

        // Set the flag to true to prevent multiple discards
        isDiscardInProgress = true;

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
                                        
                                        // Reset the flag when the discard sequence is complete
                                        isDiscardInProgress = false;
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

    // === Turn Management Methods ===
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
        
        // Add a small delay before AI's action to make it feel more natural
        StartCoroutine(AIDiscardWithDelay());
    }

    IEnumerator AIDiscardWithDelay()
    {
        // Small delay before AI makes its move
        yield return new WaitForSeconds(0.5f);
        AIDiscardCard();
    }

    void EndAITurn()
    {
        if (isGameOver) return; // Prevent ending the AI's turn if the game is over

        Debug.Log("Ending AI's turn");
        StartPlayerTurn();
    }

    // === AI Strategy Methods ===
    void AIDiscardCard()
    {
        if (isGameOver || isDiscardInProgress) return; // Prevent AI from discarding if the game is over or a discard is in progress

        // Set the flag to true to prevent multiple discards
        isDiscardInProgress = true;

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
                                            
                                            // Reset the flag when the discard sequence is complete
                                            isDiscardInProgress = false;
                                        });
                                });
                        });
                });
        }
        else
        {
            Debug.Log("AI has no card to discard");
            // Reset the flag even if no card is discarded
            isDiscardInProgress = false;
            EndAITurn(); // End AI's turn if no card to discard
        }
    }

    GameObject GetAICardToDiscard()
    {
        if (AI_hand.Count == 0) return null;

        // Count cards of each suit in the AI's hand
        Dictionary<string, List<GameObject>> handSuitCounts = new Dictionary<string, List<GameObject>>();
        handSuitCounts["hearts"] = new List<GameObject>();
        handSuitCounts["diamonds"] = new List<GameObject>();
        handSuitCounts["clubs"] = new List<GameObject>();
        handSuitCounts["spades"] = new List<GameObject>();

        // Count cards of each suit in the discard pile
        Dictionary<string, int> discardSuitCounts = new Dictionary<string, int>();
        discardSuitCounts["hearts"] = 0;
        discardSuitCounts["diamonds"] = 0;
        discardSuitCounts["clubs"] = 0;
        discardSuitCounts["spades"] = 0;

        // Categorize AI's hand by suit
        foreach (GameObject card in AI_hand)
        {
            string suit = card.GetComponent<Card>().card_suit;
            handSuitCounts[suit].Add(card);
            Debug.Log("AI Hand - Card: " + card.name + " with suit: " + suit);
        }

        // Count suits in the discard pile
        foreach (GameObject card in discard_pile)
        {
            string suit = card.GetComponent<Card>().card_suit;
            discardSuitCounts[suit]++;
            Debug.Log("Discard pile - Card with suit: " + suit);
        }

        // Calculate potential for each suit and find the least discarded suit (which player might be collecting)
        // Strategy: Avoid the least discarded suit and prioritize the second least discarded suit
        
        // First, find the suit with the fewest discards (likely what player is collecting)
        string leastDiscardedSuit = "";
        int minDiscards = int.MaxValue;
        
        foreach (var suit in discardSuitCounts.Keys)
        {
            if (discardSuitCounts[suit] < minDiscards)
            {
                minDiscards = discardSuitCounts[suit];
                leastDiscardedSuit = suit;
            }
        }
        
        Debug.Log("Least discarded suit (likely player target): " + leastDiscardedSuit + " with " + minDiscards + " discards");

        // Calculate potential for each suit, but penalize the least discarded suit heavily
        Dictionary<string, float> suitPotential = new Dictionary<string, float>();

        foreach (var suit in handSuitCounts.Keys)
        {
            // Skip suits that aren't in our hand
            if (handSuitCounts[suit].Count == 0) continue;

            int totalCardsOfSuitDiscarded = discardSuitCounts[suit];
            float potential;
            
            // Special handling for the least discarded suit - heavily penalize it
            if (suit == leastDiscardedSuit)
            {
                // Make this suit very unattractive to collect (but not impossible)
                potential = 0.1f * handSuitCounts[suit].Count;
                Debug.Log("Penalizing least discarded suit: " + suit + " (likely player's target)");
            }
            else
            {
                // Normal calculation for other suits
                if (totalCardsOfSuitDiscarded > 0)
                {
                    potential = 1.0f - (totalCardsOfSuitDiscarded / 13.0f);
                }
                else
                {
                    // If this isn't the least discarded suit but has 0 discards, give it good potential
                    potential = 0.9f; // Slightly less than max to prefer suits with some but few discards
                }
                
                // Adjust potential based on how many cards of this suit we have
                potential *= handSuitCounts[suit].Count;
            }
            
            suitPotential[suit] = potential;
            Debug.Log("Suit: " + suit + " has " + handSuitCounts[suit].Count + " cards in hand, " +
                      totalCardsOfSuitDiscarded + " cards in discard, potential score: " + potential);
        }

        // Find the suit with the highest potential (best suit to keep)
        string bestSuit = "";
        float bestPotential = float.MinValue;

        foreach (var suitPair in suitPotential)
        {
            if (suitPair.Value > bestPotential && handSuitCounts[suitPair.Key].Count > 0)
            {
                bestPotential = suitPair.Value;
                bestSuit = suitPair.Key;
                Debug.Log("New best suit to keep: " + bestSuit + " with potential: " + bestPotential);
            }
        }

        // Now find the worst suit to discard from (avoid the best suit)
        string worstSuit = "";
        float worstPotential = float.MaxValue;

        foreach (var suitPair in suitPotential)
        {
            if (suitPair.Value < worstPotential && handSuitCounts[suitPair.Key].Count > 0)
            {
                worstPotential = suitPair.Value;
                worstSuit = suitPair.Key;
                Debug.Log("Worst suit to keep (will discard from): " + worstSuit + " with potential: " + worstPotential);
            }
        }

        // If we found the worst suit to discard from, discard a card of that suit
        if (!string.IsNullOrEmpty(worstSuit) && handSuitCounts[worstSuit].Count > 0)
        {
            Debug.Log("AI Strategy: Discarding card from worst potential suit: " + worstSuit + 
                     " (avoiding player's likely target: " + leastDiscardedSuit + ")");
            return handSuitCounts[worstSuit][0];
        }

        // Fallback to the old strategy: find the suit with least cards in hand
        string leastSuit = "";
        int minCount = int.MaxValue;

        foreach (var suitPair in handSuitCounts)
        {
            if (suitPair.Value.Count > 0 && suitPair.Value.Count < minCount)
            {
                minCount = suitPair.Value.Count;
                leastSuit = suitPair.Key;
            }
        }

        if (!string.IsNullOrEmpty(leastSuit) && handSuitCounts[leastSuit].Count > 0)
        {
            Debug.Log("Fallback to least cards strategy: Discarding from least suit: " + leastSuit);
            return handSuitCounts[leastSuit][0];
        }

        // Ultimate fallback: just return the first card in the hand
        Debug.Log("Using ultimate fallback - discarding first card in hand");
        return AI_hand[0];
    }

    // === Game State Methods ===
    void EndGameCheck()
    {
        if (deck.Count == 0)
        {
            EndGameText(CalculatePoints(player_hand, AI_hand));
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

    bool CalculatePoints(List<GameObject> player_hand, List<GameObject> AI_hand)
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

        // Get the screen width to calculate the center position
        float screenWidth = Screen.width;

        // Move the restartButton and endGameText to the center of the screen
        restartButton.GetComponent<RectTransform>().DOMoveX(screenWidth / 2, 1.0f);
        endGameText.GetComponent<RectTransform>().DOMoveX(screenWidth / 2, 1.0f);

        deckCountText.gameObject.SetActive(false); // Hide the deck count text
        // Set the game over flag to true
        isGameOver = true;
    }

    public void RestartGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(0);
    }
}
