using UnityEngine;

public class PickUp : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            interactable?.TryInteract(GetComponentInParent<PlayerController>());
        }
    }
}
