using System.Collections.Generic;
using UnityEngine;

public class RoomContext
{
    public GameContentDatabase content;
    public float overallScale = 1f;
}

public abstract class RoomRuntime : MonoBehaviour
{
    protected RoomSpawnPoints spawnPoints;
    protected RoomContext context;

    protected List<GameObject> enemyInstances = new();
    protected List<GameObject> itemInstances = new();

    protected bool isPlayerInside { get; private set; }

    public bool IsPlayerInside => isPlayerInside;

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

    protected virtual void OnPlayerEntered() => isPlayerInside = true;
    protected virtual void OnPlayerExited() => isPlayerInside = false;

    protected GameObject SpawnEnemy(GameObject prefab, Transform point)
    {
        if (!prefab || !point) return null;
        var enemy = Instantiate(prefab, point.position, Quaternion.identity);
        enemyInstances.Add(enemy);
        return enemy;
    }

    protected GameObject SpawnItem(GameObject prefab, Transform point)
    {
        if (!prefab || !point) return null;
        var item = Instantiate(prefab, point.position, Quaternion.identity);
        itemInstances.Add(item);
        return item;
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