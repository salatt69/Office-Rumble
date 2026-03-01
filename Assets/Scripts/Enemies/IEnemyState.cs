public interface IEnemyState
{
    void Enter(EnemyBrain brain);
    void Tick(EnemyBrain brain);
    void Exit(EnemyBrain brain);
}