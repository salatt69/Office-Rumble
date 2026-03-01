public class ChaseState : IEnemyState
{
    public void Enter(EnemyBrain b) { }

    public void Tick(EnemyBrain b)
    {
        var t = b.sensor.target;
        if (!t) { b.SetState(new IdleState()); return; }

        // If too close, flee
        if (b.sensor.distanceToTarget <= b.fleeDistance)
        {
            b.SetState(new FleeState());
            return;
        }

        // If in shoot range and has shooter, attack
        if (b.shooter != null && b.sensor.distanceToTarget <= b.preferredShootDistance)
        {
            b.SetState(new ShootState());
            return;
        }

        b.mover.MoveTowards(t.position);
    }

    public void Exit(EnemyBrain b) => b.mover.Stop();
}