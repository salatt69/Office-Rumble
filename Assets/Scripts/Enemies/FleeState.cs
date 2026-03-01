using UnityEngine;

public class FleeState : IEnemyState
{
    float fleeTime;
    float endTime;

    public void Enter(EnemyBrain b)
    {
        fleeTime = 0.6f; // short burst
        endTime = Time.time + fleeTime;
    }

    public void Tick(EnemyBrain b)
    {
        var t = b.sensor.target;
        if (!t) { b.SetState(new IdleState()); return; }

        b.mover.MoveAwayFrom(t.position);

        if (Time.time >= endTime)
            b.SetState(new ChaseState());
    }

    public void Exit(EnemyBrain b) => b.mover.Stop();
}