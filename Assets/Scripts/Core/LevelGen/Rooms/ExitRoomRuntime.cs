using UnityEngine;

public class ExitRoomRuntime : RoomRuntime
{
    [SerializeField] GameObject exitTriggerPrefab;

    protected override void SetupRoom()
    {
        if (!exitTriggerPrefab || !spawnPoints) return;

        Transform exitPoint = spawnPoints.GetRandomExitPoint();
        if (!exitPoint) return;

        Instantiate(exitTriggerPrefab, exitPoint.position, Quaternion.identity);
    }
}