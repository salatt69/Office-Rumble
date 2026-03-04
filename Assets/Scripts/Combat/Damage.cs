using UnityEngine;

public class Damage : MonoBehaviour, IDamager
{
    [SerializeField] DamageData damageData;
    [SerializeField] bool destroyOnHit = false;
    [SerializeField] LayerMask targetLayers;

    public DamageData Data => damageData;

    void Reset()
    {
        if (damageData == null)
            damageData = new DamageData(gameObject, 10f, Vector2.zero);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) == 0) return;

        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null) return;

        Vector2 direction = (other.transform.position - transform.position).normalized;
        damageData.direction = direction;

        damageable.TakeDamage(damageData);

        if (destroyOnHit)
            Destroy(gameObject);
    }
}
