using System.Collections.Generic;
using UnityEngine;

public class StartRoomRuntime : RoomRuntime
{
    private GameObject astarObjectPrefab;
    private GameObject astarObjectInstance;

    protected override void Awake()
    {
        astarObjectPrefab = Resources.Load<GameObject>("Prefabs/World/Astar");
    }

    protected override void SetupRoom()
    {

        if (context?.content?.playerPrefab && spawnPoints)
        {
            Transform playerPoint = spawnPoints.GetRandomPlayerPoint();
            if (playerPoint)
            {
                Instantiate(
                    context.content.playerPrefab,
                    playerPoint.position,
                    Quaternion.identity
                );

                //if (astarObjectPrefab)
                //{
                //    astarObjectInstance = Instantiate(
                //        astarObjectPrefab,
                //        playerPoint.position,
                //        Quaternion.identity);
                //}
            }

        }

        Transform itemPoint = spawnPoints ? spawnPoints.GetRandomItemPoint() : null;

        List<GameObject> pool = context?.content?.itemPrefabs;
        if (pool == null || pool.Count == 0) return;

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