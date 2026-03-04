// PlayerController.cs
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] InputRouter input;
    [SerializeField] EntityMotor2D motor;
    [SerializeField] PlayerHandAim aim;
    [SerializeField] CameraFollow2D cam;
    [SerializeField] PlayerCombat combat;
    [SerializeField] Inventory inventory;
    [SerializeField] Animator animator;

    [Header("Pickup")]
    [SerializeField] float defaultPickupCooldown = 2f;

    float pickupCooldownExitTime;
    public bool CanPickup => Time.time >= pickupCooldownExitTime;

    void Awake()
    {
        if (!input) input = GetComponent<InputRouter>();
        if (!motor) motor = GetComponent<EntityMotor2D>();
        if (!aim) aim = GetComponent<PlayerHandAim>();
        if (!cam) cam = GetComponent<CameraFollow2D>();
        if (!combat) combat = GetComponent<PlayerCombat>();
        if (!inventory) inventory = GetComponent<Inventory>();
        if (!animator) animator = GetComponentInChildren<Animator>(true);

        // Let input router call back into controller for inventory actions
        if (input) input.SetOwner(this);
    }

    void Start()
    {
        inventory?.SelectSlot(inventory.SelectedIndex);
    }

    void Update()
    {
        if (input && input.AttackHeld)
            combat.TryFire();

        if (animator && input)
        {
            Vector2 m = input.MoveInput;
            animator.SetFloat("Horizontal", m.x);
            animator.SetFloat("Vertical", m.y);
            animator.SetFloat("Speed", m.sqrMagnitude);
        }
    }

    void FixedUpdate()
    {
        if (motor && input)
            motor.SetMoveInput(input.MoveInput);

        cam?.TickFixed();
        motor?.FixedTick();
    }

    void LateUpdate()
    {
        if (aim && input)
            aim.SetScoped(input.IsScoped);

        aim?.Tick();
    }

    public void AddItemPickupCooldown(float cooldownTime = -1f)
    {
        float cd = cooldownTime >= 0f ? cooldownTime : defaultPickupCooldown;
        pickupCooldownExitTime = Mathf.Max(pickupCooldownExitTime, Time.time + cd);
    }

    public Vector3 GetItemDropPosition()
    {
        return aim ? aim.GetItemDropPosition() : transform.position;
    }

    public void ApplyKnockbackLock(float duration)
    {
        if (motor) motor.ApplyKnockbackLock(duration);
    }

    // Inventory actions triggered by input router
    public void TryDrop()
    {
        if (!inventory) return;

        // Dropping is not the same as picking up, but if you want to block it during pickup lock,
        // keep this check. If you DON'T want that, remove this if.
        if (!CanPickup)
            return;

        Vector3 dropPos = GetItemDropPosition();
        inventory.Drop(dropPos);
        AddItemPickupCooldown(); // optional: add cooldown after dropping too
    }

    public void SelectSlot(int index) => inventory?.SelectSlot(index);
    public void SelectNext() => inventory?.SelectNext();
    public void SelectPrevious() => inventory?.SelectPrevious();
}