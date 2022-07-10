// Main author: David Huynh

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTargeting : MonoBehaviour, IPooledObject
{
    // an array of string tags used when deciding if the projectile should pass through the collider or not
    [SerializeField]
    protected string[] tagsToPassThrough;
    //Kirby added Audio Variables
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip shootSound;

    // reference to Animator component used when projectile collides with certain objects
    protected Animator animator;

    private float animationTime = 0.1f;

    // how long before the projectile will simply return to the object pool
    protected float maxAirTime = 5;

    protected float airTime = 0;

    // used to have projectile only hit one object before returning to object pool
    protected bool hitACollider = false;

    // how fast projectile moves
    [SerializeField]
    protected float Speed;

    // how much damage projectile does to characters' healthPoints
    [SerializeField]
    protected int Damage;

    // the amount of distance added when the projectile is first spawned
    [SerializeField]
    protected float extraDistanceWhenSpawned = 0;

    private void Awake()
    {
        // this is done because the ObjectPooler instance keeps a reference to all projectile objects; avoids having instantiate more projectile when a new level loads or game resets
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetBool("impacted", false);
        }
    }


    private void FixedUpdate()
    {
        airTime += Time.deltaTime;

        // stops the projectile from moving after it hits another object
        if (!hitACollider)
        {
            // makes the projectile move forward in its current direction to some speed
            transform.Translate(Vector3.up * Time.deltaTime * Speed);
        }

        if (airTime > maxAirTime)
        {
            OnObjectReturn();
        }
    }

    // called in inherited classes' OnObjectReturn methods
    // coroutine used to animate projectile impact on collision
    protected IEnumerator ImpactAnimation(GameObject projectile)
    {
        float timeElapsed = 0f;

        while (timeElapsed < animationTime)
        {
            yield return null;
            timeElapsed += Time.deltaTime;
        }

        // resets projectile to be reused later
        airTime = 0;
        hitACollider = false;
        animator.SetBool("impacted", false);
        
        // returns projectile to object pool
        ObjectPooler.instance.ReturnToPool(projectile);
    }

    // called to set up the projectile
    public void OnObjectSpawn()
    {
        
        // to make the projectile appear more forward from the character's center position, so it looks like it's coming out of his barrel
        gameObject.SetActive(false);
        transform.Translate(Vector3.up * extraDistanceWhenSpawned);
        gameObject.SetActive(true);
        //Kirby Added Sound effect for enemy bullets
        audioSource.PlayOneShot(shootSound);
    }

    // called to reset the state of the projectile
    public void OnObjectReturn()
    {
        if (animator != null && hitACollider)
        {
            animator.SetBool("impacted", true);
            StartCoroutine(ImpactAnimation(gameObject));
        }
        else
        {
            airTime = 0;
            hitACollider = false;
            ObjectPooler.instance.ReturnToPool(gameObject);
        }
    }
}
