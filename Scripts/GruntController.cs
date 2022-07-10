// Author: David Huynh

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntController : MonoBehaviour
{
    // reference to the player game object
    [SerializeField]
    protected GameObject playerTarget;

    // prefab of the grunt projectile
    [SerializeField]
    protected GameObject gruntProjectile;

    // reference to the player's position
    protected Vector3 targetPos;

    // distance used to determine grunt's behavior (to move closer to the player or to shoot)
    [SerializeField]
    protected float attackRadius;

    // how fast the grunt shoots between projectiles
    [SerializeField]
    protected float fireRate;

    // how long it's been since the grunt has shot a projectile
    protected float TimeSinceFired = 0;

    // the movement speed of the grunt
    protected int Speed = 3;

    // a range of int that decide how the grunt moves
    protected int movementDecider;

    private void Start()
    {
        // initializes playerTarget variable with the instance of the Player prefab in-game
        playerTarget = PlayerCharacterStatus.playerStatus.gameObject;

        // intializes movementDecider as a random int (0, 1, or 2) that correspond some movement direction (right, left, or none)
        movementDecider = UnityEngine.Random.Range(0, 3);
    }

    private void FixedUpdate()
    {
        // checks if player is still alive (is not null)
        if (playerTarget)
        {
            // makes the enemy track the player
            targetPos = playerTarget.transform.position;

            targetPos.x = targetPos.x - transform.position.x;
            targetPos.y = targetPos.y - transform.position.y;

            var angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            // controls enemy behavior (whether to shoot or move forward)
            if (Vector3.Distance(transform.position, playerTarget.transform.position) <= attackRadius)
            {
                if (TimeSinceFired >= fireRate)
                {
                    TimeSinceFired = 0;
                    ObjectPooler.instance.SpawnFromPool(gruntProjectile.tag, transform.position, Quaternion.Euler(transform.eulerAngles));
                    
                    // deciding which direction enemy moves in (left or right)
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        movementDecider = UnityEngine.Random.Range(0, 2);
                    }
                }
            }
            else
            {
                // has the enemy move forward to get into attackRadius
                transform.Translate(Vector3.up * Time.deltaTime * Speed);
            }

            // moving left or right
            if (movementDecider == 0)
            {
                transform.Translate(Vector3.right * Time.deltaTime * Speed);
            }
            else if (movementDecider == 1)
            {
                transform.Translate(Vector3.left * Time.deltaTime * Speed);
            }

            // calculating time for fire rate
            TimeSinceFired += Time.deltaTime;
        }
    }

    // this changes grunt's movement behavior such that it doesn't get caught on/keep running into a wall
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            if (movementDecider == 0)
            {
                movementDecider = 1;
            }
            else
            {
                movementDecider = 0;
            }
        }
    }
}
