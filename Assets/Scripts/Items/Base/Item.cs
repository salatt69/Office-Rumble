using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] ItemData data;

    [Tooltip("GameObjects to disable when this item is equipped.")]
    [SerializeField] GameObject[] gameObjectsToDisableOnEquip;

    protected SpriteRenderer spriteRenderer;
    protected GameObject owner;

    public ItemData Data => data;
    public GameObject Owner => owner;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"{name}: No SpriteRenderer found in children.");
        }
    }

    public virtual void Initialize(ItemData newData)
    {
        data = newData;
    }

    public virtual void OnEquip(GameObject owner)
    {
        if (owner == null) return;
        this.owner = owner;
        
        if (gameObjectsToDisableOnEquip != null)
        {
            SetGameObjectsActive(false);
        }
        if (spriteRenderer)
        {
            spriteRenderer.sprite = data.equipped;
        }
    }

    public virtual void OnUnequip()
    {
        if (this.owner == null) return;
        this.owner = null;

        if (gameObjectsToDisableOnEquip != null)
        {
            SetGameObjectsActive(true);
        }
        if (spriteRenderer)
        {
            spriteRenderer.sprite = data.unequipped;
        }
    }

    private void SetGameObjectsActive(bool isActive)
    {
        foreach (GameObject go in gameObjectsToDisableOnEquip)
        {
            if (go != null)
            {
                go.SetActive(isActive);
            }
        }
    }
}
