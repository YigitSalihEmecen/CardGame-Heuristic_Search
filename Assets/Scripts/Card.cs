using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public int card_value;
    public string card_type;

    private game_controller gameController;
    private List<GameObject> hand;

    public void Initialize(game_controller controller, List<GameObject> hand)
    {
        this.gameController = controller;
        this.hand = hand;
    }

    void OnMouseDown()
    {
        if (gameController != null && hand != null)
        {
            gameController.DiscardCard(gameObject, hand);
        }
    }
}
