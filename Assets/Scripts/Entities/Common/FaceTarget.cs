using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FaceTarget : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Transform target;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!target || !spriteRenderer) return;

        bool isLeft = target.position.x < transform.position.x;
        spriteRenderer.flipX = isLeft;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ResetSpriteFlip()
    {
        spriteRenderer.flipX = false;
    }
}