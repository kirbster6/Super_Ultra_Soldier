// Author: David Huynh

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperController : MonoBehaviour
{
    // reference to the player game object
    [SerializeField]
    private GameObject playerTarget;

    // prefab of the sniper projectile
    [SerializeField]
    private GameObject sniperProjectile;

    // reference to the player's position
    private Vector3 targetPos;

    // how long it takes between projectile shots
    private float fireRate = 5;

    // keeps track of how long it's been since the sniper has fired a projectile
    private float TimeSinceFired = 0;

    private void Start()
    {

        // initializes playerTarget field with the instance of the Player in-game
        playerTarget = PlayerCharacterStatus.playerStatus.gameObject;

        TimeSinceFired += UnityEngine.Random.Range(0, 3);
    }

    private void FixedUpdate()
    {
        // checks if player is still alive
        if (playerTarget)
        {
            // makes the enemy track the player
            targetPos = playerTarget.transform.position;

            targetPos.x = targetPos.x - transform.position.x;
            targetPos.y = targetPos.y - transform.position.y;

            var angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            // controls sniper's shooting behavior
            if (TimeSinceFired >= fireRate)
            {
                // ADD sound effect and enemy animation for shooting
                TimeSinceFired = 0;
                ObjectPooler.instance.SpawnFromPool(sniperProjectile.tag, transform.position, Quaternion.Euler(transform.eulerAngles));
            }

            TimeSinceFired += Time.deltaTime;
        }
    }
}
