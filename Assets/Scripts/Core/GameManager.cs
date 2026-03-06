using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public HUD hud { get; private set; }

    DamageNumber damageNumberPrefab;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void AutoInit()
    {
        if (Instance == null)
        {
            new GameObject("GameManager").AddComponent<GameManager>();
        }
    }

    public void RegisterHUD(HUD hud)
    {
        this.hud = hud;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!damageNumberPrefab)
        {
            damageNumberPrefab = Resources.Load<DamageNumber>("Prefabs/UI/DamageNumber");
        }
    }

    public void SpawnDamageNumber(float amount, Vector3 worldPos)
    {
        if (!damageNumberPrefab) return;

        Vector3 jitter = new(
            Random.Range(-damageNumberPrefab.spawnJitter.x, damageNumberPrefab.spawnJitter.x),
            Random.Range(-damageNumberPrefab.spawnJitter.y, damageNumberPrefab.spawnJitter.y),
            0f
        );

        var dn = Instantiate(damageNumberPrefab, worldPos + jitter, Quaternion.identity);
        dn.Init(amount);
    }
}