// Author: David Huynh

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBController : MonoBehaviour
{
    // used by FBCharacterStatus when the Final Boss reaches a certain health threshold to make its attacks faster and stronger
    [SerializeField]
    public bool secondPhase = false;
    //Kirby added audio
    [SerializeField]
    private AudioClip beamSound;

    [SerializeField]
    private AudioClip FBShootSound;

    [SerializeField]
    private AudioClip handSound;

    [SerializeField]
    public AudioSource audioSource;

    // reference to the player
    [SerializeField]
    private GameObject playerTarget;

    // FIELDS USED FOR PROJECTILE ATTACK //

    // projectile prefab
    [SerializeField]
    private GameObject FBProjectile;

    // used to ensure multiple projectile attacks don't occur simultaneously
    private bool currentlyProjectiling = false;


    // FIELDS USED FOR ARM ATTACK //

    // references to child game objects of arms
    [SerializeField]
    private GameObject LeftArmIdle;

    [SerializeField]
    private GameObject LeftArmWindUp;

    [SerializeField]
    private GameObject LeftArmAttack;

    [SerializeField]
    private GameObject RightArmIdle;

    [SerializeField]
    private GameObject RightArmWindUp;

    [SerializeField]
    private GameObject RightArmAttack;

    // to ensure multiple arm attacks don't happen at the same time
    private bool currentlyArmAttacking = false;

    // for how long windup arms appear
    private float windUpThreshold = 0.75f;

    // for how long attack arms appear
    private float armAttackThreshold = 1.75f;

    // used to keep reference to each arm used being used in the first phase for arm attack
    private GameObject currentArmIdle;
    private GameObject currentArmWindUp;
    private GameObject currentArmAttack;


    // FIELDS USED FOR BEAM ATTACK //

    // list of all beam origin child objects
    [SerializeField]
    private GameObject[] FBBeamOrigins;

    // list of all beam child objects
    [SerializeField]
    private GameObject[] FBBeams;

    // used to ensure multiple beam attacks don't happen at the same time
    private bool currentlyBeaming = false;

    // for how long beam carge up lasts
    private float beamChargeUpThreshold = 1f;

    // for how long beam attack lasts
    private float beamAttackThreshold = 1.5f;

    // FIELDS USED FOR TARGETING AND GENERAL BEHAVIOR

    // position of the player
    private Vector3 targetPos;

    // how fast Final Boss rotates
    private float rotationSpeed = 1f;
    
    // reference to base rotation speed
    private float initialRotationSpeed;

    // used to check whether or not to update facing direction of Final Boss
    private bool faceLocked = false;

    private float movementSpeed = 1f;

    // how far away player can be before Final Boss moves closer to them
    private float attackRadius = 10;

    // used to determine which attack to use (arm or beam)
    private int attackDecider;

    // how often Final Boss attacks
    [SerializeField]
    private int attackRate;

    private float timeSinceAttacked;

    private void Start()
    {
        // initializes playerTarget variable with the instance of the Player in-game
        playerTarget = PlayerCharacterStatus.playerStatus.gameObject;

        // making it so the Final Boss begins with only body and idle arms (no other child game object) active
        LeftArmWindUp.SetActive(false);
        LeftArmAttack.SetActive(false);
        RightArmWindUp.SetActive(false);
        RightArmAttack.SetActive(false);
        ToggleActiveGameObjects(FBBeams, false);
        ToggleActiveGameObjects(FBBeamOrigins, false);


        // setting up general behavior
        timeSinceAttacked = attackRate / 2;
        initialRotationSpeed = rotationSpeed;
    }

    private void FixedUpdate()
    {
        // used to decide whether or not to reset timeSincAttacked field
        bool attacked = false;

        // checks if player is still alive
        if (playerTarget)
        {
            // behavior for tracking (locking onto) the player
            if (!faceLocked)
            {
                targetPos = playerTarget.transform.position;

                targetPos.x = targetPos.x - transform.position.x;
                targetPos.y = targetPos.y - transform.position.y;
                
                float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;

                Quaternion wantedRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

                // makes there be a delay in rotation of Final Boss
                transform.rotation = Quaternion.Lerp(transform.rotation, wantedRotation, Time.deltaTime * rotationSpeed);
                
                // has Final Boss move towards the player if too far away
                if (Vector3.Distance(transform.position, targetPos) > attackRadius)
                {
                    transform.Translate(Vector3.up * Time.deltaTime * movementSpeed);
                }
            }

            // Final Boss attack behavior
            if (timeSinceAttacked >= attackRate)
            {
                // projectile attack
                if (!currentlyProjectiling)
                {
                    StartCoroutine(TimeBetweenProjectiles());
                }

                // used to decide which attack to do (arm or beam)
                int attackDecider = UnityEngine.Random.Range(0, 2);

                // arm attack
                if (attackDecider == 0 && !currentlyArmAttacking)
                {
                    attacked = true;

                    // if not in second phase, decides which arm to use
                    if (!secondPhase)
                    {
                        int armDecider = UnityEngine.Random.Range(0, 2);

                        if (armDecider == 0)
                        {
                            currentArmIdle = LeftArmIdle;
                            currentArmWindUp = LeftArmWindUp;
                            currentArmAttack = LeftArmAttack;
                            
                        }
                        else
                        {
                            currentArmIdle = RightArmIdle;
                            currentArmWindUp = RightArmWindUp;
                            currentArmAttack = RightArmAttack;
                        }
                    }

                    // begins arm attack
                    StartCoroutine(armAttackAnimation());
                    //Kirby Added Audio here
                    audioSource.PlayOneShot(handSound);
                }
                // beam attack
                else if (attackDecider == 1 && !attacked && !currentlyBeaming)
                {
                    attacked = true;

                    // begins beam attack
                    StartCoroutine(beamAttackAnimation());
                    //Kirby Added Audio here
                    audioSource.PlayOneShot(beamSound);
                }
            }

            // if the Final Boss uses an arm attack or beam attack, restarts timeSinceAttacked
            if (attacked)
            {
                timeSinceAttacked = 0;
            }
            else
            {
                timeSinceAttacked += Time.deltaTime;
            }
        }
    }

    private IEnumerator TimeBetweenProjectiles()
    {
        // sets currentlyProjectiling to true to avoid having multiple projectile attacks at the same time
        currentlyProjectiling = true;

        // set up
        float timeThreshold;
        if (!secondPhase)
        {
            timeThreshold = 1;
        }
        else
        {
            timeThreshold = 0.65f;
        }
        float timeElapsed = 0f;

        // shoots 10 waves of projectiles
        for (int i = 0; i < 10; i++)
        {
            // waiting period between each wave
            while (timeElapsed < timeThreshold)
            {
                yield return null;
                timeElapsed += Time.deltaTime;
            }

            // resets timeElapsed for the next wave
            timeElapsed = 0;

            // 5 projectiles shot for the wave
            for (int n = 0; n < 5; n++)
            {
                ObjectPooler.instance.SpawnFromPool(FBProjectile.tag, transform.position, Quaternion.Euler(transform.eulerAngles) * Quaternion.Euler(0, 0, 90 - (45 * n)));
            }

            // extra projectiles per wave if in second phase
            if (secondPhase)
            {
                if (i % 2 == 0)
                {
                    for (int n = 0; n < 4; n++)
                    {
                        ObjectPooler.instance.SpawnFromPool(FBProjectile.tag, transform.position, Quaternion.Euler(transform.eulerAngles) * Quaternion.Euler(0, 0, 67.5f - (45 * n)));
                    }
                }
            }
        }

        // sets to false to let Final Boss start another projectile attack
        currentlyProjectiling = false;
    }

    private IEnumerator armAttackAnimation()
    {
        // sets to true to prevent simulatenous arm attacks
        currentlyArmAttacking = true;

        // slows down rotation speed of Final Boss while doing arm attack
        rotationSpeed = rotationSpeed * 0.75f;

        // set up
        float windUpTime = 0;
        float armAttackTime = 0;

        // if not in second phase, uses currentArm(Idle, WindUp, Attack) fields to attack with only one arm
        if (!secondPhase)
        {
            SwitchStateOfArm(currentArmIdle, currentArmWindUp);

            while (windUpTime < windUpThreshold)
            {
                yield return null;
                windUpTime += Time.deltaTime;
            }

            faceLocked = true;
            SwitchStateOfArm(currentArmWindUp, currentArmAttack);

            while (armAttackTime < armAttackThreshold)
            {
                yield return null;
                armAttackTime += Time.deltaTime;
            }

            SwitchStateOfArm(currentArmAttack, currentArmIdle);
        }

        // uses both arms when in second phase
        else
        {
            SwitchStateOfArm(LeftArmIdle, LeftArmWindUp);
            SwitchStateOfArm(RightArmIdle, RightArmWindUp);

            while (windUpTime < windUpThreshold)
            {
                yield return null;
                windUpTime += Time.deltaTime;
            }

            faceLocked = true;
            SwitchStateOfArm(LeftArmWindUp, LeftArmAttack);
            SwitchStateOfArm(RightArmWindUp, RightArmAttack);

            while (armAttackTime < armAttackThreshold)
            {
                yield return null;
                armAttackTime += Time.deltaTime;
            }

            SwitchStateOfArm(LeftArmAttack, LeftArmIdle);
            SwitchStateOfArm(RightArmAttack, RightArmIdle);
        }

        // resets fields to make Final Boss behave normally
        currentlyArmAttacking = false;
        rotationSpeed = initialRotationSpeed;
        faceLocked = false;
    }

    // helper method to remove clutter in code of arm attack; switches which arm state is active in the scene
    private void SwitchStateOfArm(GameObject prevState, GameObject newState)
    {
        prevState.SetActive(false);
        newState.SetActive(true);
    }

    private IEnumerator beamAttackAnimation()
    {
        // sets to true to avoid having multiple beam attacks simultaneously
        currentlyBeaming = true;

        // set up
        float chargeUpTime = 0;
        float beamAttackTime = 0;

        // if not in second phase, only uses the front beam
        if (!secondPhase)
        {
            FBBeamOrigins[0].SetActive(true);

            while (chargeUpTime < beamChargeUpThreshold)
            {
                yield return null;
                chargeUpTime += Time.deltaTime;
            }

            FBBeams[0].SetActive(true);

            while (beamAttackTime < beamAttackThreshold)
            {
                yield return null;
                beamAttackTime += Time.deltaTime;
            }

            FBBeams[0].SetActive(false);
            FBBeamOrigins[0].SetActive(false);
        }
        // in second phase, uses all three beams
        else
        {
            ToggleActiveGameObjects(FBBeamOrigins, true);

            while (chargeUpTime < beamChargeUpThreshold)
            {
                yield return null;
                chargeUpTime += Time.deltaTime;
            }

            rotationSpeed = rotationSpeed * 1.25f;

            ToggleActiveGameObjects(FBBeams, true);
               
            while(beamAttackTime < beamAttackThreshold)
            {
                yield return null;
                beamAttackTime += Time.deltaTime;
            }

            ToggleActiveGameObjects(FBBeams, false);
            ToggleActiveGameObjects(FBBeamOrigins, false);

            rotationSpeed = initialRotationSpeed;
        }

        currentlyBeaming = false;
    }

    // helper method to remove clutter in beam attack; makes all gameobjects in the first arg active/inactive
    private void ToggleActiveGameObjects(GameObject[] gameObjs, bool boolSet)
    {
        foreach (GameObject obj in gameObjs)
        {
            obj.SetActive(boolSet);
        }
    }
}
