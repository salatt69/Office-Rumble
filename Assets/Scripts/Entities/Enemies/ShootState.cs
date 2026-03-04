public class ShootState : IEnemyState
{
    public void Enter(EnemyBrain b) { }

    public void Tick(EnemyBrain b)
    {
        var t = b.sensor.target;
        if (!t) { b.SetState(new IdleState()); return; }

        float d = b.sensor.distanceToTarget;

        // Maintain distance: if too close, back up; if too far, chase
        if (d < b.fleeDistance)
        {
            b.mover.MoveAwayFrom(t.position);
        }
        else if (d > b.preferredShootDistance)
        {
            b.SetState(new ChaseState());
            return;
        }
        else
        {
            b.mover.Stop();
        }

        b.shooter.TryShootAt(t);
    }

    public void Exit(EnemyBrain b) => b.mover.Stop();
}