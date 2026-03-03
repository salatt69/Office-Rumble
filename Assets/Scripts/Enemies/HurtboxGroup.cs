using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class HurtboxGroup : MonoBehaviour
{
    [SerializeField] Collider2D[] hurtColliders;

    public void SetColliderActive(bool isActive)
    {
        foreach (var col in hurtColliders)
        {
            if (col) col.enabled = isActive;
        }
    }

    public bool CompareColliderToHurtbox(Collider2D collider)
    {
        foreach (var col in hurtColliders)
        {
            if (col == collider) return true;
        }
        return false;
    }
}
