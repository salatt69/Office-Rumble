using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform hand;
    [SerializeField] ItemHolder holder;

    void Awake()
    {
        if (!hand) hand = transform.Find("Hand");
        if (!holder && hand)
            holder = hand.GetComponentInChildren<ItemHolder>(true);
    }

    public void TryUse()
    {
        if (!holder)
        {
            Debug.LogWarning($"{name}: Can't fire - no ItemHolder found.");
            return;
        }

        //holder.UseCurrentItem();
    }
}