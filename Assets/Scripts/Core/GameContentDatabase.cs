using System.Collections.Generic;
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
}