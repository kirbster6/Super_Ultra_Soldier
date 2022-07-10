// Main author: David Huynh

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FBCharacterStatus : CharacterStatus
{
    // singleton
    public static FBCharacterStatus FBStatus;

    // instantiates a game object when Final Boss is defeated; used to reset the game and return to title
    [SerializeField]
    GameObject masterOrbPrefab;

    // reference to the script controlling the behavior of the Final Boss; used when switching to second phase
    [SerializeField]
    private FBController fbController;

    // reference to a child game object of Final Boss; used to set up animation
    [SerializeField]
    private GameObject body;

    private void Awake()
    {
        if (FBStatus == null)
        {
            // creating singleton
            FBStatus = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        // initializing healthPoints to max of Final Boss
        healthPoints = maxHealthPoints;
        healthBar.SetMaxHealthPoints(maxHealthPoints); //Set healthBar to  MaxHealthPoints Tien-Yi
        // keeping a reference to the body child object's Animator
        animator = body.GetComponent<Animator>();

        // setting up animator's parameter
        if (animator != null)
        {
            animator.SetBool("tookDamage", false);
        }
    }

    new public void TakeDamage(int damage)
    {
        // calls CharacterStatus.TakeDamage method and then begins determining if to begin second phase
        base.TakeDamage(damage);

        if (!fbController.secondPhase && healthPoints <= maxHealthPoints * 0.75)
        {
            fbController.secondPhase = true;
        }
    }

    new private void OnDestroy()
    {
        if (deathInGame)
        {
            // spawns death particles for every child object
            float i = 0;
            foreach (Transform child in transform)
            {
                ObjectPooler.instance.SpawnFromPool(deathParticles.tag, child.position, Quaternion.Euler(child.eulerAngles * i));
                i += 10;
            }

            // spawns death particles on the parent object
            ObjectPooler.instance.SpawnFromPool(deathParticles.tag, transform.position, Quaternion.Euler(transform.eulerAngles));

            // instantiates a master orb
            GameObject.Instantiate(masterOrbPrefab, transform.position, Quaternion.Euler(transform.eulerAngles));
        }
    }
}
