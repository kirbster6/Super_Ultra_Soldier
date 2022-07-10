// Author: David Huynh

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimController : MonoBehaviour
{
    // prefan of the player projectile
    [SerializeField]
    private GameObject playerProjectile;
    //Kirby added audio
    [SerializeField]
    private AudioClip reloadSound;

    [SerializeField]
    public AudioSource audioSource;
    // max amount of ammo the player can have at a time
    [SerializeField]
    private int ammoCapacity;

    // how much ammo the player currently has
    [SerializeField]
    private int ammoAvailable;

    // how long it takes to reload
    [SerializeField]
    private float reloadTime;

    // how long it's been since the player started reloading
    private float SecondsSinceReload = 0;

    // booll whether or not the player is currently reloading
    private bool reloading = false;

    private void Update()
    {
        if (GameStateManager.state == GameStateManager.GAMESTATE.PLAYING)
        {
            // makes player track the mouse
            var mousePos = Input.mousePosition;
            mousePos.z = transform.position.z;
            var playerPos = Camera.main.WorldToScreenPoint(transform.position);
            mousePos.x = mousePos.x - playerPos.x;
            mousePos.y = mousePos.y - playerPos.y;

            var angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            // when player presses R to reload
            if (Input.GetKey(KeyCode.R) && !reloading && ammoAvailable < ammoCapacity)
            {
                //Kirby Added Sound effect for reloading
                audioSource.PlayOneShot(reloadSound);
                reloading = true;
            }

            // when reloading
            if (reloading)
            {
                if (SecondsSinceReload >= reloadTime)
                {
                    ammoAvailable = ammoCapacity;
                    reloading = false;
                    SecondsSinceReload = 0;
                    // ADD sound effect
                }
                else
                {
                    SecondsSinceReload += Time.deltaTime;
                }
            }

            // when player pushes left mouse button to shoot
            if (Input.GetMouseButtonDown(0) && ammoAvailable > 0 && !reloading)
            {
                ObjectPooler.instance.SpawnFromPool(playerProjectile.tag, transform.position, Quaternion.Euler(transform.eulerAngles));
                ammoAvailable--;
            }
        }
    }
}
