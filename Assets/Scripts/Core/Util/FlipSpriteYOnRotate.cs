using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FlipSpriteYOnRotate : MonoBehaviour
{
    [Tooltip("Whether to follow the target's rotation or the GameObject's own rotation.")]
    [SerializeField] bool followTargetRotation = false;
    [SerializeField] Transform target;

    [SerializeField] bool applyFlipToChildren = true;

    readonly List<SpriteRenderer> spriteRenderers = new();

    void Awake()
    {
        if (followTargetRotation)
        {
            if (target == null)
            {
                Debug.LogWarning("FlipSpriteYOnRotate: followTargetRotation is enabled but no target is assigned. Defaulting to self rotation.");
                followTargetRotation = false;
            }
        }
    }

    void Update()
    {
        Transform finalTarget = (followTargetRotation && target) ? target : transform;
        bool isFlipped = finalTarget.rotation.eulerAngles.z > 90f && finalTarget.rotation.eulerAngles.z < 270f;

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.flipY = isFlipped;
        }
    }

    private void LateUpdate()
    {
        RefreshRenderers();
    }

    void RefreshRenderers()
    {
        spriteRenderers.Clear();

        if (applyFlipToChildren)
        {
            spriteRenderers.AddRange(GetComponentsInChildren<SpriteRenderer>(true));
        }
        else
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr) spriteRenderers.Add(sr);
        }
    }
}
