using System.Collections.Generic;
using UnityEngine;

public class NormalRoomRuntime : RoomRuntime
{
    [SerializeField] int minEnemies = 1;
    [SerializeField] int maxEnemies = 3;
    [SerializeField] GameObject doors;

    int diedAmount;
    bool roomCleared;
    GameObject lastKilledEnemy;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void SetupRoom()
    {
        if (spawnPoints == null) return;

        int count = Random.Range(minEnemies, maxEnemies + 1);

        var usedPoints = new List<Transform>();

        for (int i = 0; i < count; i++)
        {
            Transform point = spawnPoints.GetRandomEnemyPoint(usedPoints);
            if (point == null) continue;
            
            usedPoints.Add(point);
            GameObject enemy = SpawnEnemy(RandomEnemy(), point);

            Health enemyHealth = enemy.GetComponent<Health>();
            enemyHealth.OnDied += () => DiedInThisRoom(enemy);

            enemy.GetComponent<EnemySensor>()?.SetRoom(this);
        }
    }

    private void DiedInThisRoom(GameObject enemy)
    {
        lastKilledEnemy = enemy;
        diedAmount++;
        if (diedAmount >= enemyInstances.Count)
        {
            if (doors != null && doors.activeInHierarchy)
            {
                roomCleared = true;
                doors.SetActive(false);
            }
            SpawnReward();
        }
    }

    private void SpawnReward()
    {
        if (lastKilledEnemy == null) return;

        GameObject randomItem = RandomItem();
        if (randomItem != null)
        {
            Instantiate(randomItem, lastKilledEnemy.transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (roomCleared) return;

        if (other.GetComponent<HurtboxGroup>() && other.GetComponentInParent<PlayerController>())
        {
            if (doors != null && !doors.activeInHierarchy)
            {
                doors.SetActive(true);
            }
            OnPlayerEntered();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<HurtboxGroup>() && other.GetComponentInParent<PlayerController>())
        {
            OnPlayerExited();
        }
    }
}
