using System.Collections.Generic;
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

        Transform itemPoint = spawnPoints ? spawnPoints.GetRandomItemPoint() : null;

        List<GameObject> pool = context?.content?.itemPrefabs;
        if (pool == null || pool.Count == 0) return;

        if (player == null || player.GetComponent<Inventory>() == null || player.GetComponent<Inventory>().Slots == null || player.GetComponent<Inventory>().Slots[0].data == null)
        {
            List<GameObject> availableItems = new(pool);
            GameObject weapon = null;

            while (pool.Count > 0)
            {
                weapon = RandomItem();
                if (weapon.GetComponent<Weapon>() != null)
                    break;
                else
                    availableItems.Remove(weapon);
            }

            SpawnItem(weapon, itemPoint);
        }
    }
}