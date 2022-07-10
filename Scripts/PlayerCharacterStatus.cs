// Main author: David Huynh

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterStatus : CharacterStatus
{
    // singleton of the player
    public static PlayerCharacterStatus playerStatus;

    private void Awake()
    {
        if (playerStatus == null)
        {
            // keeps a reference of the PlayerCharacterStatus instance that contains the player's healthPoints field and other methods
            playerStatus = this;
            DontDestroyOnLoad(playerStatus);
        }
        else
        {
            Destroy(this);
        }
    }

    new public void TakeDamage(int damage)
    {
        healthPoints -= damage;
        healthBar.SetHealthPoints(healthPoints);//Tien-Yi Lee

        if (animator != null)
        {
            // sends signal to the Animator component to start animating
            animator.SetBool("tookDamage", true);

            // has the animation restart by stopping the coroutine that is currently running if it is
            if (takingDamageAnimationRunning)
            {
                StopCoroutine(animationCoroutine);
            }

            // starts a coroutine and keeps a reference of it to keep track of how long the animation should last
            animationCoroutine = TakeDamageAnimation();
            StartCoroutine(animationCoroutine);
        }

        // checks to see if the character is dead
        if (healthPoints <= 0)
        {
            // sets deathInGame field to true to avoid creating death particles when unneeded
            deathInGame = true;

            // sets up for when GameStateManager loads in Defeat Scene
            GameStateManager.OnQuitToTitle += PlayerDefeat;
            GameStateManager.LoadEndingScene(GameStateManager.defeatSceneName, 1.75f);
            
            Destroy(this.gameObject);
        }
    }

    // function that GameStateManager.OnQuitToTitle is set to and called when quitting to title
    private void PlayerDefeat()
    {

    }
}
