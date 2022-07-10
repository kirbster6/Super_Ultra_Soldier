// Author: David Huynh

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileTargeting : ProjectileTargeting
{
    // behavior for what to do when colliding with another object
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!hitACollider && Array.IndexOf(tagsToPassThrough, collider.tag) == -1) // checks if the collider's tag is in tagsToPassThrough; this decides if the grunt projectile should pass through the collider or not
        {
            hitACollider = true;

            // deals damage to the player if colliding with them
            if (collider.tag == "Player")
            {
                PlayerCharacterStatus.playerStatus.TakeDamage(Damage);
            }

            // returns to object pool
            OnObjectReturn();
        }
        else if (airTime > maxAirTime)
        {
            OnObjectReturn();
        }
    }
}
