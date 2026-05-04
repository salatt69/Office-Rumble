using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LevelDifficulty
{
    public int width;
    public int height;
    public int targetNormalRooms;
    public int minEnemies;
    public int maxEnemies;
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void AutoInit()
    {
        if (Instance == null)
        {
            var go = new GameObject(nameof(LevelManager));
            go.AddComponent<LevelManager>();
        }
    }

    [SerializeField] LevelDifficulty[] difficulties = new LevelDifficulty[]
    {
        new() { width = 9, height = 9, targetNormalRooms = 6, minEnemies = 1, maxEnemies = 2 },
        new() { width = 9, height = 9, targetNormalRooms = 10, minEnemies = 2, maxEnemies = 4 },
        new() { width = 9, height = 9, targetNormalRooms = 10, minEnemies = 3, maxEnemies = 5 },
        new() { width = 12, height = 12, targetNormalRooms = 14, minEnemies = 3, maxEnemies = 5 },
    };

    int currentLevel = 0;
    PlayerState savedState;

    public int CurrentLevel => currentLevel;
    public int TotalLevels => difficulties.Length;
    public bool HasMoreLevels => currentLevel < TotalLevels - 1;

    public LevelDifficulty GetCurrentDifficulty()
    {
        return difficulties[Mathf.Clamp(currentLevel, 0, difficulties.Length - 1)];
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
    }

    public void SavePlayerState(GameObject player)
    {
        if (player == null) return;

        savedState = new PlayerState(player);
    }

    public void RestorePlayerState(GameObject player, GameContentDatabase db)
    {
        if (player == null || savedState == null) return;

        savedState.Apply(player, db);
        savedState = null;
    }

    public void AdvanceLevel()
    {
        if (currentLevel < TotalLevels - 1)
            currentLevel++;
    }

    public void ResetProgress()
    {
        currentLevel = 0;
        savedState = null;
    }
}

[Serializable]
public class PlayerState
{
    int money;
    List<InventorySaveSlot> inventorySlots = new();
    List<BuffStat> buffs = new();

    [Serializable]
    class InventorySaveSlot
    {
        public string itemDataName;
        public int uses;
        public int slotIndex;
    }

    public PlayerState(GameObject player)
    {
        var wallet = player.GetComponent<PlayerWallet>();
        if (wallet != null)
            money = wallet.Money;

        var inventory = player.GetComponent<Inventory>();
        if (inventory != null && inventory.Slots != null)
        {
            for (int i = 0; i < inventory.Slots.Length; i++)
            {
                var slot = inventory.Slots[i];
                if (slot.data != null)
                {
                    inventorySlots.Add(new InventorySaveSlot
                    {
                        itemDataName = slot.data.name,
                        uses = slot.uses,
                        slotIndex = i
                    });
                }
            }
        }

        var body = player.GetComponent<EntityBody>();
        if (body != null)
        {
            buffs = new List<BuffStat>(body.GetCurrentBuffs());
        }
    }

    public void Apply(GameObject player, GameContentDatabase db)
    {
        var wallet = player.GetComponent<PlayerWallet>();
        if (wallet != null)
            wallet.SetMoney(money);

        var inventory = player.GetComponent<Inventory>();
        if (inventory != null && db != null)
        {
            foreach (var slot in inventorySlots)
            {
                var itemData = db.GetItemDataByName(slot.itemDataName);
                if (itemData != null)
                {
                    inventory.RestoreSlot(slot.slotIndex, itemData, slot.uses);
                }
            }
        }

        var body = player.GetComponent<EntityBody>();
        if (body != null)
        {
            foreach (var buff in buffs)
            {
                body.AddBuff(new BuffStat { stat = buff.stat, mode = buff.mode, value = buff.value });
            }
        }
    }
}
