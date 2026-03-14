using System.Collections.Generic;
using UnityEngine;

public class MerchantRoomRuntime : RoomRuntime
{
    protected override void SetupRoom()
    {
        if (!spawnPoints || context == null || context.content == null)
            return;

        Transform merchantPoint = spawnPoints.GetMerchantPoint(0);
        GameObject merchantPrefab = Resources.Load<GameObject>("Prefabs/Entities/Dummy");
        if (merchantPrefab)
        {
            GameObject merchantObject = Instantiate(merchantPrefab, merchantPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Failed to load merchantPrefab from 'Prefabs/Entities/Dummy'.");
        }

        List<GameObject> pool = context.content.itemPrefabs;
        if (pool == null || pool.Count == 0)
            return;

        int spawnCount = spawnPoints.ItemPoints.Count;

        // If you want strict 100% fill, pool must have enough unique items
        if (pool.Count < spawnCount)
        {
            Debug.LogWarning($"MerchantRoomRuntime: Not enough unique items in pool ({pool.Count}) for merchant points ({spawnCount}).");
            spawnCount = pool.Count;
        }

        List<GameObject> availableItems = new(pool);

        for (int i = 0; i < spawnCount; i++)
        {
            Transform point = spawnPoints.GetItemPoint(i);
            if (!point) continue;

            int randomIndex = Random.Range(0, availableItems.Count);
            GameObject itemPrefab = availableItems[randomIndex];
            availableItems.RemoveAt(randomIndex);

            if (!itemPrefab) continue;

            GameObject item = Instantiate(itemPrefab, point.position, Quaternion.identity);
            ItemPickup itemPickup = item.GetComponentInChildren<ItemPickup>();
            itemPickup.SetFreeItem(false);
        }
    }
}
