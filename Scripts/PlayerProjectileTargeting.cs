// Author: David Huynh

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileTargeting : ProjectileTargeting
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // destroys projectile on collision
        if (!hitACollider && Array.IndexOf(tagsToPassThrough, collider.tag) == -1) // checks to see if the player projectile should pass through this collider or not
        {
            hitACollider = true;
            // checks if collider is an enemy (ignores if not), and decreasing their healthPoints and destroying it if healthPoints <= 0
            if (collider.tag == "Enemy")
            {
                collider.gameObject.GetComponent<CharacterStatus>().TakeDamage(Damage);
            }
            else if (collider.tag == "FinalBoss")
            {
                FBCharacterStatus.FBStatus.TakeDamage(Damage);
            }

            OnObjectReturn();
        }
    }
}
