using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(PlayerController))]
public class HighlightItemsForPlayer : MonoBehaviour
{
    [SerializeField] Collider2D interactionCollider;

    readonly List<IHighlight> nearbyInteractables = new();
    IHighlight currentInteractable;

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.TryGetComponent<IHighlight>(out var interactable))
        {
            RegisterInteractable(interactable);
            // TODO: Add UIManager to display GetInteractionText()
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IHighlight>(out var interactable))
        {
            interactable.SetHighlight(false);

            UnregisterInteractable(interactable);
            // TODO: Add UIManager to clear GetInteractionText()
        }
    }

    void UpdateCurrentInteractable()
    {
        if (nearbyInteractables.Count == 0)
        {
            if (currentInteractable != null)
            {
                currentInteractable.SetHighlight(false);
                currentInteractable = null;
            }
            return;
        }

        // find nearest interactable
        IHighlight nearest = null;
        float minDistance = float.MaxValue;

        foreach (var interactable in nearbyInteractables)
        {
            float dist = Vector2.Distance(transform.position,
                ((MonoBehaviour)interactable).transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = interactable;
            }
        }

        // switch highlight if needed
        if (nearest != currentInteractable)
        {
            currentInteractable?.SetHighlight(false);
            currentInteractable = nearest;
            currentInteractable.SetHighlight(true);
        }
    }

    void RegisterInteractable(IHighlight interactable)
    {
        if (!nearbyInteractables.Contains(interactable))
            nearbyInteractables.Add(interactable);

        UpdateCurrentInteractable();
    }

    void UnregisterInteractable(IHighlight interactable)
    {
        nearbyInteractables.Remove(interactable);

        UpdateCurrentInteractable();
    }

    public IHighlight GetCurrentInteractable() { return currentInteractable; }
}
