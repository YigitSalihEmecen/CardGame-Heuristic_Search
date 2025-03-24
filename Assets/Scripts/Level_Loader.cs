using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_Loader : MonoBehaviour
{
    public void LoadScene0(int numberOfCards)
    {
        GameMode.NumberOfCards = 3;
        SceneManager.LoadScene(0);
    }

    public void LoadScene1(int numberOfCards)
    {
        GameMode.NumberOfCards = 6;
        SceneManager.LoadScene(0);
    }
}
