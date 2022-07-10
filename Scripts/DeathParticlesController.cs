// Author: David Huynh

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DeathParticlesController : MonoBehaviour, IPooledObject
{
    // used to determine how long the game object should last before returning to pool
    [SerializeField]
    private int timeThreshold;

    // used to keep track of how long game object has existed
    private float timeElapsed = 0;

    private void Update()
    {
        // has the sprite do its animation for some time before returning to the object pool
        if (timeElapsed > timeThreshold * 0.5f)
        {
            OnObjectReturn();
        }
        else
        {
            timeElapsed += Time.deltaTime;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void OnObjectSpawn()
    {

    }

    // resets fields to be reused later
    public void OnObjectReturn()
    {
        timeElapsed = 0;

        ObjectPooler.instance.ReturnToPool(gameObject);
    }
}
