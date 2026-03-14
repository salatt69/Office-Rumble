using UnityEngine;

public class NormalRoomRuntime : RoomRuntime
{
    [SerializeField] int minEnemies = 1;
    [SerializeField] int maxEnemies = 3;

    protected override void SetupRoom()
    {
        if (spawnPoints == null) return;

        int count = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < count; i++)
        {
            Transform point = spawnPoints.GetRandomEnemyPoint();
            SpawnEnemy(RandomEnemy(), point);
        }
    }
}
