// Author: David Huynh

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterOrbController : MonoBehaviour
{
    // used to make master orb transparent over time
    private SpriteRenderer masterOrbRenderer;

    // used to only call ResetGame method once
    private bool activated = false;

    // how long animation lasts
    private float animationTime = 2;

    // the rate at which the master orb fades when touched by the player
    private float fadingRate = 0.75f;

    private void Start()
    {
        masterOrbRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        // calls ResetGame when player enters master orb
        if (!activated && collider.tag == "Player")
        {
            activated = true;
            
            GameStateManager.ToggleEndOfLevel();
            StartCoroutine(AbsorbedAnimation());
        }
    }

    // coroutine used to animate the master orb fading when collided by the player
    private IEnumerator AbsorbedAnimation()
    {
        while (animationTime > 0)
        {
            animationTime -= Time.deltaTime;
            
            // makes master orb more transparent
            masterOrbRenderer.color = new Color(1f, 1f, 1f, animationTime * fadingRate);
            
            yield return null;
        }

        if (PlayerCharacterStatus.playerStatus != null)
        {
            GameStateManager.OnQuitToTitle += PlayerVictory;
            GameStateManager.LoadEndingScene(GameStateManager.victorySceneName, 0.5f);
        }
    }

    private void PlayerVictory()
    {
        if (PlayerCharacterStatus.playerStatus != null)
        {
            Destroy(PlayerCharacterStatus.playerStatus.gameObject);
        }
    }
}
