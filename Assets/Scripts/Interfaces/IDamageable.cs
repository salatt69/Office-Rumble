public interface IDamageable
{
    void TakeDamage(DamageData damageData);
    bool IsAlive { get; }
}
