using UnityEngine;

public class StartRoomRuntime : RoomRuntime
{
    protected override void SetupRoom()
    {
        GameObject player = null;

        if (context?.content?.playerPrefab && spawnPoints)
        {
            Transform playerPoint = spawnPoints.GetRandomPlayerPoint();
            if (playerPoint)
            {
                player = Instantiate(
                    context.content.playerPrefab,
                    playerPoint.position,
                    Quaternion.identity
                );
            }
        }

        if (player != null && LevelManager.Instance != null && context?.content != null)
        {
            LevelManager.Instance.RestorePlayerState(player, context.content);
        }

        if (context?.content?.itemPrefabs == null || spawnPoints == null) return;

        GameObject orangePrefab = null;
        GameObject energyDrinkPrefab = null;

        foreach (var prefab in context.content.itemPrefabs)
        {
            if (prefab == null) continue;
            var item = prefab.GetComponent<Item>();
            if (item?.Data == null) continue;

            if (item.Data.itemName == "ORANGE")
                orangePrefab = prefab;
            else if (item.Data.itemName == "ENERGY DRINK")
                energyDrinkPrefab = prefab;
        }

        Transform leftPoint = spawnPoints.GetItemPoint(0);
        Transform rightPoint = spawnPoints.GetItemPoint(1);

        if (orangePrefab && leftPoint)
            SpawnItem(orangePrefab, leftPoint);

        if (energyDrinkPrefab && rightPoint)
            SpawnItem(energyDrinkPrefab, rightPoint);
    }
}