using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles level loading and game mode configuration for the card game.
/// This class is responsible for setting up game parameters before scene transitions.
/// </summary>
public class Level_Loader : MonoBehaviour
{
    /// <summary>
    /// Loads the 3-card game mode and transitions to the gameplay scene.
    /// </summary>
    public void LoadScene0()
    {
        // Configure for 3-card mode
        GameMode.NumberOfCards = 3;
        SceneManager.LoadScene(1); // Load the gameplay scene
    }

    /// <summary>
    /// Loads the 6-card game mode and transitions to the gameplay scene.
    /// </summary>
    public void LoadScene1()
    {
        // Configure for 6-card mode
        GameMode.NumberOfCards = 6;
        SceneManager.LoadScene(1); // Load the gameplay scene
    }
}
