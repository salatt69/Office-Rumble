using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class PlayerUIBootstrap : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds1 = new(1f);

    [Header("UI Prefabs")]
    [SerializeField] GameObject healthUIPrefab;
    [SerializeField] GameObject inventoryUIPrefab;
    [SerializeField] GameObject walletUIPrefab;
    [SerializeField] GameObject statsUIPrefab;
    [SerializeField] GameObject consumableNotificationUIPrefab;
    [SerializeField] GameObject mapUIPrefab;
    [SerializeField] GameObject afterDeathUIPrefab;

    [Header("Optional Hierarchy Parent")]
    [SerializeField] Transform uiParent;

    Health health;
    Inventory inventory;
    PlayerWallet wallet;
    EntityBody body;
    PlayerController playerController;

    GameObject spawnedHealthUI;
    GameObject spawnedInventoryUI;
    GameObject spawnedWalletUI;
    GameObject spawnedStatsUI;
    GameObject spawnedNotificationUI;
    GameObject spawnedMapUI;
    GameObject spawnedAfterDeathUI;

    bool deathHandled;

    void Awake()
    {
        health = GetComponentInChildren<Health>(true);
        inventory = GetComponentInChildren<Inventory>(true);
        wallet = GetComponentInChildren<PlayerWallet>(true);
        body = GetComponentInChildren<EntityBody>(true);
        playerController = GetComponentInChildren<PlayerController>(true);
    }

    void Start()
    {
        SpawnAndBindAll();

        if (health != null)
            health.OnDied += HandlePlayerDeath;
    }

    void OnDestroy()
    {
        if (health != null)
            health.OnDied -= HandlePlayerDeath;
    }

    void HandlePlayerDeath()
    {
        if (deathHandled) return;
        deathHandled = true;

        playerController?.SetDead();

        StartCoroutine(ShowAfterDeathUIAfterDelay());
    }

    IEnumerator ShowAfterDeathUIAfterDelay()
    {
        yield return _waitForSeconds1;
        ShowAfterDeathUI();
    }

    void ShowAfterDeathUI()
    {
        if (!afterDeathUIPrefab || spawnedAfterDeathUI) return;

        spawnedAfterDeathUI = InstantiateUI(afterDeathUIPrefab);
    }

    void SpawnAndBindAll()
    {
        SpawnHealthUI();
        SpawnInventoryUI();
        SpawnWalletUI();
        SpawnStatsUI();
        SpawnNotificationUI();
        SpawnMapUI();
    }

    void SpawnHealthUI()
    {
        if (!healthUIPrefab || !health || spawnedHealthUI) return;

        spawnedHealthUI = InstantiateUI(healthUIPrefab);

        var healthUI = spawnedHealthUI.GetComponent<HeatlhBarUIDocumentBinder>();
        if (!healthUI)
            healthUI = spawnedHealthUI.GetComponentInChildren<HeatlhBarUIDocumentBinder>(true);

        if (healthUI)
            healthUI.Bind(health);
        else
            Debug.LogWarning($"{name}: Spawned health UI prefab has no HeatlhBarUIDocumentBinder.");
    }

    void SpawnInventoryUI()
    {
        if (!inventoryUIPrefab || !inventory || spawnedInventoryUI) return;

        spawnedInventoryUI = InstantiateUI(inventoryUIPrefab);

        var inventoryUI = spawnedInventoryUI.GetComponent<InventoryUIDocumentBinder>();
        if (!inventoryUI)
            inventoryUI = spawnedInventoryUI.GetComponentInChildren<InventoryUIDocumentBinder>(true);

        if (inventoryUI)
            inventoryUI.Bind(inventory);
        else
            Debug.LogWarning($"{name}: Spawned inventory UI prefab has no InventoryUIDocumentBinder.");
    }

    void SpawnWalletUI()
    {
        if (!walletUIPrefab || !wallet || spawnedWalletUI) return;

        spawnedWalletUI = InstantiateUI(walletUIPrefab);

        var walletUI = spawnedWalletUI.GetComponent<ScrapDisplayUIDocumentBinder>();
        if (!walletUI)
            walletUI = spawnedWalletUI.GetComponentInChildren<ScrapDisplayUIDocumentBinder>(true);

        if (walletUI)
            walletUI.Bind(wallet);
        else
            Debug.LogWarning($"{name}: Spawned wallet UI prefab has no ScrapDisplayUIDocumentBinder.");
    }

    void SpawnStatsUI()
    {
        if (!statsUIPrefab || !body || spawnedStatsUI) return;

        spawnedStatsUI = InstantiateUI(statsUIPrefab);

        var statsUI = spawnedStatsUI.GetComponent<StatsDisplayUIDocumentBinder>();
        if (!statsUI)
            statsUI = spawnedStatsUI.GetComponentInChildren<StatsDisplayUIDocumentBinder>(true);

        if (statsUI)
            statsUI.Bind(body);
        else
            Debug.LogWarning($"{name}: Spawned stats UI prefab has no StatsDisplayUIDocumentBinder.");
    }

    void SpawnNotificationUI()
    {
        if (!consumableNotificationUIPrefab || spawnedNotificationUI) return;

        spawnedNotificationUI = InstantiateUI(consumableNotificationUIPrefab);
    }

    void SpawnMapUI()
    {
        if (!mapUIPrefab || spawnedMapUI) return;

        spawnedMapUI = InstantiateUI(mapUIPrefab);

        var mapUI = spawnedMapUI.GetComponent<MapUIDocumentBinder>();
        if (!mapUI)
            mapUI = spawnedMapUI.GetComponentInChildren<MapUIDocumentBinder>(true);

        if (mapUI)
        {
            var levelGen = FindAnyObjectByType<ModularLevelGeneration>();
            mapUI.Bind(levelGen, transform);
        }
        else
        {
            Debug.LogWarning($"{name}: Spawned map UI prefab has no MapUIDocumentBinder.");
        }
    }

    GameObject InstantiateUI(GameObject prefab)
    {
        if (uiParent)
            return Instantiate(prefab, uiParent);

        return Instantiate(prefab);
    }
}