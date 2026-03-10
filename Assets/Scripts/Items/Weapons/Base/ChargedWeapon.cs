using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ChargedWeapon : Weapon
{   
    public ChargedWeaponData CWD => (ChargedWeaponData)WD;
    public float ChargeTime => CWD.weaponChargeTime / GetAttackSpeed();

    ProgressRingUI chargeRing;
    ProgressRingUI chargeRingPrefab;
    Transform chargeRingTarget;

    public float currentCharge { get; private set; }
    float lastUseTime;
    bool charging;
    GameObject lastOwner;

    protected override void Awake()
    {
        base.Awake();

        chargeRingPrefab = Resources.Load<ProgressRingUI>("Prefabs/UI/ProgressRing");
        if (!chargeRingPrefab)
            Debug.LogWarning("EnergyDrinkWeapon: Could not load ProgressRing prefab from Resources/Prefabs/UI/ProgressRing");
    }

    void Update()
    {
        if (charging && (Time.time - lastUseTime) > CWD.releaseGrace)
        {
            Fire(lastOwner);
            charging = false;
            currentCharge = 0f;

            if (chargeRing)
            {
                chargeRing.SetProgress(0f);
                chargeRing.Hide();
            }
        }
    }

    public override void Use()
    {
        if (!Owner) return;

        EnsureChargeRing();

        charging = true;
        lastOwner = Owner;
        lastUseTime = Time.time;

        currentCharge += Time.deltaTime;
        if (currentCharge > CWD.weaponChargeTime)
            currentCharge = CWD.weaponChargeTime;

        if (chargeRing)
        {
            chargeRing.Show();
            chargeRing.SetProgress(currentCharge / CWD.weaponChargeTime);
        }
    }

    public virtual void Fire(GameObject owner)
    {

    }

    void EnsureChargeRing()
    {
        if (!Owner) return;

        if (!chargeRingTarget)
            chargeRingTarget = Owner.transform;

        if (!chargeRing)
        {
            chargeRing = Owner.GetComponentInChildren<ProgressRingUI>(true);

            if (!chargeRing && chargeRingPrefab)
                chargeRing = Instantiate(chargeRingPrefab, chargeRingTarget);
        }

        if (chargeRing)
        {
            chargeRing.transform.SetParent(chargeRingTarget, false);
            chargeRing.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            chargeRing.SetTarget(chargeRingTarget);
        }
    }
}
