using UnityEngine;

public class NormalRoomRuntime : RoomRuntime
{
    [SerializeField] int minEnemies = 1;
    [SerializeField] int maxEnemies = 3;
    [SerializeField] GameObject doors;

    int diedAmount;
    bool roomCleared;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void SetupRoom()
    {
        if (spawnPoints == null) return;

        int count = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < count; i++)
        {
            Transform point = spawnPoints.GetRandomEnemyPoint();
            GameObject enemy = SpawnEnemy(RandomEnemy(), point);

            Health enemyHealth = enemy.GetComponent<Health>();
            enemyHealth.OnDied += DiedInThisRoom;

            enemy.GetComponent<EnemySensor>()?.SetRoom(this);
        }
    }

    private void DiedInThisRoom()
    {
        diedAmount++;
        if (diedAmount >= enemyInstances.Count)
        {
            if (doors != null && doors.activeInHierarchy)
            {
                roomCleared = true;
                doors.SetActive(false);
            }
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
