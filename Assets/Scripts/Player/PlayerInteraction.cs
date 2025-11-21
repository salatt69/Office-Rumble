using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteraction : MonoBehaviour
{
    List<IInteractable> nearbyInteractables = new();
    IInteractable currentInteractable;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            RegisterInteractable(interactable);
            // TODO: Add UIManager to display GetInteractionText()
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
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
        IInteractable nearest = null;
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

    void RegisterInteractable(IInteractable interactable)
    {
        if (!nearbyInteractables.Contains(interactable))
            nearbyInteractables.Add(interactable);

        UpdateCurrentInteractable();
    }

    void UnregisterInteractable(IInteractable interactable)
    {
        nearbyInteractables.Remove(interactable);

        UpdateCurrentInteractable();
    }

    public IInteractable GetCurrentInteractable() { return currentInteractable; }
}
