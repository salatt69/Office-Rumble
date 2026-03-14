using System.Collections.Generic;
using UnityEngine;

public class RoomContext
{
    public GameContentDatabase content;
    public float overallScale = 1f;
}

public abstract class RoomRuntime : MonoBehaviour
{
    [SerializeField] protected RoomSpawnPoints spawnPoints;

    protected RoomContext context;

    protected virtual void Awake()
    {
        if (!spawnPoints)
            spawnPoints = GetComponentInChildren<RoomSpawnPoints>(true);
    }

    public virtual void Initialize(RoomContext roomContext)
    {
        context = roomContext;
        SetupRoom();
    }

    protected abstract void SetupRoom();

    protected GameObject SpawnEnemy(GameObject prefab, Transform point)
    {
        if (!prefab || !point) return null;
        return Instantiate(prefab, point.position, Quaternion.identity);
    }

    protected GameObject SpawnItem(GameObject item, Transform point)
    {
        if (item == null || !point) return null;
        return Instantiate(item, point.position, Quaternion.identity);
    }

    protected GameObject RandomEnemy()
    {
        if (context?.content == null || context.content.enemyPrefabs.Count == 0) return null;
        return context.content.enemyPrefabs[Random.Range(0, context.content.enemyPrefabs.Count)];
    }

    protected GameObject RandomItem()
    {
        if (context?.content == null || context.content.itemPrefabs.Count == 0) return null;
        return context.content.itemPrefabs[Random.Range(0, context.content.itemPrefabs.Count)];
    }
}