using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtMouse : MonoBehaviour
{
    void Update()
    {
        // Get mouse position in world space
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        // Calculate direction from object to mouse
        Vector2 direction = (mouseWorld - transform.position);

        // Compute rotation angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply rotation (z-axis for 2D)
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
