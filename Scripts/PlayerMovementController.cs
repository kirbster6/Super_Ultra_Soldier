// Author: David Huynh

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed;

    // reference to the main camera used to keep it following the player's movements
    [SerializeField]
    private GameObject playerCamera;

    private void FixedUpdate()
    {
        // player movement 
        if (GameStateManager.state == GameStateManager.GAMESTATE.PLAYING)
        {
            if (Input.GetKey(KeyCode.W)) // moves Player up while W key is pushed
            {
                transform.position += new Vector3(0, 1, 0) * movementSpeed;
            }

            if (Input.GetKey(KeyCode.A)) // moves Player left while A key is pushed
            {
                transform.position += new Vector3(-1, 0, 0) * movementSpeed;
            }

            if (Input.GetKey(KeyCode.S)) // moves Player down while S key is pushed
            {
                transform.position += new Vector3(0, -1, 0) * movementSpeed;
            }

            if (Input.GetKey(KeyCode.D)) // moves Player right while D key is pushed
            {
                transform.position += new Vector3(1, 0, 0) * movementSpeed;
            }


            // keeps player camera centered on Player
            if (playerCamera == null)
            {
                playerCamera = GameObject.FindWithTag("MainCamera");
            }
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, playerCamera.transform.position.z);
        }
    }
}
