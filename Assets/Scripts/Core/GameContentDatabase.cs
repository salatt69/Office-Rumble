using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Game Content Database")]
public class GameContentDatabase : ScriptableObject
{
    [Header("Player")]
    public GameObject playerPrefab;

    [Header("Enemies")]
    public List<GameObject> enemyPrefabs = new();

    [Header("Items")]
    public List<GameObject> itemPrefabs = new();

    public ItemData GetItemDataByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        foreach (var prefab in itemPrefabs)
        {
            if (prefab == null) continue;
            var item = prefab.GetComponent<Item>();
            if (item != null && item.Data != null && (item.Data.name == name || item.Data.itemName == name))
                return item.Data;
        }

        return null;
    }
}