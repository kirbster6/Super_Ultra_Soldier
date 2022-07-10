// Authors: David Huynh and Kirby Ammari

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    // used to check whether or not to let player pass through
    [SerializeField]
    private List<string> enemyTags = new List<string>();

    [SerializeField]
    private int enemiesRemainingThreshold;

    [SerializeField]
    private bool isLevelLoader;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.tag == "Player")
        {
            // checks to see if the player has killed all game objects with the specified tags
            //Kirby made changes to allow for the 2st gate to open with all enemies dead
            //And only open the second gate when the mini boss is killed.
            List<GameObject> enemies = new List<GameObject>();

            foreach (string tag in enemyTags)
            {
                GameObject[] enemiesToAdd = GameObject.FindGameObjectsWithTag(tag);

                foreach (GameObject enemy in enemiesToAdd)
                {
                    enemies.Add(enemy);
                }

            }

            if (enemies.Count <= enemiesRemainingThreshold)
            {
                if (isLevelLoader)
                {
                    GameStateManager.NextLevel();
                }

                gameObject.SetActive(false);
            }            
        }
    }
}
