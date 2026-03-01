public class IdleState : IEnemyState
{
    public void Enter(EnemyBrain b) => b.mover.Stop();

    public void Tick(EnemyBrain b)
    {
        if (b.sensor.target == null) return;

        // if player is close enough, start chasing or attacking depending on your design
        if (b.sensor.distanceToTarget <= b.activationRadius)
            b.SetState(new ChaseState());
    }

    public void Exit(EnemyBrain b) { }
}