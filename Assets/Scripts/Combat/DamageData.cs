using UnityEngine;

public enum DamageType { Generic, Melee, Projectile, Explosion }

[System.Serializable]
public class DamageData
{
    public GameObject source;
    public float amount;
    public Vector2 direction;
    public float knockbackForceMultiplier;
    public DamageType type;

    public Vector2 NormalizedDirection => direction.normalized;
    public float knockbackForce => knockbackForceMultiplier * amount;

    public DamageData(
        GameObject source,
        float amount,
        Vector2 direction,
        DamageType type = DamageType.Generic
        )
    {
        this.source = source;
        this.amount = amount;
        this.direction = direction;
        this.type = type;
    }

    public DamageData CloneWith(GameObject newSource)
    {
        return new DamageData(newSource, amount, direction, type);
    }
}
