public interface IInteractable
{
    string GetInteractionText();
    void SetHighlight(bool state);
    void Interact(PlayerController player);
}