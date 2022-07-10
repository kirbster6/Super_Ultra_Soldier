// Author: David Huynh

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBBodyPartAttackController : MonoBehaviour
{
    // how much damage the game object does
    [SerializeField]
    private int Damage;

    // bool tracking if the player is colliding with the body part
    private bool playerColliding = false;

    // how long the player must maintain contact with a Final Boss body part before taking damage
    private float timeStayedBeforeDamage = 0.5f;
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            PlayerCharacterStatus.playerStatus.TakeDamage(Damage);
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            if (!playerColliding)
            {
                StartCoroutine(OnStayTimer());
            }
        }
    }

    private IEnumerator OnStayTimer()
    {
        playerColliding = true;
        float timeElapsed = 0;

        while (timeElapsed < timeStayedBeforeDamage && playerColliding)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (timeElapsed >= timeStayedBeforeDamage)
        {
            PlayerCharacterStatus.playerStatus.TakeDamage(Damage);
        }

        playerColliding = false;
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            playerColliding = false;
        }
    }
}
