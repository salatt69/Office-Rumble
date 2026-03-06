using UnityEngine;

public class DamageNumberSystem : MonoBehaviour
{
    public static DamageNumberSystem Instance { get; private set; }

    [SerializeField] DamageNumber prefab;

    void Awake()
    {
        Instance = this;
    }

    public void Spawn(float amount, Vector3 worldPos)
    {
        if (!prefab) return;

        var dn = Instantiate(prefab, worldPos, Quaternion.identity);
        dn.Init(amount);
    }
}