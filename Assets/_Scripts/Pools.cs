using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pools : MonoBehaviour
{
    public static Pools instance;
    
    [System.Serializable]
    public class Pool
    {
        public string name;
        public GameObject go;
        public int amount;
    }

    public Dictionary<string, Queue<GameObject>> dictionaryPools;
    public List<Pool> l_pools;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        SpawnAllPools();
    }

  

    void SpawnAllPools()
    {
        dictionaryPools = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in l_pools)
        {
            Queue<GameObject> goPool = new Queue<GameObject>();

            for (int i = 0; i < pool.amount; i++)
            {
                GameObject obj = Instantiate(pool.go);
                obj.transform.parent = transform;
                obj.transform.position = new Vector3(100, 100, 100);
                obj.SetActive(false);

                goPool.Enqueue(obj);
            }

            dictionaryPools.Add(pool.name, goPool);
        }
    }

    /// <summary>
    /// Reset all Elements of the pool 
    /// </summary>
    public void ResetAllElements()
    {
        l_pools.Clear();
    }

    public void ResetUnits()
    {
        foreach (Pool pool in l_pools)
        {
            pool.amount = 0;
        }
    }

    /// <summary>
    /// This funtion returns a GameObject from the pool and changes the position
    /// </summary>
    /// <returns></returns>
    public GameObject GrabFromPool(string name, Vector3 position, Quaternion rotation)
    {
        if (!dictionaryPools.ContainsKey(name))
        {
            return null;
        }

        if (dictionaryPools[name].Count == 0)
        {
            Debug.LogError("Not enought elements in Pool");
            return null;
        }
       
        GameObject spawnedGO = dictionaryPools[name].Dequeue();

        spawnedGO.SetActive(true);
        spawnedGO.transform.position = position;
        spawnedGO.transform.rotation = rotation;

        dictionaryPools[name].Enqueue(spawnedGO);

        return spawnedGO;
    }

    /// <summary>
    /// Checks if an element is or not in the pool
    /// </summary>
    public bool CheckElementInPool(string poolElementName)
    {
        if (dictionaryPools.ContainsKey(poolElementName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
