// Author: David Huynh

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    // list of all game object prefabs
    [SerializeField]
    private List<GameObject> prefabs;

    // a dictionary used for referencing Queues that lets objects spawn a pool's objects
    private Dictionary<string, Queue<GameObject>> poolQueueDictionary = new Dictionary<string, Queue<GameObject>>();

    // singleton of ObjectPooler
    public static ObjectPooler instance;

    // creates the singleton and initializes poolQueueDictionary with (tag, prefab) pairs
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);

            foreach (GameObject prefab in prefabs)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                poolQueueDictionary.Add(prefab.tag, objectPool);
            }
        }
        else
        {
            Destroy(this);
        }
    }

    // method other classes use to spawn a GameObject from their respective Pool
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (poolQueueDictionary.ContainsKey(tag))
        {
            // spawning an object by taking it out of its object pool queue
            Queue<GameObject> queuePool = poolQueueDictionary[tag];
            if (queuePool.Count > 0)
            {
                GameObject spawnedObject = queuePool.Dequeue();

                spawnedObject.SetActive(true);
                spawnedObject.transform.position = position;
                spawnedObject.transform.rotation = rotation;

                // calling the spawned object's OnObjectSpawn method for its specific set-up
                IPooledObject pooledObj = spawnedObject.GetComponent<IPooledObject>();
                if (pooledObj != null)
                {
                    pooledObj.OnObjectSpawn();
                }

                return spawnedObject;
            }
            // otherwise instantiating a new instance of the prefab
            else
            {
                // finding the GameObject prefab in prefabs to instantiate a new object
                GameObject prefab = null;
                foreach (GameObject p in prefabs)
                {
                    if (p.tag == tag)
                    {
                        prefab = p;
                        break;
                    }
                }
                if (prefab == null)
                {
                    Debug.LogFormat("No GameObject in prefabs list contains {0} tag", tag);
                    return null;
                }

                GameObject instantiatedObject = Instantiate(prefab);

                instantiatedObject.transform.position = position;
                instantiatedObject.transform.rotation = rotation;

                IPooledObject pooledObj = instantiatedObject.GetComponent<IPooledObject>();
                if (pooledObj != null)
                {
                    pooledObj.OnObjectSpawn();
                }

                return instantiatedObject;
            }
        }
        else
        {
            Debug.LogFormat("poolDictionary does not contain {0} tag", tag);
            return null;
        }
    }

    // method called in other classes when returning to their object pool
    public void ReturnToPool(GameObject returningObject)
    {
        returningObject.SetActive(false);
        poolQueueDictionary[returningObject.tag].Enqueue(returningObject);
    }
}
