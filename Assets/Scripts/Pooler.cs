using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public struct PoolSpawnOptions
{
    public GameObject Obj;
    public bool SetPosition;
    public Vector3 Position;
    public bool UseWorldSpace;
    public Transform Parent;
}

public class Pooler : MonoBehaviour
{
    public static Pooler Singleton;

    [SerializeField]
    [BoxGroup("Debug")]
    private bool PrintDebugLogs;

    [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "pooledObject")]
    [SerializeField]
    [ReadOnly]
    private List<Pool> ActivePools;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
#if UNITY_EDITOR
            if (PrintDebugLogs)
                Debug.LogError("Pooler: Found duplicate pooler!");
#endif
            Destroy(this.gameObject);
        }
    }

    public static GameObject Spawn(GameObject prefab, Transform parent = null, bool useWorldSpace = false)
    {
        PoolSpawnOptions options = new()
        {
            Obj = prefab,
            SetPosition = false,
            UseWorldSpace = useWorldSpace,
            Parent = parent
        };
        return Singleton.GetPooledObject(options);
    }
    public static GameObject SpawnAt(GameObject prefab, Vector3 spawnLocation, Transform parent = null, bool useWorldSpace = false)
    {
        PoolSpawnOptions options = new()
        {
            Obj = prefab,
            SetPosition = true,
            Position = spawnLocation,
            UseWorldSpace = useWorldSpace,
            Parent = parent
        };
        return Singleton.GetPooledObject(options);
    }

    private GameObject GetPooledObject(PoolSpawnOptions options)
    {
        if (options.Obj == null)
        {
#if UNITY_EDITOR
            if (PrintDebugLogs)
                Debug.Log($"Pooler Log: Attempted to spawn with null targetObject.");
#endif
            return null;
        }
        
        var pool = ActivePools.FirstOrDefault(o => o.pooledObject.name == options.Obj.name);
        pool ??= CreateNewPool(options);
        if (options.Parent == null)
            options.Parent = pool.poolContent;

        var obj = pool?.spawnedObjects.FirstOrDefault(o => o.activeSelf == false);

        if (obj == null)
        {
            StartCoroutine(IncreasePoolVolume());
            IEnumerator IncreasePoolVolume()
            {
                var amountToSpawn = pool.increaseBoundsAmount;
                while (amountToSpawn > 0)
                {
                    var obj = Instantiate(pool.GetPooledObject(), options.Parent, options.UseWorldSpace);
                    obj.name = pool.GetPooledObject().name;
                    pool.spawnedObjects.Add(obj);
                    obj.SetActive(false);
                    amountToSpawn--;
                }
                obj = pool.spawnedObjects.FirstOrDefault(o => o.activeSelf == false);
                yield return new WaitForEndOfFrame();

            }

        }

        if (options.SetPosition)
            obj.transform.position = options.Position;
        obj.SetActive(true);
        return obj;
    }

    private Pool CreateNewPool(PoolSpawnOptions Options)
    {
        var newPool = new Pool
        {
            poolContent = Instantiate(new GameObject(), this.transform).transform,
            pooledObject = Options.Obj,
            initialAmount = 3,
            increaseBoundsAmount = 3
        };
        newPool.poolContent.name = Options.Obj.name;
#if UNITY_EDITOR
        if (PrintDebugLogs)
            Debug.Log($"Pooler Log: Created new pool for {Options.Obj}.");
#endif
        ActivePools.Add(newPool);
        return newPool;
    }

#if UNITY_EDITOR
    [Button]
    private void SortPooledObjects()
    {
        ActivePools = ActivePools.OrderBy(o => o.pooledObject.name).ToList();
    }
#endif
}

[Serializable]
public class Pool
{
    public Transform poolContent;
    public GameObject pooledObject;
    public int initialAmount;
    public int increaseBoundsAmount;
    public List<GameObject> spawnedObjects = new();

    public GameObject GetPooledObject()
    {
        pooledObject.SetActive(false);
        return pooledObject;
    }
}