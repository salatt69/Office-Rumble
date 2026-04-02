using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomSpawnPoints : MonoBehaviour
{
    [Header("Spawn Point Groups")]
    [SerializeField] Transform playerSpawnPoints;
    [SerializeField] Transform enemySpawnPoints;
    [SerializeField] Transform itemSpawnPoints;
    [SerializeField] Transform merchantSpawnPoints;
    [SerializeField] Transform exitSpawnPoints;

    readonly List<Transform> playerPoints = new();
    readonly List<Transform> enemyPoints = new();
    readonly List<Transform> itemPoints = new();
    readonly List<Transform> merchantPoints = new();
    readonly List<Transform> exitPoints = new();

    public IReadOnlyList<Transform> PlayerPoints => playerPoints;
    public IReadOnlyList<Transform> EnemyPoints => enemyPoints;
    public IReadOnlyList<Transform> ItemPoints => itemPoints;
    public IReadOnlyList<Transform> MerchantPoints => merchantPoints;
    public IReadOnlyList<Transform> ExitPoints => exitPoints;

    void Awake()
    {
        RebuildLists();
    }

    [ContextMenu("Rebuild Spawn Point Lists")]
    public void RebuildLists()
    {
        FillList(playerSpawnPoints, playerPoints);
        FillList(enemySpawnPoints, enemyPoints);
        FillList(itemSpawnPoints, itemPoints);
        FillList(merchantSpawnPoints, merchantPoints);
        FillList(exitSpawnPoints, exitPoints);
    }

    void FillList(Transform root, List<Transform> list)
    {
        list.Clear();

        if (!root) return;

        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child != null)
                list.Add(child);
        }
    }

    public Transform GetRandomPlayerPoint() => GetRandomFrom(playerPoints);
    public Transform GetRandomEnemyPoint(List<Transform> excluded = null)
    {
        if (excluded == null || excluded.Count == 0) return GetRandomFrom(enemyPoints);
        
        var available = enemyPoints.Where(p => !excluded.Contains(p)).ToList();
        return GetRandomFrom(available);
    }
    public Transform GetRandomItemPoint() => GetRandomFrom(itemPoints);
    public Transform GetRandomMerchantPoint() => GetRandomFrom(merchantPoints);
    public Transform GetRandomExitPoint() => GetRandomFrom(exitPoints);

    public Transform GetPlayerPoint(int index) => GetAt(playerPoints, index);
    public Transform GetEnemyPoint(int index) => GetAt(enemyPoints, index);
    public Transform GetItemPoint(int index) => GetAt(itemPoints, index);
    public Transform GetMerchantPoint(int index) => GetAt(merchantPoints, index);
    public Transform GetExitPoint(int index) => GetAt(exitPoints, index);

    static Transform GetRandomFrom(List<Transform> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }

    static Transform GetAt(List<Transform> list, int index)
    {
        if (list == null || index < 0 || index >= list.Count) return null;
        return list[index];
    }
}