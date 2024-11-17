using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager Instance;

    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of FirebaseManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
    }

    /// <summary>
    /// Creates a new pool for a specified tag and prefab.
    /// </summary>
    /// <param name="tag">The unique tag for the pool.</param>
    /// <param name="prefab">The prefab to pool.</param>
    /// <param name="size">The initial size of the pool.</param>
    public void CreatePool(string tag, GameObject prefab, int size,Transform parent = null)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} already exists.");
            return;
        }

        Queue<GameObject> objectPool = new Queue<GameObject>();

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.parent = parent;
            objectPool.Enqueue(obj);
        }

        poolDictionary.Add(tag, objectPool);
    }

    /// <summary>
    /// Spawns an object from the pool with the specified tag.
    /// </summary>
    /// <param name="tag">The tag of the pool.</param>
    /// <param name="position">The position to spawn the object.</param>
    /// <param name="rotation">The rotation to spawn the object.</param>
    /// <returns>The spawned GameObject, or null if the pool doesn't exist.</returns>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        if (objectToSpawn == null)
        {
            Debug.LogWarning($"No available objects in pool with tag {tag}.");
            return null;
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }

    /// <summary>
    /// Returns an object to the pool, deactivating it.
    /// </summary>
    /// <param name="tag">The tag of the pool.</param>
    /// <param name="objectToReturn">The GameObject to return to the pool.</param>
    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return;
        }

        objectToReturn.SetActive(false);
        poolDictionary[tag].Enqueue(objectToReturn);
    }
}