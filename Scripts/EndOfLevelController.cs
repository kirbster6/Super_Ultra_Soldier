// Author: David Huynh

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfLevelController : MonoBehaviour
{
    // toggles GameStateManager.reachedEndOfLevel field between true and false when collided by the player
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            GameStateManager.ToggleEndOfLevel();
            this.gameObject.SetActive(false);
        }
    }
}
