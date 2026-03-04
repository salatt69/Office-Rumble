using UnityEngine;

public class PickUpOnCollide : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            interactable?.TryInteract(GetComponentInParent<PlayerController>());
        }
    }
}
