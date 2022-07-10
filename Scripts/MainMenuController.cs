// Author: Tien-Yi Lee

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour //Tien-Yi Lee
{

    public void PlayGame()
    {
        GameStateManager.NewGame();
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit(); //Close the game
    }

}