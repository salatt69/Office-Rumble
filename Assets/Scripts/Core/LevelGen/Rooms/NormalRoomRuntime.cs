using UnityEngine;

public class NormalRoomRuntime : RoomRuntime
{
    [SerializeField] int minEnemies = 1;
    [SerializeField] int maxEnemies = 3;
    [SerializeField, Range(0f, 1f)] float itemSpawnChance = 0.35f;

    protected override void SetupRoom()
    {
        if (spawnPoints == null) return;

        int count = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < count; i++)
        {
            Transform point = spawnPoints.GetRandomEnemyPoint();
            SpawnEnemy(RandomEnemy(), point);
        }

        if (Random.value <= itemSpawnChance)
        {
            Transform point = spawnPoints.GetRandomItemPoint();
            SpawnItem(RandomItem(), point);
        }
    }
}