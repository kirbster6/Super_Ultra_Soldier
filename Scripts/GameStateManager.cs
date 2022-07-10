// Author: David Huynh

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    // a list reference of all levels
    [SerializeField]
    private List<string> levels = new List<string>();

    private int levelNumber = 0;

    public static string defeatSceneName = "Defeat";

    public static string victorySceneName = "Victory";

    public bool reachedEndOfLevel = false;

    public static Action OnQuitToTitle { get; set; }

    // singleton of GameStateManager
    private static GameStateManager instance;

    public enum GAMESTATE
    {
        MENU,
        PLAYING,
        PAUSED,
        GAMEOVER
    }

    public static GAMESTATE state { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
            GameStateManager.state = GAMESTATE.MENU; // replace with GAMESTATE.MENU when title menu implemented and use NewGame() instead
        }
        else
        {
            Destroy(this);
        }
    }

    // allows player to pause the game with Escape key
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (state == GAMESTATE.PLAYING || state == GAMESTATE.PAUSED))
        {
            GameStateManager.TogglePause();
        }
    }

    // toggles between paused and unpaused
    public static void TogglePause()
    {
        if (state == GAMESTATE.PLAYING)
        {
            state = GAMESTATE.PAUSED;
            Time.timeScale = 0;
        }
        else
        {
            state = GAMESTATE.PLAYING;
            Time.timeScale = 1; 
        }

    }

    // can be scrapped/rewritten when title screen is implemented
    public static void NewGame()
    {
        state = GAMESTATE.PLAYING;
        instance.levelNumber++;
        SceneManager.LoadScene(instance.levels[instance.levelNumber]);
    }

    // sets up fields to load in next level
    public static void NextLevel()
    {
        instance.levelNumber++;
        instance.reachedEndOfLevel = false;

        var operation = SceneManager.LoadSceneAsync(instance.levels[instance.levelNumber]);
        operation.allowSceneActivation = false;

        instance.StartCoroutine(instance.WaitForLoading(operation));
    }

    // coroutine used in NextLevel method to asynchronously load in next level
    private IEnumerator WaitForLoading(AsyncOperation operation)
    {
        // waits while next level is not 90% loaded and the player has yet to reach the end of the current level
        while (operation.progress < 0.9f || !instance.reachedEndOfLevel)
        {
            yield return null;
        }

        // loads in next level
        operation.allowSceneActivation = true;
    }

    // used at the starts and ends of levels to properly keep track of level loading
    public static void ToggleEndOfLevel()
    {
        if (instance.reachedEndOfLevel == true)
        {
            instance.reachedEndOfLevel = false;
        }
        else
        {
            instance.reachedEndOfLevel = true;
        }
    }

    public static void LoadEndingScene(string sceneName, float loadTime)
    {
        instance.StartCoroutine(instance.EndingLoadTime(sceneName, loadTime));
    }

    private IEnumerator EndingLoadTime(string sceneName, float loadTime)
    {
        float timeElapsed = 0;

        while (timeElapsed < loadTime)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        state = GAMESTATE.GAMEOVER;
        SceneManager.LoadScene(sceneName);
    }

    public static void QuitToTitle()
    {
        if (OnQuitToTitle != null)
        {
            OnQuitToTitle();
            OnQuitToTitle = null;
        }

        state = GAMESTATE.MENU;
        instance.levelNumber = 0;
        SceneManager.LoadScene(instance.levels[instance.levelNumber]);
    }
}
