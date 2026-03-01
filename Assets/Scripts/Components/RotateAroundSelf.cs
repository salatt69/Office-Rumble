using UnityEngine;

public class RotateAroundSelf : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 180f; // degrees per second
    [SerializeField] bool clockwise = true;
    [SerializeField] bool randomStartRotation = true;

    void Start()
    {
        if (randomStartRotation)
        {
            float randomAngle = Random.Range(0f, 360f);
            transform.rotation = Quaternion.Euler(0f, 0f, randomAngle);
        }
    }

    void Update()
    {
        float direction = clockwise ? -1f : 1f;
        transform.Rotate(0f, 0f, rotationSpeed * direction * Time.deltaTime);
    }
}