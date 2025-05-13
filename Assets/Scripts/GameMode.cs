using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static class for sharing game mode data between scenes.
/// This class maintains game state information that persists across scene transitions.
/// </summary>
public static class GameMode
{
    /// <summary>
    /// The number of cards each player will use in the current game mode.
    /// Valid values are typically 3 or 6.
    /// </summary>
    public static int NumberOfCards { get; set; } = 3; // Default to 3 cards if not explicitly set
}
