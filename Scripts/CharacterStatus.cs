// Main author: David Huynh

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class CharacterStatus : MonoBehaviour
{
    // max health of the character
    [SerializeField]
    public int maxHealthPoints;
    //Kirby Added Audio Source and Sound Parameters
    //Audio Sound for Enemy Dying
    [SerializeField]
    private AudioClip deathSound;
    //Audio Source
    [SerializeField]
    public AudioSource audioSource;

    // how much health the character has currently
    [SerializeField]
    public int healthPoints;
    //Health Bar
    [SerializeField]
    public HealthBar healthBar; 

    // avoids having the characters instantiate death particles unless they are killed in-game
    protected bool deathInGame = false;

    // a reference to the game object that's spawned when the character dies
    [SerializeField]
    protected GameObject deathParticles;

    // used to animate a character taking damage
    protected Animator animator;

    // keeping a reference to only have a single coroutine running at a time
    protected IEnumerator animationCoroutine;

    // used decide whether to stop the current coroutine and start a new one, or simply start a new one since there's no current coroutine running
    protected bool takingDamageAnimationRunning = false;

    private void Start()
    {
        // initializing healthPoints to the character's max
        healthPoints = maxHealthPoints;
        healthBar.SetMaxHealthPoints(maxHealthPoints); //Set healthBar to maxHealthPoints Tien-Yi Lee

        // keeping a reference to this GameObject's Animator component to be used when animating them taking damage
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("tookDamage", false);
        }
    }

    // method called when the character is hit with an attack and takes damage
    public async Task TakeDamage(int damage)
    {
        healthPoints -= damage;
        healthBar.SetHealthPoints(healthPoints); //Set healthBar to current healthPoints Tien-Yi

        if (animator != null)
        {
            // sends signal to the Animator component to start animating
            animator.SetBool("tookDamage", true);

            // has the animation restart by stopping the coroutine that is currently running if it is
            if (takingDamageAnimationRunning)
            {
                StopCoroutine(animationCoroutine);
            }
            
            // starts a coroutine and keeps a reference of it to keep track of how long the animation should last
            animationCoroutine = TakeDamageAnimation();
            StartCoroutine(animationCoroutine);
        }

        // checks to see if the character is dead
        if (healthPoints <= 0)
        {
            // sets deathInGame field to true to avoid creating death particles when unneeded
            deathInGame = true;
            //Kirby Added Sound Effect Here and also the await Task.Delay to allow the sound to play
            audioSource.PlayOneShot(deathSound);
            await Task.Delay(105);
            Destroy(this.gameObject);
        }
    }

    // coroutine used to signal when the taking damage animation should end
    protected IEnumerator TakeDamageAnimation()
    {
        // keeps track that a coroutine is now running
        takingDamageAnimationRunning = true;

        // keeps animation running for 0.1 seconds
        float timeElapsed = 0f;
        
        while (timeElapsed < 0.1)
        {
            yield return null;
            timeElapsed += Time.deltaTime;
        }

        // sends signal to Animator component to stop animating
        animator.SetBool("tookDamage", false);

        // keeps track that there is now no coroutine running
        takingDamageAnimationRunning = false;
    }

    // instantiates a death particles sprite where the character was killed
    protected void OnDestroy()
    {
        if (deathInGame)
        {
            ObjectPooler.instance.SpawnFromPool(deathParticles.tag, transform.position, Quaternion.Euler(transform.eulerAngles));
            //Instantiate(deathParticles, transform.position, Quaternion.Euler(transform.eulerAngles));
        }
    }
}
