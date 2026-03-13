using UnityEngine;

public class HiddenRoomRuntime : RoomRuntime
{
    [SerializeField, Range(0f, 1f)] float itemInsteadOfEnemyChance = 0.7f;

    protected override void SetupRoom()
    {
        if (spawnPoints == null) return;

        if (Random.value <= itemInsteadOfEnemyChance)
            SpawnItem(RandomItem(), spawnPoints.GetRandomItemPoint());
        else
            SpawnEnemy(RandomEnemy(), spawnPoints.GetRandomEnemyPoint());
    }
}