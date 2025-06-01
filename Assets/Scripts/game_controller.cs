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

        Debug.Log("=== AI DECISION MAKING PROCESS ===");

        // STEP 1: Count cards of each suit in AI's hand
        Dictionary<string, List<GameObject>> handSuitCards = new Dictionary<string, List<GameObject>>();
        Dictionary<string, int> handSuitCounts = new Dictionary<string, int>();
        
        // Initialize all suits
        handSuitCards["hearts"] = new List<GameObject>();
        handSuitCards["diamonds"] = new List<GameObject>();
        handSuitCards["clubs"] = new List<GameObject>();
        handSuitCards["spades"] = new List<GameObject>();
        
        handSuitCounts["hearts"] = 0;
        handSuitCounts["diamonds"] = 0;
        handSuitCounts["clubs"] = 0;
        handSuitCounts["spades"] = 0;

        // Categorize AI's hand by suit
        foreach (GameObject card in AI_hand)
        {
            string suit = card.GetComponent<Card>().card_suit;
            handSuitCards[suit].Add(card);
            handSuitCounts[suit]++;
        }

        Debug.Log("STEP 1 - AI Hand Analysis:");
        Debug.Log("Hearts: " + handSuitCounts["hearts"] + " cards");
        Debug.Log("Diamonds: " + handSuitCounts["diamonds"] + " cards");
        Debug.Log("Clubs: " + handSuitCounts["clubs"] + " cards");
        Debug.Log("Spades: " + handSuitCounts["spades"] + " cards");

        // STEP 2: Count discarded cards by suit and create ordered list
        Dictionary<string, int> discardSuitCounts = new Dictionary<string, int>();
        discardSuitCounts["hearts"] = 0;
        discardSuitCounts["diamonds"] = 0;
        discardSuitCounts["clubs"] = 0;
        discardSuitCounts["spades"] = 0;

        foreach (GameObject card in discard_pile)
        {
            string suit = card.GetComponent<Card>().card_suit;
            discardSuitCounts[suit]++;
        }

        // Create ordered list from least discarded to most discarded
        List<KeyValuePair<string, int>> discardOrder = new List<KeyValuePair<string, int>>();
        foreach (var pair in discardSuitCounts)
        {
            discardOrder.Add(pair);
        }
        discardOrder.Sort((x, y) => x.Value.CompareTo(y.Value)); // Sort by discard count (ascending)

        Debug.Log("STEP 2 - Discard Pile Analysis (least to most discarded):");
        for (int i = 0; i < discardOrder.Count; i++)
        {
            Debug.Log((i + 1) + ". " + discardOrder[i].Key + ": " + discardOrder[i].Value + " discarded");
        }

        // STEP 3: Find suit with least amount of cards in hand
        List<string> suitsWithLeastCards = new List<string>();
        int minCardsInHand = int.MaxValue;

        // Find minimum number of cards in hand (only count suits we actually have)
        foreach (var pair in handSuitCounts)
        {
            if (pair.Value > 0 && pair.Value < minCardsInHand)
            {
                minCardsInHand = pair.Value;
            }
        }

        // Find all suits with this minimum count
        foreach (var pair in handSuitCounts)
        {
            if (pair.Value == minCardsInHand && pair.Value > 0)
            {
                suitsWithLeastCards.Add(pair.Key);
            }
        }

        Debug.Log("STEP 3 - Suits with least cards in hand (" + minCardsInHand + " cards):");
        foreach (string suit in suitsWithLeastCards)
        {
            Debug.Log("- " + suit);
        }

        // STEP 4: Choose which suit to discard from
        string suitToDiscardFrom = "";

        if (suitsWithLeastCards.Count == 1)
        {
            // Only one suit has the minimum cards, choose it
            suitToDiscardFrom = suitsWithLeastCards[0];
            Debug.Log("STEP 4 - Decision: Only one suit with least cards, choosing: " + suitToDiscardFrom);
        }
        else if (suitsWithLeastCards.Count > 1)
        {
            // Multiple suits have same minimum cards, choose the most discarded one among them
            Debug.Log("STEP 4 - Multiple suits tied, checking discard rates...");
            
            string mostDiscardedAmongTied = "";
            int highestDiscardCount = -1;
            
            foreach (string suit in suitsWithLeastCards)
            {
                int discardCount = discardSuitCounts[suit];
                Debug.Log("- " + suit + " has " + discardCount + " discards");
                
                if (discardCount > highestDiscardCount)
                {
                    highestDiscardCount = discardCount;
                    mostDiscardedAmongTied = suit;
                }
            }

            // Check if there are still ties in discard count
            List<string> tiedInDiscards = new List<string>();
            foreach (string suit in suitsWithLeastCards)
            {
                if (discardSuitCounts[suit] == highestDiscardCount)
                {
                    tiedInDiscards.Add(suit);
                }
            }

            if (tiedInDiscards.Count == 1)
            {
                suitToDiscardFrom = mostDiscardedAmongTied;
                Debug.Log("STEP 4 - Decision: Choosing most discarded among tied suits: " + suitToDiscardFrom);
            }
            else
            {
                // Still tied, choose randomly
                int randomIndex = Random.Range(0, tiedInDiscards.Count);
                suitToDiscardFrom = tiedInDiscards[randomIndex];
                Debug.Log("STEP 4 - Decision: Still tied in discards, choosing randomly: " + suitToDiscardFrom);
            }
        }

        // STEP 5: Return a card from the chosen suit
        if (!string.IsNullOrEmpty(suitToDiscardFrom) && handSuitCards[suitToDiscardFrom].Count > 0)
        {
            GameObject cardToDiscard = handSuitCards[suitToDiscardFrom][0];
            Debug.Log("FINAL DECISION: Discarding " + cardToDiscard.name + " from suit: " + suitToDiscardFrom);
            Debug.Log("=== END AI DECISION PROCESS ===");
            return cardToDiscard;
        }

        // Ultimate fallback (should never reach here with proper logic)
        Debug.Log("ERROR: Fallback triggered - discarding first card in hand");
        Debug.Log("=== END AI DECISION PROCESS ===");
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
